using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotesApp.Models;

namespace NotesApp.Utils
{
    /// <summary>
    /// Class for helping with paths and extensions for the whole application.
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// The file extension of notes.
        /// </summary>
        public const string NoteExtension = ".json";

        static PathHelper()
        {
            Directory.CreateDirectory(ToolCachePath);
            Directory.CreateDirectory(NotesPath);
        }

        /// <summary>
        /// The main folder of this tool. All of the files will be saved in here.
        /// </summary>
        public static string ToolCachePath =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ForkingNotes");

        /// <summary>
        /// The path where all notes will be saved.
        /// </summary>
        public static string NotesPath =>
            Path.Combine(ToolCachePath, "Notes");

        /// <summary>
        /// The file path of the settings file of this application.
        /// </summary>
        public static string SettingsFilePath =>
            Path.Combine(ToolCachePath, "Settings.json");

        /// <summary>
        /// Gets the file path of a note.
        /// </summary>
        /// <param name="note">The note for which a file path will be calculated.</param>
        /// <returns>Returns the full path, file name and extension of a note.</returns>
        public static string GetNoteFilePath(Note note)
        {
            return Path.Combine(NotesPath, $"{note.Name}{NoteExtension}");
        }
    }
}
