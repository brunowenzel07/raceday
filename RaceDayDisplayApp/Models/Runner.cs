using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceDayDisplayApp.Models
{
    /// Each of these boolean properties will be rendered as checkboxes in the view.
    /// In order to be properly linked to the columns of the grid, their names should match the names of the Runner class properties
    /// The order they are rendered is determined by the attribute Display.Order


    /// <summary>
    /// Base class with the dynamic fields, those which are going to be refreshed periodically
    /// </summary>
    public class RunnerBase
    {
        [Key]
        [CustomDisplay(DisplayOn.NONE)]
        public int RunnerId { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public int RaceId { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public bool isScratched { get; set; } //when it is true, the font is set red and strikethrough 

        [CustomDisplay(DisplayOn.NONE)]
        public bool isDone { get; set; }

        [Display(Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public int ODDSLAST1 { get; set; }  

        [Display(Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public int ODDSLAST2 { get; set; }  

        [Display(Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public int ODDSLAST3 { get; set; }  
    }

    /// <summary>
    /// Static fields
    /// </summary>
    public class Runner: RunnerBase
    {
        [Display(Name = "Tab No.", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public int HorseNumber { get; set; }

        [Display(Name = "BP", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public int Barrier { get; set; }
        
        [Display(Name = "Horse", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH, colSize: ConfigValues.NAME_COLUMN_WIDTH)]
        public string Name { get; set; }

        [Display(Name = "Chinese Name", Order = 0)]
        [CustomDisplay(DisplayOn.HK, colSize: ConfigValues.NAME_COLUMN_WIDTH)]
        public string HK_ChineseName { get; set; }

        [Display(Name = "Jockey", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH, colSize: ConfigValues.NAME_COLUMN_WIDTH)]
        public string Jockey { get; set; }
        
        [Display(Name = "Trainer", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH, colSize: ConfigValues.NAME_COLUMN_WIDTH)]
        public string Trainer { get; set; }

        [Display(Name = "Gear", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public string Gear { get; set; }

        [Display(Name = "New Trainer", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public bool Z_newTrainerSinceLastStart { get; set; }

        [CustomDisplay(DisplayOn.BOTH, CustomFormatters.percentageFormatter)]
        [Display(Name = "Barrier Advantage", Order = 0)]
        public int Z_BPAdvFactor { get; set; }

        [CustomDisplay(DisplayOn.BOTH)]
        [Display(Name = "Age", Order = 0)]
        public string Age { get; set; }

        [Display(Name = "Sex", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public string Sex { get; set; }

        [Display(Name = "Color", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public string Color { get; set; }

        [Display(Name = "Weight", Order = 0)]
        [CustomDisplay(DisplayOn.AUS)]
        public float AUSHcpWT { get; set; }

        [Display(Name = "Carried Wt.", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public float CarriedWt { get; set; }

        [Display(Name = "Horse Wt.", Order = 0)]
        [CustomDisplay(DisplayOn.HK)]
        public float HK_DeclaredHorseWtLbs { get; set; }

        [Display(Name = "HcpRtg", Order = 0)]
        [CustomDisplay(DisplayOn.AUS)]
        public int AUS_HcpRatingAtJump { get; set; }

        [Display(Name = "Wt.", Order = 0)]
        [CustomDisplay(DisplayOn.HK)]
        public float HK_ActualWtLbs { get; set; }

        [Display(Name = "Rating", Order = 0)]
        [CustomDisplay(DisplayOn.HK)]
        public int HK_Rating { get; set; }

        [Display(Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public string Place { get; set; }

        [Display(Name = "SP Win", Order = 0)]
        [CustomDisplay(DisplayOn.AUS, CustomFormatters.currencyFormatter)] 
        public int AUS_SPW { get; set; }

        [Display(Name = "SP Place", Order = 0)]
        [CustomDisplay(DisplayOn.AUS, CustomFormatters.currencyFormatter)]
        public int AUS_SPP { get; set; }

        [Display(Name = "Win Odds", Order = 0)]
        [CustomDisplay(DisplayOn.HK, CustomFormatters.currencyFormatter)]
        public int HK_WinOdds { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public DateTime Z_AUS_FinishTime { get; set; }

        [Display(Name = "Time", Order = 0)]
        [CustomDisplay(DisplayOn.AUS)]
        public string Formatted_Z_AUS_FinishTime { get { return Z_AUS_FinishTime.ToString("mm:ss.ff"); } }

        [CustomDisplay(DisplayOn.NONE)]
        public DateTime HK_FinishTime { get; set; }

        [Display(Name = "Time", Order = 0)]
        [CustomDisplay(DisplayOn.HK)]
        public string Formatted_HK_FinishTime { get { return HK_FinishTime.ToString("mm:ss.ff"); } }

        [Display(Name = "Jockey Challenge", Order = 0)]
        [CustomDisplay(DisplayOn.BOTH)]
        public int JockeyPoints { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public float STATISTICS1 { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public float STATISTICS2 { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public float STATISTICS3 { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public float STATISTICS4 { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public static IList<Runner> DummyRunnerList 
        {
            get 
            {
                return FizzWare.NBuilder.Builder<Runner>.CreateListOfSize(10).Build();
            }
        }

        internal void Update(RunnerBase runner)
        {
            this.isScratched = runner.isScratched;
            this.isDone = runner.isDone;
            this.ODDSLAST1 = runner.ODDSLAST1;
            this.ODDSLAST2 = runner.ODDSLAST2;
            this.ODDSLAST3 = runner.ODDSLAST3;
        }
    }

}
