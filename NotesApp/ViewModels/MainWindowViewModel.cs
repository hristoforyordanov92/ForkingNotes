using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

        public MainWindowViewModel(Window window)
        {
            _window = window;

            CreateNoteCommand = new RelayCommand(CreateNote);
            DeleteNoteCommand = new RelayCommand(DeleteNote);
            SaveNoteChangesCommand = new RelayCommand(SaveNoteChanges);
            OpenSettingsWindowCommand = new RelayCommand(OpenSettingsWindow);
            AddSearchTagCommand = new RelayCommand(AddSearchTag);
            RemoveSearchTagCommand = new RelayCommand<object>(RemoveSearchTag);
            AddTagCommand = new RelayCommand(AddTag);
            RemoveTagCommand = new RelayCommand<object>(RemoveTag);
            RenameSelectedNoteCommand = new RelayCommand(RenameSelectedNote);

            AllNotes = new ObservableCollection<Note>(NoteManager.AllNotes);
            FilteredNotesView = CollectionViewSource.GetDefaultView(AllNotes);
            FilteredNotesView.Filter = ShouldShowNotePredicate;

            SearchTags.CollectionChanged += (sender, e) => FilteredNotesView.Refresh();

            _searchQueryDelayTimer.AutoReset = false;
            _searchQueryDelayTimer.Elapsed += OnTimerElapsed;
        }

        public RelayCommand AddSearchTagCommand { get; set; }
        public RelayCommand<object> RemoveSearchTagCommand { get; set; }
        public RelayCommand CreateNoteCommand { get; set; }
        public RelayCommand DeleteNoteCommand { get; set; }
        public RelayCommand SaveNoteChangesCommand { get; set; }
        public RelayCommand OpenSettingsWindowCommand { get; set; }
        public RelayCommand AddTagCommand { get; set; }
        public RelayCommand<object> RemoveTagCommand { get; set; }
        public RelayCommand RenameSelectedNoteCommand { get; set; }

        /// <summary>
        /// Indicates if the application is currently ran in debug mode.
        /// </summary>
        public bool IsDebugModeEnabled { get; set; } = SettingsManager.CurrentSettings.IsDebugModeEnabled;

        // todo: maybe save these so that search tags could persist between app restarts
        public ObservableCollection<string> SearchTags { get; set; } = [];

        private ObservableCollection<Note> AllNotes { get; set; }

        public ICollectionView FilteredNotesView { get; set; }

        private string _selectedNoteNewTag = string.Empty;
        public string SelectedNoteNewTag
        {
            get => _selectedNoteNewTag;
            set => SetField(ref _selectedNoteNewTag, value);
        }

        // todo: need to sanitize the tags
        private string _searchTag = string.Empty;
        public string SearchTag
        {
            get => _searchTag;
            set => SetField(ref _searchTag, value);
        }

        private string _searchQuery = string.Empty;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                SetField(ref _searchQuery, value);
                _searchQueryDelayTimer.Restart();
            }
        }

        private Note? _selectedNote;
        public Note? SelectedNote
        {
            get => _selectedNote;
            set => SetField(ref _selectedNote, value);
        }

        private void AddSearchTag()
        {
            if (string.IsNullOrWhiteSpace(SearchTag))
                return;

            SearchTags.Add(SearchTag);
            SearchTag = string.Empty;
        }

        private void RemoveSearchTag(object? parameter)
        {
            if (parameter is not string tag)
                return;

            SearchTags.Remove(tag);
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            FilteredNotesView.Refresh();
        }

        private void CreateNote()
        {
            var window = new CreateNoteView
            {
                Owner = _window
            };

            var res = window.ShowDialog();
            if (res != true)
                return;

            var note = window.Note;
            if (note == null)
                return;

            note.Save();

            AllNotes.Add(note);

            FilteredNotesView.Refresh();
        }

        private void DeleteNote()
        {
            if (SelectedNote == null)
                return;

            var note = SelectedNote;

            // todo: ask the user if they want to delete the note before deleting it :)

            AllNotes.Remove(note);
            NoteManager.DeleteNote(note);

            FilteredNotesView.Refresh();
        }

        private void SaveNoteChanges()
        {
            if (SelectedNote == null || !SelectedNote.IsDirty)
                return;

            SelectedNote.Save();
        }

        private void OpenSettingsWindow()
        {
            var window = new SettingsView
            {
                Owner = _window
            };

            window.ShowDialog();
        }

        private void AddTag()
        {
            if (SelectedNote == null)
                return;

            SelectedNote.Tags.Add(SelectedNoteNewTag);
            SelectedNoteNewTag = string.Empty;
        }

        private void RemoveTag(object? parameter)
        {
            if (SelectedNote == null || parameter is not string tag)
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
    }
}
