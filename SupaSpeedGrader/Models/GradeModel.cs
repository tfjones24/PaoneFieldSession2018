using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupaSpeedGrader.Models
{
    public class GradeModel
    {
        public int questionNum;   
        public int numStudent;
        public int gradeOutOf;
        public int questionOn;
        public string questionName;
        public string question;
        public List<string> names = new List<string>();

        public Dictionary<string, string> namesAnswer = new Dictionary<string, string>();

        public Dictionary<string, string> namesGrade = new Dictionary<string, string>();
        public Dictionary<string, string> namesComment = new Dictionary<string, string>();
        public int rubicRows;
        public int rubicCols;
        public int width;
        public List<List<string>> rubic = new List<List<string>>();

        public string rubicTitle = "Rubric";
        public int studentAt;
        public GradeModel(int numStudents, int gradeOutof, List<string> names, Dictionary<string, string> namesAnswer, Dictionary<string, string> namesGrade, int rubicRows, int rubicCols, List<List<string>> rubric)
        {
            this.numStudent = numStudents;
            this.gradeOutOf = gradeOutof;
            this.names = names;
            this.namesAnswer = namesAnswer;
            this.namesGrade = namesGrade;
            this.rubicRows = rubicRows;
            this.rubicCols = rubicCols;
            this.rubic = rubric;

            this.width = 100 / rubicCols;
        }

        public GradeModel()
        {
            string[] alphabet = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            numStudent = 5;
            gradeOutOf = 20;
            names = new List<string>();
            names.Add("A");
            names.Add("B");
            names.Add("C");
            names.Add("D");
            names.Add("E");
            namesAnswer = new Dictionary<string, string>();
            namesAnswer.Add(names[0], "Alpha");
            namesAnswer.Add(names[1], "Bravo");
            namesAnswer.Add(names[2], "Charlie");
            namesAnswer.Add(names[3], "Delta");
            namesAnswer.Add(names[4], "Epsilon");
            namesComment.Add(names[0], "Alpha");
            namesComment.Add(names[1], "Bravo");
            namesComment.Add(names[2], "Charlie");
            namesComment.Add(names[3], "Delta");
            namesComment.Add(names[4], "Epsilon");
            namesGrade = new Dictionary<string, string>();
            for (int x = 0; x < names.Count(); ++x)
            {
                namesGrade.Add(names[x], "0");
            }
            rubicRows = 5;
            rubicCols = 3;
            width = 100 / rubicCols;
            rubic = new List<List<string>>();
            int y = 0;
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
            questionNum = 20;
            questionOn = 2;
            questionName = "Question Name";
            question = "Question";
            studentAt = 0;
        }

    }
}
