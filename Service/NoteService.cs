using NotePaddle.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NotePaddle.Service
{
    public class NoteService
    {
        /// <summary>
        /// Retrieves a List of All Notes From the Repository ordered by last edit date
        /// </summary>
        /// <returns></returns>
        public List<Model.Note> GetAllNotes()
        {
            try
            {
                Repository.NoteRepository noteRepository = new Repository.NoteRepository();
                return noteRepository.GetAllNotes().OrderByDescending(i => i.LastEdited).ToList();
            }
            catch (Exception ex)
            {
                if (ex.Message == "SQLConnectionFail")
                {
                    throw new Exception("SQLConnectionFail");
                }
            }
            return null;
        }

        /// <summary>
        /// Deletes the given Model.Note
        /// </summary>
        /// <param name="note"></param>
        public void DeleteNote(Note note)
        {
            Repository.NoteRepository notesRepository = new Repository.NoteRepository();
            notesRepository.DeleteNote(note);
        }

        /// <summary>
        /// Saves the given Model.Note
        /// </summary>
        /// <param name="workingNote"></param>
        public void SaveNote(Note workingNote)
        {
            Repository.NoteRepository notesRepository = new Repository.NoteRepository();
            notesRepository.SaveNote(workingNote);
        }

        /// <summary>
        /// Takes the given List<Note> and filters it by title and content according to the string
        /// </summary>
        /// <param name="noteList"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public List<Note> FilterNotesByTitleAndContent(List<Note> noteList, string text)
        {
            var result = new List<Note>();

            result = noteList.Where(i => i.Title.ToLower().Contains(text.ToLower()) || i.Content.ToLower().Contains(text.ToLower())).ToList();

            return result;
        }
    }
}