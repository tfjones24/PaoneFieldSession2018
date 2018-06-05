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

        public int rubicRows = 5;
        public int rubicCols = 3;
        public int width;
        public List<List<string>> rubic = new List<List<string>>();
        public int y = 0;

        public string rubicTitle = "Rubic";

        public GradeModel(int numStudents, int gradeOutof, List<string> names, Dictionary<string, string> namesAnswer, Dictionary<string, string> namesGrade, int rubicRows, int rubicCols)
        {
            width = 100 / rubicCols;
        }

        public void addHardValue()
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

            string[] alphabet = { "a", "b", "c", "d", "e", "f", "g", "h", "'i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

            for (int i = 0; i < rubicRows; ++i)
            {
                List<String> row = new List<string>();
                for (int j = 0; j < rubicCols; ++j)
                {
                    row.Add(alphabet[y]);
                    y++;
                }
                rubic.Add(row);
            }
        }
    }
}
