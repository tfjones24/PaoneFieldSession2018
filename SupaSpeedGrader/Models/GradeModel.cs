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
        public int questionStartShow;
        public string questionName;
        public string question;
        public List<string> names = new List<string>();

        public Dictionary<string, string[]> namesGrades = new Dictionary<string, string[]>();
        
        public int rubicRows;
        public int rubicCols;
        public int width;
        public List<List<string>> rubic = new List<List<string>>();

        public string rubicTitle = "Rubric";
        public int studentAt;

        public void AddOneToStudentAt(ref int studentAt){
            studentAt++;
        }


        public GradeModel(int numStudents, int gradeOutof, List<string> names, Dictionary<string, string[]> namesGrades, int rubicRows, int rubicCols, List<List<string>> rubric)
        {
            this.numStudent = numStudents;
            this.gradeOutOf = gradeOutof;
            this.names = names;

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
            string[] A = { "Alpha", "0", "Alpha" };
            string[] B = { "Beta", "0", "Beta" };
            string[] C = { "Charlie", "0", "Charlie" };
            string[] D = { "Delta", "0", "Delta" };
            string[] E = { "Echo", "0", "Echo" };
            namesGrades.Add(names[0], A);
            namesGrades.Add(names[1], B);
            namesGrades.Add(names[2], C);
            namesGrades.Add(names[3], D);
            namesGrades.Add(names[4], E);
            rubicRows = 5;
            rubicCols = 4;
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
            questionNum = 30;
            questionOn = 12;
            questionStartShow = (questionOn / 10) * 10;
            questionName = "Question Name";
            question = "Question";
            
        }

    }
}
