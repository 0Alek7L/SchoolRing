using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolRing.Interfaces
{
    public interface IVacationalDays
    {
        DateTime StartDate { get; set; }
        DateTime EndDate { get; set; }
        string Argument { get; set; }
    }
}
