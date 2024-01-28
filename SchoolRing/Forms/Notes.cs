using OfficeOpenXml;
using SchoolRing.Interfaces;
using SchoolRing.IO;
using SchoolRing.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
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
        INote[] notes;
        INote selectedNote = null;
        bool searchMode = false;

        public Notes()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 200;
            timer.Tick += Timer_Tick;
            timer.Start();
            Program.ShowTheCurrentIcon(pictureBox3);
            CheckTheDate(monthCalendar1.SelectionStart);
            FillTheListBox();
            selectedDate = DateTime.Today;
            panelWrite.Parent = this;
            textBox1.Text = Program.textSizeForNotes.ToString();
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
            this.Dispose();
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
        private void buttonSearch_MouseEnter(object sender, EventArgs e) =>
             buttonSearch.FlatStyle = FlatStyle.Flat;

        private void buttonSearch_MouseLeave(object sender, EventArgs e) =>
             buttonSearch.FlatStyle = FlatStyle.Popup;
        private void UpdateDayOfWeek()
        {
            switch (monthCalendar1.SelectionStart.DayOfWeek)
            {
                case DayOfWeek.Sunday: selectedDayOfWeek = $"{TimeForClockAndText.dayOfWeekSunday}"; break;
                case DayOfWeek.Monday: selectedDayOfWeek = $"{TimeForClockAndText.dayOfWeekMonday}"; break;
                case DayOfWeek.Tuesday: selectedDayOfWeek = $"{TimeForClockAndText.dayOfWeekTuesday}"; break;
                case DayOfWeek.Wednesday: selectedDayOfWeek = $"{TimeForClockAndText.dayOfWeekWednesday}"; break;
                case DayOfWeek.Thursday: selectedDayOfWeek = $"{TimeForClockAndText.dayOfWeekThursday}"; break;
                case DayOfWeek.Friday: selectedDayOfWeek = $"{TimeForClockAndText.dayOfWeekFriday}"; break;
                case DayOfWeek.Saturday: selectedDayOfWeek = $"{TimeForClockAndText.dayOfWeekSaturday}"; break;
            }
        }
        private void FillTheListBox()
        {
            UpdateDayOfWeek();
            UpdateModels();
        }

        private void UpdateModels()
        {
            classes = Program.GetModels()
                .Where(sc => sc.Day == selectedDayOfWeek)
                .OrderBy(sc => !sc.IsPurvaSmqna)
                .ThenBy(sc => sc.Num)
                .ToList();
            listBoxClasses.Items.Clear();
            foreach (var c in classes)
            {
                if (Program.noteRepo.IsThereAModel(selectedDate, c.Num, c.IsPurvaSmqna))
                    listBoxClasses.Items.Add("✏️" + c.ShowTheRecord());
                else
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
                if (Program.noteRepo.IsThereAModel(selectedDate, selectedClass.Num, selectedClass.IsPurvaSmqna))
                {
                    buttonWriteNewNote.Text = "->РЕДАКТИРАНЕ<-";
                }
                else
                {
                    buttonWriteNewNote.Text = "->ПИСАНЕ<-";
                }
                UpdateLabels();

            }
        }

        private void UpdateLabels()
        {
            try
            {
                labelClassName.Text = $"{selectedDayOfWeek} {selectedDate.ToString("dd/MM/yyyy")} {selectedClass.ShowTheRecord()}";
                labelShowFirstDateAndTime.Text = $"{selectedDayOfWeek} {selectedDate.ToString("dd/MM/yyyy")}";
            }
            catch
            {
                //Sth
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
            UpdateDayOfWeek();
            UpdateLabels();
        }

        private void buttonWriteNewNote_Click(object sender, EventArgs e)
        {
            panelWrite.Show();
            //labelClassName.Text = $"{selectedDayOfWeek} {selectedDate.ToShortDateString()} {selectedClass.ShowTheRecord()}";
            if (Program.noteRepo.IsThereAModel(selectedDate, selectedClass.Num, selectedClass.IsPurvaSmqna))
            {
                textBoxNote.Text = Program.noteRepo
                    .FirstModel(selectedDate, selectedClass.Num, selectedClass.IsPurvaSmqna).Text;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && textBox1.Text.ToCharArray().All(x => char.IsDigit(x)))
            {
                if (float.Parse(textBox1.Text) > 10 && float.Parse(textBox1.Text) <= 99)
                {
                    FontFamily fontFamily = new FontFamily("Jura");
                    textBoxNote.Font = new Font(fontFamily, float.Parse(textBox1.Text));
                    Program.textSizeForNotes = int.Parse(textBox1.Text);
                }
            }
        }

        private void textBoxNote_TextChanged(object sender, EventArgs e)
        {
            textBox1.Text = textBoxNote.Font.Size.ToString();
        }

        private void pictureBoxDeleteNote_Click(object sender, EventArgs e)
        {
            if (!Program.noteRepo.IsThereAModel(selectedDate, selectedClass.Num, selectedClass.IsPurvaSmqna) && !searchMode)
            {
                INote note = new Note(textBoxNote.Text, selectedDate, DateTime.Now, selectedClass.Num, selectedClass.IsPurvaSmqna);
                Program.noteRepo.AddModel(note);
                Program.noteRepo.RemoveModel(Program.noteRepo
                    .FirstModel(selectedDate, selectedClass.Num, selectedClass.IsPurvaSmqna));
            }
            else
            {
                DialogResult dialog = MessageBox
                    .Show("Сигурни ли сте, че искате да изтриете записките за този час?", "Предупреждение", MessageBoxButtons.YesNo);
                if (dialog == DialogResult.Yes)
                {
                    Program.noteRepo.RemoveModel(Program.noteRepo
                        .FirstModel(selectedDate, selectedClass.Num, selectedClass.IsPurvaSmqna));
                    MessageBox.Show("Успешно премахнахте записките за този час!");
                }
            }
            panelWrite.Hide();
            textBoxNote.Text = "";
            if (listBoxClasses.Items.Count > 0)
            {
                listBoxClasses.ClearSelected();
                listBoxClasses.SelectedIndex = 0;
            }
            if (searchMode)
                panelSearch.Show();
            UpdateModels();
        }

        private void pictureBoxSaveNote_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBoxNote.Text.Length < 5)
                {
                    throw new ArgumentException("Дължината на записките за един час трябва да е поне 5 символа!");
                }
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
                if (searchMode)
                    panelSearch.Show();
                if (listBoxClasses.Items.Count > 0)
                {
                    listBoxClasses.ClearSelected();
                    listBoxClasses.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Грешка: " + ex.Message);
            }
            finally
            {
                UpdateModels();
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            panelSearch.Show();
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            searchMode = false;
            panelSearch.Hide();
            textBoxNote.Text = "";
            textBoxSearchInput.Text = "";
            listBoxShowSearch.Items.Clear();
            monthCalendar1.SelectionStart = DateTime.Now;
            if (listBoxClasses.Items.Count > 0)
            {
                listBoxClasses.ClearSelected();
                listBoxClasses.SelectedIndex = 0;
            }
        }

        private void textBoxSearchInput_TextChanged(object sender, EventArgs e)
        {
            try
            {
                listBoxShowSearch.Items.Clear();
                pictureBoxEditSearchNote.Hide();
                if (!string.IsNullOrEmpty(textBoxSearchInput.Text))
                {
                    if (!radioButtonClassOrParalelka.Checked && !radioButtonContent.Checked)
                        throw new ArgumentException("Моля, изберете критерии за търсене на записки!");
                    if (radioButtonContent.Checked)
                    {
                        notes = Program.noteRepo.GetModels().Where(n => n.Text.Contains(textBoxSearchInput.Text)).ToArray();
                    }
                    else if (radioButtonClassOrParalelka.Checked)
                    {
                        notes = Program.noteRepo.ClassName(textBoxSearchInput.Text).ToArray();
                    }
                    if (notes.Length > 0)
                    {
                        notes.OrderBy(n => n.Date)
                            .ThenBy(n => n.ClassNum)
                            .ThenBy(n => n.Purva)
                            .ToList();
                        foreach (var note in notes)
                        {
                            string day = "";
                            string containingText = "";
                            switch (note.Date.DayOfWeek)
                            {
                                case DayOfWeek.Monday: day = TimeForClockAndText.dayOfWeekMonday; break;
                                case DayOfWeek.Tuesday: day = TimeForClockAndText.dayOfWeekTuesday; break;
                                case DayOfWeek.Wednesday: day = TimeForClockAndText.dayOfWeekWednesday; break;
                                case DayOfWeek.Thursday: day = TimeForClockAndText.dayOfWeekThursday; break;
                                case DayOfWeek.Friday: day = TimeForClockAndText.dayOfWeekFriday; break;
                            }
                            int stringIndex = 0;
                            if (radioButtonContent.Checked)
                                stringIndex = note.Text.IndexOf(textBoxSearchInput.Text);
                            for (int i = 5; i > 0; i--)
                            {
                                if (note.Text.Length >= stringIndex + i)
                                {
                                    containingText += note.Text.Substring(stringIndex, i);
                                    break;
                                }
                            }

                            listBoxShowSearch.Items.Add($"{note.Date.ToString("dd/MM/yyyy")} {Program.GetModels().First(x => x.Day == day && x.Num == note.ClassNum && x.IsPurvaSmqna == note.Purva).Day} " +
                                $"{Program.GetModels().First(x => x.Day == day && x.Num == note.ClassNum && x.IsPurvaSmqna == note.Purva).ShowTheRecord()}" +
                                $" -> \"{containingText}\"");
                        }
                    }
                    else
                    {
                        if (radioButtonClassOrParalelka.Checked)
                        {
                            listBoxShowSearch.Items.Add($"Не са намерени записки за часове с \"{textBoxSearchInput.Text}\" клас/паралелка!");
                            listBoxShowSearch.Items.Add($"Ако записките са за свободен час моля, въведете \"0\"!");
                        }
                        else
                            listBoxShowSearch.Items.Add($"Не са намерени записки, съдържащи \"{textBoxSearchInput.Text}\"!");
                        pictureBoxEditSearchNote.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                textBoxSearchInput.Text = "";
            }
        }

        private void listBoxShowSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBoxEditSearchNote.Hide();
            if (listBoxShowSearch.SelectedIndex > -1 && notes.Count() > 0)
            {
                string day = "";
                selectedNote = notes[listBoxShowSearch.SelectedIndex];
                switch (selectedNote.Date.DayOfWeek)
                {
                    case DayOfWeek.Monday: day = TimeForClockAndText.dayOfWeekMonday; break;
                    case DayOfWeek.Tuesday: day = TimeForClockAndText.dayOfWeekTuesday; break;
                    case DayOfWeek.Wednesday: day = TimeForClockAndText.dayOfWeekWednesday; break;
                    case DayOfWeek.Thursday: day = TimeForClockAndText.dayOfWeekThursday; break;
                    case DayOfWeek.Friday: day = TimeForClockAndText.dayOfWeekFriday; break;
                }
                selectedDate = selectedNote.Date;
                selectedClass = Program.GetModels().First(x => x.Day == day && x.Num == selectedNote.ClassNum && x.IsPurvaSmqna == selectedNote.Purva);
                textBoxNote.Text = selectedNote.Text;
                pictureBoxEditSearchNote.Show();
            }
            else
            {
                selectedNote = null;
            }
        }

        private void pictureBoxEditSearchNote_Click(object sender, EventArgs e)
        {
            panelWrite.Show();
            panelSearch.Hide();
            searchMode = true;
            textBoxSearchInput.Text = "";
            UpdateLabels();
        }

        private void radioButtonClassOrParalelka_CheckedChanged(object sender, EventArgs e)
        {
            textBoxSearchInput.Text = "";
            if (radioButtonClassOrParalelka.Checked)
                MessageBox.Show("Паралелките се въвеждат с главна буква! Ако записките са за свободен час моля, въведете \"0\"!", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void radioButtonContent_CheckedChanged(object sender, EventArgs e)
        {
            textBoxSearchInput.Text = "";
        }
        private void pictureBoxLowerTextSize_Click(object sender, EventArgs e)
        {
            int temp;
            if (textBox1.Text == "")
                temp = 16;
            else
                temp = int.Parse(textBox1.Text);
            temp--;
            if (temp < 11)
                MessageBox.Show("Минималният размер на текста е 11.", "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                textBox1.Text = temp.ToString();
        }

        private void pictureBoxEnlargeTextSize_Click(object sender, EventArgs e)
        {
            int temp;
            if (textBox1.Text == "")
                temp = 16;
            else
                temp = int.Parse(textBox1.Text);
            temp++;
            textBox1.Text = temp.ToString();
        }


    }
}
