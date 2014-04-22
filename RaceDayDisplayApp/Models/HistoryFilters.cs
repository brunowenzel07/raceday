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

        [Range(1, int.MaxValue)]
        [Display(Name = "Country")]
        public int SelectedCountryId { get; set; }
        public IEnumerable<SelectListItem> CountryItems { get; set; }

        [Range(1, int.MaxValue)]
        [Display(Name = "Racecourse")]
        public int SelectedRaceCourseId { get; set; }
        public IEnumerable<SelectListItem> RaceCourseItems { get; set; }
        public string AllRaceCourseItems { get; set; }


        [Range(0, int.MaxValue)]
        [Display(Name = "Season")]
        public int SelectedSeasonId { get; set; }
        public IEnumerable<SelectListItem> SeasonItems { get; set; }


        [Range(0, int.MaxValue)]
        [Display(Name = "Meeting Type")]
        public int SelectedSuperMeetTypeId { get; set; }
        public IEnumerable<SelectListItem> SuperMeetTypeItems { get; set; }
        public string AllSuperMeetTypeItems { get; set; }


        [Range(0, int.MaxValue)]
        [Display(Name = "Race Type")]
        public int SelectedSuperRaceTypeId { get; set; }
        public IEnumerable<SelectListItem> SuperRaceTypeItems { get; set; }
        public string AllSuperRaceTypeItems { get; set; }


        [Range(0, 40)]
        [Display(Name = "No Runners")]
        public int SelectedNumRunnersId { get; set; }
        public IEnumerable<SelectListItem> NumRunnersItems { get; set; }
    }

    class ComboItem
    {
        public int Id;
        public string Name;
        public int CountryId;
    }
}