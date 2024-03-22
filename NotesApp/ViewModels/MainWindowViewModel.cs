using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using NotesApp.Managers;
using NotesApp.Models;
using WPFBasics;
using Core;
using Timer = System.Timers.Timer;
using NotesApp.Views;

namespace NotesApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Timer used for delaying each search query to avoid constant filtering and searching
        /// while the user still haven't finished typing their search query.
        /// </summary>
        private readonly Timer _searchQueryDelayTimer = new(300d);

        public MainWindowViewModel()
        {
            SearchNotesCommand = new RelayCommand(SearchNotes);
            CreateNoteCommand = new RelayCommand(CreateNote);

            AllNotes = NoteManager.AllNotes;
            FilteredNotes = AllNotes;

            _searchQueryDelayTimer.AutoReset = false;
            _searchQueryDelayTimer.Elapsed += OnTimerElapsed;
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

        private List<Note> AllNotes { get; set; }

        private IEnumerable<Note> _filteredNotes = [];
        public IEnumerable<Note> FilteredNotes
        {
            get => _filteredNotes;
            set => SetField(ref _filteredNotes, value);
        }

        public RelayCommand SearchNotesCommand { get; set; }
        public RelayCommand CreateNoteCommand { get; set; }

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
            var window = new CreateNoteView();
            var res = window.ShowDialog();
            if (res != true)
                return;

            var note = window.Note;
            if (note == null)
                return;

            NoteManager.SaveNote(note);

            AllNotes = NoteManager.AllNotes;
            SearchNotes();
        }
    }
}
