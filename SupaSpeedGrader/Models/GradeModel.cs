using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupaSpeedGrader.Models
{
    public class GradeModel
    {
        public int numStudent = 5;
        public int gradeOutOf = 20;
        public List<string> names = new List<string>();

        public Dictionary<string, string> namesAnswer = new Dictionary<string, string>();
        public Dictionary<string, string> namesGrade = new Dictionary<string, string>();

        public GradeModel()
        {
            /* Hardcoded values */
            names.Add("A");
            names.Add("B");
            names.Add("C");
            names.Add("D");
            names.Add("E");
            namesAnswer.Add(names[0], "Alpha");
            namesAnswer.Add(names[1], "Bravo");
            namesAnswer.Add(names[2], "Charlie");
            namesAnswer.Add(names[3], "Delta");
            namesAnswer.Add(names[4], "Epsilon");
            /* */

            for (int x = 0; x < names.Count(); ++x)
            {
                namesGrade.Add(names[x], "0");
            }
        }
    }
}
