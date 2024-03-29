using System.Diagnostics;
using System.IO;
using NotesApp.Models;

namespace NotesApp.Utils
{
    /// <summary>
    /// Class for helping with paths and extensions for the whole application.
    /// </summary>
    public static class PathHelper
    {
        private static readonly HashSet<char> _invalidCharacters = [];

        /// <summary>
        /// The file extension of notes.
        /// </summary>
        public const string NoteExtension = ".json";

        static PathHelper()
        {
            foreach (var @char in Path.GetInvalidPathChars())
                _invalidCharacters.Add(@char);

            foreach (var @char in Path.GetInvalidFileNameChars())
                _invalidCharacters.Add(@char);

            Directory.CreateDirectory(ToolSaveFolderPath);
            Directory.CreateDirectory(NotesPath);
        }

        /// <summary>
        /// The main folder of this tool. All of the files will be saved in here.
        /// </summary>
        public static string ToolSaveFolderPath =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ForkingNotes");

        /// <summary>
        /// The path where all notes will be saved.
        /// </summary>
        public static string NotesPath =>
            Path.Combine(ToolSaveFolderPath, "Notes");

        /// <summary>
        /// The file path of the settings file of this application.
        /// </summary>
        public static string SettingsFilePath =>
            Path.Combine(ToolSaveFolderPath, "Settings.json");

        /// <summary>
        /// Gets the file path of a note.
        /// </summary>
        /// <param name="note">The note for which a file path will be calculated.</param>
        /// <returns>Returns the full path, file name and extension of a note.</returns>
        public static string GetNoteFilePath(Note note)
        {
            return GetNoteFilePath(note.FileName);
        }

        public static string GetNoteFilePath(string fileName)
        {
            return Path.Combine(NotesPath, $"{fileName}{NoteExtension}");
        }

        public static bool IsFileNameValid(
            string fileName,
            out HashSet<char> invalidCharacters)
        {
            invalidCharacters = [];

            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            foreach (var @char in fileName)
            {
                if (_invalidCharacters.Contains(@char))
                    invalidCharacters.Add(@char);
            }

            if (invalidCharacters.Count > 0)
                return false;

            return true;
        }

        //public static string SanitizeFileName(string fileName)
        //{

        //}

        public static void OpenFolder(string folder)
        {
            Process.Start("explorer", folder);
        }
    }
}
