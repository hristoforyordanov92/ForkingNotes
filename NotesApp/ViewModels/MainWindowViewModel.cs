using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Data;
using Core.Extensions;
using Core.MVVM;
using NotesApp.Managers;
using NotesApp.Models;
using NotesApp.Views;
using Timer = System.Timers.Timer;

namespace NotesApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Timer used for delaying each search query to avoid constant filtering and searching
        /// while the user still haven't finished typing their search query.
        /// </summary>
        private readonly Timer _searchQueryDelayTimer = new(TimeSpan.FromMilliseconds(300));

        /// <summary>
        /// The current window. Used to set as an owner of other windows spawned from within this class.
        /// </summary>
        private readonly Window _window;

        /// <summary>
        /// Collection of notes which have been modified and haven't been saved yet.
        /// </summary>
        private readonly HashSet<Note> _dirtyNotes = [];

        private string _selectedNoteNewTag = string.Empty;
        private string _searchTag = string.Empty;
        private string _searchQuery = string.Empty;
        private Note? _selectedNote;

        public MainWindowViewModel(Window window)
        {
            _window = window;

            CreateNoteCommand = new RelayCommand(CreateNote);
            DeleteNoteCommand = new RelayCommand(DeleteNote);
            SaveNoteChangesCommand = new RelayCommand(SaveNoteChanges);
            SaveAllNotesChangesCommand = new RelayCommand(SaveAllNotesChanges);
            OpenSettingsWindowCommand = new RelayCommand(OpenSettingsWindow);
            AddSearchTagCommand = new RelayCommand(AddSearchTag);
            RemoveSearchTagCommand = new RelayCommand<string>(RemoveSearchTag);
            AddTagCommand = new RelayCommand(AddTagToSelectedNote);
            RemoveTagCommand = new RelayCommand<string>(RemoveTagFromSelectedNote);
            RenameSelectedNoteCommand = new RelayCommand(RenameSelectedNote);

            AllNotes = new ObservableCollection<Note>(NoteManager.AllNotes);
            FilteredNotesView = CollectionViewSource.GetDefaultView(AllNotes);
            FilteredNotesView.Filter = ShouldShowNotePredicate;

            AvailableTags = new ObservableCollection<string>(NoteManager.AvailableTags);
            FilteredAvailableTagsView = CollectionViewSource.GetDefaultView(AvailableTags);
            FilteredAvailableTagsView.Filter = ShouldShowTagPredicate;
            FilteredAvailableTagsForSelectedNoteView = CollectionViewSource.GetDefaultView(AvailableTags);
            FilteredAvailableTagsForSelectedNoteView.Filter = ShouldShowTagForSelectedNotePredicate;

            SearchTags.CollectionChanged += (sender, e) => FilteredNotesView.Refresh();

            _searchQueryDelayTimer.AutoReset = false;
            _searchQueryDelayTimer.Elapsed += OnSearchQueryTimerElapsed;
        }

        public RelayCommand AddSearchTagCommand { get; set; }
        public RelayCommand<string> RemoveSearchTagCommand { get; set; }
        public RelayCommand CreateNoteCommand { get; set; }
        public RelayCommand DeleteNoteCommand { get; set; }
        public RelayCommand SaveNoteChangesCommand { get; set; }
        public RelayCommand SaveAllNotesChangesCommand { get; set; }
        public RelayCommand OpenSettingsWindowCommand { get; set; }
        public RelayCommand AddTagCommand { get; set; }
        public RelayCommand<string> RemoveTagCommand { get; set; }
        public RelayCommand RenameSelectedNoteCommand { get; set; }

        private ObservableCollection<Note> AllNotes { get; set; }

        private ObservableCollection<string> AvailableTags { get; set; }

        /// <summary>
        /// Indicates if the application is started in debug mode.
        /// </summary>
        public bool IsDebugModeEnabled { get; set; } = SettingsManager.CurrentSettings.IsDebugModeEnabled;

        // todo: maybe save these so that search tags could persist between app restarts
        public ObservableCollection<string> SearchTags { get; set; } = [];

        public ICollectionView FilteredNotesView { get; set; }
        public ICollectionView FilteredAvailableTagsView { get; set; }
        public ICollectionView FilteredAvailableTagsForSelectedNoteView { get; set; }

        public string SelectedNoteNewTag
        {
            get => _selectedNoteNewTag;
            set
            {
                if (SetField(ref _selectedNoteNewTag, value))
                    FilteredAvailableTagsForSelectedNoteView.Refresh();
            }
        }

        // todo: need to sanitize the tags
        public string SearchTag
        {
            get => _searchTag;
            set
            {
                if (SetField(ref _searchTag, value))
                    FilteredAvailableTagsView.Refresh();
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                SetField(ref _searchQuery, value);
                _searchQueryDelayTimer.Restart();
            }
        }

        public Note? SelectedNote
        {
            get => _selectedNote;
            set
            {
                Note? previousNote = _selectedNote;

                if (SetField(ref _selectedNote, value))
                {
                    if (previousNote != null)
                        previousNote.DirtyChanged -= OnSelectedNoteDirtyChanged;

                    if (_selectedNote != null)
                        _selectedNote.DirtyChanged += OnSelectedNoteDirtyChanged;

                    RaisePropertyChanged(nameof(HasSelectedNote));
                    RaisePropertyChanged(nameof(IsSelectedNoteDirty));
                }
            }
        }

        public bool HasSelectedNote => SelectedNote != null;

        public bool IsSelectedNoteDirty => SelectedNote != null && SelectedNote.IsDirty;

        public bool IsAnyNoteDirty => _dirtyNotes.Count > 0;

        private void OnSelectedNoteDirtyChanged()
        {
            if (SelectedNote == null)
                return;

            if (SelectedNote.IsDirty)
            {
                _dirtyNotes.Add(SelectedNote);
            }
            else
            {
                _dirtyNotes.Remove(SelectedNote);
            }

            RaisePropertyChanged(nameof(IsSelectedNoteDirty));
            RaisePropertyChanged(nameof(IsAnyNoteDirty));
        }

        #region Searching

        private void AddSearchTag()
        {
            if (string.IsNullOrWhiteSpace(SearchTag) || SearchTags.Contains(SearchTag))
                return;

            SearchTags.Add(SearchTag);
            SearchTag = string.Empty;
        }

        private void RemoveSearchTag(string tag)
        {
            SearchTags.Remove(tag);
        }

        private void OnSearchQueryTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            FilteredNotesView.Refresh();
        }

        private bool ShouldShowNotePredicate(object obj)
        {
            if (obj is not Note note)
                return false;

            if (string.IsNullOrEmpty(SearchQuery) && SearchTags.Count == 0)
                return true;

            if (SearchTags.Count > 0)
            {
                // todo: improve the performance of this whenever the tags become classes and we have a graph
                foreach (var tag in SearchTags)
                {
                    if (note.Tags.Contains(tag))
                        return true;
                }
            }

            if (!string.IsNullOrEmpty(SearchQuery))
            {
                if (note.Content.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private bool ShouldShowTagPredicate(object obj)
        {
            if (string.IsNullOrWhiteSpace(SearchTag))
                return true;

            if (obj is not string tag)
                return false;

            if (string.IsNullOrWhiteSpace(tag))
                return true;

            return tag.Contains(SearchTag, StringComparison.OrdinalIgnoreCase);
        }

        private bool ShouldShowTagForSelectedNotePredicate(object obj)
        {
            if (obj is not string tag || SelectedNote == null)
                return false;

            if (string.IsNullOrWhiteSpace(tag))
                return true;

            if (SelectedNote.Tags.Contains(tag))
                return false;

            if (string.IsNullOrWhiteSpace(SelectedNoteNewTag))
                return true;

            return tag.Contains(SelectedNoteNewTag, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Note Manipulation

        private void CreateNote()
        {
            CreateNoteView window = new()
            {
                Owner = _window
            };

            bool? result = window.ShowDialog();
            if (result != true)
                return;

            Note? createdNote = window.Note;
            if (createdNote == null)
                return;

            createdNote.Save();

            AllNotes.Add(createdNote);

            // todo: how does this react to a note that is filtered by the FIlteredNotesView? maybe test and rework
            SelectedNote = createdNote;

            FilteredNotesView.Refresh();
        }

        private void AddTagToSelectedNote()
        {
            if (SelectedNote == null)
                return;

            SelectedNote.Tags.Add(SelectedNoteNewTag);
            SelectedNoteNewTag = string.Empty;
        }

        private void RemoveTagFromSelectedNote(string tag)
        {
            if (SelectedNote == null)
                return;

            SelectedNote.Tags.Remove(tag);
        }

        private void RenameSelectedNote()
        {
            if (SelectedNote == null)
                return;

            var renameNoteDialog = new CreateNoteView(true)
            {
                Owner = _window
            };
            renameNoteDialog.ShowDialog();

            if (renameNoteDialog.Note == null)
                return;

            NoteManager.ChangeNoteFileName(SelectedNote, renameNoteDialog.Note.FileName);
        }

        private void SaveNoteChanges()
        {
            if (SelectedNote == null || !SelectedNote.IsDirty)
                return;

            SelectedNote.Save();
        }

        private void SaveAllNotesChanges()
        {
            // todo: optimize by actually tracking all edited notes and stop itterating over them every single time.
            foreach (var note in _dirtyNotes)
            {
                if (!note.IsDirty)
                    continue;

                note.Save();
            }

            _dirtyNotes.Clear();
            RaisePropertyChanged(nameof(IsAnyNoteDirty));
        }

        private string _testTagBoxText = string.Empty;
        public string TestTagBoxText
        {
            get => _testTagBoxText;
            set
            {
                if (SetField(ref _testTagBoxText, value))
                {
                    IsPopupOpen = true;
                    RaisePropertyChanged(nameof(IsPopupOpen));
                }
            }
        }

        public bool IsPopupOpen { get; set; } = false;

        private void DeleteNote()
        {
            if (SelectedNote == null)
                return;

            // todo: replace this window with a custom one that fits the theme too and is potentially more generic and reusable
            string warningMessage =
                $"You are about to delete the following note:{Environment.NewLine}" +
                $"\"{SelectedNote.Name}\"{Environment.NewLine}{Environment.NewLine}" +
                "Are you sure you want to delete it?";

            MessageBoxResult dialogResults =
                MessageBox.Show(
                    warningMessage,
                    "Delete Selected Note?",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

            if (dialogResults != MessageBoxResult.Yes)
                return;

            NoteManager.DeleteNote(SelectedNote);
            AllNotes.Remove(SelectedNote);

            FilteredNotesView.Refresh();
        }

        #endregion

        private void OpenSettingsWindow()
        {
            SettingsView window = new()
            {
                Owner = _window
            };

            window.ShowDialog();
        }
    }
}
