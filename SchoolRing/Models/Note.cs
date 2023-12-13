using SchoolRing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolRing.Models
{
    public class Note : INote
    {
        private string text;
        private DateTime date;
        private DateTime dateCreated;
        private int classNum;
        private bool purva;

        public Note(string _text, DateTime _date, DateTime _dateCreated, int _classNum, bool _purva)
        {
            Text = _text;
            Date = _date;
            DateCreated = _dateCreated;
            ClassNum = _classNum;
            Purva = _purva;
        }

        public string Text
        {
            get { return text; }
            private set { text = value; }
        }

        public DateTime Date
        {
            get { return date; }
            private set { date = value; }
        }

        public DateTime DateCreated
        {
            get { return dateCreated; }
            private set { dateCreated = value; }
        }

        public int ClassNum
        {
            get { return classNum; }
            private set { classNum = value; }
        }

        public bool Purva
        {
            get { return purva; }
            private set { purva = value; }
        }

    }
}
