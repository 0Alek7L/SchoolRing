﻿using SchoolRing.Interfaces;
using SchoolRing.IO;
using SchoolRing.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

namespace SchoolRing
{

    public partial class SchoolProgram : Form
    {
        System.Windows.Forms.Timer timer;

        public SchoolProgram()
        {
            InitializeComponent();

            UpdateLabelDayAndSetVariable(5);
            Program.WithClassSchedule = true;
            list = listBoxMonday;
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 200;
            timer.Tick += Timer_Tick;
            //timer.Tick += Timer_TickForMovingLabel;
            timer.Start();
            Program.ShowTheCurrentIcon(pictureBox3);
            if (radioButtonFirst.Checked || radioButtonSecond.Checked)
            {
                comboBox1ClassNumber.Show();
                comboBoxClassGrade.Show();
                comboBoxClassParalelka.Show();
                checkBox1.Show();
                listBoxMonday.Show();
                listBoxTuesday.Show();
                listBoxWednesday.Show();
                listBoxThursday.Show();
                listBoxFriday.Show();
            }
            else
            {
                comboBox1ClassNumber.Hide();
                comboBoxClassGrade.Hide();
                comboBoxClassParalelka.Hide();
                checkBox1.Hide();
                listBoxMonday.Hide();
                listBoxTuesday.Hide();
                listBoxWednesday.Hide();
                listBoxThursday.Hide();
                listBoxFriday.Hide();
                temp = "Изберете учебна смяна.";
                labelTextSuggestion.Text = temp;
                pictureBox7.Hide();
                pictureBox8.Hide();
                labelDOW.Hide();
            }
        }

        private void Timer_TickForMovingLabel(object sender, EventArgs e)
        {
            //if(labelTextSuggestion.Right<=panel1.Left)
            //{
            //    labelTextSuggestion.Left = panel1.Right;
            //}
            //labelTextSuggestion.Left -= 10;
        }

