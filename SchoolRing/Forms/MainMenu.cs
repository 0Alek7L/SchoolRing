using NAudio.Wave;
using SchoolRing.Forms;
using SchoolRing.Interfaces;
using SchoolRing.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media.Animation;

namespace SchoolRing
{
    public partial class MainMenu : Form
    {
        private static MainMenu instance;
        System.Windows.Forms.Timer timer;
        System.Windows.Forms.Timer timerForMelody;
        List<ISchoolClass> classesLeftAsObjects;
        public MainMenu()
        {
            InitializeComponent();
            //AskForMelody();
            Program.HaveBeenIntoMainMenu = true;
            //SaveTheData.SaveSchoolClasses();
            timer = new System.Windows.Forms.Timer();
            timerForMelody = new System.Windows.Forms.Timer();
            timer.Interval = 50;
            timerForMelody.Interval = 200;//501
            timer.Tick += Timer_Tick;
            timer.Tick += Timer_TickForMovingLabel;
            //timer.Tick += Timer_TickForSpecialTasks;
            timerForMelody.Tick += Timer_TickForMelody;
            timer.Start();
            timerForMelody.Start();
            Program.LastForms.Clear();
            currentClass = null;
            nextClass = null;
            labelShowNextClass.Text = "-";
            labelShowCurrentClass.Text = "-";
            labelShowClassesLeft.Text = "-";
            labelShowTimeLeft.Text = "00:00:00";
            classesLeftAsObjects = new List<ISchoolClass>();
            if (Program.allowRinging)
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
            }
            try
            {
                if (Program.melodyForStartOfClassPath.Length <= 0)
                {
                    Program.allowRinging = false;
                    Program.melodyForStartOfClassPath = null;
                }
                if (Program.melodyForEndOfClassPath.Length <= 0)
                {
                    Program.allowRinging = false;
                    Program.melodyForEndOfClassPath = null;
                }
            }
            catch
            {
                Program.allowRinging = false;
                Program.melodyForStartOfClassPath = null;
                Program.melodyForEndOfClassPath = null;

            }
            Program.ShowTheCurrentIcon(pictureBox3);

        }


