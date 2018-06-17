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

        public RubricModel()
        {
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}