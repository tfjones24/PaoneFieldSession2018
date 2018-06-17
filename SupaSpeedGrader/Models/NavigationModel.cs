using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupaSpeedGrader.Models
{
    public class NavigationModel
    {
        //Public data to fill from API calls
        public List<string> sections = new List<string>();
        public Dictionary<string, string> sectionID = new Dictionary<string, string>(); 

        public List<string> quizzes = new List<string>();
        public Dictionary<string, string> quizID = new Dictionary<string, string>();
        public Dictionary<string, string> quizColor = new Dictionary<string, string>();

        public Dictionary<string, List<string>> quizdata = new Dictionary<string, List<string>>();
        public List<string> rubic = new List<string>();
        //Garbage to ignore, only for hardcoded bullshit
        public List<string> questions = new List<string>();
        public Dictionary<string, string> questionColor = new Dictionary<string, string>();

        public string state = null;

        public NavigationModel()
        {
        }

        //Rebuilds quizdata dictionary with quiz data FOR HARDCODING ONLY
        private void updateQuizData()
        {
            quizdata = new Dictionary<string, List<string>>();

            for (int i = 0; i < quizzes.Count; i++)
            {
                quizdata.Add(quizzes[i], questions);
                quizID.Add(quizzes[i], quizzes[i]);
                sectionID.Add(sections[i], sections[i]);
            }
        }

        // Add hardcoded values
        // For test purposes only
        public void addHardValue()
        {
            questions.Add("Alpha");
            questions.Add("Bravo");
            questions.Add("Charlie");
            sections.Add("A");
            sections.Add("B");
            sections.Add("C");
            quizzes.Add("Apple");
            quizzes.Add("Banana");
            quizzes.Add("Cucumber");
            rubic.Add("A");
            rubic.Add("B");
            rubic.Add("C");
            
            questionColor.Add(questions[0], "started");
            questionColor.Add(questions[1], "done");
            questionColor.Add(questions[2], "unstarted");
            quizColor.Add(quizzes[0],"started");
            quizColor.Add(quizzes[1],"done");
            quizColor.Add(quizzes[2], "unstarted");
            updateQuizData();

        }

    
    }
}