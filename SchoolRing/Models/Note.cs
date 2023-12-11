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
        private string title;
        private string text;
        private DateTime date;
        private DateTime dateCreated;
        private int classNum;
        private bool purva;

        public Note(string _title, string _text, DateTime _date, DateTime _dateCreated, int _classNum, bool _purva)
        {
            Title = _title;
            Text = _text;
            Date = _date;
            DateCreated = _dateCreated;
            ClassNum = _classNum;
            Purva = _purva;
        }

        public string Title
        {
            get { return title; }
            private set { title = value; }
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
            get { return date; }
            private set { date = value; }
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

        public void ChangeNoteText(string _text)
        {
            Text = _text;
        }

        public void ChangeNoteTitle(string _text)
        {
            Title = _text;
        }
    }
}
