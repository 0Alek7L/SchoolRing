using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolRing.Interfaces
{
    public interface INote
    {
        string Text { get;}
        DateTime Date { get;}
        DateTime DateCreated { get;}
        int ClassNum { get;}
        bool Purva { get;}
    }
}
