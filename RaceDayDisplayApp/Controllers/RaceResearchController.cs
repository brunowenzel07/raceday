﻿using RaceDayDisplayApp.Common;
using RaceDayDisplayApp.DAL;
using RaceDayDisplayApp.Filters;
using RaceDayDisplayApp.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RaceDayDisplayApp.Controllers
{
    [Authorize(Roles = ConfigValues.FullAccessRole)]
    //[Authorize]
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

        //public ActionResult Tables(int country, int racecourse, int season,
        //                           int meetType, int raceType, int noRunners)
        public ActionResult Tables(HistoryFilters filters)
        {
            if (ModelState.IsValid)
            {
                var result = new RaceResearchViewModel();
                result.MarketData = entities.GetMarketData(filters, 0);
                result.FormFactors = entities.GetFormFactors(filters);
                return View("_Tables", result);
            }
            return null;
        }

        public ActionResult StatsTable(HistoryFilters filters)
        {
            return View("_StatsTable", entities.GetMarketData(filters, ConfigValues.MaxStatisticsResults));
        }
    }
}
