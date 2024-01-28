using SchoolRing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolRing.Repository
{
    internal interface INoteRepository<T>
    {
        IReadOnlyCollection<T> GetModels();
        void AddModel(T model);
        void RemoveModel(T model);
        T FirstModel(DateTime _date, int _classNum, bool _purva);
        void UpdateModel(T model);
        bool IsThereAModel(DateTime _date, int _classNum, bool _purva);
        List<INote> ClassName(string _name);
    }
}
