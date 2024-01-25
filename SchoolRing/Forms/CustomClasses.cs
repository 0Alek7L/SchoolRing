using SchoolRing.Interfaces;
using SchoolRing.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace SchoolRing
{
    public partial class CustomClasses : Form
    {
        System.Windows.Forms.Timer timer;
        ISchoolClass chosenClass;
        public CustomClasses()
        {
            InitializeComponent();
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 200;
            timer.Tick += Timer_Tick;
            timer.Start();
            chosenClass = null;
            Program.ShowTheCurrentIcon(pictureBox3);
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
            this.Hide();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            Help help = new Help();
            help.Show();
            this.Hide();
        }

        private void comboBoxDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDay.SelectedIndex != -1)
            {
                listBox1.Items.Clear();
                comboBoxClass.Items.Clear();
                textBoxClassName.Clear();
                listBox1.Items.Add("ПЪРВА СМЯНА");
                foreach (var item in Program.GetModels().Where(x => x.Day == comboBoxDay.Text && x.IsPurvaSmqna).ToList().OrderBy(x => x.Num))
                {
                    listBox1.Items.Add(item.ShowTheRecord());
                    comboBoxClass.Items.Add(item.ShowTheRecord());
                }
                listBox1.Items.Add("ВТОРА СМЯНА");
                foreach (var item in Program.GetModels().Where(x => x.Day == comboBoxDay.Text && !x.IsPurvaSmqna).ToList().OrderBy(x => x.Num))
                {
                    listBox1.Items.Add(item.ShowTheRecord());
                    comboBoxClass.Items.Add(item.ShowTheRecord());
                }
            }
        }

        private void comboBoxClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxClass.SelectedIndex != -1)
            {
                if (comboBoxClass.SelectedIndex < 7)
                    chosenClass = Program.GetRecord(comboBoxDay.Text, comboBoxClass.SelectedIndex + 1, true);
                else
                    chosenClass = Program.GetRecord(comboBoxDay.Text, comboBoxClass.SelectedIndex - 6, false);
                if (!chosenClass.IsFree)
                {
                    if (chosenClass.ClassGrade != 0)
                        textBoxClassName.Text = chosenClass.ClassGrade + chosenClass.Paralelka;
                    else
                        textBoxClassName.Text = chosenClass.Paralelka;

                }
                else
                    textBoxClassName.Text = "";
                if (comboBoxClass.SelectedIndex < 7 && comboBoxClass.SelectedIndex > -1)
                    listBox1.SelectedIndex = comboBoxClass.SelectedIndex + 1;
                if (comboBoxClass.SelectedIndex >= 7)
                    listBox1.SelectedIndex = comboBoxClass.SelectedIndex + 2;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxDay.SelectedIndex < 0)
                    throw new ArgumentException("Моля, изберете ден и час!");
                if (comboBoxClass.SelectedIndex < 0)
                    throw new ArgumentException("Моля, изберете час!");
                if (textBoxClassName.Text.Length == 0)
                    throw new ArgumentException("Моля, въведете име!");

                ISchoolClass schoolClass = new SchoolClass(chosenClass.Day, chosenClass.Num, chosenClass.IsPurvaSmqna, false, 0, textBoxClassName.Text, chosenClass.StartHours, chosenClass.StartMinutes, chosenClass.EndHours, chosenClass.EndMinutes);
                if (schoolClass.Num > 1 && Program.GetRecord(schoolClass.Day, schoolClass.Num - 1, schoolClass.IsPurvaSmqna).IsMerging)
                    Program.GetRecord(schoolClass.Day, schoolClass.Num - 1, schoolClass.IsPurvaSmqna).ResetMergeStatus();
                if (schoolClass.Num < 7 && Program.GetRecord(schoolClass.Day, schoolClass.Num + 1, schoolClass.IsPurvaSmqna).IsMerged)
                    Program.GetRecord(schoolClass.Day, schoolClass.Num + 1, schoolClass.IsPurvaSmqna).ResetMergeStatus();

                if (schoolClass.IsPurvaSmqna && schoolClass.Num == 7)
                {
                    if (Program.GetRecord(schoolClass.Day, 1, false).IsMerged)
                        Program.GetRecord(schoolClass.Day, 1, false).ResetMergeStatus();
                }
                Program.AddRecord(schoolClass);
                comboBoxDay_SelectedIndexChanged(sender, e);
                SaveTheData.SaveSchoolClasses();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(217, 108, 6);
            button1.ForeColor = Color.White;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.ForeColor = Color.FromArgb(217, 108, 6);
            button1.BackColor = Color.White;
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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                if (listBox1.SelectedIndex == 0)
                    listBox1.SelectedIndex = 1;
                if (listBox1.SelectedIndex == 8)
                    listBox1.SelectedIndex = 9;
                if (listBox1.SelectedIndex < 8)
                    comboBoxClass.SelectedIndex = listBox1.SelectedIndex - 1;
                else
                    comboBoxClass.SelectedIndex = listBox1.SelectedIndex - 2;
            }
        }
    }
}
