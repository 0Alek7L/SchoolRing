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
    public partial class Login : Form
    {
        Timer timer;
        public Login()
        {
            InitializeComponent();
            timer = new Timer();
            timer.Interval = 200;
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
            this.Hide();

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            Help help = new Help();
            this.Hide();
            help.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.WithClassSchedule = true;
            Program.LastForms.Push(this);
            this.Hide();
            SchoolProgram schoolProgram = new SchoolProgram();
            schoolProgram.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.WithClassSchedule = false;            
            Program.LastForms.Push(this);
            MainMenu main = new MainMenu();
            this.Hide();
            main.Show();
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

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.ForeColor = Color.FromArgb(34, 146, 164);
            button1.BackColor = Color.FromArgb(245, 239, 237);
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.FromArgb(34, 146, 164);
            button1.ForeColor = Color.FromArgb(245, 239, 237);
        }
    }
}
