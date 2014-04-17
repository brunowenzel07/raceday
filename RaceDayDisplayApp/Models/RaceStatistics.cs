using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RaceDayDisplayApp.Models
{
    public class RaceStatistics
    {
        [Display(Name = "MarketPos", Order = 0)]
        public int oddsrank { get; set; }

        [Display(Name = "TotalRuns", Order = 1)]
        public int totalruns { get; set; }

        [DisplayFormat(DataFormatString = "{0}%")]
        [Display(Name = "1", Order = 2)]
        public float pc_firsts { get; set; }

        [DisplayFormat(DataFormatString = "{0}%")]
        [Display(Name = "2", Order = 3)]
        public float pc_seconds { get; set; }

        [DisplayFormat(DataFormatString = "{0}%")]
        [Display(Name = "3", Order = 4)]
        public float pc_thirds { get; set; }

        [DisplayFormat(DataFormatString = "{0}%")]
        [Display(Name = "4", Order = 5)]
        public float pc_fourths { get; set; }

        [DisplayFormat(DataFormatString = "{0}%")]
        [Display(Name = "U", Order = 6)]
        public float pc_unplaceds { get; set; }

        [Display(Name = "AvgWinDiv", Order = 7)]
        public float avgwindiv { get; set; }

        [Display(Name = "AvgPlaceDiv", Order = 8)]
        public float avgplacediv { get; set; }

        [Display(Name = "MaxLosingRun", Order = 9)]
        public int maxlosingstreak { get; set; }

        [Display(Name = "AvgLosingRun", Order = 10)]
        public int avglosingstreak { get; set; }

        [Display(Name = "ROIW", Order = 11)]
        public float roiwin { get; set; }

        [Display(Name = "ROIP", Order = 12)]
        public float roiplace { get; set; }
    }

    public class RaceResearchViewModel
    {
        [UIHint("_RaceStatistics")]
        public List<RaceStatistics> MarketData { get; set; }
        [UIHint("_RaceStatistics")]
        public List<RaceStatistics> FormFactors { get; set; }
    }
}