using SchoolRing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolRing.Repository
{
    internal interface IRepository<T>
    {
        IReadOnlyCollection<T> GetModels();
        void AddModel(T model);
        void RemoveModel(T model);
        T FirstModel(string day, int num, bool isPurvaSmqna);
        void UpdateModel(T model);
        bool IsThereAModel(string day, int num, bool isPurvaSmqna);
        void ClearTheSchedule();
    }
}
