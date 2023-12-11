using SchoolRing.Interfaces;
using System.Collections.Generic;

namespace SchoolRing
{
    public interface IController
    {
        void AddNewClass(ISchoolClass model);
        IReadOnlyCollection<ISchoolClass> GetModel();
        ISchoolClass GetTheModel(string day, int num, bool isPurvaSmqna);
        void ClearTheSchedule();
    }
}