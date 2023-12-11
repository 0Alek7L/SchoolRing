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

namespace SchoolRing
{
    public partial class MergeClassesForm : Form
    {
        System.Windows.Forms.Timer timer;
        internal List<ISchoolClass> classesAvailableForMerging; //make it able to be saved!!!
        ISchoolClass selectedMergedClass;
        ISchoolClass selectedMergingClass;
        public MergeClassesForm()
        {
            InitializeComponent();
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 50;
            timer.Tick += Timer_Tick;
            timer.Start();
            classesAvailableForMerging = new List<ISchoolClass>();
            selectedMergedClass = null;
            selectedMergingClass = null;
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
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedMergingClass = null;
            selectedMergedClass = null;
            classesAvailableForMerging= new List<ISchoolClass>();
            listBox1.Items.Clear();
            comboBox2.Items.Clear();
            labelShowSecondClass.Text = "";
            listBox1.Items.Add("ПЪРВА СМЯНА");
            for (int i = 1; i < 8; i++)
            {
                foreach (var item in Program.GetModels().Where(x => x.Day == comboBox1.Text && x.IsPurvaSmqna).ToList().OrderBy(x => x.Num))
                {
                    if (item.Num == i)
                        listBox1.Items.Add(item.ShowTheRecord());
                }
            }
            listBox1.Items.Add("ВТОРА СМЯНА");
            for (int i = 1; i < 8; i++)
            {
                foreach (var item in Program.GetModels().Where(x => x.Day == comboBox1.Text && !x.IsPurvaSmqna).ToList().OrderBy(x => x.Num))
                {
                    if (item.Num == i)
                        listBox1.Items.Add(item.ShowTheRecord());
                }
            }
            List<ISchoolClass> mergable = Program.MergableClasses(comboBox1.Text);
            mergable = mergable.OrderBy(x => x.IsPurvaSmqna).ThenBy(x => x.Num).ToList();

            if (mergable.Count == 0)
                System.Windows.Forms.MessageBox.Show("За този ден няма 2 поредни часа с един и същ клас.");
            else
            {
                foreach (var klas in mergable)
                {
                    comboBox2.Items.Add(klas.ShowTheRecord());
                    classesAvailableForMerging.Add(klas);
                }
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex >= 0)
            {
                //comboBox1_SelectedIndexChanged(sender, e);
                button1.Enabled = true; 
                ISchoolClass mergingClass;
                if (classesAvailableForMerging.Count <= 0||comboBox2.SelectedIndex>classesAvailableForMerging.Count-1)
                    System.Windows.Forms.MessageBox.Show("Грешка!");
                else
                {
                    selectedMergingClass = classesAvailableForMerging[comboBox2.SelectedIndex];
                    mergingClass = selectedMergingClass;

                    if (mergingClass.Num < 7)
                    {
                        selectedMergedClass = Program.GetRecord(comboBox1.Text, selectedMergingClass.Num + 1, selectedMergingClass.IsPurvaSmqna);
                        labelShowSecondClass.Text = selectedMergedClass.ShowTheRecord();
                    }
                    else if (mergingClass.Num == 7 && mergingClass.IsPurvaSmqna)
                    {
                        selectedMergedClass = Program.GetRecord(comboBox1.Text, 1, false);
                        labelShowSecondClass.Text = selectedMergedClass.ShowTheRecord();
                    }
                }
            }
        }

        private void button1_MouseEnter(object sender, EventArgs e) =>
            button1.FlatStyle = FlatStyle.Flat;

        private void button1_MouseLeave(object sender, EventArgs e) =>
            button1.FlatStyle = FlatStyle.Popup;

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                if (comboBox2.SelectedIndex < 0)
                    throw new ArgumentException("Моля изберете часове за сливане!");
                else if (selectedMergedClass.MergedWith == selectedMergingClass.SaveMergingReference() && selectedMergingClass.MergedWith == selectedMergedClass.SaveMergingReference())
                {
                    DialogResult ThereIsAlreadyASchedule = System.Windows.Forms.MessageBox.Show("Тези часове са вече сляти! Желаете ли да ги разделите на 2 отделни?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (ThereIsAlreadyASchedule == DialogResult.Yes)
                    {
                        selectedMergingClass.ResetMergeStatus();
                        selectedMergedClass.ResetMergeStatus();
                        System.Windows.Forms.MessageBox.Show("Вие разделихте този час на два отделни.");
                        comboBox1_SelectedIndexChanged(sender, e);
                    }
                    //else
                    //{
                    //    button1.Enabled = false;
                    //}
                }
                else if (selectedMergedClass.MergedWith == null && selectedMergingClass.MergedWith == null)
                {
                    selectedMergingClass.MergeClassWith(selectedMergedClass);
                    selectedMergedClass.IsMerged = true;
                    System.Windows.Forms.MessageBox.Show($"Вие сляхте 2 часа с/със {selectedMergingClass.GetClassGradeAndParalelka()} клас");
                    comboBox1_SelectedIndexChanged(sender, e);
                    //classesAvailableForMerging.Clear();
                    selectedMergedClass = null;
                    selectedMergingClass = null;
                }
                else
                {
                    comboBox1_SelectedIndexChanged(sender, e);
                    throw new ArgumentException("Операцията не може да бъде изпълнена! Един от часовете вече е слят.");
                }
                SaveTheData.SaveSchoolClasses();                
            }
            catch (ArgumentException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
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
    }
}
