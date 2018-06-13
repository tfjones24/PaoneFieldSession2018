﻿using Newtonsoft.Json.Linq;
using SupaSpeedGrader.API;
using SupaSpeedGrader.Helpers;
using SupaSpeedGrader.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SupaSpeedGrader.Controllers
{
    public class apiController : Controller
    {
        public JsonResult getQuestions(string quiz)
        {
            return Json(new { Result = String.Format("Fist item in list: '{0}'", quiz) });
        }
    }
}