        private void ShowMessageForIdiots()
        {
            Program.isMessageShown = true;
            MessageBox.Show("В това меню трябва да изберете УЧЕБНА СМЯНА, ДЕН и ЧАС за да попълните разписанието!");
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!Program.isMessageShown)
                ShowMessageForIdiots();
            TimeForClockAndText time = new TimeForClockAndText();
            labelDayOfWeek.Text = time.PrintDay();
            Program.ShowTheCurrentIcon(pictureBox3);
        }
        ListBox list;
        bool purvaSmqna = false;
        bool isFree = true;

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Program.LastForms.Pop().Show();
            this.Hide();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            Help help = new Help();
            this.Hide();
            help.Show();
        }

        private void comboBox1ClassNumber_Click_1(object sender, EventArgs e)
        {
            if (!radioButtonFirst.Checked && !radioButtonSecond.Checked)
                MessageBox.Show("Моля, изберете учебна смяна!");
        }

        private void comboBoxClassGrade_Click(object sender, EventArgs e)
        {
            if (comboBox1ClassNumber.Text == "")
                MessageBox.Show("Моля, изберете час чрез падащото меню или чрез кликване върху ред от списък с часове за ден от седмицата по-долу!");
        }

        private void listBoxMonday_Click(object sender, EventArgs e) => UpdateLabelDayAndSetVariable(1);

        private void listBoxTuesday_Click(object sender, EventArgs e) => UpdateLabelDayAndSetVariable(2);

        private void listBoxWednesday_Click(object sender, EventArgs e) => UpdateLabelDayAndSetVariable(3);

        private void listBoxThursday_Click(object sender, EventArgs e) => UpdateLabelDayAndSetVariable(4);

        private void listBoxFriday_Click(object sender, EventArgs e) => UpdateLabelDayAndSetVariable(5);

        private void UpdateLabelDayAndSetVariable(int day)
        {
            switch (day)
            {
                case 1: labelDOW.Text = "ПОНЕДЕЛНИК"; list = listBoxMonday; break;
                case 2: labelDOW.Text = "ВТОРНИК"; list = listBoxTuesday; break;
                case 3: labelDOW.Text = "СРЯДА"; list = listBoxWednesday; break;
                case 4: labelDOW.Text = "ЧЕТВЪРТЪК"; list = listBoxThursday; break;
                case 5: labelDOW.Text = "ПЕТЪК"; list = listBoxFriday; break;
            }
            ChangeArrowLocation();
            ChangeColorOfListBox();
        }


        private void comboBoxClassGrade_SelectedIndexChanged(object sender, EventArgs e)
        {
            isFree = false;
        }
        string temp = "";
        private async void buttonSaveRecord_Click(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked)
                    isFree = true;
                else
                    isFree = false;
                if (!radioButtonFirst.Checked && !radioButtonSecond.Checked)
                    throw new ArgumentException("Моля, изберете учебна смяна!");
                if (comboBox1ClassNumber.SelectedIndex == -1 && !isFree)
                    throw new ArgumentException("Моля, изберете час!");
                if (comboBoxClassGrade.SelectedIndex == -1 && !isFree)
                    throw new ArgumentException("Моля, изберете клас!");
                if (comboBoxClassParalelka.SelectedIndex == -1 && !isFree)
                    throw new ArgumentException("Моля, изберете паралелка!");
                if (isFree)
                {
                    AddToListBox(list, purvaSmqna, true, int.Parse(comboBox1ClassNumber.Text), 0, null);
                    ShowCheck();
                }
                else
                {
                    AddToListBox(list, purvaSmqna, false, int.Parse(comboBox1ClassNumber.Text), int.Parse(comboBoxClassGrade.Text), comboBoxClassParalelka.Text);
                    ShowCheck();
                }
                comboBox1ClassNumber.SelectedIndex = -1;
                comboBoxClassGrade.SelectedIndex = -1;
                comboBoxClassParalelka.SelectedIndex = -1;
                temp = "Записът е успешен!";
                labelTextSuggestion.Text = temp;
                await Task.Delay(2000);
                if (!IsThereASelectedIndexInListbox())
                    temp = "Натиснете на листа с часовете за съответния ден, за който искате да попълните разписание.";
                labelTextSuggestion.Text = temp;

            }
            catch (FormatException)
            {
                MessageBox.Show("Моля, въведете всички нужни стойности за да запазите часа!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Грешка: " + ex.Message);
            }
            finally
            {
                SaveTheData.SaveSchoolClasses();
            }
        }
        System.Windows.Forms.Timer tempTimer = new System.Windows.Forms.Timer();
        private void ShowCheck()
        {
            tempTimer.Interval = 2000;
            tempTimer.Tick += TempTimer_Tick;
            pictureBoxGreenCheck.Image = Properties.Resources.gc2;
            pictureBoxGreenCheck.Refresh();
            pictureBoxGreenCheck.Show();
            tempTimer.Start();
        }

        private void TempTimer_Tick(object sender, EventArgs e)
        {
            pictureBoxGreenCheck.Image = null;
            pictureBoxGreenCheck.Hide();
            tempTimer.Stop();
        }

        private void AddToListBox(ListBox _listBox, bool _isPurvaSmqna, bool _isFree, int _num, int _grade, string _paralelka)
        {

            List<int> list = Program.time.CalculateClassDuration(_num, _isPurvaSmqna);
            int startH = list[0];
            int startM = list[1];
            int endH = list[2];
            int endM = list[3];
            ISchoolClass schoolClass = new SchoolClass(labelDOW.Text, _num, _isPurvaSmqna, _isFree, _grade, _paralelka, startH, startM, endH, endM);
            Program.AddRecord(schoolClass);

            if (_num > 1 && Program.GetRecord(labelDOW.Text, _num - 1, _isPurvaSmqna).IsMerging)
                Program.GetRecord(labelDOW.Text, _num - 1, _isPurvaSmqna).ResetMergeStatus();
            if (_num < 7 && Program.GetRecord(labelDOW.Text, _num + 1, _isPurvaSmqna).IsMerged)
                Program.GetRecord(labelDOW.Text, _num + 1, _isPurvaSmqna).ResetMergeStatus();

            if (_isPurvaSmqna && _num == 7)
            {
                if (Program.GetRecord(labelDOW.Text, 1, false).IsMerged)
                    Program.GetRecord(labelDOW.Text, 1, false).ResetMergeStatus();
            }
            _listBox.Items[_num] = schoolClass.ShowTheRecord();
            switch (labelDOW.Text)
            {
                case "ПОНЕДЕЛНИК": FirstTimeFillListBoxes(1, _isPurvaSmqna); break;
                case "ВТОРНИК": FirstTimeFillListBoxes(2, _isPurvaSmqna); break;
                case "СРЯДА": FirstTimeFillListBoxes(3, _isPurvaSmqna); break;
                case "ЧЕТВЪРТЪК": FirstTimeFillListBoxes(4, _isPurvaSmqna); break;
                case "ПЕТЪК": FirstTimeFillListBoxes(5, _isPurvaSmqna); break;
            }
        }

        private void FirstTimeFillListBoxes(int day, bool isPurva)
        {
            string dayAsText = string.Empty;
            switch (day)
            {
                case 1: list = listBoxMonday; dayAsText = "ПОНЕДЕЛНИК"; break;
                case 2: list = listBoxTuesday; dayAsText = "ВТОРНИК"; break;
                case 3: list = listBoxWednesday; dayAsText = "СРЯДА"; break;
                case 4: list = listBoxThursday; dayAsText = "ЧЕТВЪРТЪК"; break;
                case 5: list = listBoxFriday; dayAsText = "ПЕТЪК"; break;
            }
            list.Items.Clear();
            if (isPurva)
                list.Items.Add("ПЪРВА СМЯНА");
            else
                list.Items.Add("ВТОРА СМЯНА");

            for (int i = 0; i < 7; i++)
            {
                list.Items.Add(string.Empty);
            }

            for (int i = 0; i < Program.GetModels().Count; i++)
            {
                if (Program.GetModels().ElementAt(i).Day == dayAsText && Program.GetModels().ElementAt(i).IsPurvaSmqna == isPurva)
                {
                    list.Items[Program.GetModels().ElementAt(i).Num] = (Program.GetModels().ElementAt(i).ShowTheRecord());
                }
            }
            //ТАКА ЩЕ СЕ ПЪЛНИ ВСЕКИ ЛИСТБОКС                
        }

        private void radioButtonFirst_CheckedChanged(object sender, EventArgs e)
        {

            pictureBox7.Show();
            pictureBox8.Show();
            labelDOW.Show();

            comboBox1ClassNumber.Show();
            comboBoxClassGrade.Show();
            comboBoxClassParalelka.Show();
            checkBox1.Show();
            listBoxMonday.Show();
            listBoxTuesday.Show();
            listBoxWednesday.Show();
            listBoxThursday.Show();
            listBoxFriday.Show();
            if (!IsThereASelectedIndexInListbox())
                temp = "Натиснете на листа с часовете за съответния ден, за който искате да попълните разписание.";
            labelTextSuggestion.Text = temp;
            if (radioButtonFirst.Checked)
            {
                radioButtonFirst.ForeColor = Color.White;
                radioButtonFirst.Font = new Font(radioButtonFirst.Font, FontStyle.Bold);
                purvaSmqna = true;
                FirstTimeFillListBoxes(1, true);
                FirstTimeFillListBoxes(2, true);
                FirstTimeFillListBoxes(3, true);
                FirstTimeFillListBoxes(4, true);
                FirstTimeFillListBoxes(5, true);
            }
            else
            {
                radioButtonFirst.ForeColor = Color.LightBlue;
                radioButtonFirst.Font = new Font(radioButtonFirst.Font, FontStyle.Italic);
                radioButtonFirst.Font = new Font(radioButtonFirst.Font, FontStyle.Strikeout);
            }
            if (radioButtonSecond.Checked)
            {
                radioButtonSecond.ForeColor = Color.White;
                radioButtonSecond.Font = new Font(radioButtonSecond.Font, FontStyle.Bold);
                purvaSmqna = false;
                FirstTimeFillListBoxes(1, false);
                FirstTimeFillListBoxes(2, false);
                FirstTimeFillListBoxes(3, false);
                FirstTimeFillListBoxes(4, false);
                FirstTimeFillListBoxes(5, false);
            }
            else
            {
                radioButtonSecond.ForeColor = Color.LightBlue;
                radioButtonSecond.Font = new Font(radioButtonSecond.Font, FontStyle.Italic);
                radioButtonSecond.Font = new Font(radioButtonSecond.Font, FontStyle.Strikeout);
            }
        }

        private void listBoxMonday_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxMonday.SelectedIndex == 0)
                listBoxMonday.SelectedIndex = 1;
            int temp = listBoxMonday.SelectedIndex;
            if (listBoxTuesday.SelectedItem != null)
                listBoxTuesday.SelectedIndex = -1;
            if (listBoxWednesday.SelectedItem != null)
                listBoxWednesday.SelectedIndex = -1;
            if (listBoxThursday.SelectedItem != null)
                listBoxThursday.SelectedIndex = -1;
            if (listBoxFriday.SelectedItem != null)
                listBoxFriday.SelectedIndex = -1;
            listBoxMonday.SelectedIndex = temp;
            if (list.SelectedIndex != 0 && list.SelectedIndex != -1)
                FillComboBoxesWithSelectedItemInListBox("ПОНЕДЕЛНИК", listBoxMonday.SelectedIndex);
        }
        private void listBoxTuesday_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTuesday.SelectedIndex == 0)
                listBoxTuesday.SelectedIndex = 1;
            int temp = listBoxTuesday.SelectedIndex;
            if (listBoxMonday.SelectedItem != null)
                listBoxMonday.SelectedIndex = -1;
            if (listBoxWednesday.SelectedItem != null)
                listBoxWednesday.SelectedIndex = -1;
            if (listBoxThursday.SelectedItem != null)
                listBoxThursday.SelectedIndex = -1;
            if (listBoxFriday.SelectedItem != null)
                listBoxFriday.SelectedIndex = -1;
            listBoxTuesday.SelectedIndex = temp;
            if (list.SelectedIndex != 0 && list.SelectedIndex != -1)
                FillComboBoxesWithSelectedItemInListBox("ВТОРНИК", listBoxTuesday.SelectedIndex);
        }

        private void listBoxWednesday_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxWednesday.SelectedIndex == 0)
                listBoxWednesday.SelectedIndex = 1;
            int temp = listBoxWednesday.SelectedIndex;
            if (listBoxMonday.SelectedItem != null)
                listBoxMonday.SelectedIndex = -1;
            if (listBoxTuesday.SelectedItem != null)
                listBoxTuesday.SelectedIndex = -1;
            if (listBoxThursday.SelectedItem != null)
                listBoxThursday.SelectedIndex = -1;
            if (listBoxFriday.SelectedItem != null)
                listBoxFriday.SelectedIndex = -1;
            listBoxWednesday.SelectedIndex = temp;
            if (list.SelectedIndex != 0 && list.SelectedIndex != -1)
                FillComboBoxesWithSelectedItemInListBox("СРЯДА", listBoxWednesday.SelectedIndex);

        }

        private void listBoxThursday_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxThursday.SelectedIndex == 0)
                listBoxThursday.SelectedIndex = 1;
            int temp = listBoxThursday.SelectedIndex;
            if (listBoxMonday.SelectedItem != null)
                listBoxMonday.SelectedIndex = -1;
            if (listBoxTuesday.SelectedItem != null)
                listBoxTuesday.SelectedIndex = -1;
            if (listBoxWednesday.SelectedItem != null)
                listBoxWednesday.SelectedIndex = -1;
            if (listBoxFriday.SelectedItem != null)
                listBoxFriday.SelectedIndex = -1;
            listBoxThursday.SelectedIndex = temp;
            if (list.SelectedIndex != 0 && list.SelectedIndex != -1)
                FillComboBoxesWithSelectedItemInListBox("ЧЕТВЪРТЪК", listBoxThursday.SelectedIndex);

        }

        private void listBoxFriday_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxFriday.SelectedIndex == 0)
                listBoxFriday.SelectedIndex = 1;
            int temp = listBoxFriday.SelectedIndex;
            if (listBoxMonday.SelectedItem != null)
                listBoxMonday.SelectedIndex = -1;
            if (listBoxTuesday.SelectedItem != null)
                listBoxTuesday.SelectedIndex = -1;
            if (listBoxWednesday.SelectedItem != null)
                listBoxWednesday.SelectedIndex = -1;
            if (listBoxThursday.SelectedItem != null)
                listBoxThursday.SelectedIndex = -1;
            listBoxFriday.SelectedIndex = temp;
            if (list.SelectedIndex != 0 && list.SelectedIndex != -1)
                FillComboBoxesWithSelectedItemInListBox("ПЕТЪК", listBoxFriday.SelectedIndex);
        }

        private void FillComboBoxesWithSelectedItemInListBox(string day, int num)
        {
            ISchoolClass schoolClass = Program.FindTheRecord(day, num, purvaSmqna);
            comboBoxClassGrade.SelectedIndex = -1;
            comboBoxClassParalelka.SelectedIndex = -1;
            if (!schoolClass.IsFree)
            {
                int selectedIndexGrade = 0;
                int selectedIndexParalelka = 0;
                switch (schoolClass.ClassGrade)
                {
                    case 1: selectedIndexGrade = 0; break;
                    case 2: selectedIndexGrade = 1; break;
                    case 3: selectedIndexGrade = 2; break;
                    case 4: selectedIndexGrade = 3; break;
                    case 5: selectedIndexGrade = 4; break;
                    case 6: selectedIndexGrade = 5; break;
                    case 7: selectedIndexGrade = 6; break;
                    case 8: selectedIndexGrade = 7; break;
                    case 9: selectedIndexGrade = 8; break;
                    case 10: selectedIndexGrade = 9; break;
                    case 11: selectedIndexGrade = 10; break;
                    case 12: selectedIndexGrade = 11; break;
                }
                switch (schoolClass.Paralelka)
                {
                    case "А": selectedIndexParalelka = 0; break;
                    case "Б": selectedIndexParalelka = 1; break;
                    case "В": selectedIndexParalelka = 2; break;
                    case "Г": selectedIndexParalelka = 3; break;
                    case "Д": selectedIndexParalelka = 4; break;
                    case "Е": selectedIndexParalelka = 5; break;
                    case "Ж": selectedIndexParalelka = 6; break;
                }
                comboBox1ClassNumber.SelectedIndex = num - 1;
                comboBoxClassGrade.SelectedIndex = selectedIndexGrade;
                comboBoxClassParalelka.SelectedIndex = selectedIndexParalelka;
                checkBox1.Checked = false;
                temp = "Изберете клас и паралелка от падащите менюта.";
                labelTextSuggestion.Text = temp;
            }
            else
            {
                comboBox1ClassNumber.SelectedIndex = num - 1;
                checkBox1.Checked = true;
                temp = "Този час е свободен. За да промените състоянието му, моля, премахнете тикчето \"свободен\".";
                labelTextSuggestion.Text = temp;
            }
        }

        private void ChangeArrowLocation()
        {
            if (labelDOW.Text == "ПОНЕДЕЛНИК")
            {
                pictureBox7.Location = new System.Drawing.Point(662, 134);
                pictureBox8.Location = new System.Drawing.Point(303, 134);
            }
            if (labelDOW.Text == "ВТОРНИК")
            {
                pictureBox7.Location = new System.Drawing.Point(613, 134);
                pictureBox8.Location = new System.Drawing.Point(350, 134);
            }
            if (labelDOW.Text == "СРЯДА")
            {
                pictureBox7.Location = new System.Drawing.Point(584, 134);
                pictureBox8.Location = new System.Drawing.Point(378, 134);
            }
            if (labelDOW.Text == "ЧЕТВЪРТЪК")
            {
                pictureBox7.Location = new System.Drawing.Point(639, 134);
                pictureBox8.Location = new System.Drawing.Point(317, 134);
            }
            if (labelDOW.Text == "ПЕТЪК")
            {
                pictureBox7.Location = new System.Drawing.Point(584, 134);
                pictureBox8.Location = new System.Drawing.Point(378, 134);
            }
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            switch (labelDOW.Text)
            {
                case "ПОНЕДЕЛНИК": UpdateLabelDayAndSetVariable(5); break;
                case "ВТОРНИК": UpdateLabelDayAndSetVariable(1); break;
                case "СРЯДА": UpdateLabelDayAndSetVariable(2); break;
                case "ЧЕТВЪРТЪК": UpdateLabelDayAndSetVariable(3); break;
                case "ПЕТЪК": UpdateLabelDayAndSetVariable(4); ; break;
            }
            comboBox1ClassNumber_SelectedIndexChanged(sender, e);

        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            switch (labelDOW.Text)
            {
                case "ПОНЕДЕЛНИК": UpdateLabelDayAndSetVariable(2); break;
                case "ВТОРНИК": UpdateLabelDayAndSetVariable(3); break;
                case "СРЯДА": UpdateLabelDayAndSetVariable(4); break;
                case "ЧЕТВЪРТЪК": UpdateLabelDayAndSetVariable(5); break;
                case "ПЕТЪК": UpdateLabelDayAndSetVariable(1); ; break;
            }
            comboBox1ClassNumber_SelectedIndexChanged(sender, e);
        }

        private void ChangeColorOfListBox()
        {
            if (list == listBoxMonday)
            {
                listBoxMonday.BackColor = Color.White;
                listBoxTuesday.BackColor = Color.FromArgb(217, 217, 217);
                listBoxWednesday.BackColor = Color.FromArgb(217, 217, 217);
                listBoxThursday.BackColor = Color.FromArgb(217, 217, 217);
                listBoxFriday.BackColor = Color.FromArgb(217, 217, 217);
            }
            else if (list == listBoxTuesday)
            {
                listBoxTuesday.BackColor = Color.White;
                listBoxMonday.BackColor = Color.FromArgb(217, 217, 217);
                listBoxWednesday.BackColor = Color.FromArgb(217, 217, 217);
                listBoxThursday.BackColor = Color.FromArgb(217, 217, 217);
                listBoxFriday.BackColor = Color.FromArgb(217, 217, 217);
            }
            else if (list == listBoxWednesday)
            {
                listBoxWednesday.BackColor = Color.White;
                listBoxTuesday.BackColor = Color.FromArgb(217, 217, 217);
                listBoxMonday.BackColor = Color.FromArgb(217, 217, 217);
                listBoxThursday.BackColor = Color.FromArgb(217, 217, 217);
                listBoxFriday.BackColor = Color.FromArgb(217, 217, 217);
            }
            else if (list == listBoxThursday)
            {
                listBoxThursday.BackColor = Color.White;
                listBoxTuesday.BackColor = Color.FromArgb(217, 217, 217);
                listBoxWednesday.BackColor = Color.FromArgb(217, 217, 217);
                listBoxMonday.BackColor = Color.FromArgb(217, 217, 217);
                listBoxFriday.BackColor = Color.FromArgb(217, 217, 217);
            }
            if (list == listBoxFriday)
            {
                listBoxFriday.BackColor = Color.White;
                listBoxTuesday.BackColor = Color.FromArgb(217, 217, 217);
                listBoxWednesday.BackColor = Color.FromArgb(217, 217, 217);
                listBoxThursday.BackColor = Color.FromArgb(217, 217, 217);
                listBoxMonday.BackColor = Color.FromArgb(217, 217, 217);
            }
        }

        private void buttonContinueToMainMenu_Click(object sender, EventArgs e)
        {
            Program.WithClassSchedule = true;
            Program.LastForms.Push(this);
            //MainMenu mainMenu = new MainMenu();
            this.Dispose();
            this.Close();
            //mainMenu.Show();
            MainMenu.Instance.Show();
            SaveTheData.SaveSchoolClasses();
        }

        private void buttonClearTheRepo_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Сигурни ли сте, че желаете да изчистите учебното разписание?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Program.ClearTheSchedule();
                FirstTimeFillListBoxes(1, true);
                FirstTimeFillListBoxes(2, true);
                FirstTimeFillListBoxes(3, true);
                FirstTimeFillListBoxes(4, true);
                FirstTimeFillListBoxes(5, true);

                FirstTimeFillListBoxes(1, false);
                FirstTimeFillListBoxes(2, false);
                FirstTimeFillListBoxes(3, false);
                FirstTimeFillListBoxes(4, false);
                FirstTimeFillListBoxes(5, false);
                MessageBox.Show("Вие изчистихте разписанието успешно!");
                radioButtonSecond.Checked = true;
                radioButtonFirst.Checked = true;
            }
        }

        private void label4_MouseEnter(object sender, EventArgs e)
        {
            label4.ForeColor = Color.FromArgb(34, 146, 164);
            label4.BackColor = Color.White;
            //label4.Font = new Font(label4.Font, FontStyle.Bold);
            label4.BorderStyle = BorderStyle.Fixed3D;
        }

        private void label4_MouseLeave(object sender, EventArgs e)
        {
            label4.BackColor = Color.FromArgb(34, 146, 164);
            label4.ForeColor = Color.White;
            //label4.Font = new Font(label4.Font, FontStyle.Regular);
            label4.BorderStyle = BorderStyle.FixedSingle;

        }

        private void label4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Сигурни ли сте, че желаете да продължите без учебно разписание?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Program.WithClassSchedule = false;
                Program.LastForms.Push(this);
                //MainMenu mainMenu = new MainMenu();
                this.Hide();
                MainMenu.Instance.Show();
                //mainMenu.Show();
            }
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                isFree = true;
                comboBoxClassParalelka.SelectedIndex = -1;
                comboBoxClassGrade.SelectedIndex = -1;
                comboBoxClassGrade.Enabled = false;
                comboBoxClassGrade.BackColor = Color.LightGray;
                comboBoxClassParalelka.Enabled = false;
                comboBoxClassParalelka.BackColor = Color.LightGray;
                checkBox1.Font = new Font(checkBox1.Font, FontStyle.Bold);
                if (IsThereASelectedIndexInListbox())
                    temp = "Този час е свободен. За да промените състоянието му, моля, премахнете тикчето \"свободен\".";
                else
                    temp = "Натиснете на листа с часовете за съответния ден, за който искате да попълните разписание.";
                labelTextSuggestion.Text = temp;
            }
            else
            {
                if (IsThereASelectedIndexInListbox())
                    temp = "Изберете клас и паралелка от падащите менюта.";
                else
                    temp = "Натиснете на листа с часовете за съответния ден, за който искате да попълните разписание.";
                labelTextSuggestion.Text = temp;
                
                isFree = false;
                comboBoxClassGrade.Enabled = true;
                comboBoxClassGrade.BackColor = Color.FromArgb(189, 191, 9);
                comboBoxClassParalelka.Enabled = true;
                comboBoxClassParalelka.BackColor = Color.FromArgb(189, 191, 9);
                checkBox1.Font = new Font(checkBox1.Font, FontStyle.Regular);
                //listBoxMonday.SelectedIndex = -1;
                //listBoxTuesday.SelectedIndex = -1;
                //listBoxWednesday.SelectedIndex = -1;
                //listBoxThursday.SelectedIndex = -1;
                //listBoxFriday.SelectedIndex = -1;
            }
        }

        private void buttonSaveRecord_MouseEnter(object sender, EventArgs e)
        {
            buttonSaveRecord.BackColor = Color.FromArgb(217, 108, 6);
            buttonSaveRecord.ForeColor = Color.White;
        }

        private void buttonSaveRecord_MouseLeave(object sender, EventArgs e)
        {
            buttonSaveRecord.ForeColor = Color.FromArgb(217, 108, 6);
            buttonSaveRecord.BackColor = Color.White;
        }

        private void buttonContinueToMainMenu_MouseEnter(object sender, EventArgs e)
        {
            buttonContinueToMainMenu.BackColor = Color.FromArgb(189, 191, 9);
            buttonContinueToMainMenu.ForeColor = Color.White;
        }

        private void buttonContinueToMainMenu_MouseLeave(object sender, EventArgs e)
        {
            buttonContinueToMainMenu.ForeColor = Color.FromArgb(189, 191, 9);
            buttonContinueToMainMenu.BackColor = Color.White;
        }

        private void buttonClearTheRepo_MouseEnter(object sender, EventArgs e)
        {
            buttonClearTheRepo.ForeColor = Color.FromArgb(34, 146, 164);
            buttonClearTheRepo.BackColor = Color.White;
        }

        private void buttonClearTheRepo_MouseLeave(object sender, EventArgs e)
        {
            buttonClearTheRepo.BackColor = Color.FromArgb(34, 146, 164);
            buttonClearTheRepo.ForeColor = Color.White;
        }

        private bool IsThereASelectedIndexInListbox()
        {
            if (listBoxMonday.SelectedItems.Count == 0 && listBoxTuesday.SelectedItems.Count == 0 &&
                listBoxWednesday.SelectedItems.Count == 0 && listBoxThursday.SelectedItems.Count == 0 && listBoxFriday.SelectedItems.Count == 0)
                return false;
            else
                return true;
        }

        private void comboBox1ClassNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1ClassNumber.SelectedIndex != -1)
            {
                if (labelDOW.Text == TimeForClockAndText.dayOfWeekMonday)
                {
                    listBoxMonday.SelectedIndex = int.Parse(comboBox1ClassNumber.SelectedItem.ToString());
                }
                else if (labelDOW.Text == TimeForClockAndText.dayOfWeekTuesday)
                {
                    listBoxTuesday.SelectedIndex = int.Parse(comboBox1ClassNumber.SelectedItem.ToString());

                }
                else if (labelDOW.Text == TimeForClockAndText.dayOfWeekWednesday)
                {
                    listBoxWednesday.SelectedIndex = int.Parse(comboBox1ClassNumber.SelectedItem.ToString());

                }
                else if (labelDOW.Text == TimeForClockAndText.dayOfWeekThursday)
                {
                    listBoxThursday.SelectedIndex = int.Parse(comboBox1ClassNumber.SelectedItem.ToString());

                }
                else if (labelDOW.Text == TimeForClockAndText.dayOfWeekFriday)
                {
                    listBoxFriday.SelectedIndex = int.Parse(comboBox1ClassNumber.SelectedItem.ToString());

                }
            }
        }
    }
}
