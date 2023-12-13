using SchoolRing.Interfaces;
using SchoolRing.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace SchoolRing
{
    public partial class Melody : Form
    {
        System.Windows.Forms.Timer timer;
        System.Windows.Forms.Timer timerForFixedLength;

        public Melody()
        {
            InitializeComponent();
            timer = new System.Windows.Forms.Timer();
            timerForFixedLength = new System.Windows.Forms.Timer();
            timer.Interval = 50;
            if (Program.fixedMelodyLength != 0)
                timerForFixedLength.Interval = Program.fixedMelodyLength * 1000;
            timer.Tick += Timer_Tick;
            timer.Tick += Timer_Tick1;
            timer.Tick += Timer_Tick2;
            timer.Tick += Timer_Tick3;
            timer.Start();
            playStart.Focus();
            Program.ShowTheCurrentIcon(pictureBox3);
        }

        private void Timer_Tick3(object sender, EventArgs e)
        {
            if (MelodyFiles.isPlayingStart)
            {
                playStart.Enabled = false;
                pauseStart.Enabled = true;
                playStart.Image = Properties.Resources.play_gray;
                pauseStart.Image = Properties.Resources.pause_dark;
                playEnd.Enabled = false;
                playEnd.Image = Properties.Resources.play_gray;
            }
            else if (MelodyFiles.isPlayingEnd)
            {
                playEnd.Enabled = false;
                pauseEnd.Enabled = true;
                playEnd.Image = Properties.Resources.play_gray;
                pauseEnd.Image = Properties.Resources.pause_dark;
                playStart.Enabled = false;
                playStart.Image = Properties.Resources.play_gray;
            }
            else
            {
                playStart.Enabled = true;
                pauseStart.Enabled = false;
                playEnd.Enabled = true;
                pauseEnd.Enabled = false;
                playStart.Image = Properties.Resources.play_dark;
                pauseStart.Image = Properties.Resources.pause_gray;
                playEnd.Image = Properties.Resources.play_dark;
                pauseEnd.Image = Properties.Resources.pause_gray;
            }
            if (Program.melodyForStartOfClassPath != null)
            {
                labelStartClassMelodyName.Text = Path.GetFileName(Program.melodyForStartOfClassPath);
                labelStartClassMelodyName.ForeColor= Color.Black;
            }
            else
            {
                labelStartClassMelodyName.Text = "НЯМА ИЗБРАН ФАЙЛ!";
                labelStartClassMelodyName.ForeColor= Color.Red;
            }
            if (Program.melodyForEndOfClassPath != null)
            {
                labelEndClassMelodyName.Text = Path.GetFileName(Program.melodyForEndOfClassPath);
                labelEndClassMelodyName.ForeColor = Color.Black;
            }
            else
            {
                labelEndClassMelodyName.Text = "НЯМА ИЗБРАН ФАЙЛ!";
                labelEndClassMelodyName.ForeColor = Color.Red;
            }

            if (textBoxMaxMelodyLength.Focused)
                textBoxMaxMelodyLength.BackColor = Color.White;
            else
                textBoxMaxMelodyLength.BackColor = Color.Gray;
        }

        private void Timer_Tick1(object sender, EventArgs e)
        {
            if (labelStartClassMelodyName.Text.ToCharArray().Count() <= 20)
            {
                labelStartClassMelodyName.AutoSize = false;
                labelStartClassMelodyName.Left = 6;
            }
            else
            {
                labelStartClassMelodyName.AutoSize = true;
                labelStartClassMelodyName.Left -= 2;
                if (labelStartClassMelodyName.Right < 200 - labelStartClassMelodyName.Width)
                {
                    labelStartClassMelodyName.Left = 200 + (labelNachaloNaChas.Width / 4);
                }
            }
        }
        private void Timer_Tick2(object sender, EventArgs e)
        {
            if (labelEndClassMelodyName.Text.ToCharArray().Count() <= 20)
            {
                labelEndClassMelodyName.AutoSize = false;
                labelEndClassMelodyName.Left = 17;
            }
            else
            {
                labelEndClassMelodyName.AutoSize = true;
                labelEndClassMelodyName.Left -= 2;
                if (labelEndClassMelodyName.Right < 235 - labelKraiNaChas.Width)
                {
                    labelEndClassMelodyName.Left = 205 + (labelKraiNaChas.Width / 3);
                }
            }
            Program.ShowTheCurrentIcon(pictureBox3);
        }
        private void labelChooseFileForStartOfClassButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Аудио файлове|*.mp3;*.wav;*.ogg;*.flac";
                    openFileDialog.Title = "Избери аудио файл";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string audioFilePath = openFileDialog.FileName;
                        string extension = Path.GetExtension(audioFilePath).ToLower();
                        if (extension == ".mp3" || extension == ".wav" || extension == ".ogg" || extension == ".flac")
                        {
                            Program.melodyForStartOfClassPath = audioFilePath;
                            if (Program.melodyForStartOfClassPath != null)
                                labelStartClassMelodyName.Text = Path.GetFileName(Program.melodyForStartOfClassPath);
                            MelodyFiles.Reinitialise();
                        }
                        else
                        {
                            throw new ArgumentException("Файлът е с невалидно разширение!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void labelChooseFileForEndOfClassButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Аудио файлове|*.mp3;*.wav;*.ogg;*.flac";
                    openFileDialog.Title = "Избери аудио файл";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string audioFilePath = openFileDialog.FileName;
                        string extension = Path.GetExtension(audioFilePath).ToLower();
                        if (extension == ".mp3" || extension == ".wav" || extension == ".ogg" || extension == ".flac")
                        {
                            Program.melodyForEndOfClassPath = audioFilePath;
                            if (Program.melodyForEndOfClassPath != null)
                                labelEndClassMelodyName.Text = Path.GetFileName(Program.melodyForEndOfClassPath);
                            MelodyFiles.Reinitialise();
                        }
                        else
                        {
                            throw new ArgumentException("Файлът е с невалидно разширение!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeForClockAndText time = new TimeForClockAndText();
            labelDayOfWeek.Text = time.PrintDay();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            Help help = new Help();
            help.Show();
            this.Hide();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            SaveTheData.SaveProperties();
            Program.LastForms.Pop().Show();
            this.Close();
        }

        private void labelChooseFileForEndOfClassButton_MouseEnter(object sender, EventArgs e) =>
            labelChooseFileForEndOfClassButton.BorderStyle = BorderStyle.FixedSingle;

        private void labelChooseFileForEndOfClassButton_MouseLeave(object sender, EventArgs e) =>
            labelChooseFileForEndOfClassButton.BorderStyle = BorderStyle.None;

        private void labelChooseFileForStartOfClassButton_MouseEnter(object sender, EventArgs e) =>
            labelChooseFileForStartOfClassButton.BorderStyle = BorderStyle.FixedSingle;

        private void labelChooseFileForStartOfClassButton_MouseLeave(object sender, EventArgs e) =>
            labelChooseFileForStartOfClassButton.BorderStyle = BorderStyle.None;

        private void playStart_Click(object sender, EventArgs e)
        {
            try
            {
                MelodyFiles.PlayStartAsync();
                playStart.Image = Properties.Resources.play_gray;
                playStart.Enabled = false;
                pauseStart.Image = Properties.Resources.pause_dark;
                pauseStart.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void pauseStart_Click(object sender, EventArgs e)
        {
            try
            {
                playStart.Image = Properties.Resources.play_dark;
                playStart.Enabled = true;
                pauseStart.Image = Properties.Resources.pause_gray;
                pauseStart.Enabled = false;
                MelodyFiles.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void playEnd_Click(object sender, EventArgs e)
        {
            try
            {
                playEnd.Image = Properties.Resources.play_gray;
                playEnd.Enabled = false;
                pauseEnd.Image = Properties.Resources.pause_dark;
                pauseEnd.Enabled = true;
                MelodyFiles.PlayEndAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void pauseEnd_Click(object sender, EventArgs e)
        {
            try
            {
                playEnd.Image = Properties.Resources.play_dark;
                playEnd.Enabled = true;
                pauseEnd.Image = Properties.Resources.pause_gray;
                pauseEnd.Enabled = false;
                MelodyFiles.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void pictureBoxButtonSetMaxLength_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBoxMaxMelodyLength.Text.Length > 0)
                {
                    if (textBoxMaxMelodyLength.Text.ToCharArray().Any(x => char.IsDigit(x) == false))
                        throw new ArgumentException("Моля въвеждайте само числа!");
                    if (int.Parse(textBoxMaxMelodyLength.Text) >= 0)
                    {
                        Program.fixedMelodyLength = int.Parse(textBoxMaxMelodyLength.Text);
                        if (int.Parse(textBoxMaxMelodyLength.Text) > 0)
                            MessageBox.Show($"Звънецът ще звъни {textBoxMaxMelodyLength.Text} секунди.");
                        else
                            MessageBox.Show($"Звънецът няма ограничение на звъненето.");
                        textBoxMaxMelodyLength.Clear();
                        playStart.Focus();
                        MelodyFiles.Reinitialise();
                    }
                    else
                        throw new ArgumentException("Моля въведете число между 1 и 99 в текстовото поле!");

                }
                else
                    throw new ArgumentException("Моля въведете число между 1 и 99 в текстовото поле!");
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void pictureBoxHelpWithFixedMelodyLength_MouseEnter(object sender, EventArgs e)=>
            pictureBoxHelpWithFixedMelodyLength.BorderStyle= BorderStyle.Fixed3D;

        private void pictureBoxHelpWithFixedMelodyLength_MouseLeave(object sender, EventArgs e)=>
            pictureBoxHelpWithFixedMelodyLength.BorderStyle= BorderStyle.FixedSingle;

        private void pictureBoxHelpWithFixedMelodyLength_Click(object sender, EventArgs e)
        {
            pictureBoxHelpWithFixedMelodyLength.BorderStyle= BorderStyle.FixedSingle;
            if (Program.fixedMelodyLength == 0)
                MessageBox.Show("В момента няма ограничение на времето за звънене.");
            else
                MessageBox.Show($"В момента ограничението на времето за звънене е {Program.fixedMelodyLength} секунди. Въведете \"0\" за да премахнете ограничението.");

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
