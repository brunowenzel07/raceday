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

namespace RaceDayDisplayApp.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class MeetingsController : Controller
    {
        private MeetingDB entities = new MeetingDB();

        //
        // GET: /Meetings/

        public ActionResult Index(bool today = true)
        {
            ViewBag.Today = today;
            return View(entities.GetCountries(today));
        }

        public ActionResult RaceIndex(bool today = true)
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
            ViewBag.UserSettings = entities.GetUserSettings(userId, meeting.MeetingId, meeting.IsHK) ??
                ModelHelper.ToViewUserSettings(UserSettings.DEFAULT, meeting.IsHK);

            //hidden fields to control the auto-refresh of the grid
            ViewBag.MinutesBeforeJumpTimeToStartRefresh = ConfigurationManager.AppSettings["MinutesBeforeJumpTimeToStartRefresh"];
            ViewBag.RefreshIntervalSeconds = ConfigurationManager.AppSettings["RefreshIntervalSeconds"];

            return View(meeting);
        }

        public ActionResult RaceDetails(int id = 0, bool today = true)
        {
            //get the meeting and Race info
            Meeting meeting = entities.GetRaceWithMeeting(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }

            //set previous and next race in the sequence
            var racesList = entities.GetRacesList(today).ToList();
            var index = racesList.FindIndex(r => r.RaceId == id);
            ViewBag.Previous = index > 0 ? racesList[index - 1] : null;
            ViewBag.Next = index + 1 < racesList.Count() ? racesList[index + 1] : null;
            ViewBag.Today = today;

            //get the user settings
            var userId = WebMatrix.WebData.WebSecurity.GetUserId(User.Identity.Name);
            ViewBag.UserSettings = entities.GetUserSettings(userId, meeting.MeetingId, meeting.IsHK) ??
                ModelHelper.ToViewUserSettings(UserSettings.DEFAULT, meeting.IsHK);

            //hidden fields to control the auto-refresh of the grid
            ViewBag.MinutesBeforeJumpTimeToStartRefresh = ConfigurationManager.AppSettings["MinutesBeforeJumpTimeToStartRefresh"];
            ViewBag.RefreshIntervalSeconds = ConfigurationManager.AppSettings["RefreshIntervalSeconds"];

            //columns to display for this race
            ViewBag.GridColumns = entities.GetGridColumns(meeting.Races[0] as Race);

            return View(meeting);
        }

        public ActionResult Race(int id)
        {
            Race r = entities.GetRace(id);
            if (r == null)
            {
                return HttpNotFound();
            }

            ViewBag.GridColumns = entities.GetGridColumns(r);
            //r.RaceJumpTimeLocal = DateTime.Now; //for testing purposes
            return View("_Race", r);
        }

        /// <summary>
        /// Called async by the jqGrid on details view
        /// </summary>
        /// <returns>A list of runners</returns>
        public JsonResult GridData(int id, bool isStarted, bool isDone, GridSettings gridSettings)
        {
            //retrieve the static fields
            var runners = entities.GetRunnersStat(id);

            //TODO when implementing cache, this can be improved
            if (isDone || isStarted)
            {
                //update the runners info with the dynamic fields
                var runnersDyn = entities.GetRunnersDyn(id);
                runners.ToList().ForEach(r => r.Update(runnersDyn.First(rd => rd.RunnerId == r.RunnerId)));
            }

            //var runners = Runner.DummyRunnerList; //for testing purposes

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
                    runners = runners.OrderBy(o => o.AUS_SPW).ToList(); //default sorting criteria
            }

            return Json(runners, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GridSettings(int meetingId, bool isHK)
        {
            //get the user settings
            var userId = WebMatrix.WebData.WebSecurity.GetUserId(User.Identity.Name);
            var settings = entities.GetUserSettings(userId, meetingId, isHK) ??
                ModelHelper.ToViewUserSettings(UserSettings.DEFAULT, isHK);

            return View("_GridSettings", settings);
        }

    }
}