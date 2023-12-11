using SchoolRing.Interfaces;
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
    public partial class SearchClasses : Form
    {
        Timer timer;
        public SearchClasses()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 50;
            timer.Tick += Timer_Tick;
            timer.Start();
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
            this.Close();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            Help help = new Help();
            help.Show();
            this.Hide();
        }

        private void textBoxSearchInput_TextChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            if (textBoxSearchInput.Text.Length > 0)
            {
                int countTotalClasses = 0;
                Dictionary<string, List<ISchoolClass>> classes = new Dictionary<string, List<ISchoolClass>>();
                classes.Add("ПОНЕДЕЛНИК", new List<ISchoolClass>());
                classes.Add("ВТОРНИК", new List<ISchoolClass>());
                classes.Add("СРЯДА", new List<ISchoolClass>());
                classes.Add("ЧЕТВЪРТЪК", new List<ISchoolClass>());
                classes.Add("ПЕТЪК", new List<ISchoolClass>());
                foreach (var item in Program.GetModels().Where(x => x.GetClassGradeAndParalelka().ToUpper().Contains(textBoxSearchInput.Text)))
                {
                    if (item.Day == "ПОНЕДЕЛНИК") classes[item.Day].Add(item);
                    if (item.Day == "ВТОРНИК") classes[item.Day].Add(item);
                    if (item.Day == "СРЯДА") classes[item.Day].Add(item);
                    if (item.Day == "ЧЕТВЪРТЪК") classes[item.Day].Add(item);
                    if (item.Day == "ПЕТЪК") classes[item.Day].Add(item);
                    countTotalClasses++;
                }
                if (countTotalClasses > 0)
                {
                    foreach (var daysAndClasses in classes)
                    {
                        if (daysAndClasses.Value.Count > 0)
                        {
                            listBox1.Items.Add(daysAndClasses.Key);
                            foreach (var klas in daysAndClasses.Value)
                            {
                                listBox1.Items.Add($"--{klas.ShowTheRecord()}");
                            }
                        }
                    }
                    listBox1.Items.Add("");
                    listBox1.Items.Add($"Намерени {countTotalClasses} часа с {textBoxSearchInput.Text} клас.");
                }
                else
                {
                    listBox1.Items.Add($"НЯМА НАМЕРЕНИ ДАННИ ЗА {textBoxSearchInput.Text}!");
                }
            }
        }

        private void pictureBox4_MouseHover(object sender, EventArgs e) =>
            textBoxSearchInput.Select();

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
