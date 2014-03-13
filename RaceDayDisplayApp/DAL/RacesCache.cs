using RaceDayDisplayApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Timers;
using System.Web;

namespace RaceDayDisplayApp.DAL
{
    public class RacesCache
    {
        public static RacesCache Instance = new RacesCache();

        Dictionary<int, RaceCache> races = new Dictionary<int, RaceCache>();

        Timer timer;

        private RacesCache()
        {
            timer = new Timer(ConfigValues.CacheRefreshIntervalMs);
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = true;
        }

        public RaceCache this[int id]
        {
            get
            {
                RaceCache race;
                if (!races.TryGetValue(id, out race))
                {
                    lock (races)
                    {
                        //try again because it may have been updated while waiting on the lock
                        if (!races.TryGetValue(id, out race))
                        {
                            race = getRace(id, null);
                            races.Add(id, race);
                        }
                    }
                }
                lock (race)
                {
                    return race;
                }
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (timer)
            {
                var now = DateTime.UtcNow;
                List<int> racesToDelete = new List<int>();
                List<RaceCache> racesToUpdate = new List<RaceCache>();

                foreach (var race in races.Values)
                {
                    if (race.RefreshVaues.NextRefresh < now)
                    {
                        lock (race)
                        {
                            //if it is refresh time, retrieve from database
                            racesToUpdate.Add(getRace(race.RaceId, race));
                        }
                    }
                    //if the race is two days old, it is removed from cache
                    else if (race.RaceJumpDateTimeUTC.Date.AddDays(1) < DateTime.UtcNow.Date)
                        racesToDelete.Add(race.RaceId);
                }

                if (racesToDelete.Count > 0 || racesToUpdate.Count > 0)
                {
                    lock (races)
                    {
                        racesToDelete.ForEach(r => races.Remove(r)); //remove old
                        racesToUpdate.ForEach(r => races[r.RaceId] = r); //update refreshed
                    }
                }
            }
        }


        private RaceCache getRace(int id, RaceCache oldRace)
        {
            var now = DateTime.UtcNow;
            var dbGateway = new DBGateway();


            //retrieve grid dynamic fields
            var runnersDyn = dbGateway.GetRunnersDyn(id);
            bool isDone = false;
            if (runnersDyn.Count() > 0)
                isDone = runnersDyn.First().isDone;


            //retrieve grid static fields
            RaceCache r;
            int refreshInterval;
            if (oldRace != null && !isDone)
            {
                bool inactive;
                refreshInterval = getCurrentRefreshInterval(oldRace, out inactive);
                //if the race is inactive (once every hour) or finished, the cached race is overwritten with the one from DB 
                r = inactive ? dbGateway.GetRaceWithRunners(id) : oldRace;
            }
            else //first time that this race is requested -> retrieve from DB
            {
                r = dbGateway.GetRaceWithRunners(id);
                refreshInterval = getCurrentRefreshInterval(oldRace);
            }


            //update the runners info with the dynamic fields
            r.Runners.ToList().ForEach(runn => runn.Update(runnersDyn.First(rd => rd.RunnerId == runn.RunnerId)));


            //update refresh info
            var secsSinceLastRefresh = dbGateway.GetSecondsSinceLastUpdate(r.RaceId);
            r.RefreshVaues.LastDBUpdate = now.AddSeconds(-secsSinceLastRefresh);
            r.RefreshVaues.LastRefresh = now;
            r.RefreshVaues.NextRefresh = r.isDone ? DateTime.MaxValue //if it is done, it is not refreshed anymore
                                                 : now.AddSeconds(refreshInterval - secsSinceLastRefresh);
            
            return r;
        }

        private static int getCurrentRefreshInterval(RaceCache r)
        {
            bool aux;
            return getCurrentRefreshInterval(r, out aux);
        }

        private static int getCurrentRefreshInterval(RaceCache r, out bool inactive)
        {
            inactive = false;
            var now = DateTime.UtcNow;
            int secsToNextRefresh = 0;

            if (r.isDone)
                secsToNextRefresh = -1;
            else
            {
                //check if race is active
                var activeTimeUTC = r.RaceJumpDateTimeUTC.AddSeconds(-ConfigValues.ActiveBeforeJumpTime);
                var finishingTimeUTC = r.RaceJumpDateTimeUTC.AddSeconds(-ConfigValues.FinishingBeforeJumpTime);
                var active = activeTimeUTC <= now;
                var finishing = finishingTimeUTC <= now;

                if (active)
                {
                    if (finishing) //last 5 minutes, decrese delay
                    {
                        secsToNextRefresh = ConfigValues.FinishingRefreshinterval;
                    }
                    else
                    {
                        //if it is after MorningLine-5, set to MorningLine-5
                        if (now.AddSeconds(ConfigValues.ActiveRefreshInterval) > finishingTimeUTC)
                            secsToNextRefresh = (int)(finishingTimeUTC - now).TotalSeconds;
                        else
                            secsToNextRefresh = ConfigValues.ActiveRefreshInterval;
                    }
                }
                else
                {
                    //if it is after MorningLine-25, set to MorningLine-25
                    if (now.AddSeconds(ConfigValues.InactiveRefreshInterval) > activeTimeUTC)
                        secsToNextRefresh = (int)(activeTimeUTC - now).TotalSeconds;
                    else
                        secsToNextRefresh = ConfigValues.InactiveRefreshInterval;

                    inactive = true;
                }
            } //else

            return secsToNextRefresh;
        }

    }
}