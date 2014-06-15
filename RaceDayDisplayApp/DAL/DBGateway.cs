using RaceDayDisplayApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Data.SqlClient;
using Dapper;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace RaceDayDisplayApp.DAL
{
    public class DBGateway
    {
        readonly string connectionString =
            ConfigurationManager.ConnectionStrings["RaceDayDB"].ConnectionString;

        /// <summary>
        /// Used in the MeetingList page
        /// </summary>
        public IEnumerable<Country> GetCountries(bool filterByToday)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var lookup = new Dictionary<int, Country>();
                var resultList = conn.Query<Country, MeetingBase, Country>(@"
                    SELECT c.Id as CountryId, c.Name, 
                           m.Id AS MeetingId, m.MeetingDate, rc.Name AS RaceCourseName,
						   MIN(r.RaceJumpDateTimeUTC) as MinRaceJumpDateTimeUTC,
                           MIN(r.LocalJumpTime) as MinRaceJumpTimeLocal
                    FROM Country AS c INNER JOIN Meeting AS m ON c.Id = m.CountryId
			                          LEFT JOIN RaceCourse AS rc ON m.RaceCourseId = rc.Id
									  LEFT JOIN Race AS r on r.MeetingId = m.Id
                    WHERE c.inUse = 1" + (filterByToday ? " AND m.MeetingDate >= CONVERT(date, getdate()) and m.MeetingDate <= DATEADD(day," + ConfigValues.UpcomingRacesDaysLimit + ",CONVERT(date, getdate()))" : "") + @"
					GROUP BY c.Id, c.Name, m.Id, m.MeetingDate, rc.Name
					ORDER BY " + (filterByToday ? "m.MeetingDate ASC, MinRaceJumpDateTimeUTC ASC" : "m.MeetingDate DESC, MinRaceJumpDateTimeUTC DESC"),
                    (c, m) =>
                    {
                        Country country;
                        if (!lookup.TryGetValue(c.CountryId, out country))
                        {
                            lookup.Add(c.CountryId, country = c);
                        }
                        if (country.Meetings == null)
                            country.Meetings = new List<MeetingBase>();
                        if (m != null)
                        {
                            country.Meetings.Add(m);
                        }
                        return country;
                    },
                    splitOn: "MeetingId"
                    );

                return lookup.Values;
            }
        }

        /// <summary>
        /// RaceList and RaceDetails page
        /// </summary>
        public IEnumerable<RaceDisplay> GetRacesList(bool today)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                return conn.Query<RaceDisplay>(@"
                    SELECT TOP " + ConfigValues.RaceListLimit + @"
                           Race.Id AS RaceId, Race.RaceNumber, Race.RaceName, Race.RaceJumpDateTimeUTC, 
                           Meeting.MeetingDate, RaceCourse.Name AS RaceCourseName, 
                           LocalJumpTime, AUS_StateId as StateId, RaceStatus, Country.Code as CountryCode
                    FROM Race INNER JOIN Meeting on Meeting.Id = Race.MeetingId
		                      INNER JOIN RaceCourse on Meeting.RaceCourseId = RaceCourse.Id
                              INNER JOIN Country on Race.CountryId = Country.Id
                    " + (today ? "WHERE Meeting.MeetingDate >= CONVERT(date, getdate()) and Meeting.MeetingDate <= DATEADD(day," + ConfigValues.UpcomingRacesDaysLimit + ",CONVERT(date, getdate()))" : "") + @"
                    ORDER BY " + (today ? "MeetingDate ASC, RaceJumpDateTimeUTC ASC" : "MeetingDate DESC, RaceJumpDateTimeUTC DESC"));
                //TODO review the WHERE condition            
            }
        }

        /// <summary>
        /// Used in the meeting details page
        /// </summary>
        public Meeting GetMeeting(int meetingId)
        {
            //return Meeting.DummyMeeting;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                    //Select Meeting.Id as MeetingId, RaceCourse.Name as RaceCourseName, Weather.Name as WeatherName, 
                    //       Going.Id as DefaultGoingName, CourseVariant.Name as CourseVariantName, Country.Code as CountryCode,
                    //       Race.Id as RaceId, Race.RaceNumber
                    //from Meeting left join RaceCourse    on Meeting.RaceCourseId=RaceCourse.Id
                    //               left join Weather       on Meeting.WeatherId=Weather.Id
                    //               left join Going         on Meeting.DefaultGoingId=Going.Id
                    //               left join CourseVariant on Meeting.CourseVariantId=CourseVariant.Id
                    //               left join Race          on Meeting.Id=Race.MeetingId
                    //               left join Country       on Meeting.CountryId=Country.Id
                    //where Meeting.Id = @MeetingId

                var lookup = new Dictionary<int, Meeting>();
                var resultList = conn.Query<Meeting,RaceBase,Meeting>(@"
                    Select meeting.*, Race.Id as RaceId, Race.RaceNumber
                    from getMeetingDetailsData(@MeetingId) meeting 
                         left join Race on meeting.MeetingId=Race.MeetingId",
                    (m, r) =>
                    {
                        Meeting meeting;
                        if (!lookup.TryGetValue(m.MeetingId, out meeting))
                        {
                            lookup.Add(m.MeetingId, meeting = m);
                        }
                        if (meeting.Races == null)
                            meeting.Races = new List<RaceBase>();
                        if (r != null)
                            meeting.Races.Add(r);
                        return meeting;
                    },
                    param: new { MeetingId = meetingId },
                    splitOn: "RaceId"
                    );

                return resultList.FirstOrDefault();
            }
        }

        /// <summary>
        /// used in the RaceDetails page
        /// </summary>
        internal Meeting GetMeetingByRaceId(int raceId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                    //Select Meeting.Id as MeetingId, RaceCourse.Name as RaceCourseName, Weather.Name as WeatherName, 
                    //       Going.Id as DefaultGoingName, CourseVariant.Name as CourseVariantName, Country.Code as CountryCode
                    //from Race left join Meeting       on Meeting.Id=Race.MeetingId
                    //          left join RaceCourse    on Meeting.RaceCourseId=RaceCourse.Id
                    //          left join Weather       on Meeting.WeatherId=Weather.Id
                    //          left join CourseVariant on Meeting.CourseVariantId=CourseVariant.Id
                    //          left join Going		    on Race.RaceGoingId=Going.Id
                    //          left join Country		on Meeting.CountryId=Country.Id
                    //where Race.Id = @RaceId

                var resultList = conn.Query<Meeting>(@"
                    declare @m int
                    Select @m = MeetingId from Race where Race.Id = @RaceId
                    Select * from getMeetingDetailsData(@m)",
                    new { RaceId = raceId });

                return resultList.FirstOrDefault();
            }
        }

        /// <summary>
        /// used in the cache layer
        /// </summary>
        public RaceCache GetRaceWithRunnersStat(int raceId, CountryEnum country)
        {
            //return Race.DummyRace;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                    //Select Race.Id as RaceId, MeetingId, RaceNumber, RaceType.Name as RaceTypeName, 
                    //       Distance.Name as DistanceName, RaceJumpDateTimeUTC, 
                    //       HK_RaceIndex, Going.Name as RaceGoingName, isTurf, 
                    //       isDone, isStarted, Country.Code as CountryCode, 
                    //       LocalJumpTime, MeetingDate, AUS_StateId as StateId
                    //from Race left join RaceType on Race.RaceTypeId=RaceType.Id
                    //            left join Distance on Race.DistanceId=Distance.Id
                    //            left join Going    on Race.RaceGoingId=Going.Id
                    //            left join Meeting on Race.MeetingId=Meeting.Id
                    //            left join Country on Meeting.CountryId=Country.Id
                    //            left join RaceCourse on Meeting.RaceCourseId=RaceCourse.Id
                    //where Race.Id = @RaceId

                var race = conn.Query<RaceCache>("Select * from getRaceDetailsData(@RaceId)",
                    new { RaceId = raceId }).FirstOrDefault();

                race.Country = country;

                if (race != null)
                {
                    string query;
                    switch (country)
                    {
                        case CountryEnum.HK:
                            query = @"
                                SELECT	RaceId, HorseNumber, Barrier, RunnerId, 
                                        Horse, isScratched, 
                                        HorseHKGId as HorseId, Jockey, jockeypoints, Trainer, Age, Color, Place,
                                        Z_WinOddsRank, AVG3WinOddsRank, nUp, [Class+/-] as Class, 
                                        Rtg, [Gld?] as Gld, CWt, [%BW] as BW, [Wt+/-] as WtPlusLess,
                                        DSLR, [BFAVL?] as BFAVL, [Mdn?] as Mdn, Sex, MktRel, JmpRnk, FinishRnk, [NewTr?] as NewTr, 
                                        [LSW?] as LSW, [FirstStart?] as FirstStart, 
                                        [KAD?] as KAD, [RanOnL?] as RanOnL, [LostLeadL?] as LostLeadL, [FUP?] as FUP, [LUP?] as LUP,
                                        [QBU?] as QBU, [GJD?] as GJD, [DRPD?] as DRPD, [H4CRSE?] as H4CRSE, 
                                        [H&J?] as HJ, [1TRICKJ?] as TRICKJ, 
                                        [NewGear?] as NewGear, BeenThere, TurfPts, AWTPts, LAST10
                                from DataGrid_staFNHKG(@RaceId)";
                            break;
                        case CountryEnum.RSA:
                            query = @"
                                SELECT	RaceId, HorseNumber, Barrier, RunnerId, 
                                        Horse, isScratched, 
                                        HorseRSAId as HorseId, Jockey, jockeypoints, Trainer, Age, Color, Place,
                                        Z_WinOddsRank, AVG3WinOddsRank, nUp, [Class+/-] as Class, 
                                        [Gld?] as Gld, Wt, [Wt+/-] as WtPlusLess,
                                        DSLR, [BFAVL?] as BFAVL, [Mdn?] as Mdn, Sex, MktRel, [NewTr?] as NewTr, 
                                        [LSW?] as LSW, [FirstStart?] as FirstStart, 
                                        [KAD?] as KAD, [ROLast?] as ROLast, [SwampedLast?] as SwampedLast,
                                        [FUP?] as FUP, [LUP?] as LUP,
                                        [QBU?] as QBU, [GJD?] as GJD, [DRPD?] as DRPD, [H4CRSE?] as H4CRSE, 
                                        [H&J?] as HJ, BeenThere, SandPts, TurfPts, PolyPts, LAST10
                                from DataGrid_staFNRSA(@RaceId)";
                            break;
                        case CountryEnum.AUS:
                            query = @"
                                SELECT	RaceId, HorseNumber, Barrier, RunnerId, 
                                        Horse, isScratched, 
                                        HorseAUSNZId as HorseId, Jockey, jockeypoints, Trainer, Age, Color, Place,
                                        Z_WinOddsRank, AVG3WinOddsRank, nUp, [Class+/-] as Class, 
                                        [Gld?] as Gld, Wt, [Wt+/-] as WtPlusLess,
                                        DSLR, [BFAVL?] as BFAVL, [Mdn?] as Mdn, Sex, MktRel, [NewTr?] as NewTr, 
                                        [LSW?] as LSW, [FirstStart?] as FirstStart, 
                                        [KAD?] as KAD, [ROLast?] as ROLast, [SwampedLast?] as SwampedLast,
                                        [FUP?] as FUP, [LUP?] as LUP,
                                        [QBU?] as QBU, [GJD?] as GJD, [DRPD?] as DRPD, [H4CRSE?] as H4CRSE, 
                                        [H&J?] as HJ, BeenThere, AWTPts, TurfPts, LAST10
                                from DataGrid_staFNAUSNZ(@RaceId)";
                            break;
                        default:
//                            query = @"
//                                SELECT	RaceId, HorseNumber, Barrier, RunnerId, 
//                                        Name, Horse, isScratched, 
//                                        HorseId, Jockey, Trainer, Gear, Age, Sex, Color,
//                                        AUS_HcpWt, AUS_HcpRatingAtJump, HK_ActualWtLbs, HK_Rating, Place, 
//                                        nUp, [Class+/-] as Class, 
//                                        Rtg, [Gld?] as Gld, CWt, [%BW] as BW, [Wt+/-] as Wt, [NewTr?] as NewTr, 
//                                        [LSW?] as LSW, [FirstStart?] as FirstStart, 
//                                        [KAD?] as KAD, [ROLast?] as ROLast, [SwampedLast?] as SwampedLast, 
//                                        [FUP?] as FUP, [LUP?] as LUP
//                                from DataGrid_staFN(@RaceId)";
                            throw new NotImplementedException();
                    }

                    var runners= conn.Query<Runner>(query, new { RaceId = raceId });

                    race.Runners = runners.ToList();
                }

                return race;
            }
        }

        /// <summary>
        /// Used in the cache layer
        /// </summary>
        public RaceDyn GetRaceWithRunnersDyn(int raceId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var race = conn.Query<RaceDyn>(@"
                    SELECT RaceId, isDone, RaceWinPool, RacePPPool, EXPoolTotal, EXDivAmount,
                    QNPoolTotal, QNDivAmount, F4PoolTotal, F4DivAmount, TFPoolTotal
                    FROM RaceDivPoolData
                    WHERE RaceId = @RaceId",
                    new { RaceId = raceId }).FirstOrDefault();

                if (race != null)
                {
                    var resultList = conn.Query<RunnerDyn>(@"
                        select isScratched, RunnerId, RaceId, HorseNumber, 
                        WinOdds, PlaceOdds, isWinFavorite, WinDropby20, WinDropby50,
                        isPlaceFavorite, PlaceDropby20, PlaceDropby50, 
                        ODDSLAST1, ODDSLAST2, ODDSLAST3, Z_WinOddsRank, AVG3WinOddsRank
                        from DataGrid_dynFN(@RaceId)",
                        new { RaceId = raceId });

                    race.Runners = resultList;
                }

                return race;
            }
        }

            
        /// <summary>
        /// used in the cache layer
        /// </summary>
        internal int GetSecondsSinceLastUpdate(Race race, out int refreshInterval)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var resultList = conn.Query(@"
                        Select DATEDIFF(second, UpdateDateTime, getdate()) as SecsSinceLastUpdate, Refreshinterval
                        from LastUpdates
                        where MeetingId = @meetingId and RaceNumber=@raceNumber",
                    new { meetingId = race.MeetingId, raceNumber = race.RaceNumber });
                
                var result = resultList.FirstOrDefault();

                if (result != null)
                {
                    refreshInterval = result.Refreshinterval;
                    return result.SecsSinceLastUpdate;
                }
                else
                {
                    refreshInterval = -1;
                    return -1;
                }
            }
        }

        /// <summary>
        /// used in meeting details page, race details page and _GridSettings partial view
        /// </summary>
        public IEnumerable<ViewUserSetting> GetUserSettings(int userId, CountryEnum country)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                //retrieve the user settings
                var settings = conn.Query<UserSettings>(@"
                    SELECT * 
                    FROM Settings
                    WHERE userId = @UserId",
                   new { UserId = userId }).FirstOrDefault();

                if (settings == null)
                    settings = UserSettings.DEFAULT;

                var viewSettings = ModelHelper.ToViewUserSettings(settings, country);
                return viewSettings;

