﻿using System.IO;
using System.Text.Json.Serialization;
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

        public static List<Note> AllNotes { get; set; }

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

                loadedNotes.Add(note);
            }

            return loadedNotes;
        }

        public static Note CreateNote(string name)
        {
            Note note = new(name);

            SaveNote(note);

            return note;
        }

        public static void SaveNote(Note note)
        {
            var path = PathHelper.GetNoteFilePath(note);
            var noteJson = JsonConvert.SerializeObject(note);
            File.WriteAllText(path, noteJson);
        }
    }
}