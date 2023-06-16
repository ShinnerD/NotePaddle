using NotePaddle.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NotePaddle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Model.Note> NoteList { get; set; }
        public List<Model.Note> FilteredNoteList { get; set; }
        public Model.Note WorkingNote { get; set; }

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                DataContext = this;
                ViewNotesButton.IsChecked = true;
                RefreshNoteList();
            }
            catch (Exception ex)
            {
                if (ex.Message == "SQLConnectionFail")
                {
                    ErrorMode("Access Denied, Request IP to be allowed.");
                }
                else
                {
                    ErrorMode("Something went wrong..");
                }
            }
        }

        /// <summary>
        /// Sets the Application main window into error mode,
        /// hides components and displays the given string as en error message.
        /// </summary>
        /// <param name="v"></param>
        private void ErrorMode(string v)
        {
            ViewNotesButton.Visibility = Visibility.Collapsed;
            EditPanel.Visibility = Visibility.Collapsed;
            NoteListViewPanel.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Collapsed;
            NoteListViewPanel.Visibility = Visibility.Collapsed;
            NewButton.Visibility = Visibility.Collapsed;
            SearchBoxContainer.Visibility = Visibility.Collapsed;

            ErrorMessageBox.Text = v;
            ErrorMessageBox.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Refreshes the NoteList from the service and resets Filtered Notes
        /// </summary>
        private void RefreshNoteList()
        {
            NoteList = new Service.NoteService().GetAllNotes().OrderByDescending(i => i.LastEdited).ToList();
            FilteredNoteList = NoteList;
            NoteListBox.ItemsSource = FilteredNoteList;
        }

        /// <summary>
        /// Sets the UI to View List Mode and Refreshes the NoteList from the DB
        /// </summary>
        private void ViewMode()
        {
            ViewNotesButton.IsChecked = true;
            EditPanel.Visibility = Visibility.Collapsed;
            NoteListViewPanel.Visibility = Visibility.Visible;
            SaveButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Visible;
            RefreshNoteList();
        }

        /// <summary>
        /// Creates a New Note and Switches to the Edit View if the paramerter is empty
        /// </summary>
        /// <param name="editedNote">When given a Model.Note sets that Note as the note being edited</param>
        private void EditMode(Model.Note editedNote = null)
        {
            EditPanel.Visibility = Visibility.Visible;
            NoteListViewPanel.Visibility = Visibility.Collapsed;
            SaveButton.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Collapsed;
            ViewNotesButton.IsChecked = false;

            if (editedNote != null)
            {
                WorkingNote = editedNote;
                titleInputBox.Text = editedNote.Title;
                contentInputBox.Text = editedNote.Content;
            }
            else
            {
                WorkingNote = new Model.Note();
                WorkingNote.CreatedDate = DateTime.Now;
                WorkingNote.LastEdited = DateTime.Now;
                titleInputBox.Text = WorkingNote.Title;
                contentInputBox.Text = WorkingNote.Content;
            }
        }

        /// <summary>
        /// Saves the Note Currently Being Edited.
        /// </summary>
        private void SaveCurrentNote()
        {
            if (titleInputBox.Text == string.Empty || titleInputBox.Text == "Please provide a title..")
            {
                titleInputBox.Text = "Please provide a title..";
            }
            else
            {
                WorkingNote.Title = titleInputBox.Text;
                WorkingNote.Content = contentInputBox.Text;
                Service.NoteService noteService = new Service.NoteService();
                noteService.SaveNote(WorkingNote);
                ViewMode();
            }
        }

        /// <summary>
        /// Deletes the Model.Note given as a parameter by calling the Noteservice
        /// </summary>
        /// <param name="note"></param>
        private void DeleteNote(Note note)
        {
            Service.NoteService service = new Service.NoteService();
            service.DeleteNote(note);
        }

        /// <summary>
        /// Sets the FilteredNoteList property to a filtered list from the NoteService based on string parameter
        /// </summary>
        /// <param name="text">filter parameter</param>
        private void PerformSearch(string text)
        {
            FilteredNoteList = new Service.NoteService().FilterNotesByTitleAndContent(NoteList, text).OrderByDescending(i => i.LastEdited).ToList();
        }

        //Calls a search on the notelist, or resets the FilteredNoteList if SearchBox is empty
        private void SearchBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (SearchBox.Text != string.Empty)
            {
                PerformSearch(SearchBox.Text);
                NoteListBox.ItemsSource = FilteredNoteList;
            }
            else
            {
                FilteredNoteList = NoteList;
                NoteListBox.ItemsSource = FilteredNoteList;
            }
        }

        //Drag the window when holding down the mouse
        private void Main_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        //Minimize button
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        //Exit Button
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //Set a Selected Note for cosmetics
        private void NoteListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NoteListBox.SelectedIndex == -1)
            {
                NoteListBox.SelectedIndex = 0;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteNote(NoteListBox.SelectedItem as Model.Note);
            ViewMode();
        }

        private void ViewNotesButton_Checked(object sender, RoutedEventArgs e)
        {
            ViewMode();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveCurrentNote();
        }

        private void NewButton_Checked(object sender, RoutedEventArgs e)
        {
            EditMode();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                EditMode(NoteListBox.SelectedItem as Model.Note);
            }
        }
    }
}