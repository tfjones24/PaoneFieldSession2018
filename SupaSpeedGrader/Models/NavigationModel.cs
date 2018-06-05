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
        public List<string> quizzes = new List<string>();

        public Dictionary<string, List<string>> quizdata = new Dictionary<string, List<string>>();

        //Garbage to ignore, only for hardcoded bullshit
        public List<string> questions = new List<string>();

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

            updateQuizData();
        }

    
    }
}