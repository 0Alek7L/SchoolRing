using NAudio.Wave.SampleProviders;
using SchoolRing.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolRing
{
    internal class VacationalDays : IVacationalDays
    {
		private DateTime startDate;
		private DateTime endDate;
		private string argument;

        public VacationalDays(string argument, DateTime startDate, DateTime endDate)
        {
            Argument = argument;
            EndDate = endDate;
            StartDate = startDate;
        }

        public string Argument
		{
			get { return argument; }
			set { argument = value; }
		}

		public DateTime EndDate
		{
			get { return endDate; }
			set { endDate = value; }
		}

		public DateTime StartDate
        {
			get { return startDate; }
			set { startDate = value; }
		}

    }
}
