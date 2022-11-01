using NotePaddle.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace NotePaddle.Repository
{
    public class NoteRepository
    {
        /// <summary>
        /// Retrieves a complete List of All Notes from the Database.
        /// </summary>
        /// <returns></returns>
        public List<Model.Note> GetAllNotes()
        {
            try
            {
            Database.NotePaddleDBDataContext dataContext = new Database.NotePaddleDBDataContext();

            var result = new List<Model.Note>();

            foreach ( var dbNote in dataContext.NotePaddles ) 
            {
                var note = new Model.Note();

                note.Id = dbNote.Id;
                note.Title = dbNote.Title;
                note.Content = dbNote.Content;
                note.CreatedDate = dbNote.CreatedDate;
                note.LastEdited = dbNote.LastEdited;

                result.Add( note );
            }
            return result;
            }
            catch (SqlException)
            {
                throw new Exception("SQLConnectionFail");
            }
        }

        /// <summary>
        /// Deletes the Given Model.Note from the Database
        /// </summary>
        /// <param name="note"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void DeleteNote(Note note)
        {
            if ( note != null )
            {
                Database.NotePaddleDBDataContext dataContext = new Database.NotePaddleDBDataContext();

                var targetNote = dataContext.NotePaddles.FirstOrDefault(i => i.Id == note.Id);

                if (targetNote != null)
                {
                    dataContext.NotePaddles.DeleteOnSubmit(targetNote);
                    dataContext.SubmitChanges();
                }
            }
            else
            {
                throw new ArgumentNullException( nameof( note ) );
            }
        }

        /// <summary>
        /// Saves the given Model.Note to the Database
        /// </summary>
        /// <param name="workingNote"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void SaveNote(Note workingNote)
        {
            if (workingNote != null)
            {
                Database.NotePaddleDBDataContext dataContext = new Database.NotePaddleDBDataContext();

                var noteTable = dataContext.NotePaddles;

                var dbContextNote = new Database.NotePaddle();

                dbContextNote.Id = workingNote.Id;
                dbContextNote.Title = workingNote.Title;
                dbContextNote.Content = workingNote.Content;
                dbContextNote.CreatedDate = workingNote.CreatedDate;
                dbContextNote.LastEdited = workingNote.LastEdited;

                if (!noteTable.Contains(dbContextNote))
                {
                    noteTable.InsertOnSubmit(dbContextNote);
                    dataContext.SubmitChanges();
                }
                else
                {
                    var dbItem = noteTable.Where(i => i.Id == dbContextNote.Id).First();
                    dbItem.Content = dbContextNote.Content;
                    dbItem.LastEdited = dbContextNote.LastEdited;
                    dbItem.Title = dbContextNote.Title;
                    dataContext.SubmitChanges();
                }
            }
            else
            {
                throw new ArgumentNullException( nameof( workingNote ) );
            }
        }
    }
}
