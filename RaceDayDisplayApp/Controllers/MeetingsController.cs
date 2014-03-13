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

namespace RaceDayDisplayApp.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class MeetingsController : Controller
    {
        private DBGateway entities = new DBGateway();

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

            return View(meeting);
        }

        public ActionResult RaceDetails(int id = 0, bool today = true)
        {
            //get the meeting and Race info
            Meeting meeting = entities.GetMeetingByRaceId(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }

            //get race info from cache
            meeting.Races = new List<RaceBase>(new[] { RacesCache.Instance[id] });

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

            //columns to display for this race
            ViewBag.GridColumns = ModelHelper.GetGridColumns(meeting.Races[0] as Race);

            return View(meeting);
        }

        public ActionResult Race(int id)
        {
            var r = RacesCache.Instance[id];
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
        public JsonResult GridData(int id, GridSettings gridSettings)
        {
            var race = RacesCache.Instance[id];
            var runners = race.Runners;
            //int secsToNextRefresh; //TODO

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
            
                //dynamic obj = runners.First();
                //obj. //todo
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