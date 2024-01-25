using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolRing.Help_info
{
    internal class HelpInfo
    {
        string path = @"C:\Users\thisi\Downloads\NVO_X_Math_Model_2023-24_15092023.txt";
        public string ShowPdf()
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                StreamReader stream = new StreamReader(fileStream);
                StringBuilder sb = new StringBuilder();
                sb.Append(stream.ReadToEnd());
                stream.Close();
                return sb.ToString();
            }
        }
    }
}
