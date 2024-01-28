using SchoolRing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolRing.Repository
{
    internal class NoteRepository : INoteRepository<INote>
    {
        private List<INote> _notes = new List<INote>();
        public IReadOnlyCollection<INote> GetModels() => _notes.AsReadOnly();

        public void AddModel(INote model)
        {
            _notes.Add(model);
        }

        public INote FirstModel(DateTime _date, int _classNum, bool _purva)
        {
            return _notes
                .FirstOrDefault(n => n.Date.ToShortDateString() == _date.ToShortDateString()
                && n.ClassNum == _classNum && n.Purva == _purva);
        }


        public bool IsThereAModel(DateTime _date, int _classNum, bool _purva)
        {
            return _notes
                .Any(n => n.Date.ToShortDateString() == _date.ToShortDateString()
                && n.ClassNum == _classNum && n.Purva == _purva);
        }

        public void RemoveModel(INote model)
        {
            _notes.Remove(model);
        }

        public void UpdateModel(INote model)
        {
            _notes.Remove(FirstModel(model.Date, model.ClassNum, model.Purva));
            _notes.Add(model);
        }

        public List<INote> ClassName(string _name)
        {
            List<INote> matchingClassName = new List<INote>();
            foreach (var note in _notes)
            {
                //date, classnum, purva
                string dayOfWeekName = "";
                switch (note.Date.DayOfWeek)
                {
                    case DayOfWeek.Monday: dayOfWeekName = TimeForClockAndText.dayOfWeekMonday; break;
                    case DayOfWeek.Tuesday: dayOfWeekName = TimeForClockAndText.dayOfWeekTuesday; break;
                    case DayOfWeek.Wednesday: dayOfWeekName = TimeForClockAndText.dayOfWeekWednesday; break;
                    case DayOfWeek.Thursday: dayOfWeekName = TimeForClockAndText.dayOfWeekThursday; break;
                    case DayOfWeek.Friday: dayOfWeekName = TimeForClockAndText.dayOfWeekFriday; break;
                }
                ISchoolClass classOfNote = Program.GetModels().FirstOrDefault(c => c.Day == dayOfWeekName && c.Num == note.ClassNum && c.IsPurvaSmqna == note.Purva);
                if (_name == "0")
                {
                    if ($"{classOfNote.ClassGrade}".Contains(_name) && classOfNote.Paralelka == null)
                        matchingClassName.Add(note);
                }
                else if ($"{classOfNote.ClassGrade}{classOfNote.Paralelka}".Contains(_name))
                    matchingClassName.Add(note);
            }
            return matchingClassName;
        }
    }
}
