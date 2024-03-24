using Newtonsoft.Json;
using NotesApp.Managers;
using Core.MVVM;
using System.ComponentModel;
using System.Collections;

namespace NotesApp.Models
{
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

        private string _name = string.Empty;
        /// <summary>
        /// The name of the note. Can be set to whatever the user wants.
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        // todo: we might need to make tags a class. then maybe have a graph of tags.
        private List<string> _tags = [];
        /// <summary>
        /// Collection of tags which are used to mark the note with.
        /// </summary>
        public List<string> Tags
        {
            get => _tags;
            set => SetField(ref _tags, value);
        }

        private string _content = string.Empty;

        /// <summary>
        /// The contents of the note.
        /// </summary>
        public string Content
        {
            get => _content;
            set => SetField(ref _content, value);
        }

        // todo: call this when a note has been changed.
        // todo: make sure it doesn't overwrite another note that has the same name in case of a note FileName change.
        /* todo: automating this is super dangerous! if you accidentally ctrl+a and backspace the note, you'll lose the note forever!
         * maybe backup notes or just let the user have manual saving.
         * maybe keep a few versions of the auto-saved files, but this could go out-of-hand real fast and it has to be done rarely. maybe on note change only.
         * we MUST have a undo/redo stack. maybe a simple back and forth would be enough, maybe would need a more complicated one with a visualization too. only stored in memory tho :/
        */
        private void SaveNote()
        {
            NoteManager.SaveNote(this);
        }
    }
}
