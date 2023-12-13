using Newtonsoft.Json;
using SchoolRing.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolRing.Models
{
    [Serializable]
    public class Note : INote
    {
        [JsonProperty("text")]
        private string text;
        [JsonProperty("date")]
        private DateTime date;
        [JsonProperty("dateCreated")]
        private DateTime dateCreated;
        [JsonProperty("classNum")]
        private int classNum;
        [JsonProperty("purva")]
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
