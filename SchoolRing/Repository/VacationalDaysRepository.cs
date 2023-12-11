using SchoolRing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolRing.Repository
{
    internal class VacationalDaysRepository : IVacationalDaysRepository
    {
        List<IVacationalDays> _days;
        public VacationalDaysRepository()
        {
            _days = new List<IVacationalDays>();
        }
        public IReadOnlyCollection<IVacationalDays> GetModels() => _days.AsReadOnly();
        public void AddModel(IVacationalDays model)
        {
            _days.Add(model);
        }
        public void RemoveModel(IVacationalDays model)
        {
            _days.Remove(model);
        }
        public IVacationalDays GetModel(DateTime start, DateTime end)
        {
            if (_days.First(x => x.StartDate == start && x.EndDate == end) != null)
                return _days.First(x => x.StartDate == start && x.EndDate == end);
            else
                return null;
        }

        public bool IsThereAModel(DateTime start, DateTime end)
        {
            if (_days.Any(x => x.EndDate > start && x.StartDate < start))
                return true;
            else if (_days.Any(x => x.EndDate > end && x.StartDate < end))
                return true;
            else if (_days.Any(x => x.StartDate == start && x.EndDate == end))
                return true;
            else if (_days.Any(x => x.StartDate > start && x.EndDate <= end))
                return true;
            else if (_days.Any(x => x.StartDate == start || x.EndDate == end))
                return true;
            else
                return false;
        }

        public bool IsTodayVacation()
        {
            DateTime dateTime = DateTime.Now;
            if(_days.Any(x=>x.StartDate<=dateTime.Date && x.EndDate>=dateTime.Date))
                return true;
            else if(_days.Any(x => x.StartDate.Date==dateTime.Date||x.EndDate.Date==dateTime.Date))
                return true;
            else
                return false;
        }
    }
}
