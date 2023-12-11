using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolRing.Interfaces
{
    public interface ISchoolClass
    {
        string Day { get; set; }
        int Num { get; set; }
        bool IsPurvaSmqna { get; set; }
        int ClassGrade { get; set; }
        string Paralelka { get; set; }
        bool IsFree { get; set; }
        int StartHours { get; set; }
        int StartMinutes { get; set; }
        int EndHours { get; set; }
        int EndMinutes { get; set; }
        bool IsMerged { get; set; }
        string MergedWith { get; set; }
        bool IsMerging { get; set; }
        string SaveMergingReference();
        string ShowTheRecord();
        void UpdateTimeScheduleOfClass();
        void SetFreeClass();
        void SetClassObliged(int classGrade, string classParalelka);
        void MergeClassWith(ISchoolClass model);
        void ResetMergeStatus();
        string GetClassGradeAndParalelka();
        string GetClassGradeAndParalelkaForPrinting();
    }
}