        public static MainMenu Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new MainMenu();
                }

                return instance;
            }
        }

        // Override method to handle form visibility
        public new void Show()
        {
            if (IsDisposed)
            {
                instance = new MainMenu();
            }

            base.Show();
        }

        public bool hasRangForStart = false;
        public bool hasRangForEnd = false;
        private void Timer_TickForMelody(object sender, EventArgs e)
        {
            if (Program.allowRinging)
                checkBox1.Checked = true;
            else
                checkBox1.Checked = false;
            Program.ShowTheCurrentIcon(pictureBox3);
            try
            {
                if (currentClass != null && Program.allowRinging)
                {
                    TimeSpan currentTime = new TimeSpan(int.Parse(labelShowHours.Text), int.Parse(labelShowMinutes.Text), int.Parse(labelShowSeconds.Text));
                    if (!hasRangForEnd && !hasRangForStart && Program.allowRinging)
                    {
                        if (currentTime == new TimeSpan(currentClass.StartHours, currentClass.StartMinutes, 0))
                        {
                            if (Program.melodyForStartOfClassPath == null)
                            {
                                System.Windows.Forms.MessageBox.Show("Не е избрана мелодия за начало на час!");
                                checkBox1.Checked = false;
                                Program.allowRinging = false;
                            }
                            if (!currentClass.IsMerged && !hasRangForStart && Program.allowRinging)//change2
                            {
                                MelodyFiles.Reinitialise();
                                MelodyFiles.PlayStartAsync();
                                hasRangForStart = true;
                            }

                        }

                        else if (currentTime == new TimeSpan(currentClass.EndHours, currentClass.EndMinutes, 0))
                        {
                            if (Program.melodyForEndOfClassPath == null)
                            {
                                System.Windows.Forms.MessageBox.Show("Не е избрана мелодия за край на час!");
                                checkBox1.Checked = false;
                                Program.allowRinging = false;
                            }
                            if (!currentClass.IsMerged && !hasRangForEnd && Program.allowRinging)//change
                            {
                                MelodyFiles.Reinitialise();
                                MelodyFiles.PlayEndAsync();
                                hasRangForEnd = true;
                            }
                        }
                    }
                    if (currentTime != new TimeSpan(currentClass.StartHours, currentClass.StartMinutes, 0) && !currentClass.IsMerged)
                    {
                        hasRangForStart = false;
                    }
                    if (currentTime != new TimeSpan(currentClass.EndHours, currentClass.EndMinutes, 0) && !currentClass.IsMerged)
                    {
                        hasRangForEnd = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Грешка: " + ex.Message);
            }
            currentClass = null;
            nextClass = null;
        }

        ISchoolClass currentClass;
        ISchoolClass nextClass;
        private void Timer_TickForMovingLabel(object sender, EventArgs e)
        {
            SaveTheData.SaveProperties();

            //temp for unchecking on Load
            if (!Program.allowRinging)
                checkBox1.Checked = false;
            //---

            labelForHolidays.Left -= 3;
            if (labelForHolidays.Right < 0)
            {
                labelForHolidays.Left = this.ClientSize.Width;
            }
            Holidays holidays = new Holidays(labelForHolidays);



            if (holidays.IsTodayHoliday() || Program.vdRepo.IsTodayVacation())
            {
                if (Program.vdRepo.IsTodayVacation())
                {
                    DateTime dateTime = DateTime.Now;
                    labelForVacation.Show();
                    labelForHolidays.Text = $"{Program.vdRepo.GetModels().First(x => x.StartDate.Date <= dateTime.Date && x.EndDate.Date >= dateTime.Date).Argument}";
                    labelShowClassesLeft.Text = "-";
                    labelShowCurrentClass.Text = "-";
                    labelShowNextClass.Text = "-";
                    labelShowTimeLeft.Text = "00:00:00";
                }
                labelForHolidays.Show();
                pictureBoxForHolidays.Show();
            }
            else
            {
                labelForVacation.Hide();
                pictureBoxForHolidays.Hide();
                labelForHolidays.Hide();
            }
            //Merging classes label show and hide
            if (Program.WithClassSchedule)
            {
                //printer
                pictureBoxPrinter.Show();
                labelPrinter.Show();
                //search
                pictureBoxSearchButton.Show();
                labelSearch.Show();
                //mergeClasses
                labelMergeClasses.Show();
                //customClasses
                pictureBoxCustomClasses.Show();
                labelCustomClasses.Show();
            }
            else
            {
                //printer
                pictureBoxPrinter.Hide();
                labelPrinter.Hide();
                //search
                pictureBoxSearchButton.Hide();
                labelSearch.Hide();
                //mergeClasses
                labelMergeClasses.Hide();
                //customClasses
                pictureBoxCustomClasses.Hide();
                labelCustomClasses.Hide();
            }

        }

        private void UpdateLabelsWithClassSchedule()
        {

            TimeForClockAndText day = new TimeForClockAndText();
            TimeSpan currentTime = new TimeSpan(int.Parse(labelShowHours.Text), int.Parse(labelShowMinutes.Text), int.Parse(labelShowSeconds.Text));

            if (day.PrintDayOnly() != "СЪБОТА" || day.PrintDayOnly() != "НЕДЕЛЯ" || !Program.vdRepo.IsTodayVacation())
            {

                if (currentTime.Hours is 7 && currentTime.Minutes < 30)
                {
                    nextClass = Program.GetRecord(day.PrintDayOnly(), 1, true);
                    currentClass = null;
                    labelShowTimeLeft.Text = "00:00:00";
                }
                foreach (var klas in Program.GetModels().Where(c => c.Day == day.PrintDayOnly()))
                {
                    TimeSpan tempClassStart = new TimeSpan(klas.StartHours, klas.StartMinutes, 0);
                    TimeSpan tempClassEnd = new TimeSpan(klas.EndHours, klas.EndMinutes, 0);
                    bool isThisTheRecord = false;
                    if (tempClassStart <= currentTime && currentTime <= tempClassEnd)
                        isThisTheRecord = true;
                    if (tempClassStart <= currentTime && currentTime <= tempClassEnd.Add(TimeSpan.FromMinutes(Program.ShortBreakLength)) && klas.Num != Program.LongBreakAfter)
                        isThisTheRecord = true;
                    if (tempClassStart <= currentTime && currentTime <= tempClassEnd.Add(TimeSpan.FromMinutes(Program.LongBreakLength)) && klas.Num == Program.LongBreakAfter)
                        isThisTheRecord = true;
                    if (isThisTheRecord is true && !klas.IsMerged)
                    {
                        currentClass = klas;
                        if (currentClass != null && currentClass.IsPurvaSmqna && currentClass.Num < 7)
                        {
                            if (Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 1, true).IsMerged &&
                                Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 1, true).Num < 6)
                                nextClass = Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 2, true);

                            if (Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 1, true).IsMerged &&
                                Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 1, true).Num == 7)
                                nextClass = Program.GetRecord(day.PrintDayOnly(), 1, false);

                            if (!Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 1, true).IsMerged)
                                nextClass = Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 1, true);
                        }
                        if (currentClass != null && currentClass.IsPurvaSmqna && currentClass.Num == 7)
                        {
                            if (Program.GetRecord(day.PrintDayOnly(), 1, false).IsMerged)
                                nextClass = Program.GetRecord(day.PrintDayOnly(), 2, false);
                            if (!Program.GetRecord(day.PrintDayOnly(), 1, false).IsMerged)
                                nextClass = Program.GetRecord(day.PrintDayOnly(), 1, false);
                        }
                        if (currentClass != null && !currentClass.IsPurvaSmqna && currentClass.Num < 7)
                        {
                            if (Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 1, false).IsMerged &&
                                Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 1, false).Num < 7)
                                nextClass = Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 2, false);
                            if (Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 1, false).IsMerged &&
                                Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 1, false).Num == 7)
                                nextClass = null;
                            if (!Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 1, false).IsMerged)
                                nextClass = Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 1, false);
                        }
                        if (currentClass != null && !currentClass.IsPurvaSmqna && currentClass.Num == 7)
                            nextClass = null;
                    }
                    //else if(isThisTheRecord)
                    //{
                    //    currentClass = klas;
                    //}

                }
                if (currentClass != null)
                {
                    TimeSpan tempClassStart = new TimeSpan(currentClass.StartHours, currentClass.StartMinutes, 0);
                    TimeSpan tempClassEnd = new TimeSpan(currentClass.EndHours, currentClass.EndMinutes, 0);
                    if (currentTime > tempClassEnd)
                    {
                        labelShowCurrentClass.ForeColor = Color.Red;
                    }
                    else
                    {
                        labelShowCurrentClass.ForeColor = Color.FromArgb(245, 239, 237);
                    }
                    if (currentTime >= tempClassEnd.Add(TimeSpan.FromMinutes(Program.ShortBreakLength)) && currentClass.Num == 7 && !currentClass.IsPurvaSmqna)
                    {
                        labelShowCurrentClass.ForeColor = Color.FromArgb(245, 239, 237);
                        currentClass = null;
                    }
                }

                //за показване на оставащи часове
                int obligedClasses = 0;
                if (currentClass != null)
                {
                    foreach (var klas in Program.GetModels().Where(x => x.Day == day.PrintDayOnly()))
                    {
                        if (currentClass.IsPurvaSmqna && !klas.IsMerged)
                        {
                            if (!klas.IsFree && klas.Num > currentClass.Num && klas.IsPurvaSmqna)
                            {
                                obligedClasses++;
                                classesLeftAsObjects.Add(klas);
                            }
                            if (!klas.IsFree && !klas.IsPurvaSmqna)
                            {
                                obligedClasses++;
                                classesLeftAsObjects.Add(klas);
                            }
                        }
                        else
                        {
                            if (!klas.IsFree && klas.Num > currentClass.Num && !klas.IsPurvaSmqna && !klas.IsMerged)
                                obligedClasses++;
                        }
                    }
                }
                else if (nextClass != null)
                {
                    foreach (var klas in Program.GetModels().Where(x => x.Day == day.PrintDayOnly()))
                    {
                        if (nextClass.IsPurvaSmqna)
                        {
                            if (!klas.IsFree && klas.Num >= nextClass.Num && klas.IsPurvaSmqna)
                            {
                                obligedClasses++;
                                classesLeftAsObjects.Add(klas);
                            }
                            if (!klas.IsFree && !klas.IsPurvaSmqna)
                            {
                                obligedClasses++;
                                classesLeftAsObjects.Add(klas);
                            }
                        }
                        else
                        {
                            if (!klas.IsFree && klas.Num > nextClass.Num && !klas.IsPurvaSmqna)
                            {
                                classesLeftAsObjects.Add(klas);
                                obligedClasses++;
                            }
                        }
                    }
                }
                if (currentClass != null && currentTime >= new TimeSpan(7, 29, 59))
                {
                    TimeSpan tempCurrentClassEnd = new TimeSpan(currentClass.EndHours, currentClass.EndMinutes, 0);
                    TimeSpan diff;
                    if (nextClass != null)
                    {
                        TimeSpan tempNextClassStart = new TimeSpan(nextClass.StartHours, nextClass.StartMinutes, 0);

                        if (currentTime <= tempCurrentClassEnd)
                        {
                            diff = tempCurrentClassEnd - currentTime;
                            if (diff.Hours == 0 && diff.Minutes == 0)
                            {
                                labelShowTimeLeft.ForeColor = Color.Red;
                            }
                            else
                                labelShowTimeLeft.ForeColor = Color.FromArgb(245, 239, 237);
                            labelShowTimeLeft.Text = $"{diff.Hours:D2}:{diff.Minutes:D2}:{diff.Seconds:D2}";
                        }
                        else if (currentTime >= tempCurrentClassEnd && currentTime <= tempNextClassStart)
                        {
                            diff = tempNextClassStart - currentTime;
                            if (diff.Hours == 0 && diff.Minutes == 0)
                            {
                                labelShowTimeLeft.ForeColor = Color.Red;
                            }
                            else
                                labelShowTimeLeft.ForeColor = Color.FromArgb(245, 239, 237);
                            labelShowTimeLeft.Text = $"{diff.Hours:D2}:{diff.Minutes:D2}:{diff.Seconds:D2}";
                        }
                    }
                    else
                    {
                        if (currentTime <= tempCurrentClassEnd)
                        {
                            diff = tempCurrentClassEnd - currentTime;
                            if (diff.Hours == 0 && diff.Minutes == 0)
                            {
                                labelShowTimeLeft.ForeColor = Color.Red;
                            }
                            else
                                labelShowTimeLeft.ForeColor = Color.FromArgb(245, 239, 237);
                            labelShowTimeLeft.Text = $"{diff.Hours:D2}:{diff.Minutes:D2}:{diff.Seconds:D2}";
                        }
                    }


                }
                else
                {
                    labelShowClassesLeft.Text = "-";
                    labelShowCurrentClass.Text = "-";
                    labelShowNextClass.Text = "-";
                    labelShowTimeLeft.Text = "00:00:00";
                }


                if (currentClass != null)
                    labelShowCurrentClass.Text = currentClass.GetClassGradeAndParalelka();
                else
                    labelShowCurrentClass.Text = "-";
                if (nextClass != null)
                    labelShowNextClass.Text = nextClass.GetClassGradeAndParalelka();
                else
                    labelShowNextClass.Text = "-";
                labelShowClassesLeft.Text = obligedClasses.ToString();
            }
            else
            {
                currentClass = null;
                nextClass = null;
                labelShowClassesLeft.Text = "-";
                labelShowCurrentClass.Text = "-";
                labelShowNextClass.Text = "-";
                labelShowTimeLeft.Text = "00:00:00";
            }

        }

        private void UpdateLabelsWithoutClassSchedule()
        {
            TimeForClockAndText day = new TimeForClockAndText();
            TimeSpan currentTime = new TimeSpan(int.Parse(labelShowHours.Text), int.Parse(labelShowMinutes.Text), int.Parse(labelShowSeconds.Text));

            if (day.PrintDayOnly() != "СЪБОТА" || day.PrintDayOnly() != "НЕДЕЛЯ" || !Program.vdRepo.IsTodayVacation())
            {

                if (currentTime.Hours is 7 && currentTime.Minutes < 30)
                {
                    nextClass = Program.GetRecord(day.PrintDayOnly(), 1, true);
                    currentClass = null;
                    labelShowTimeLeft.Text = "00:00:00";
                }
                foreach (var klas in Program.GetModels().Where(c => c.Day == day.PrintDayOnly()))
                {
                    TimeSpan tempClassStart = new TimeSpan(klas.StartHours, klas.StartMinutes, 0);
                    TimeSpan tempClassEnd = new TimeSpan(klas.EndHours, klas.EndMinutes, 0);
                    bool isThisTheRecord = false;
                    if (tempClassStart <= currentTime && currentTime <= tempClassEnd)
                        isThisTheRecord = true;
                    if (tempClassStart <= currentTime && currentTime <= tempClassEnd.Add(TimeSpan.FromMinutes(Program.ShortBreakLength)) && klas.Num != Program.LongBreakAfter)
                        isThisTheRecord = true;
                    if (tempClassStart <= currentTime && currentTime <= tempClassEnd.Add(TimeSpan.FromMinutes(Program.LongBreakLength)) && klas.Num == Program.LongBreakAfter)
                        isThisTheRecord = true;
                    if (isThisTheRecord is true)
                    {
                        currentClass = klas;
                        if (currentClass.IsPurvaSmqna && currentClass.Num < 7)
                            nextClass = Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 1, true);
                        if (currentClass.IsPurvaSmqna && currentClass.Num == 7)
                            nextClass = Program.GetRecord(day.PrintDayOnly(), 1, false);
                        if (!currentClass.IsPurvaSmqna && currentClass.Num < 7)
                            nextClass = Program.GetRecord(day.PrintDayOnly(), currentClass.Num + 1, false);
                        if (!currentClass.IsPurvaSmqna && currentClass.Num == 7)
                            nextClass = null;
                    }

                }
                if (currentClass != null)
                {
                    TimeSpan tempClassStart = new TimeSpan(currentClass.StartHours, currentClass.StartMinutes, 0);
                    TimeSpan tempClassEnd = new TimeSpan(currentClass.EndHours, currentClass.EndMinutes, 0);
                    if (currentTime > tempClassEnd)
                    {
                        labelShowCurrentClass.ForeColor = Color.Red;
                    }
                    else
                    {
                        labelShowCurrentClass.ForeColor = Color.FromArgb(245, 239, 237);
                    }
                    if (currentTime >= tempClassEnd.Add(TimeSpan.FromMinutes(Program.ShortBreakLength)) && currentClass.Num == 7 && !currentClass.IsPurvaSmqna)
                    {
                        labelShowCurrentClass.ForeColor = Color.FromArgb(245, 239, 237);
                        currentClass = null;
                    }
                }

                //за показване на оставащи часове
                int obligedClasses = 0;
                if (currentClass != null)
                {
                    foreach (var klas in Program.GetModels().Where(x => x.Day == day.PrintDayOnly()))
                    {
                        if (currentClass.IsPurvaSmqna)
                        {
                            if (klas.Num > currentClass.Num && klas.IsPurvaSmqna)
                                obligedClasses++;
                            if (!klas.IsPurvaSmqna)
                                obligedClasses++;
                        }
                        else
                        {
                            if (klas.Num > currentClass.Num && !klas.IsPurvaSmqna)
                                obligedClasses++;
                        }
                    }
                }
                else if (nextClass != null)
                {
                    foreach (var klas in Program.GetModels().Where(x => x.Day == day.PrintDayOnly()))
                    {
                        if (nextClass.IsPurvaSmqna)
                        {
                            if (klas.Num >= nextClass.Num && klas.IsPurvaSmqna)
                                obligedClasses++;
                            if (!klas.IsPurvaSmqna)
                                obligedClasses++;
                        }
                        else
                        {
                            if (klas.Num > nextClass.Num && !klas.IsPurvaSmqna)
                                obligedClasses++;
                        }
                    }
                }
                if (currentClass != null && currentTime >= new TimeSpan(7, 29, 59))
                {
                    TimeSpan tempCurrentClassEnd = new TimeSpan(currentClass.EndHours, currentClass.EndMinutes, 0);
                    TimeSpan diff;
                    if (nextClass != null)
                    {
                        TimeSpan tempNextClassStart = new TimeSpan(nextClass.StartHours, nextClass.StartMinutes, 0);

                        if (currentTime <= tempCurrentClassEnd)
                        {
                            diff = tempCurrentClassEnd - currentTime;
                            if (diff.Hours == 0 && diff.Minutes == 0)
                            {
                                //if (diff.Seconds == 0)
                                //    MessageBox.Show("End");
                                labelShowTimeLeft.ForeColor = Color.Red;
                            }
                            else
                                labelShowTimeLeft.ForeColor = Color.FromArgb(245, 239, 237);
                            labelShowTimeLeft.Text = $"{diff.Hours:D2}:{diff.Minutes:D2}:{diff.Seconds:D2}";
                        }
                        else if (currentTime >= tempCurrentClassEnd && currentTime <= tempNextClassStart)
                        {
                            diff = tempNextClassStart - currentTime;
                            if (diff.Hours == 0 && diff.Minutes == 0)
                            {
                                //if (diff.Seconds == 0)
                                //    MessageBox.Show("End");
                                labelShowTimeLeft.ForeColor = Color.Red;
                            }
                            else
                                labelShowTimeLeft.ForeColor = Color.FromArgb(245, 239, 237);
                            labelShowTimeLeft.Text = $"{diff.Hours:D2}:{diff.Minutes:D2}:{diff.Seconds:D2}";
                        }
                    }
                    else
                    {
                        if (currentTime <= tempCurrentClassEnd)
                        {
                            diff = tempCurrentClassEnd - currentTime;
                            if (diff.Hours == 0 && diff.Minutes == 0)
                            {
                                //if (diff.Seconds == 0)
                                //    MessageBox.Show("End");
                                labelShowTimeLeft.ForeColor = Color.Red;
                            }
                            else
                                labelShowTimeLeft.ForeColor = Color.FromArgb(245, 239, 237);
                            labelShowTimeLeft.Text = $"{diff.Hours:D2}:{diff.Minutes:D2}:{diff.Seconds:D2}";
                        }
                    }
                }
                else
                {
                    labelShowClassesLeft.Text = "-";
                    labelShowCurrentClass.Text = "-";
                    labelShowNextClass.Text = "-";
                    labelShowTimeLeft.Text = "00:00:00";
                }


                if (currentClass != null)
                    labelShowCurrentClass.Text = currentClass.Num.ToString();
                else
                    labelShowCurrentClass.Text = "-";
                if (nextClass != null)
                    labelShowNextClass.Text = nextClass.Num.ToString();
                else
                    labelShowNextClass.Text = "-";
                labelShowClassesLeft.Text = obligedClasses.ToString();
            }
            else
            {
                currentClass = null;
                nextClass = null;
                labelShowClassesLeft.Text = "-";
                labelShowCurrentClass.Text = "-";
                labelShowNextClass.Text = "-";
                labelShowTimeLeft.Text = "00:00:00";
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeForClockAndText time = new TimeForClockAndText();
            labelShowHours.Text = $"{Program.time.PrintHour():D2}";
            labelShowMinutes.Text = $"{Program.time.PrintMinutes():D2}";
            labelShowSeconds.Text = $"{Program.time.PrintSeconds():D2}";
            labelDayOfWeek.Text = Program.time.PrintDay();
            if (!Program.vdRepo.IsTodayVacation())
            {
                if (Program.WithClassSchedule)
                    UpdateLabelsWithClassSchedule();
                else
                    UpdateLabelsWithoutClassSchedule();
            }
            if (this.Visible)
            {
                timerForMelody.Enabled = true;
                timer.Enabled = true;
            }
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            Help help = new Help();
            help.Show();
            this.Hide();
        }

        private void pictureBoxProgram_Click(object sender, EventArgs e)
        {

            if (Program.WithClassSchedule is false)
            {
                DialogResult doYouWantToUseSchedule = MessageBox.Show("Сигурни ли сте, че желаете да използвате учебен график за учители?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (doYouWantToUseSchedule == DialogResult.Yes)
                {
                    SchoolProgram schoolProgram = new SchoolProgram();
                    if (Program.GetModels().Any(x => x.IsFree == false))
                    {
                        DialogResult ThereIsAlreadyASchedule = MessageBox.Show("Вече има програма с въведени часове. Желаете ли да я използвате?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (ThereIsAlreadyASchedule == DialogResult.Yes)
                        {
                            Program.LastForms.Push(this);
                            schoolProgram.Show();
                            this.Hide();
                        }
                        else
                        {
                            Program.ClearTheSchedule();
                            Program.LastForms.Push(this);
                            schoolProgram.Show();
                            this.Hide();
                        }
                    }
                    Program.LastForms.Push(this);
                    schoolProgram.Show();
                    this.Hide();
                }
            }
            else
            {
                Program.LastForms.Push(this);
                SchoolProgram schoolProgram = new SchoolProgram();
                schoolProgram.Show();
                this.Hide();
            }

        }

        private void labelChangeClassLength_MouseEnter(object sender, EventArgs e)
        {
            labelChangeClassLength.BackColor = Color.FromArgb(34, 146, 164);
            labelChangeClassLength.ForeColor = Color.FromArgb(245, 239, 237);
            labelChangeClassLength.BorderStyle = BorderStyle.FixedSingle;
        }

        private void labelChangeClassLength_MouseLeave(object sender, EventArgs e)
        {
            labelChangeClassLength.BackColor = Color.FromArgb(245, 239, 237);
            labelChangeClassLength.ForeColor = Color.FromArgb(34, 146, 164);
            labelChangeClassLength.BorderStyle = BorderStyle.Fixed3D;

        }
        private void labelMergeClasses_MouseEnter(object sender, EventArgs e)
        {
            labelMergeClasses.BackColor = Color.FromArgb(34, 146, 164);
            labelMergeClasses.ForeColor = Color.FromArgb(245, 239, 237);
            labelMergeClasses.BorderStyle = BorderStyle.FixedSingle;

        }

        private void labelMergeClasses_MouseLeave(object sender, EventArgs e)
        {
            labelMergeClasses.BackColor = Color.FromArgb(245, 239, 237);
            labelMergeClasses.ForeColor = Color.FromArgb(34, 146, 164);
            labelMergeClasses.BorderStyle = BorderStyle.Fixed3D;
        }

        private void labelChangeClassLength_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            Templates templates = new Templates();
            templates.Show();
            this.Hide();
        }

        private void pictureBoxHelpNextClass_MouseEnter(object sender, EventArgs e) =>
            pictureBoxHelpNextClass.BorderStyle = BorderStyle.Fixed3D;


        private void pictureBoxHelpNextClass_MouseLeave(object sender, EventArgs e) =>
            pictureBoxHelpNextClass.BorderStyle = BorderStyle.FixedSingle;

        private void pictureBoxHelpCurrentClass_MouseEnter(object sender, EventArgs e) =>
            pictureBoxHelpCurrentClass.BorderStyle = BorderStyle.Fixed3D;

        private void pictureBoxHelpCurrentClass_MouseLeave(object sender, EventArgs e) =>
            pictureBoxHelpCurrentClass.BorderStyle = BorderStyle.FixedSingle;

        private void pictureBoxHelpClassesLeft_MouseEnter(object sender, EventArgs e) =>
            pictureBoxHelpClassesLeft.BorderStyle = BorderStyle.Fixed3D;

        private void pictureBoxHelpClassesLeft_MouseLeave(object sender, EventArgs e) =>
            pictureBoxHelpClassesLeft.BorderStyle = BorderStyle.FixedSingle;

        private void pictureBoxHelpTimeLeft_MouseLeave(object sender, EventArgs e) =>
            pictureBoxHelpTimeLeft.BorderStyle = BorderStyle.FixedSingle;

        private void pictureBoxHelpTimeLeft_MouseEnter(object sender, EventArgs e) =>
            pictureBoxHelpTimeLeft.BorderStyle = BorderStyle.Fixed3D;

        private void pictureBoxHelpClassesLeft_Click(object sender, EventArgs e)
        {
            if (Program.WithClassSchedule)
                MessageBox.Show("В това поле се показва броят на заетите часове за деня. Свободните не се броят!");
            else
                MessageBox.Show("В това поле се показва броят на оставащите часове за деня.");
            pictureBoxHelpClassesLeft.BorderStyle = BorderStyle.FixedSingle;
        }

        private void pictureBoxHelpCurrentClass_Click(object sender, EventArgs e)
        {
            MessageBox.Show("В това поле се показва текущият час. Ако е в червено означава, че часът е минал и в момента тече междучасие. Ако е обозначено с \"-\" означава, че няма повече часове.");
            pictureBoxHelpCurrentClass.BorderStyle = BorderStyle.FixedSingle;
        }

        private void pictureBoxHelpNextClass_Click(object sender, EventArgs e)
        {
            MessageBox.Show("В това поле се показва следващият час. Ако е обозначено с \"-\" означава, че няма повече часове.");
            pictureBoxHelpNextClass.BorderStyle = BorderStyle.FixedSingle;
        }

        private void pictureBoxHelpTimeLeft_Click(object sender, EventArgs e)
        {
            MessageBox.Show("В това поле се показва оставащото време от часа/междучасието. Когато остава по-малко от една минута до края на часа/междучасието се изобразява в червено.");
            pictureBoxHelpTimeLeft.BorderStyle = BorderStyle.FixedSingle;
        }

        public void pictureBoxRefresh_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void labelMergeClasses_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            MergeClassesForm mergeClasses = new MergeClassesForm();
            mergeClasses.Show();
            this.Hide();
        }

        private void pictureBoxMelodyButton_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            Melody melody = new Melody();
            melody.Show();
            this.Hide();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Program.allowRinging = true;
                pictureBoxMelodyButton.Show();
                labelMelody.Show();
                checkBox1.ForeColor = Color.White;
                checkBox1.Font = new Font(checkBox1.Font, FontStyle.Bold);
                SaveTheData.SaveProperties();
            }
            else
            {
                Program.allowRinging = false;
                checkBox1.ForeColor = Color.Red;
                pictureBoxMelodyButton.Hide();
                labelMelody.Hide();
                checkBox1.Font = new Font(checkBox1.Font, FontStyle.Strikeout);
                SaveTheData.SaveProperties();
            }
            SaveTheData.SaveProperties();
        }

        private void pictureBoxSearchButton_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            SearchClasses search = new SearchClasses();
            search.Show();
            this.Hide();
        }

        private void pictureBoxVacations_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            Vacations vacations = new Vacations();
            vacations.Show();
            this.Hide();
        }

        private void pictureBoxPrinter_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            Printer printer = new Printer();
            printer.Show();
            this.Hide();
        }
        private void pictureBoxCustomClasses_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            CustomClasses custom = new CustomClasses();
            custom.Show();
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

        private void pictureBoxCloseMenu_Click(object sender, EventArgs e)
        {
            panelMenu.Hide();
            pictureBoxShowMenu.Show();
        }

        private void pictureBoxShowMenu_Click(object sender, EventArgs e)
        {
            panelMenu.Show();
            pictureBoxShowMenu.Hide();
        }

        private void panelMenu_Click(object sender, EventArgs e) => pictureBoxCloseMenu_Click(sender, e);

        private void pictureBoxNotes_Click(object sender, EventArgs e)
        {
            Program.LastForms.Push(this);
            Notes notes = new Notes();
            notes.Show();
            this.Hide();
        }
    }
}
