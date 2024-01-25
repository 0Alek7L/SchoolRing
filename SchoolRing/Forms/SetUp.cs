using System;
using System.Linq;
using System.Windows.Forms;

namespace SchoolRing
{
    public partial class SetUp : Form
    {
        System.Windows.Forms.Timer timer;
        public SetUp()
        {
            InitializeComponent();
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 200;
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeForClockAndText time = new TimeForClockAndText();
            labelDayOfWeek.Text = time.PrintDay();
        }

        private void pictureBox5_Click(object sender, System.EventArgs e)
        {
            Program.LastForms.Peek().Show();
            this.Hide();
        }

        private void buttonContinue_Click(object sender, System.EventArgs e)
        {
            bool problem = false;
            try
            {
                
                if (textBoxClassLength.Text.ToCharArray().Any(x => char.IsDigit(x) == false))
                {

                    textBoxClassLength.Text = string.Empty;
                    throw new ArgumentException("Моля въвеждайте само числа!");
                }
                if (textBoxSmallBreak.Text.ToCharArray().Any(x => char.IsDigit(x) == false))
                {
                    textBoxSmallBreak.Text = string.Empty;
                    throw new ArgumentException("Моля въвеждайте само числа!");
                }
                if (textBoxLongBreak.Text.ToCharArray().Any(x => char.IsDigit(x) == false))
                {
                    textBoxLongBreak.Text = string.Empty;
                    throw new ArgumentException("Моля въвеждайте само числа!");
                }
                if (textBoxLongBreakAfter.Text.ToCharArray().Any(x => char.IsDigit(x) == false))
                {
                    textBoxLongBreakAfter.Text = string.Empty;
                    throw new ArgumentException("Моля въвеждайте само числа!");
                }
                if (string.IsNullOrWhiteSpace(textBoxClassLength.Text) ||
                    string.IsNullOrWhiteSpace(textBoxSmallBreak.Text) ||
                    string.IsNullOrWhiteSpace(textBoxLongBreak.Text) ||
                    string.IsNullOrWhiteSpace(textBoxLongBreakAfter.Text))
                {
                    throw new ArgumentException("Моля въведете всички стойности!");
                }
                if (int.Parse(textBoxClassLength.Text) < 1 || int.Parse(textBoxClassLength.Text) > 90)
                {
                    textBoxClassLength.Text = string.Empty;
                    throw new ArgumentException("Моля въведете стойност между 1 и 90 в полето за продължителност на час!");
                }
                if (int.Parse(textBoxSmallBreak.Text) < 1 || int.Parse(textBoxSmallBreak.Text) > 60)
                {
                    textBoxSmallBreak.Text = string.Empty;
                    throw new ArgumentException("Моля въведете стойност между 1 и 60 в полето за продължителност на малко междучасие!");
                }
                if (int.Parse(textBoxLongBreak.Text) < 1 || int.Parse(textBoxLongBreak.Text) > 60)
                {
                    textBoxLongBreak.Text = string.Empty;
                    throw new ArgumentException("Моля въведете стойност между 1 и 60 в полето за продължителност на голямо междучасие!");
                }
                if (int.Parse(textBoxLongBreakAfter.Text) < 1 || int.Parse(textBoxLongBreakAfter.Text) > 6)
                {
                    textBoxLongBreakAfter.Text = string.Empty;
                    throw new ArgumentException("Моля въведете стойност между 1 и 6 в полето за голямо междучасие след .. час!");
                }
                if(int.Parse(textBoxLongBreak.Text)<int.Parse(textBoxSmallBreak.Text))
                {
                    throw new ArgumentException("Моля въведете стойност за продължителност на малко междучасие, която е по-малка или равна спрямо тази за голямо междучасие!");
                }
                problem = false;
            }
            catch (Exception ex)
            {
                problem = true;
                MessageBox.Show(ex.Message);
            }
            if (!problem)
            {
                Program.ClassLength = int.Parse(textBoxClassLength.Text);
                Program.ShortBreakLength = int.Parse(textBoxSmallBreak.Text);
                Program.LongBreakLength = int.Parse(textBoxLongBreak.Text);
                Program.LongBreakAfter = int.Parse(textBoxLongBreakAfter.Text);
                if (!Program.HaveBeenIntoMainMenu)
                    Log();
                else
                    LogIntoMain();
            }
        }

        void Log()
        {
            Program.LastForms.Push(this);
            this.Hide();
            Login login = new Login();
            login.Show();
        }
        void LogIntoMain()
        {
            this.Hide();
            MainMenu.Instance.Show();
            Program.LastForms.Push(this);
        }
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            this.Hide();
            Help help = new Help();
            help.Show();
            Program.LastForms.Push(this);
        }

        private void buttonContinue_MouseEnter(object sender, EventArgs e)
        {
            buttonContinue.ForeColor = System.Drawing.Color.White;
            buttonContinue.BackColor = System.Drawing.Color.FromArgb(189, 191, 9);
        }

        private void buttonContinue_MouseLeave(object sender, EventArgs e)
        {
            buttonContinue.BackColor = System.Drawing.Color.White;
            buttonContinue.ForeColor = System.Drawing.Color.FromArgb(189, 191, 9);
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
