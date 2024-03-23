using System.Timers;
using NotesApp.Managers;
using NotesApp.Models;
using Core;
using Core.MVVM;
using Timer = System.Timers.Timer;
using NotesApp.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace NotesApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Timer used for delaying each search query to avoid constant filtering and searching
        /// while the user still haven't finished typing their search query.
        /// </summary>
        private readonly Timer _searchQueryDelayTimer = new(300d);

        private readonly Window _window;

        public MainWindowViewModel(Window window)
        {
            _window = window;

            SearchTags.CollectionChanged += (sender, e) => FilterNotes();

            SearchNotesCommand = new RelayCommand(SearchNotes);
            CreateNoteCommand = new RelayCommand(CreateNote);
            OpenSettingsWindowCommand = new RelayCommand(OpenSettingsWindow);
            AddSearchTagCommand = new RelayCommand(AddSearchTag);
            RemoveSearchTagCommand = new RelayCommand<object>(RemoveSearchTag);

            AllNotes = new ObservableCollection<Note>(NoteManager.AllNotes);
            FilteredNotes = AllNotes;

            _searchQueryDelayTimer.AutoReset = false;
            _searchQueryDelayTimer.Elapsed += OnTimerElapsed;
        }

        public ObservableCollection<string> SearchTags { get; set; } = [];

        // todo: need to sanitize the tags
        private string _tag = string.Empty;
        public string Tag
        {
            get => _tag;
            set => SetField(ref _tag, value);
        }

        public RelayCommand AddSearchTagCommand { get; set; }
        public RelayCommand<object> RemoveSearchTagCommand { get; set; }

        private void AddSearchTag()
        {
            if (string.IsNullOrWhiteSpace(Tag))
                return;

            SearchTags.Add(Tag);
            Tag = string.Empty;
        }

        private void RemoveSearchTag(object? parameter)
        {
            if (parameter is not string tag)
                return;

            SearchTags.Remove(tag);
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

        private ObservableCollection<Note> AllNotes { get; set; }

        private IEnumerable<Note> _filteredNotes = [];
        public IEnumerable<Note> FilteredNotes
        {
            get => _filteredNotes;
            set => SetField(ref _filteredNotes, value);
        }

        public RelayCommand SearchNotesCommand { get; set; }
        public RelayCommand CreateNoteCommand { get; set; }
        public RelayCommand OpenSettingsWindowCommand { get; set; }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            SearchNotes();
        }

        public enum Keyword
        {
            Tags,
            Content,
            File
        }

        private void FilterNotes()
        {
            if (SearchTags.Count == 0)
            {
                FilteredNotes = AllNotes;
                return;
            }

            FilteredNotes = AllNotes
                .Where(note =>
                {
                    foreach (var tag in SearchTags)
                    {
                        if (note.Tags.Contains(tag))
                            return true;
                    }

                    return false;
                });
        }

        private void SearchNotes()
        {
            // todo: need to implement the parsing of the SearchQuery
            //var matches = Regex.Matches(SearchQuery, @"(?<KEYWORD>[a-z]+:)", RegexOptions.IgnoreCase);

            //Dictionary<Keyword, int> kvp = [];
            //foreach (Match match in matches)
            //{
            //    string keywordName = match.Value.ToString()[..^1];
            //    keywordName = $"{keywordName[..1].ToUpper()}{keywordName[1..].ToLower()}";
            //    if (!Enum.TryParse(keywordName, out Keyword keyword))
            //        continue;

            //    //Keyword keyword = (Keyword)keywordObject;
            //    kvp.Add(keyword, match.Index);
            //}



            //List<(Keyword keyword, int index)> keywordIndexes = [];
            //foreach (var keyword in Enum.GetValues<Keyword>())
            //{
            //    var index = SearchQuery.IndexOf($"{keyword}:", StringComparison.OrdinalIgnoreCase);
            //    if (index == -1)
            //        continue;

            //    keywordIndexes.Add((keyword, index));
            //}
            //keywordIndexes = [.. keywordIndexes.OrderBy(tuple => tuple.index)];

            //foreach (var keyword in keywordIndexes)
            //{

            //}

            //var tags = Regex.Match(SearchQuery, @"tag:(?<TAGS>([a-zA-Z0-9]+))");

            // todo: this is too easy and only supports tags. replace with a proper parsing of the SearchQuery
            var tags = SearchQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (tags.Length == 0)
            {
                FilteredNotes = AllNotes;
                return;
            }

            FilteredNotes = AllNotes
                .Where(note =>
                {
                    foreach (var tag in tags)
                    {
                        if (note.Tags.Contains(tag))
                            return true;
                    }

                    return false;
                });
        }

        private void CreateNote()
        {
            // the logic for the create note command
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

            NoteManager.SaveNote(note);

            AllNotes.Add(note);

            SearchNotes();
        }

        private void OpenSettingsWindow()
        {
            var window = new SettingsView
            {
                Owner = _window
            };

            window.ShowDialog();
        }
    }
}
