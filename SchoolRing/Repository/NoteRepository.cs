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
        private List<INote> _notes=new List<INote>();
        public IReadOnlyCollection<INote> GetModels()=>_notes.AsReadOnly();

        public void AddModel(INote model)
        {
            _notes.Add(model);
        }

        public INote FirstModel(DateTime _date, int _classNum, bool _purva)
        {
            return _notes
                .First(n=>n.Date.ToShortDateString()==_date.ToShortDateString() 
                && n.ClassNum==_classNum&&n.Purva==_purva);
        }


        public bool IsThereAModel(DateTime _date, int _classNum, bool _purva)
        {
            return _notes
                .Any(n => n.Date.ToShortDateString() == _date.ToShortDateString()
                && n.ClassNum == _classNum && n.Purva == _purva);
        }


        public void RemoveModel(INote model)
        {
            _notes .Remove(model);
        }

        public void UpdateModel(INote model)
        {
            _notes.Remove(FirstModel(model.Date,model.ClassNum, model.Purva));
            _notes.Add(model);
        }
    }
}
