﻿using RaceDayDisplayApp.Models;
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
        Dictionary<int, object> racesLocks = new Dictionary<int, object>();

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
                lock (races)
                {
                    if (!races.TryGetValue(id, out race))
                        racesLocks.Add(id, new object());
                }

                lock (racesLocks[id])
                {
                    //try again because it may have been updated while waiting on the lock
                    if (race == null && !races.TryGetValue(id, out race))
                    {
                        race = getRace(id, null);
                        lock (races)
                        {
                            races.Add(id, race);
                        }
                    }
                }
                return race;
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (timer)
            {
                var now = DateTime.UtcNow;
                List<int> racesToDelete = new List<int>();
                List<int> racesToUpdate = new List<int>();

                lock (races)
                {
                    foreach (var race in races.Values)
                    {
                        //if it is refresh time, retrieve from database
                        if (race.RefreshValues.NextRefresh < now)
                            racesToUpdate.Add(race.RaceId);
                        //if the race hasn't been requested for a while, it is removed from cache
                        else if (race.RefreshValues.LastUserRequest.AddSeconds(ConfigValues.CacheExpireTimeSec) < now)
                            racesToDelete.Add(race.RaceId);
                    }
                }

                if (racesToDelete.Count > 0 || racesToUpdate.Count > 0)
                {
                    var updatedRaces = new List<RaceCache>(racesToUpdate.Count());
                    racesToUpdate.ForEach(id =>
                        {
                            lock (racesLocks[id])
                            {
                                //retrieve from database and update
                                updatedRaces.Add(getRace(id, races[id]));
                            }
                        });

                    lock (races)
                    {
                        updatedRaces.ForEach(r => races[r.RaceId] = r); //update refreshed
                        racesToDelete.ForEach(id => //remove old
                            {
                                races.Remove(id);
                                racesLocks.Remove(id);
                            }); 
                    }
                }
            }
        }


        private RaceCache getRace(int id, RaceCache oldRace)
        {
            var now = DateTime.UtcNow;
            var dbGateway = new DBGateway();


            //retrieve grid dynamic fields
            var raceDyn = dbGateway.GetRaceWithRunnersDyn(id);

            //retrieve grid static fields
            RaceCache r;
            int refreshInterval;
            if (oldRace != null && !raceDyn.isDone)
            {
                bool inactive;
                refreshInterval = getCurrentRefreshInterval(oldRace, out inactive);
                //if the race is inactive (once every hour) or finished, the cached race is overwritten with the one from DB 
                r = inactive ? dbGateway.GetRaceWithRunnersStat(id) : oldRace;
            }
            else //first time that this race is requested -> retrieve from DB
            {
                r = dbGateway.GetRaceWithRunnersStat(id);
                refreshInterval = getCurrentRefreshInterval(r);
            }

            //update the race and runners info with the dynamic fields
            r.Update(raceDyn);

            //update refresh info
            var secsSinceLastRefresh = dbGateway.GetSecondsSinceLastUpdate(r);
            r.RefreshValues.LastDBUpdate = now.AddSeconds(-secsSinceLastRefresh);
            r.RefreshValues.LastServerRefresh = now;
            if (r.isDone) //if it is done, it is not refreshed anymore
                r.RefreshValues.NextRefresh = DateTime.MaxValue;
            else if (secsSinceLastRefresh < refreshInterval)
                r.RefreshValues.NextRefresh = now.AddSeconds(refreshInterval - secsSinceLastRefresh + ConfigValues.ServerAddedDelay);
            else
                r.RefreshValues.NextRefresh = now.AddSeconds(refreshInterval);

            if (oldRace == null) //update this value if it was a user request
                r.RefreshValues.LastUserRequest = now;

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
            else if (r.RaceJumpDateTimeUTC == default(DateTime)) //if jump time wasn't set
                secsToNextRefresh = ConfigValues.InactiveRefreshInterval; 
            else
            {
                //check if race is active
                var activeTimeUTC = r.RaceJumpDateTimeUTC.AddSeconds(-ConfigValues.ActiveBeforeJumpTime);
                var finishingTimeUTC = r.RaceJumpDateTimeUTC.AddSeconds(-ConfigValues.FinishingBeforeJumpTime);
                var active = activeTimeUTC <= now;
                var finishing = finishingTimeUTC <= now;

                if (active) //TODO check if it's possible to use r.isStarted
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