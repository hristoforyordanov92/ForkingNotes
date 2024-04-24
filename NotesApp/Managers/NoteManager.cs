using System.IO;
using Newtonsoft.Json;
using NotesApp.Models;
using NotesApp.Utils;

namespace NotesApp.Managers
{
    public static class NoteManager
    {
        // todo: maybe make the notes to be loaded slowly over time.
        // first enumerate them as files and only show their names.
        // then slowly in a background task load them.
        // if a user wants to open a note that hasn't been loaded - put it in high prio and load it first.
        // notes should be in a ConcurrentDictionary<notePath, note> or something like this.

        public static List<Note> AllNotes { get; set; } = [];

        public static List<string> AvailableTags { get; set; } = [];

        static NoteManager()
        {
            AllNotes = GetSavedNotes();
        }

        private static List<Note> GetSavedNotes()
        {
            List<Note> loadedNotes = [];

            IEnumerable<string> noteFiles = Directory.EnumerateFiles(
                PathHelper.NotesPath,
                $"*{PathHelper.NoteExtension}",
                SearchOption.AllDirectories);

            foreach (var noteFile in noteFiles)
            {
                string noteJson = File.ReadAllText(noteFile);
                Note? note = JsonConvert.DeserializeObject<Note>(noteJson);
                if (note == null)
                {
                    // log or error message
                    continue;
                }

                note.FileName = Path.GetFileNameWithoutExtension(noteFile);
                loadedNotes.Add(note);
                AvailableTags.AddRange(note.Tags);
            }

            return loadedNotes;
        }

        public static bool ChangeNoteFileName(Note note, string newFileName)
        {
            string oldNotePath = PathHelper.GetNoteFilePath(note);
            string oldNoteName = note.Name;
            string oldNoteFileName = note.FileName;

            // todo: Name must be handled properly. Currenly we rename both, but maybe we shouldn't.
            // Aliases should be allowed to be changed independently from file name.
            note.Name = newFileName;
            note.FileName = newFileName;
            if (!SaveNote(note) || !DeleteNote(oldNotePath))
            {
                note.Name = oldNoteName;
                note.FileName = oldNoteFileName;
                return false;
            }

            return true;
        }

        public static Note CreateNote(string fileName)
        {
            Note note = new(fileName);

            SaveNote(note);

            return note;
        }

        private static bool DeleteNote(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception)
            {
                // todo: log?
                return false;
            }

            return true;
        }

        public static void DeleteNote(Note note)
        {
            DeleteNote(PathHelper.GetNoteFilePath(note));
        }

        public static bool SaveNote(Note note)
        {
            try
            {
                string path = PathHelper.GetNoteFilePath(note);
                string noteJson = JsonConvert.SerializeObject(note, Formatting.Indented);
                File.WriteAllText(path, noteJson);
                note.IsDirty = false;
            }
            catch (Exception)
            {
                // todo: log?
                return false;
            }

            return true;
        }

        public static bool NoteFileExists(string fileName)
        {
            string noteFilePath = PathHelper.GetNoteFilePath(fileName);
            return File.Exists(noteFilePath);
        }

        public static bool NoteFileExists(Note note)
        {
            return NoteFileExists(note.FileName);
        }
    }
}
