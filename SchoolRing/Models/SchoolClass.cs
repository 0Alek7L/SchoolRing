using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using SchoolRing.Interfaces;
using SchoolRing.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SchoolRing
{
    [Serializable]
    public class SchoolClass : ISchoolClass
    {
        [JsonProperty("isMerging")]
        private bool isMerging = false;

        [JsonProperty("isMerged")]
        private bool isMerged = false;

        [JsonProperty("mergedWith")]
        private string mergedWith = null;

        [JsonProperty("day")]
        private string day = null;

        [JsonProperty("isPurvaSmqna")]
        private bool isPurvaSmqna = false;

        [JsonProperty("num")]
        private int num = 0;

        [JsonProperty("classGrade")]
        private int classGrade = 0;

        [JsonProperty("paralelka")]
        private string paralelka = null;

        [JsonProperty("isFree")]
        private bool isFree = false;

        [JsonProperty("startHours")]
        private int startHours = 0;

        [JsonProperty("startMinutes")]
        private int startMinutes = 0;

        [JsonProperty("endMinutes")]
        private int endMinutes = 0;

        [JsonProperty("endHours")]
        private int endHours = 0;

        public SchoolClass()
        {

        }
        public SchoolClass(string _day, int _num, bool _isPurvaSmqna, bool _isFree, int _classGrade, string _paralelka, int _startHours, int _startMinutes, int _endHours, int _endMinutes)
        {
            Day = _day;
            Num = _num;
            isFree = _isFree;
            ClassGrade = _classGrade;
            Paralelka = _paralelka;
            IsPurvaSmqna = _isPurvaSmqna;
            startHours = _startHours;
            startMinutes = _startMinutes;
            endHours = _endHours;
            endMinutes = _endMinutes;

            //day = _day;
            //num = _num;
            //isFree = _isFree;
            //classGrade = _classGrade;
            //paralelka = _paralelka;
            //isPurvaSmqna = _isPurvaSmqna;
            //startHours = _startHours;
            //startMinutes = _startMinutes;
            //endHours = _endHours;
            //endMinutes = _endMinutes;

        }
        public void UpdateTimeScheduleOfClass()
        {
            TimeForClockAndText time2 = new TimeForClockAndText();
            List<int> list = time2.CalculateClassDuration(Num, IsPurvaSmqna);
            int startH = list[0];
            int startM = list[1];
            int endH = list[2];
            int endM = list[3];
            TimeSpan time = new TimeSpan(endH, endM, 0);
            if (isMerging && !isMerged)
            {
                StartHours = startH;
                StartMinutes = startM;
                time = time.Add(TimeSpan.FromMinutes(Program.ClassLength));
                EndHours = time.Hours;
                EndMinutes = time.Minutes;
            }
            else if (isMerged && !isMerging)
            {
                TimeSpan timeSpan = new TimeSpan(startH, startM, 0);
                if (num != 1 && !isPurvaSmqna || isPurvaSmqna)
                {
                    ISchoolClass temp = Program.GetRecord(day, num - 1, isPurvaSmqna);
                    timeSpan = new TimeSpan(temp.EndHours, temp.EndMinutes, 0);
                    EndHours = timeSpan.Hours;
                    EndMinutes = timeSpan.Minutes;
                    timeSpan = timeSpan.Subtract(TimeSpan.FromMinutes(Program.ClassLength));
                    StartHours = timeSpan.Hours;
                    StartMinutes = timeSpan.Minutes;
                }
                else if (num == 1 && !isPurvaSmqna)
                {
                    ISchoolClass temp = Program.GetRecord(day, 7, true);
                    timeSpan = new TimeSpan(temp.EndHours, temp.EndMinutes, 0);
                    EndHours = timeSpan.Hours;
                    EndMinutes = timeSpan.Minutes;
                    timeSpan = timeSpan.Subtract(TimeSpan.FromMinutes(Program.ClassLength));
                    StartHours = timeSpan.Hours;
                    StartMinutes = timeSpan.Minutes;
                }
            }
            else
            {
                StartHours = startH;
                StartMinutes = startM;
                EndHours = endH;
                EndMinutes = endM;
            }
        }

        public void SetFreeClass()
        {
            IsFree = true;
            ClassGrade = 0;
            Paralelka = null;
        }

        public void SetClassObliged(int classGrade, string classParalelka)
        {
            IsFree = false;
            ClassGrade = classGrade;
            Paralelka = classParalelka;
        }
        public string ShowTheRecord()
        {
            if (classGrade == 0 && !IsFree)
            {
                if (IsMerged)
                    return $"{num}-{paralelka}-СЛЯТ-край-{EndHours:D2}:{EndMinutes:D2}";
                else if (IsMerging)
                    return $"{num}-{paralelka}-СЛЯТ-начало-{StartHours:D2}:{StartMinutes:D2}";
                else
                    return $"{num}-{paralelka}-{StartHours:D2}:{StartMinutes:D2}-{EndHours:D2}:{EndMinutes:D2}";
            }
            if (isFree)
                return $"{num}-Св.-{StartHours:D2}:{StartMinutes:D2}-{EndHours:D2}:{EndMinutes:D2}";

            else if (IsMerged)
                return $"{num}-{ClassGrade}{paralelka}-СЛЯТ-край-{EndHours:D2}:{EndMinutes:D2}";
            else if (IsMerging)
                return $"{num}-{ClassGrade}{paralelka}-СЛЯТ-начало-{StartHours:D2}:{StartMinutes:D2}";
            else
                return $"{num}-{classGrade}{paralelka}-{StartHours:D2}:{StartMinutes:D2}-{EndHours:D2}:{EndMinutes:D2}";
        }
        public string SaveMergingReference()
        {
            if (isPurvaSmqna)
                return $"{Day} {Num.ToString()} true";
            else
                return $"{Day} {Num.ToString()} false";

        }
        public string GetClassGradeAndParalelka()
        {
            if (classGrade == 0 && !IsFree)
                return $"{Paralelka}";
            else if (!isFree && classGrade != 0)
                return ClassGrade + Paralelka;
            else
                return "Св.";
        }

        public void ResetMergeStatus()
        {
            IsMerged = false;
            IsMerging = false;
            mergedWith = null;
            UpdateTimeScheduleOfClass();
        }

        public void MergeClassWith(ISchoolClass model)
        {
            this.ResetMergeStatus();
            model.ResetMergeStatus();

            model.IsMerged = true;
            model.MergedWith = this.SaveMergingReference();
            this.IsMerging = true;
            this.MergedWith = model.SaveMergingReference();
            TimeSpan time = new TimeSpan(endHours, endMinutes, 0);
            model.StartHours = time.Hours;
            model.StartMinutes = time.Minutes;
            time = time.Add(TimeSpan.FromMinutes(Program.ClassLength));
            this.EndHours = time.Hours;
            this.EndMinutes = time.Minutes;
            model.EndHours = time.Hours;
            model.EndMinutes = time.Minutes;
        }

        public string GetClassGradeAndParalelkaForPrinting()
        {
            if (!isFree)
            {
                if (!isMerged && !isMerging)
                {
                    if (classGrade == 0 && !IsFree)
                        return $"{Paralelka}";
                    else 
                        return ClassGrade + Paralelka;
                }
                else
                {
                    if (classGrade == 0)
                    {
                        return $"{Paralelka} сл.";
                    }
                    else
                    {
                        return ClassGrade + Paralelka + " сл.";
                    }
                }
            }
            else
                return "-";
        }

        //[JsonProperty("IsPurvaSmqna")]
        public bool IsPurvaSmqna
        {
            get { return isPurvaSmqna; }
            set { isPurvaSmqna = value; }
        }

        //[JsonProperty("ClassGrade")]
        public int ClassGrade
        {
            get { return classGrade; }
            set { classGrade = value; }
        }

        //[JsonProperty("Paralelka")]
        public string Paralelka
        {
            get { return paralelka; }
            set { paralelka = value; }
        }

        //[JsonProperty("IsFree")]
        public bool IsFree
        {
            get { return isFree; }
            set { isFree = value; }
        }

        //[JsonProperty("Num")]
        public int Num
        {
            get { return num; }
            set { num = value; }
        }

        //[JsonProperty("Day")]
        public string Day
        {
            get { return day; }
            set { day = value.ToUpper(); }
        }

        //[JsonProperty("EndHours")]
        public int EndHours
        {
            get { return endHours; }
            set { endHours = value; }
        }

        //[JsonProperty("EndMinutes")]
        public int EndMinutes
        {
            get { return endMinutes; }
            set { endMinutes = value; }
        }

        //[JsonProperty("StartMinutes")]
        public int StartMinutes
        {
            get { return startMinutes; }
            set { startMinutes = value; }
        }

        //[JsonProperty("StartHours")]
        public int StartHours
        {
            get { return startHours; }
            set { startHours = value; }
        }

        //[JsonProperty("MergedWith")]
        public string MergedWith
        {
            get { return mergedWith; }
            set { mergedWith = value; }
        }

        //[JsonProperty("IsMerged")]
        public bool IsMerged
        {
            get { return isMerged; }
            set { isMerged = value; }
        }

        //[JsonProperty("IsMerging")]
        public bool IsMerging
        {
            get { return isMerging; }
            set { isMerging = value; }
        }

    }
}
