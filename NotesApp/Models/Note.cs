using Newtonsoft.Json;
using NotesApp.Managers;
using Core.MVVM;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace NotesApp.Models
{
    // todo: possibly create an underlying data structure to use for serialization
    // and use this class for a model. this will make deserialization easier and will allow
    // for greater flexibility when deserializing tags. that, or use custom jsonconverters/jsonserializers/jsonwhatevers per property type
    [JsonObject(memberSerialization: MemberSerialization.OptIn)]
    public class Note : ViewModelBase
    {
        /// <summary>
        /// The constructor used by the Json serializer.
        /// </summary>
        [JsonConstructor]
        private Note()
        {
        }

        /// <summary>
        /// Constructor of the note class.
        /// </summary>
        /// <param name="fileName">The file name of the note.</param>
        public Note(string fileName)
        {
            FileName = fileName;
            Name = fileName;
            SetupNote();
        }

        private bool _isDirty = false;
        /// <summary>
        /// Indicator if the note has undergone changes without being saved.
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            set => SetField(ref _isDirty, value);
        }

        private string _fileName = string.Empty;
        /// <summary>
        /// The name of the note's file. This must not contain any of the file system's invalid characters.
        /// </summary>
        public string FileName
        {
            get => _fileName;
            set => SetField(ref _fileName, value);
        }

        [JsonProperty(nameof(Name))]
        private string _name = string.Empty;
        /// <summary>
        /// The name of the note. Can be set to whatever the user wants.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (SetField(ref _name, value))
                    IsDirty = true;
            }
        }

        [JsonProperty(nameof(Tags))]
        // todo: we might need to make tags a class. then maybe have a graph of tags.
        private ObservableCollection<string> _tags = [];
        /// <summary>
        /// Collection of tags which are used to mark the note with.
        /// </summary>
        public ObservableCollection<string> Tags
        {
            get => _tags;
            private set => SetField(ref _tags, value);
        }

        private string _content = string.Empty;
        [JsonProperty(nameof(Content))]
        /// <summary>
        /// The contents of the note.
        /// </summary>
        public string Content
        {
            get => _content;
            set
            {
                if (SetField(ref _content, value))
                    IsDirty = true;
            }
        }

        // todo: call this when a note has been changed. this is to implement an auto-save feature for notes.
        // todo: make sure it doesn't overwrite another note that has the same name in case of a note FileName change.
        /* todo: automating this is super dangerous! if you accidentally ctrl+a and backspace the note, you'll lose the note forever!
         * maybe backup notes or just let the user have manual saving.
         * maybe keep a few versions of the auto-saved files, but this could go out-of-hand real fast and it has to be done rarely. maybe on note change only.
         * we MUST have a undo/redo stack. maybe a simple back and forth would be enough, maybe would need a more complicated one with a visualization too. only stored in memory tho :/
        */
        public void Save()
        {
            NoteManager.SaveNote(this);
        }

        private void SetupNote()
        {
            Tags.CollectionChanged += OnTagsChanged;
        }

        private void OnTagsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            IsDirty = true;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext streamingContext)
        {
            SetupNote();
            IsDirty = false;
        }
    }
}
