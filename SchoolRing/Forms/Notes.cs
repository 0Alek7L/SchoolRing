using OfficeOpenXml;
using SchoolRing.Interfaces;
using SchoolRing.IO;
using SchoolRing.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolRing.Forms
{
    public partial class Notes : Form
    {
        Timer timer;
        DateTime selectedDate;
        string selectedDayOfWeek = string.Empty;
        List<ISchoolClass> classes = new List<ISchoolClass>();
        ISchoolClass selectedClass = null;
        public Notes()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 50;
            timer.Tick += Timer_Tick;
            timer.Start();
            Program.ShowTheCurrentIcon(pictureBox3);
            CheckTheDate(monthCalendar1.SelectionStart);
            FillTheListBox();
            selectedDate= DateTime.Today;
            //foreach (var item in Program.noteRepo.GetModels())
            //{
            //    MessageBox.Show(item.Date.ToString() + " " + selectedDate.ToString());
            //}
            MessageBox.Show("В момента запаметяването на записките не работи");
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeForClockAndText time = new TimeForClockAndText();
            labelDayOfWeek.Text = time.PrintDay();
            Program.ShowTheCurrentIcon(pictureBox3);
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Program.LastForms.Pop().Show();
            SaveTheData.SaveNotes();
            this.Close();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            Help help = new Help();
            help.Show();
            this.Hide();
        }

        private void dEFAULTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.ChangeCustomIcon(pictureBox3, false);
            Program.customIconPath = null;
        }

        private void cUSTOMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.ChoosePathForCustomIcon(contextMenuStrip1);
            Program.ChangeCustomIcon(pictureBox3, true);
        }

        private void buttonWriteNewNote_MouseEnter(object sender, EventArgs e) =>
             buttonWriteNewNote.FlatStyle = FlatStyle.Flat;

        private void buttonWriteNewNote_MouseLeave(object sender, EventArgs e) =>
            buttonWriteNewNote.FlatStyle = FlatStyle.Popup;
        private void FillTheListBox()
        {
            switch (monthCalendar1.SelectionStart.DayOfWeek)
            {
                case DayOfWeek.Monday: selectedDayOfWeek = "ПОНЕДЕЛНИК"; break;
                case DayOfWeek.Tuesday: selectedDayOfWeek = "ВТОРНИК"; break;
                case DayOfWeek.Wednesday: selectedDayOfWeek = "СРЯДА"; break;
                case DayOfWeek.Thursday: selectedDayOfWeek = "ЧЕТВЪРТЪК"; break;
                case DayOfWeek.Friday: selectedDayOfWeek = "ПЕТЪК"; break;
            }
            classes = Program.GetModels()
                //.Where(sc => sc.IsPurvaSmqna || !sc.IsPurvaSmqna)
                .Where(sc => sc.Day == selectedDayOfWeek)
                .OrderBy(sc => !sc.IsPurvaSmqna)
                .ThenBy(sc => sc.Num)
                .ToList();
            listBoxClasses.Items.Clear();
            foreach (var c in classes)
            {
                listBoxClasses.Items.Add(c.ShowTheRecord());
                listBoxClasses.SelectedIndex = 0;
            }
        }
        private void CheckTheDate(DateTime date)
        {
            if (Program.vdRepo.IsThisDayVacation(date) || date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                listBoxClasses.Hide();
                labelClass.Hide();
                buttonWriteNewNote.Hide();
                listBoxClasses.SelectedIndex = -1;
            }
            else
            {
                if (listBoxClasses.Items.Count > 0)
                    listBoxClasses.SelectedIndex = 0;
                listBoxClasses.Show();
                labelClass.Show();
            }
            selectedDate = date;
        }

        private void listBoxClasses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxClasses.SelectedIndex > -1)
            {
                if (!buttonWriteNewNote.Visible)
                {
                    buttonWriteNewNote.Show();
                }
                selectedClass = classes[listBoxClasses.SelectedIndex];
            }
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            CheckTheDate(monthCalendar1.SelectionStart);
            selectedDate = monthCalendar1.SelectionStart;
            if (listBoxClasses.Visible)
            {
                FillTheListBox();
            }
        }

        private void buttonWriteNewNote_Click(object sender, EventArgs e)
        {
            panelWrite.Show();
            labelClassName.Text = $"{selectedDayOfWeek} {selectedDate.ToShortDateString()} {selectedClass.ShowTheRecord()}";
            if (Program.noteRepo.IsThereAModel(selectedDate, selectedClass.Num, selectedClass.IsPurvaSmqna))
            {
                pictureBoxDeleteNote.Show();
                textBoxNote.Text = Program.noteRepo
                    .FirstModel(selectedDate, selectedClass.Num, selectedClass.IsPurvaSmqna).Text;
            }
            else
            {
                pictureBoxDeleteNote.Hide();
            }
            //foreach (var item in Program.noteRepo.GetModels())
            //{
            //    MessageBox.Show(item.Date.ToString() + " " + selectedDate.ToString());
            //}
        }

        private void pictureBoxCloseMenu_Click(object sender, EventArgs e)
        {
            panelWrite.Hide();
            textBoxNote.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && textBox1.Text.ToCharArray().All(x => char.IsDigit(x)))
            {
                if (float.Parse(textBox1.Text) > 10 && float.Parse(textBox1.Text) <= 99)
                {
                    FontFamily fontFamily = new FontFamily("Jura");
                    textBoxNote.Font = new Font(fontFamily, float.Parse(textBox1.Text));
                }
            }
        }

        private void textBoxNote_TextChanged(object sender, EventArgs e)
        {
            textBox1.Text = textBoxNote.Font.Size.ToString();
        }

        private void pictureBoxDeleteNote_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox
                .Show("Сигурни ли сте, че искате да изтриете записките за този час?", "Предупреждение", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                panelWrite.Hide();

                Program.noteRepo.RemoveModel(Program.noteRepo
                    .FirstModel(selectedDate, selectedClass.Num, selectedClass.IsPurvaSmqna));

                MessageBox.Show("Успешно премахнахте записките за този час!");
                textBoxNote.Text = "";
            }
        }

        private void pictureBoxSaveNote_Click(object sender, EventArgs e)
        {
            INote note = new Note(textBoxNote.Text, selectedDate, DateTime.Now, selectedClass.Num, selectedClass.IsPurvaSmqna);
            if (!Program.noteRepo.IsThereAModel(selectedDate, selectedClass.Num, selectedClass.IsPurvaSmqna))
            {
                Program.noteRepo.AddModel(note);
            }
            else
            {
                Program.noteRepo.UpdateModel(note);
            }
            MessageBox.Show("Записът е успешен!");
            textBoxNote.Text = "";
            panelWrite.Hide();
        }
    }
}
