using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupaSpeedGrader.Models
{
    public class GradeModel
    {
        //Quiz name and ID
        public string quizName;
        public string quizID;
        //Question name and ID
        public string questionName;
        public string questionID;

        //State ID
        public string state;


        public List<int> navBarQuestions = new List<int>(); //List of question numbers nav bar populates with
        public int rubricParsed;            //tells whether we are using the rubric or not. set to 1 for no rubric, 0 for a rubric
        public int numStudent;              //number of students who have responded to this question
        public int gradeOutOf;              //max number of points for the question


        public List<string> names = new List<string>(); //I think this is each student.
        public Dictionary<string, string> userNameToID = new Dictionary<string, string>(); //key for this is student name, returns user id


        /* element 0 = answer, element 1 = grade, element 2 = comment*/
        //names is the key for this dictionary. in the string array that is returned for each key, the first entry will be the student response, second will be their grade, and the third will be the comment
        public Dictionary<string, string[]> namesGrades = new Dictionary<string, string[]>();
        
        public int rubicRows;   //number of rows of the rubric
        public int rubicCols;   //number of columns of the rubric
        public int width;       //a variable used for setting the width of some element. not sure what michelle did here
        public List<List<string>> rubic = new List<List<string>>(); //the actual 2d list to store the rubric things.        TODO:need to rework to fit with michelle's page

        public string rubicTitle = "Rubric";        //title of rubric, needs to be updated to set name
        public int studentAt;                   //basically useless but we have it. don't change this?

        public GradeModel()
        {

        }
       


        public GradeModel(string quiz, string quizID, string question, string questionID, int numStudents, int gradeOutof, List<string> names, Dictionary<string, string[]> namesGrades, int rubicRows, int rubicCols, List<List<string>> rubric)
        {
            this.numStudent = numStudents;
            this.gradeOutOf = gradeOutof;
            this.names = names;
            this.namesGrades = namesGrades;
            this.rubicRows = rubicRows;
            this.rubicCols = rubicCols;
            this.rubic = rubric;

            this.width = 100 / rubicCols;
        }

        public GradeModel(bool custom)
        {
            string[] alphabet = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            numStudent = 11;
            gradeOutOf = 20;
            names.Add("A");
            names.Add("B");
            names.Add("C");
            names.Add("D");
            names.Add("E");
            names.Add("F");
            names.Add("A1");
            names.Add("B1");
            names.Add("C1");
            names.Add("D1");
            names.Add("E1");
            string[] A = { "Alpha", "0", "Alpha" };
            string[] B = { "Beta", "0", "Beta" };
            string[] C = { "Charlie", "0", "Charlie" };
            string[] D = { "Delta", "0", "Delta" };
            string[] E = { "Echo", "0", "Echo" };
            string[] F = {"Foxtrot","0", "Foxtrot"};
            namesGrades.Add(names[0], A);
            namesGrades.Add(names[1], B);
            namesGrades.Add(names[2], C);
            namesGrades.Add(names[3], D);
            namesGrades.Add(names[4], E);
            namesGrades.Add(names[5], F);
            namesGrades.Add(names[6], A);
            namesGrades.Add(names[7], B);
            namesGrades.Add(names[8], C);
            namesGrades.Add(names[9], D);
            namesGrades.Add(names[10], E);
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


            quizName = "Quiz Name";
            questionName = "Question name";
            for(int i=0; i < 25; i++)
            {
                navBarQuestions.Add(i);
            }
            rubricParsed = 0;

        }

        public void buildNavBar(int numQuestions)
        {
            for (int i = 0; i < numQuestions; i++)
            {
                navBarQuestions.Add(i);
            }
        }

    }
}
