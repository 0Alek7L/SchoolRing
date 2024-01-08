using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace SchoolRing
{
    public class TimeForClockAndText
    {
        static DateTime date;
        public const string dayOfWeekMonday = "ПОНЕДЕЛНИК";
        public const string dayOfWeekTuesday = "ВТОРНИК";
        public const string dayOfWeekWednesday = "СРЯДА";
        public const string dayOfWeekThursday = "ЧЕТВЪРТЪК";
        public const string dayOfWeekFriday = "ПЕТЪК";
        public const string dayOfWeekSaturday = "СЪБОТА";
        public const string dayOfWeekSunday = "НЕДЕЛЯ";
        public TimeForClockAndText()
        {
            date = DateTime.Now;
        }
        public string PrintHour()
        {
            return date.ToString("HH");
        }
        public string PrintMinutes()
        {
            return date.ToString("mm");
        }
        public string PrintSeconds()
        {
            return date.ToString("ss");
        }
        public string PrintDay()
        {
            switch(date.DayOfWeek.ToString())
            {
                case "Monday": return $"{TimeForClockAndText.dayOfWeekMonday} {date.Day:D2}.{date.Month:D2}";
                case "Tuesday": return $"{TimeForClockAndText.dayOfWeekTuesday} { date.Day:D2}.{ date.Month:D2}";
                case "Wednesday": return $"{TimeForClockAndText.dayOfWeekWednesday} {date.Day:D2}.{date.Month:D2}";
                case "Thursday": return $"{TimeForClockAndText.dayOfWeekThursday} {date.Day:D2}.{date.Month:D2}";
                case "Friday": return $"{TimeForClockAndText.dayOfWeekFriday} {date.Day:D2}.{date.Month:D2}";
                case "Saturday": return $"{TimeForClockAndText.dayOfWeekSaturday} {date.Day:D2}.{date.Month:D2}";
                case "Sunday": return $"{TimeForClockAndText.dayOfWeekSunday} {date.Day:D2}.{date.Month:D2}";
                default: return null;
            }
        }

        public string PrintDayOnly()
        {
            switch (date.DayOfWeek.ToString())
            {
                case "Monday": return $"{TimeForClockAndText.dayOfWeekMonday}";
                case "Tuesday": return $"{TimeForClockAndText.dayOfWeekTuesday}";
                case "Wednesday": return $"{TimeForClockAndText.dayOfWeekWednesday}";
                case "Thursday": return $"{TimeForClockAndText.dayOfWeekThursday}";
                case "Friday": return $"{TimeForClockAndText.dayOfWeekFriday}";
                case "Saturday": return $"{TimeForClockAndText.dayOfWeekSaturday}";
                case "Sunday": return $"{TimeForClockAndText.dayOfWeekSunday}";
                default: return null;
            }
        }
        public List<int> CalculateClassDuration(int num, bool purvaSmqna)
        {
            int totalDuration = 0;
            TimeSpan timeSpan = new TimeSpan(7, 30, 0);
            for (int i = 1; i < num; i++)
            {
                totalDuration += Program.ClassLength;
                if (i == Program.LongBreakAfter)
                    totalDuration += Program.LongBreakLength;
                else
                    totalDuration += Program.ShortBreakLength;
            }
            if (purvaSmqna)
                timeSpan = timeSpan.Add(TimeSpan.FromMinutes(totalDuration));
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    totalDuration += Program.ClassLength;
                    if (i == Program.LongBreakAfter)
                        totalDuration += Program.LongBreakLength;
                    else
                        totalDuration += Program.ShortBreakLength;
                }
                timeSpan = timeSpan.Add(TimeSpan.FromMinutes(totalDuration));
            }
            int startH = timeSpan.Hours;
            int startM = timeSpan.Minutes;
            timeSpan = timeSpan.Add(TimeSpan.FromMinutes(Program.ClassLength));
            int endH = timeSpan.Hours;
            int endM = timeSpan.Minutes;
            return new List<int> { startH, startM, endH, endM };
        }
    }
}
