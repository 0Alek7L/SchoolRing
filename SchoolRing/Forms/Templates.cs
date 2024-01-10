using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace SchoolRing
{
    public partial class Templates : Form
    {
        Timer timer;
        public Templates()
        {
            InitializeComponent();
            SetLabelsTemplate1(40, 10, 20, 3);
            SetLabelsTemplate2(20, 5, 10, 3);
            if (!Program.HaveBeenIntoMainMenu)
                Program.LastForms.Push(this);
            timer = new Timer();
            timer.Interval = 200;
            timer.Tick += Timer_Tick;
            timer.Start();
            if (Program.HaveBeenIntoMainMenu)
                pictureBox7.Visible = true;
            Program.ShowTheCurrentIcon(pictureBox3);
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeForClockAndText time = new TimeForClockAndText();
            labelDayOfWeek.Text = time.PrintDay();
            Program.ShowTheCurrentIcon(pictureBox3);
        }

        void SetLabelsTemplate1(int classLength, int smallBreakLength, int longBreakLength, int longBreakAfter)
        {
            labelTemplate1ClassLength.Text = classLength.ToString();
            labelTemplate1SmallBreak.Text = smallBreakLength.ToString();
            labelTemplate1LongBreak.Text = longBreakLength.ToString();
            labelTemplate1LongBreakAfter.Text = longBreakAfter.ToString();
        }
        void SetLabelsTemplate2(int classLength, int smallBreakLength, int longBreakLength, int longBreakAfter)
        {
            labelTemplate2ClassLength.Text = classLength.ToString();
            labelTemplate2SmallBreak.Text = smallBreakLength.ToString();
            labelTemplate2LongBreak.Text = longBreakLength.ToString();
            labelTemplate2LongBreakAfter.Text = longBreakAfter.ToString();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            SetUp setUp = new SetUp();
            Program.LastForms.Push(this);
            this.Hide();
            setUp.Show();
        }

        private void Template1_Click(object sender, EventArgs e)
        {
            Program.ClassLength = 40;
            Program.ShortBreakLength = 10;
            Program.LongBreakLength = 20;
            Program.LongBreakAfter = 3;
            if (!Program.HaveBeenIntoMainMenu)
                Log();
            else
                LogIntoMain();
        }

        private void Template2_Click(object sender, EventArgs e)
        {
            Program.ClassLength = 20;
            Program.ShortBreakLength = 5;
            Program.LongBreakLength = 10;
            Program.LongBreakAfter = 3;
            if (!Program.HaveBeenIntoMainMenu)
                Log();
            else
                LogIntoMain();
        }
        void Log()
        {
            this.Hide();
            Login login = new Login();
            login.Show();
            Program.LastForms.Push(this);
        }
        void LogIntoMain()
        {
            this.Hide();
            MainMenu.Instance.Show();
            Program.LastForms.Push(this);
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            Help help = new Help();
            help.Show();
            this.Hide();
        }

        private void Template1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox5.BorderStyle = BorderStyle.FixedSingle;

        }

        private void Template2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox6.BorderStyle = BorderStyle.FixedSingle;

        }

        private void Template1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox5.BorderStyle = BorderStyle.None;
        }

        private void Template2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox6.BorderStyle = BorderStyle.None;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            LogIntoMain();
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
