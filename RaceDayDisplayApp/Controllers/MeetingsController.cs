using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RaceDayDisplayApp.Models;
using MvcJqGrid;
using System.Configuration;
using RaceDayDisplayApp.Filters;
using RaceDayDisplayApp.DAL;
using System.Dynamic;

namespace RaceDayDisplayApp.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class MeetingsController : Controller
    {
        private DBGateway entities = new DBGateway();


        public ActionResult Index()
        {
            if (User.IsInRole(ConfigValues.FullAccessRole))
                return View();
            else
                return RedirectToAction("RaceList");
        }

        //
        // GET: /Meetings/

        public ActionResult MeetingList(bool today = true)
        {
            ViewBag.Today = today;
            return View(entities.GetCountries(today));
        }

        public ActionResult RaceList(bool today = true)
        {
            ViewBag.Today = today;
            return View(entities.GetRacesList(today));
        }

        //
        // GET: /Meetings/Details/5

        public ActionResult Details(int id = 0)
        {
            //get the meeting info
            Meeting meeting = entities.GetMeeting(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }

            //get the user settings
            var userId = WebMatrix.WebData.WebSecurity.GetUserId(User.Identity.Name);
            ViewBag.UserSettings = entities.GetUserSettings(userId, meeting.CountryEnum);

            //to render or not the statistics sections
            ViewBag.FullAccess = User.IsInRole(ConfigValues.FullAccessRole);

            return View(meeting);
        }

        public ActionResult RaceDetails(int id, CountryEnum country, bool today = true)
        {
            //get the meeting and Race info
            Meeting meeting = entities.GetMeetingByRaceId(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }

            //get race info from cache
            meeting.Races = new List<RaceBase>(new[] { RacesCache.Instance.GetRace(id, country) });

            //set previous and next race in the sequence
            var racesList = entities.GetRacesList(today).ToList();
            var index = racesList.FindIndex(r => r.RaceId == id);
            ViewBag.Previous = index > 0 ? racesList[index - 1] : null;
            ViewBag.Next = index + 1 < racesList.Count() ? racesList[index + 1] : null;
            ViewBag.Today = today;

            //get the user settings
            var userId = WebMatrix.WebData.WebSecurity.GetUserId(User.Identity.Name);
            ViewBag.UserSettings = entities.GetUserSettings(userId, meeting.CountryEnum);

            //columns to display for this race
            ViewBag.GridColumns = ModelHelper.GetGridColumns(meeting.Races[0] as Race);

            //to render or not the statistics sections
            ViewBag.FullAccess = User.IsInRole(ConfigValues.FullAccessRole);

            return View(meeting);
        }

        public ActionResult Race(int id, CountryEnum country)
        {
            var r = RacesCache.Instance.GetRace(id, country);
            if (r == null)
            {
                return HttpNotFound();
            }

            ViewBag.GridColumns = ModelHelper.GetGridColumns(r);
            return View("_Race", r);
        }

        /// <summary>
        /// Called async by the jqGrid on details view
        /// </summary>
        /// <returns>A list of runners</returns>
        public JsonResult GridData(int id, CountryEnum country, GridSettings gridSettings)
        {
            var race = RacesCache.Instance.GetRace(id, country);
            var runners = race.Runners;

            if (runners == null)
            {
                return  null;
            }

            if (runners.Count() > 0)
            {
                //runners.First().isDone; //for testing purposes

                //sorting
                if (gridSettings.SortColumn != "")
                {
                    if (gridSettings.SortOrder == "asc")
                        runners = runners.OrderBy(o => typeof(Runner).GetProperty(gridSettings.SortColumn).GetValue(o)).ToList();
                    else
                        runners = runners.OrderByDescending(o => typeof(Runner).GetProperty(gridSettings.SortColumn).GetValue(o)).ToList();
                }
                else
                    runners = runners.Cast<Runner>().OrderBy(o => o.HorseNumber).ToList(); //default sorting criteria
            }
            
            var now = DateTime.UtcNow;
            var jsonData = new
            {
                total = runners.Count(),
                page = gridSettings.PageIndex,
                records = runners.Count(),
                rows = runners,
                race = race.GetLightCopy(),
                dbUpdatedSecs = race.RefreshValues.LastDBUpdate == default (DateTime) ? -1 :
                                (now - race.RefreshValues.LastDBUpdate).TotalSeconds,
                serverUpdatedSecs = (now - race.RefreshValues.LastServerRefresh).TotalSeconds,
                //add delay to avoid waiting in the lock
                nextRefreshSecs = race.isDone ? -1 : 
                        (race.RefreshValues.NextRefresh - now).TotalSeconds + ConfigValues.ClientAddedDelay 
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GridSettings(CountryEnum country)
        {
            //get the user settings
            var userId = WebMatrix.WebData.WebSecurity.GetUserId(User.Identity.Name);
            var settings = entities.GetUserSettings(userId, country);

            return View("_GridSettings", settings);
        }

        public ActionResult RunnerHistory(int id, string countryCode)
        {
            dynamic model = new ExpandoObject();
            model.HorseDetails = entities.GetHorseDetailsData(id);

            var runnerHistory = entities.GetRunnerHistory(id, countryCode);
            model.RunnerHistory = runnerHistory;
            model.Fields = RunnerHistoryHelper.GetFieldsIndexes(runnerHistory.First());

            return View(model);
        }

    }
}