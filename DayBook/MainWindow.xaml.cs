using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Linq;
using System;

namespace DayBook
{
    public partial class MainWindow : Window
    {       
        private Dictionary<DateTime, List<Note>> notes = new Dictionary<DateTime, List<Note>>();
        public MainWindow()
        {
            InitializeComponent();
            DatePicker.Text = DateTime.Now.ToString();      
        }
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = DatePicker.SelectedDate ?? DateTime.Today;
            string title = Title.Text;
            string contents = Contents.Text;

            Note note = new Note { Title = title, Contents = contents };

            if (!notes.ContainsKey(selectedDate))
            {
                notes[selectedDate] = new List<Note>();
            }           
            notes[selectedDate].Add(note);
            NoteListBox.Items.Add(title);         
        }
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {          
            DateTime selectedDate = DatePicker.SelectedDate ?? DateTime.Today;
            NoteListBox.Items.Clear();

            if (notes.ContainsKey(selectedDate))
            {
                foreach (var note in notes[selectedDate])
                {
                    NoteListBox.Items.Add(note.Title);
                }
            }           
        }
        private void NoteListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedTitle = (string)NoteListBox.SelectedItem;
            DateTime selectedDate = DatePicker.SelectedDate.Value.Date;

            if (notes.ContainsKey(selectedDate))
            {
                foreach (var note in notes[selectedDate])
                {
                    if (note.Title == selectedTitle)
                    {
                        Title.Text = note.Title;
                        Contents.Text = note.Contents;
                        break;
                    }
                }
            }
            
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string selectedTitle = (string)NoteListBox.SelectedItem;
            DateTime selectedDate = DatePicker.SelectedDate.Value.Date;

            if (notes.ContainsKey(selectedDate))
            {
                var noteToSave = notes[selectedDate].FirstOrDefault(note => note.Title == selectedTitle);
                if (noteToSave != null)
                {
                    noteToSave.Date = selectedDate;
                    noteToSave.Title = Title.Text;
                    noteToSave.Contents = Contents.Text;
                    NoteListBox.Items[NoteListBox.SelectedIndex] = noteToSave.Title;
                }                
            }
            else MessageBox.Show("Saving can only be applied to an existing note");
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            string selectedTitle = (string)NoteListBox.SelectedItem;
            DateTime selectedDate = DatePicker.SelectedDate.Value.Date;
            UpdateNoteListBox();
            if (notes.ContainsKey(selectedDate))
            {
                var notesToRemove = notes[selectedDate].Where(note => note.Title == selectedTitle).ToList();
                foreach (var noteToRemove in notesToRemove)
                {
                    notes[selectedDate].Remove(noteToRemove);
                    NoteListBox.Items.Remove(selectedTitle);
                }
                if (notes[selectedDate].Count == 0)
                {
                    notes.Remove(selectedDate);
                }
                
            }
            else MessageBox.Show("Deletions can only be applied to an existing note");
            
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DeSerializer serializer = new DeSerializer();
            serializer.Serialize(notes, "DayBook.json");
        }
        private void UpdateNoteListBox()
        {
            DateTime currentDate = DateTime.Now.Date; 
            List<Note> notesForCurrentDate = notes.ContainsKey(currentDate) ? notes[currentDate] : new List<Note>();
   
            NoteListBox.Items.Clear();
            foreach (var note in notesForCurrentDate)
            {
                NoteListBox.Items.Add(note.Title);
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DeSerializer serializer = new DeSerializer();
            notes = serializer.Deserialize<Dictionary<DateTime, List<Note>>>("DayBook.json") ?? new Dictionary<DateTime, List<Note>>();
/*
            if (noteList != null)
            {
                notes.Clear();
                foreach (var note in noteList)
                {
                    DateTime date = note.Key.Date;
                    List<Note> notesForDate = note.Value;
                    if (!notes.ContainsKey(date))
                    {
                        notes[date] = new List<Note>();
                    }
                    notes[date].AddRange(notesForDate);
                }               
            }*/
           UpdateNoteListBox();
        }
    }
}