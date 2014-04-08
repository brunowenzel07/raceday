using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RaceDayDisplayApp.Models
{
    public class HistoryFilters
    {
        //public class RaceCourseFilterItem
        //{
        //    public string Id { get; set; }
        //    public string Name { get; set; }
        //    public string CountryId { get; set; }
        //}

        [Display(Name = "Country")]
        public int SelectedCountryId { get; set; }
        public IEnumerable<SelectListItem> CountryItems { get; set; }
        
        [Display(Name = "Racecourse")]
        public int SelectedRaceCourseId { get; set; }
        public IEnumerable<SelectListItem> RaceCourseItems { get; set; }
        
        public string AllRaceCourseItems { get; set; }

        [Display(Name = "Season")]
        public int SelectedSeasonId { get; set; }
        public IEnumerable<SelectListItem> SeasonItems { get; set; }

        [Display(Name = "Meeting Type")]
        public int SelectedSuperMeetTypeId { get; set; }
        public IEnumerable<SelectListItem> SuperMeetTypeItems { get; set; }

        [Display(Name = "Race Type")]
        public int SelectedSuperRaceTypeId { get; set; }
        public IEnumerable<SelectListItem> SuperRaceTypeItems { get; set; }

        [Display(Name = "No of Runners")]
        public int SelectedNumRunnersId { get; set; }
        public IEnumerable<SelectListItem> NumRunnersItems { get; set; }
    }
}