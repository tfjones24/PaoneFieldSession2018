using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SupaSpeedGrader.Models
{
    public class RubricModel
    {
        // GET: RubicModel
        public List<string> questions = new List<string>();

        // State and other junk
        public string state;

        public RubricModel()
        {
        }

    }
}