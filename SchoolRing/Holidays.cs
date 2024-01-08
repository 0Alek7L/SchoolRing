using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SchoolRing
{
    internal class Holidays
    {
        DateTime date;
        private bool todayIsHoliday;
        public Holidays(System.Windows.Forms.Label label)
        {
            date = DateTime.Now;
            todayIsHoliday = false;
            ShowHoliday(label);
        }

        public bool IsTodayHoliday() => todayIsHoliday;

        private void ShowHoliday(System.Windows.Forms.Label label)
        {
            if (date.Month == 1 && date.Day == 1)
            {
                todayIsHoliday = true;
                label.Text = NewYears;
            }
            else if (date.Month == 3 && date.Day == 3)
            {
                todayIsHoliday = true;
                label.Text = ThirdOfMarch;
            }
            else if (date.Month == 5 && date.Day == 1)
            {
                todayIsHoliday = true;
                label.Text = FirstOfMay;
            }
            else if (date.Month == 5 && date.Day == 6)
            {
                todayIsHoliday = true;
                label.Text = SixthOfMay;
            }
            else if (date.Month == 5 && date.Day == 24)
            {
                todayIsHoliday = true;
                label.Text = TwentyFourthOfMay;
            }
            else if (date.Month == 9 && date.Day == 6)
            {
                todayIsHoliday = true;
                label.Text = SixthOfSeptember;
            }
            else if (date.Month == 9 && date.Day == 15)
            {
                todayIsHoliday = true;
                label.Text = FifteenthOfSeptember;
            }
            else if (date.Month == 9 && date.Day == 22)
            {
                todayIsHoliday = true;
                label.Text = TwentySecondOfSeptember;
            }
            else if (date.Month == 11 && date.Day == 1)
            {
                todayIsHoliday = true;
                label.Text = FirstOfNovember;
            }
            else if (date.Month == 12 && date.Day >= 25 && date.Day <= 26)
            {
                todayIsHoliday = true;
                label.Text = MerryChristmas;
            }
            else if (date.Month == 12 && date.Day >= 21 && date.Day <= 31)
            {
                todayIsHoliday = true;
                label.Text = HappyHolidays;
            }
        }

        private static string NewYears = "Честита нова година!";  //01.01
        private static string ThirdOfMarch = "Честит трети март!";  //03.03
        private static string FirstOfMay = "Честит празник! Днес е ден на труда и на международната работническа солидарност!"; //01.05
        private static string SixthOfMay = "Честит празник! Днес е Гергьовден, ден на храбростта и празник на Българската армия!";  //06.05
        private static string TwentyFourthOfMay = "Честит ден на светите братя Кирил и Методий, на българската азбука, просвета и култура и на славянската книжовност!";  //24.05
        private static string SixthOfSeptember = "Честит празник! Днес е ден на съединението на Източна Румелия с Княжество България!";  //06.09
        private static string FifteenthOfSeptember = "Честит първи учебен ден!";
        private static string TwentySecondOfSeptember = "Честит празник! Днес е ден на независимостта!"; //22.09
        private static string FirstOfNovember = "Честит празник! Днес е ден на народните будители!"; //01.11
        private static string MerryChristmas = "Честито Рождество Христово!";//25th and 26th of december
        private static string HappyHolidays = "ВЕСЕЛИ ПРАЗНИЦИ!";//from 21th of December to 31th of december
    }
}
