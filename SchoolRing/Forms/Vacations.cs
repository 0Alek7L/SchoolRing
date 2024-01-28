using Microsoft.Office.Interop.Excel;
using SchoolRing.Interfaces;
using SchoolRing.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolRing
{
    public partial class Vacations : Form
    {
        Timer timer;
        public Vacations()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 200;
            timer.Tick += Timer_Tick;
            timer.Start();
            RefreshTheListBox();
            pictureBoxRemove.Hide();
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
            Program.ShowTheCurrentIcon(pictureBox3);
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeForClockAndText time = new TimeForClockAndText();
            labelDayOfWeek.Text = time.PrintDay();
            Program.ShowTheCurrentIcon(pictureBox3);
            if (comboBox1.SelectedIndex != -1 && comboBox1.SelectedIndex != 0)
            {
                pictureBoxClearTheCB.Show();
                pictureBoxRemove.Show();
                //dateTimePicker1.Enabled=false;
                //dateTimePicker2.Enabled=false;
                label2.Text = $"В момента редактирате избраната ваканция!";
            }
            else
            {
                pictureBoxClearTheCB.Hide();
                pictureBoxRemove.Hide();
                //dateTimePicker1.Enabled = true;
                //dateTimePicker2.Enabled = true;
                label2.Text = $"Изберете съществуваща ваканция от тук";
            }

            if (Program.vdRepo.GetModels().Any())
            {
                comboBox1.Show();
                label2.Show();
            }
            else
            {
                comboBox1.Hide();
                label2.Hide();
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Program.LastForms.Pop().Show();
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

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime now = DateTime.Now;
                //DateTime start = dateTimePicker1.Value;
                //DateTime end = dateTimePicker2.Value;
                if (dateTimePicker1.Value.Date < now.Date && dateTimePicker2.Value.Date < now.Date)
                    throw new ArgumentException("Тази ваканция вече е отминала! Моля въведете настояща/бъдеща ваканция!");
                if (dateTimePicker1.Value.Year > now.Year + 1 || dateTimePicker2.Value.Year > now.Year + 1)
                    throw new ArgumentException("Моля въведете ваканция за тази учебна година!");

                if (textBox1.Text.Length <= 0)
                    throw new ArgumentException("Моля въведете име!");
                if (dateTimePicker2.Value < dateTimePicker1.Value)
                    throw new ArgumentException("Не може началото на ваканцията да е след края!");
                if (comboBox1.SelectedIndex == -1 || comboBox1.SelectedIndex == 0)
                {
                    if (Program.vdRepo.IsThereAModel(dateTimePicker1.Value, dateTimePicker2.Value))
                        throw new ArgumentException("Тази ваканция се застъпва с вече съществуваща!");
                    if (Program.vdRepo.GetModels().Any(x => x.Argument == textBox1.Text))
                        throw new ArgumentException("Вече има ваканция с такова наименование!");
                }
                else
                {
                    if (Program.vdRepo.IsThereAModel(dateTimePicker1.Value, dateTimePicker2.Value, start, end, currentVCName))
                        throw new ArgumentException("Тази ваканция се застъпва с вече съществуваща!");
                    if (Program.vdRepo.GetModels().Any(x => x.Argument == textBox1.Text && x != Program.vdRepo.GetModel(start, end, currentVCName)))
                        throw new ArgumentException("Вече има ваканция с такова наименование!");
                }
                IVacationalDays vacation = new VacationalDays(textBox1.Text, dateTimePicker1.Value, dateTimePicker2.Value);
                if (comboBox1.SelectedIndex >0)
                {
                    Program.vdRepo.RemoveModel(Program.vdRepo.GetModel(start, end, currentVCName));
                    //comboBox1.SelectedIndex = -1;
                }
                Program.vdRepo.AddModel(vacation);
                if (comboBox1.SelectedIndex <0)
                    MessageBox.Show($"Вие добавихте ваканция на име \"{vacation.Argument}\"", "Операцията е извършена успешно");
                else
                    MessageBox.Show($"Вие редактирахте ваканция на име \"{vacation.Argument}\"", "Операцията е извършена успешно");
                RefreshTheListBox();
                textBox1.Clear();
                SaveTheData.SaveVacation();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                comboBox1.SelectedIndex = -1;

            }
        }
        private void RefreshTheListBox()
        {
            listBox1.Items.Clear();
            comboBox1.Items.Clear();
            comboBox1.Items.Add("");
            if (Program.vdRepo.GetModels().Count > 0)
            {
                foreach (var item in Program.vdRepo.GetModels())
                {
                    listBox1.Items.Add(item.Argument);
                    listBox1.Items.Add(item.StartDate.ToString("-- от dd.MM.yyyy"));
                    listBox1.Items.Add(item.EndDate.ToString("-- до dd.MM.yyyy"));
                    listBox1.Items.Add("");
                    comboBox1.Items.Add(item.Argument);
                }
            }
            comboBox1.SelectedIndex = -1;
            pictureBoxRemove.Hide();
        }
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        string currentVCName = "";
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                RefreshTheListBox();
                textBox1.Text = "";
                dateTimePicker1.Value = DateTime.Now;
                dateTimePicker2.Value = DateTime.Now;
            }
            else if (comboBox1.SelectedIndex != -1&&comboBox1.SelectedIndex!=0)
            {
                IVacationalDays vacation = Program.vdRepo.GetModels().First(x => x.Argument == comboBox1.Text);
                start = vacation.StartDate;
                end = vacation.EndDate;
                currentVCName = comboBox1.Text;
                textBox1.Text = vacation.Argument;
                dateTimePicker1.Value = start;
                dateTimePicker2.Value = end;
            }
            else
                textBox1.Text = "";

        }

        private void pictureBoxRemove_Click(object sender, EventArgs e)
        {
            DialogResult ask = MessageBox.Show("Сигурни ли сте, че желаете да премахнете тази ваканция?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (ask == DialogResult.Yes)
            {
                MessageBox.Show($"Вие премахнахте {Program.vdRepo.GetModel(start, end).Argument}.", "Операцията е извършена успешно");
                Program.vdRepo.RemoveModel(Program.vdRepo.GetModel(start, end));
                comboBox1.SelectedIndex = -1;
                pictureBoxRemove.Hide();
                RefreshTheListBox();
                textBox1.Text = "";
                SaveTheData.SaveVacation();
            }
            else
                comboBox1.SelectedIndex = -1;
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


        private void pictureBoxClearTheCB_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }
    }
}
