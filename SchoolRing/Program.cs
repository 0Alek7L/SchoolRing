﻿using SchoolRing.Interfaces;
using SchoolRing.IO;
using SchoolRing.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace SchoolRing
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                if (!IsRunAsAdministrator())
                {
                    // Restart the application with administrative privileges
                    RestartAsAdministrator();
                    return; // Exit the current instance
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                time = new TimeForClockAndText();

                controller = new Controller();
                vdRepo = new VacationalDaysRepository();
                noteRepo = new NoteRepository();

                if (File.Exists(SaveTheData.filePathProperties))
                {
                    SaveTheData.ReadProperties();
                }
                if (File.Exists(SaveTheData.filePathTimes))
                {
                    SaveTheData.ReadTimes();
                }
                if (File.Exists(SaveTheData.filePath))
                {
                    SaveTheData.ReadSchoolClasses();

                }
                if (File.Exists(SaveTheData.filePathVacations))
                {
                    SaveTheData.ReadVacations();
                }
                if (File.Exists(SaveTheData.filePathNotes))
                {
                    SaveTheData.ReadNotes();
                }
                foreach (var item in GetModels())
                {
                    if (item.IsMerging)
                    {
                        ISchoolClass schoolClass;
                        if (item.Num < 7)
                            schoolClass = GetModels().First(x => x.Day == item.Day && x.Num == item.Num + 1 && x.IsPurvaSmqna == item.IsPurvaSmqna);
                        else
                            schoolClass = GetModels().First(x => x.Day == item.Day && x.Num == 1 && x.IsPurvaSmqna == false);
                        item.MergeClassWith(schoolClass);
                    }
                }
                timer = new System.Windows.Forms.Timer();
                timer.Interval = 200;
                timer.Tick += Timer_Tick;
                timer.Tick += Timer_Tick1;
                timer.Start();
                try
                {
                    Image a = Image.FromFile(customIconPath);
                }
                catch
                {
                    customIconPath = null;
                }
                if (!HaveBeenIntoMainMenu)
                {
                    Application.Run(new Templates());
                    allowRinging = false;
                }
                else
                    Application.Run(MainMenu.Instance);

                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("Нещо се обърка! Работата на приложението бе прекратена. Моля, отворете го наново!");
                //RestartAsAdministrator();
            }
        }

        private static bool IsRunAsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void RestartAsAdministrator()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Application.ExecutablePath;
            startInfo.Verb = "runas"; // Run as administrator

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception)
            {
                //TODO
            }

            Environment.Exit(0); // Exit the current instance
        }

        static Mutex mutex = new Mutex(true, "{SchoolRing}");

        public static TimeForClockAndText time;
        static System.Windows.Forms.Timer timer;
        public static IVacationalDaysRepository vdRepo;
        public static INoteRepository<INote> noteRepo;
        private static IController controller;
        //ЗА БУТОНА ВРЪЩАНЕ НАЗАД
        public static Stack<Form> LastForms = new Stack<Form>();
        //ПРООМЕНЛИВИ ЗА ВРЕМЕВИТЕ РАМКИ НА ЧАСОВЕТЕ
        public static int ClassLength = 0;
        public static int ShortBreakLength = 0;
        public static int LongBreakLength = 0;
        public static int LongBreakAfter = 0;
        //ПРОМЕНЛИВИ ЗА ПРОГРАМАТА
        public static bool WithClassSchedule = true;
        public static bool HaveBeenIntoMainMenu = false;
        public static string customIconPath = null;
        //ВРЕМЕННИ ПРОМЕНЛИВИ
        private static int tempClassLength = 0;
        private static int tempShortBreakLength = 0;
        private static int tempLongBreakLength = 0;
        private static int tempLongBreakAfter = 0;

        public static bool isMessageShown = false;
        public static string melodyForStartOfClassPath = null;
        public static string melodyForEndOfClassPath = null;
        public static bool allowRinging = true;
        public static int fixedMelodyLength = 0;

        //ПРОМЕНЛИВА ЗА ГОЛЕМИНА НА ТЕКСТ НА ЗАПИСКИ
        public static int textSizeForNotes = 16;

        //ПУБЛИЧЕН МЕТОД ЗА ДОБАВЯНЕ НА ЧАС КЪМ РЕПО
        public static void AddRecord(ISchoolClass model)
        {
            controller.AddNewClass(model);
        }

        public static ISchoolClass FindTheRecord(string day, int num, bool purva)
        {
            return controller.GetTheModel(day, num, purva);
        }

        public static void ShowTheCurrentIcon(PictureBox picture)
        {
            if (customIconPath == null)
            {
                ChangeCustomIcon(picture, false);
            }
            else
            {
                ChangeCustomIcon(picture, true);
            }
        }

        public static void ChangeCustomIcon(PictureBox picture, bool IsCustom)
        {
            picture.BackgroundImage = null;
            if (IsCustom&&customIconPath!=null)
            {
                picture.BackgroundImage = Image.FromFile(customIconPath);
                picture.SizeMode.Equals(PictureBoxSizeMode.Zoom);
            }
            else
            {
                picture.BackgroundImage = Properties.Resources.geo_milev_logo;
                picture.SizeMode.Equals(PictureBoxSizeMode.Zoom);
            }
        }

        public static void ChoosePathForCustomIcon(ContextMenuStrip cms)
        {

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.png;*.jpeg|All files (*.*)|*.*";

                DialogResult result = openFileDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    try
                    {
                        string filePath = openFileDialog.FileName;
                        customIconPath = filePath;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Грешка: {ex.Message}", "Грешка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }

        public static void ClearTheSchedule()
        {
            controller.ClearTheSchedule();
            FillTheRepo();
        }

        //ПРОВЕРКА ДАЛИ ИМА ВИДИМИ ФОРМИ
        private static void Timer_Tick(object sender, EventArgs e)
        {

            bool formIsVisible = false;
            //Point point = new Point();
            foreach (Form form in Application.OpenForms)
            {
                if (form.Visible)
                {
                    //point = form.Location;
                    formIsVisible = true;
                    break;
                }
                //else
                //{
                //    if (formIsVisible)
                //        form.Location = point;
                //}
            }
            //АКО НЯМА ВИДИМИ ДА ЗАТВОРИ ВСИЧКИ СКРИТИ
            if (!formIsVisible)
            {
                timer.Stop();

                SaveTheData.SaveSchoolClasses();
                SaveTheData.SaveVacation();
                SaveTheData.SaveTimes();
                SaveTheData.SaveProperties();
                SaveTheData.SaveNotes();
                try
                {
                    foreach (Form form in Application.OpenForms)   //ERROR
                    {
                        form.Close();
                    }
                }
                catch
                {
                }
            }
        }

        public static IReadOnlyCollection<ISchoolClass> GetModels() => controller.GetModel();
        public static ISchoolClass GetRecord(string day, int num, bool isPurvaSmqna) => controller.GetTheModel(day, num, isPurvaSmqna);

        private static void Timer_Tick1(object sender, EventArgs e)
        {
            if (GetModels().Count < 1 && ClassLength != 0)
                FillTheRepo();
            if (tempClassLength != ClassLength || tempShortBreakLength != ShortBreakLength || tempLongBreakLength != LongBreakLength || tempLongBreakAfter != LongBreakAfter)
            {
                tempClassLength = ClassLength;
                tempShortBreakLength = ShortBreakLength;
                tempLongBreakLength = LongBreakLength;
                tempLongBreakAfter = LongBreakAfter;
                foreach (var classItem in GetModels())
                {
                    classItem.UpdateTimeScheduleOfClass();
                }

            }

        }
        public static List<ISchoolClass> MergableClasses(string day)
        {
            List<ISchoolClass> mergable = new List<ISchoolClass>();
            foreach (var klas in GetModels().Where(x => x.Day == day && !x.IsFree))
            {
                if (klas.Num < 7)
                {
                    if (GetRecord(day, klas.Num + 1, klas.IsPurvaSmqna).GetClassGradeAndParalelka() == klas.GetClassGradeAndParalelka())
                    {
                        mergable.Add(klas);
                    }
                }
                else if (klas.Num == 7 && klas.IsPurvaSmqna)
                {
                    if (GetRecord(day, 1, false).GetClassGradeAndParalelka() == klas.GetClassGradeAndParalelka())
                    {
                        mergable.Add(klas);
                    }
                }
            }
            return mergable;
        }

        public static void FillTheRepo()
        {
            for (int i = 1; i <= 5; i++)
            {
                string day = string.Empty;
                if (i == 1) day = $"{TimeForClockAndText.dayOfWeekMonday}";
                if (i == 2) day = $"{TimeForClockAndText.dayOfWeekTuesday}";
                if (i == 3) day = $"{TimeForClockAndText.dayOfWeekWednesday}";
                if (i == 4) day = $"{TimeForClockAndText.dayOfWeekThursday}";
                if (i == 5) day = $"{TimeForClockAndText.dayOfWeekFriday}";
                for (int j = 1; j <= 7; j++)
                {
                    List<int> list = time.CalculateClassDuration(j, true);
                    int startH = list[0];
                    int startM = list[1];
                    int endH = list[2];
                    int endM = list[3];
                    ISchoolClass schoolClass = new SchoolClass(day, j, true, true, 0, null, startH, startM, endH, endM);
                    AddRecord(schoolClass);
                }
                for (int j = 1; j <= 7; j++)
                {
                    List<int> list = time.CalculateClassDuration(j, false);
                    int startH = list[0];
                    int startM = list[1];
                    int endH = list[2];
                    int endM = list[3];
                    ISchoolClass schoolClass = new SchoolClass(day, j, false, true, 0, null, startH, startM, endH, endM);
                    AddRecord(schoolClass);
                }
            }
        }

        private static bool IsFormOpen(Type formType)
        {
            int a = 0;
            foreach (Form openForm in Application.OpenForms)
            {
                // Check if the form matches the specified type
                if (openForm.GetType() == formType)
                {
                    a++;
                }
            }
            if (a > 0)
                return true; // Form is already open
            else
                return false; // Form is not open
        }
    }
}

