using RaceDayDisplayApp.DAL;
using RaceDayDisplayApp.Filters;
using RaceDayDisplayApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RaceDayDisplayApp.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class RaceResearchController : Controller
    {
        private DBGateway entities = new DBGateway();

        //
        // GET: /RunnerHistory/

        public ActionResult Index()
        {
            var filters = entities.GetHistoryFilters();

            return View(filters);
        }

        public ActionResult Tables(int country, int racecourse, int season,
                                   int meetType, int raceType, int noRunners)
        {
            return View("_Tables");
        }
    }
}