//                //retrieve the statistical columns
//                var resultList2 = conn.Query(@"
//                    SELECT rs.LongName, rs.Ordering
//                    FROM RaceStatistics rs
//                    WHERE rs.MeetingId = @MeetingId",
//                    new { MeetingId = meetingId });

//                string aux;
//                var statSettings = resultList2.Select(rs => new ViewUserSetting
//                {
//                    PropertyName = aux = "STATISTICS" + rs.Ordering,
//                    DisplayName = rs.LongName,
//                    Checked = (bool)typeof(UserSettings).GetProperty(aux).GetValue(settings)
//                });

                ////concat user settings and statistical columns
                //return viewSettings.Concat(statSettings);
            }
        }

        /// <summary>
        /// Used in RaceDetails page and _Race partial view
        /// </summary>
        public IEnumerable<MvcJqGrid.Column> GetGridColumns(Race race)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                //retrieve the statistical columns

                var resultList = conn.Query(@"
                    SELECT rs.LongName, rs.Ordering
                    FROM RaceStatistics rs
                    WHERE rs.RaceId = @RaceId",
                    new { RaceId = race.RaceId });

                return resultList.Select(e => new MvcJqGrid.Column("STATISTICS" + e.Ordering)
                       .SetLabel(e.LongName)).Cast<MvcJqGrid.Column>();
            }
        }

        public Dictionary<int, string> GetTimeZones()
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                var q = @"select AUS_State.Id as AUSStateId, CountryState_Timezone.Timezone as Code
                            from CountryState_Timezone inner join AUS_State 
                                    on AUS_State.Code = CountryState_Timezone.Code";

                var timezones = new Dictionary<int, string>();
                conn.Query(q).ToList().ForEach(x => timezones.Add(x.AUSStateId, x.Code));
                return timezones;
            }
        }


        public IEnumerable<dynamic> GetRunnerHistory(int horseId, string countryCode)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                return conn.Query("Select * from getRunnerHistory(@horseid, @date, @countrycode)",
                                    new 
                                    { 
                                        horseid = horseId,
                                        date = DateTime.UtcNow.Date,
                                        countrycode = countryCode
                                    });
            }
        }

        public dynamic GetHorseDetailsData(int horseId)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                return conn.Query("Select * from getHorseDetailsDataHKG(@horseId)",
                                    new { horseId = horseId }).FirstOrDefault();
            }
        }

        //        /// <summary>
        //        /// True if the race is held in Hong Kong
        //        /// </summary>
        //        public bool IsHK(int raceId)
        //        {
        //            using (var conn = new SqlConnection(connectionString))
        //            {
        //                conn.Open();

        //                var code = conn.Query<string>(@"
        //                    select Country.Code
        //                    from Race inner join Meeting on Race.MeetingId = Meeting.Id
        //		                      inner join Country on Meeting.CountryId = Country.Id
        //                    where Race.Id = @RaceId",
        //                   new { RaceId = raceId }).FirstOrDefault();

        //                return code == "HKG";
        //            }
        //        }


        static HistoryFilters historyFiltersInstance = null;

        public HistoryFilters GetHistoryFilters()
        {
            //no need to retrieve from database every time
            if (historyFiltersInstance != null)
                return historyFiltersInstance;

            HistoryFilters result = new HistoryFilters();
            historyFiltersInstance = result;

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                //COUNTRY
                var countries = conn.Query("Select Id, Name from Country").ToList();
                result.CountryItems = countries.Select(c =>
                    new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    });
                result.SelectedCountryId = int.Parse(result.CountryItems.FirstOrDefault(c => c.Text == "Hong Kong SAR").Value);


                //RACECOURSES (COUNTRY-DEPENDENT)
                var allRaceCourses = conn.Query<ComboItem>("Select Id, Name, CountryId from RaceCourse");
                result.AllRaceCourseItems = new JavaScriptSerializer().Serialize(allRaceCourses);

                result.RaceCourseItems = allRaceCourses
                    .Where(rc => rc.CountryId == result.SelectedCountryId)
                    .Select(rc =>
                    new SelectListItem
                    {
                        Value = rc.Id.ToString(),
                        Text = rc.Name
                    });


                //SUPERMEETINGTYPE (COUNTRY-DEPENDENT)
                var allMeetingTypes = conn.Query<ComboItem>("Select Id, Name, CountryId from SuperMeetingType");
                //add "ALL" values (one per country)
                allMeetingTypes = countries.Select(c => new ComboItem
                    { 
                        Id = 0,
                        Name = "ALL MEETING TYPES",
                        CountryId = c.Id
                    }).Concat(allMeetingTypes);

                result.AllSuperMeetTypeItems = new JavaScriptSerializer().Serialize(allMeetingTypes);

                result.SuperMeetTypeItems = allMeetingTypes
                    .Where(mt => mt.CountryId == result.SelectedCountryId)
                    .Select(c =>
                    new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    });
                result.SelectedSuperMeetTypeId = 0;


                //SUPERRACETYPE (COUNTRY-DEPENDENT)
                var allRaceTypes = conn.Query<ComboItem>("Select Id, Name, CountryId from SuperRaceType");
                //add "ALL" values (one per country)
                allRaceTypes = countries.Select(c => new ComboItem
                    {
                        Id = 0,
                        Name = "ALL",
                        CountryId = c.Id
                    }).Concat(allRaceTypes);
                
                result.AllSuperRaceTypeItems = new JavaScriptSerializer().Serialize(allRaceTypes);

                result.SuperRaceTypeItems = allRaceTypes
                    .Where(rt => rt.CountryId == result.SelectedCountryId)
                    .Select(c =>
                    new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    });
                result.SelectedSuperRaceTypeId = 0;


                //SEASON
                result.SeasonItems = (new[] {new SelectListItem
                    {
                        Value = "0",
                        Text = "ALL SEASONS"
                    }})
                    .Concat(conn.Query("Select Id, Name from Season").Select(c =>
                    new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }));
                result.SelectedSeasonId = 0;


                //NUMBEROFRUNNERS (HARDCODED)
                var numRun = new List<SelectListItem>(39);
                numRun.Add(new SelectListItem
                {
                    Value = "0",
                    Text = "ALL"
                });
                for (int i = 2; i <= 40; i++)
			    {
                    numRun.Add(new SelectListItem
                        {
                            Value = i.ToString(),
                            Text = i.ToString()
                        });
			    }
                result.NumRunnersItems = numRun;
                result.SelectedNumRunnersId = 0;

            }

            return result;
        }


        public List<RaceStatistics> GetMarketData(HistoryFilters filters, int maxResults)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                return conn.Query<RaceStatistics>("Select" +
                    (maxResults > 0 ? " top " + maxResults : "") +
                    @" totalruns,
                    'F'+CONVERT(varchar(5), oddsrank) as MarketPos,
                    pc_firsts,
                    pc_seconds,
                    pc_thirds,
                    pc_fourths,
                    pc_unplaceds,
                    avgwindiv,
                    avgplacediv,
                    maxlosingstreak,
                    avglosingstreak,
                    roiwin,
                    roiplace
                    from RaceStatistics
                    where countryid=@countryid
                    and racecourseId=@racecourseId" +
                    (filters.SelectedSuperMeetTypeId > 0 ? " and supermeetingtypeId=@supermeetingtypeId" : "") +
                    (filters.SelectedSuperRaceTypeId > 0 ? " and superracetypeid=@superracetypeid" : "") +
                    (filters.SelectedNumRunnersId > 0 ? " and NumberOfRunners=@numberofrunners" : "") +
                    " order by roiwin DESC",
                    new 
                    { 
                        countryid = filters.SelectedCountryId,
                        racecourseId = filters.SelectedRaceCourseId,
                        supermeetingtypeId = filters.SelectedSuperMeetTypeId,
                        superracetypeid = filters.SelectedSuperRaceTypeId,
                        numberofrunners = filters.SelectedNumRunnersId
                    }).ToList();
            }
        }


        internal List<RaceStatistics> GetFormFactors(HistoryFilters filters)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                return conn.Query<RaceStatistics>(@"Select
                    totalruns,
                    Formfactors.Name as MarketPos,
                    pc_firsts,
                    pc_seconds,
                    pc_thirds,
                    pc_fourths,
                    pc_unplaceds,
                    avgwindiv,
                    avgplacediv,
                    maxlosingstreak,
                    avglosingstreak,
                    roiwin,
                    roiplace
                    from FormFactorsTable inner join Formfactors
                    on FormFactorsTable.formfactorsId=Formfactors.Id
                    where countryid=@countryid
                    and racecourseId=@racecourseId" +
                    (filters.SelectedSuperMeetTypeId > 0 ? " and supermeetingtypeId=@supermeetingtypeId" : "") +
                    (filters.SelectedSuperRaceTypeId > 0 ? " and superracetypeid=@superracetypeid" : "") +
                    (filters.SelectedNumRunnersId > 0 ? " and NumberOfRunners=@numberofrunners" : "") +
                    " order by roiwin DESC",
                    new
                    {
                        countryid = filters.SelectedCountryId,
                        racecourseId = filters.SelectedRaceCourseId,
                        supermeetingtypeId = filters.SelectedSuperMeetTypeId,
                        superracetypeid = filters.SelectedSuperRaceTypeId,
                        numberofrunners = filters.SelectedNumRunnersId
                    }).ToList();
            }
        }
    }

}
