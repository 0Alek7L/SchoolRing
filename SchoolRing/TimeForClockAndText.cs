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
                case "Monday": return $"ПОНЕДЕЛНИК {date.Day:D2}.{date.Month:D2}";
                case "Tuesday": return $"ВТОРНИК { date.Day:D2}.{ date.Month:D2}";
                case "Wednesday": return $"СРЯДА {date.Day:D2}.{date.Month:D2}";
                case "Thursday": return $"ЧЕТВЪРТЪК {date.Day:D2}.{date.Month:D2}";
                case "Friday": return $"ПЕТЪК {date.Day:D2}.{date.Month:D2}";
                case "Saturday": return $"СЪБОТА {date.Day:D2}.{date.Month:D2}";
                case "Sunday": return $"НЕДЕЛЯ {date.Day:D2}.{date.Month:D2}";
                default: return null;
            }
        }

        public string PrintDayOnly()
        {
            switch (date.DayOfWeek.ToString())
            {
                case "Monday": return $"ПОНЕДЕЛНИК";
                case "Tuesday": return $"ВТОРНИК";
                case "Wednesday": return $"СРЯДА";
                case "Thursday": return $"ЧЕТВЪРТЪК";
                case "Friday": return $"ПЕТЪК";
                case "Saturday": return $"СЪБОТА";
                case "Sunday": return $"НЕДЕЛЯ";
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
