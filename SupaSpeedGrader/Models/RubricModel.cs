using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SupaSpeedGrader.Models
{
    public class RubricModel : Controller
    {
        // GET: RubicModel
        public List<string> questions = new List<string>();
        public int numWidth;

        public RubricModel()
        {
            addHardValue();
        }
        public ActionResult Index()
        {
            return View();
        }
        public void addHardValue()
        {
            for (int x = 1; x < 11; ++x)
            {
                questions.Add(x.ToString());
            }
            numWidth = 100 / (questions.Count + 1);
        }
    }
}