using SchoolRing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolRing.Repository
{
    internal class Repository : ISchoolClassRepository
    {
        private List<ISchoolClass> _classes=new List<ISchoolClass>();
        public IReadOnlyCollection<ISchoolClass> GetModels()=>_classes.AsReadOnly();

        public void AddModel(ISchoolClass model)
        {
            _classes.Add(model);
        }

        public ISchoolClass FirstModel(string day, int num, bool isPurvaSmqna)
        {
            return _classes.Find(x => x.Day == day && x.Num == num&&x.IsPurvaSmqna==isPurvaSmqna);
        }

        public bool IsThereAModel(string day, int num, bool isPurvaSmqna)
        {
            return _classes.Any(x => x.Day == day && x.Num == num&&x.IsPurvaSmqna==isPurvaSmqna);
        }

        public void UpdateModel(ISchoolClass model)
        {
            ISchoolClass temp = _classes.Find(x => x.Day == model.Day
                && x.Num == model.Num 
                && x.IsPurvaSmqna==model.IsPurvaSmqna);
            RemoveModel(temp);
            AddModel(model);
        }
        
        public void RemoveModel(ISchoolClass model)
        {
            _classes.Remove(model);
        }
        public void ClearTheSchedule()
        {
            _classes.Clear();
        }
    }
}
