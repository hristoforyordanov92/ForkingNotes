using Newtonsoft.Json;
using WPFBasics;

namespace NotesApp.Models
{
    public class Note : ViewModelBase
    {
        /// <summary>
        /// The constructor used by the Json serialized.
        /// </summary>
        [JsonConstructor]
        private Note()
        {
        }

        /// <summary>
        /// Constructor of the note class.
        /// </summary>
        /// <param name="name">The name of the note. Should already be sanitized from illegal path/file name characters!</param>
        /// <param name="tags">The tags to associate the note with.</param>
        /// <param name="content">The contents of the note.</param>
        public Note(string name, List<string>? tags = null, string content = "")
        {
            Name = name;
            Tags = tags ?? [];
            Content = content;
        }

        /// <summary>
        /// The name of the note, which must also comply with the file system naming rules.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        // todo: maybe create a CustomName property to allow for replacing the Name property in the UI, but keep using Name as a file path.

        // todo: we might need to make tags a class. then maybe have a graph of tags.
        /// <summary>
        /// Collection of tags which are used to mark the note with.
        /// </summary>
        public List<string> Tags { get; set; } = [];

        /// <summary>
        /// The contents of the note.
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }
}
