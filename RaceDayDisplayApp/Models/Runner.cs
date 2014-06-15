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
    public class RunnerDyn
    {
        [CustomDisplay(DisplayOn.NONE)]
        public bool isScratched { get; set; } //when it is true, the font is set red and strikethrough 

        [Key]
        [CustomDisplay(DisplayOn.NONE)]
        public int RunnerId { get; set; }

        //[CustomDisplay(DisplayOn.NONE)]
        //public DateTime CurrentTime { get; set; }

        //[Display(Name = "Time", Order = 12)]
        //[CustomDisplay(DisplayOn.BOTH, checkbox: false)]
        //public string _CurrentTime { get { return CurrentTime.ToString("mm:ss.fff"); } }

        [CustomDisplay(DisplayOn.NONE)]
        public int RaceId { get; set; }

        [Display(Name = "Tab No.", Order = 1)]
        [CustomDisplay(DisplayOn.ALL, checkbox: false)]
        public int HorseNumber { get; set; }

        [Display(Order = 7)]
        [CustomDisplay(DisplayOn.ALL, CustomFormatters.winOddsFormatter, checkbox:false)]
        public decimal WinOdds { get; set; }

        [Display(Order = 8)]
        [CustomDisplay(DisplayOn.ALL, CustomFormatters.placeOddsFormatter, checkbox:false)]
        public decimal PlaceOdds { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public bool isWinFavorite { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public bool WinDropby20 { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public bool WinDropby50 { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public bool isPlaceFavorite { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public bool PlaceDropby20 { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public bool PlaceDropby50 { get; set; }

        [Display(Order = 4)]
        [CustomDisplay(DisplayOn.ALL, CustomFormatters.currencyFormatter, checkbox:false)]
        public int ODDSLAST1 { get; set; }  

        [Display(Order = 5)]
        [CustomDisplay(DisplayOn.ALL, CustomFormatters.currencyFormatter, checkbox: false)]
        public int ODDSLAST2 { get; set; }  

        [Display(Order = 6)]
        [CustomDisplay(DisplayOn.ALL, CustomFormatters.currencyFormatter, checkbox: false)]
        public int ODDSLAST3 { get; set; }

        [Display(Order = 9)]
        [CustomDisplay(DisplayOn.ALL, checkbox: false)]
        public int Z_WinOddsRank { get; set; }

        [Display(Order = 10)]
        [CustomDisplay(DisplayOn.ALL, checkbox: false)]
        public int AVG3WinOddsRank { get; set; } 
    }

    /// <summary>
    /// Static fields
    /// </summary>
    public class Runner: RunnerDyn
    {
        [CustomDisplay(DisplayOn.NONE)]
        public int HorseId { get; set; }

        [Display(Name = "BP", Order = 3)]
        [CustomDisplay(DisplayOn.ALL, checkbox: false)]
        public int Barrier { get; set; }
        
        //[Display(Name = "Horse", Order = 2)]
        //[CustomDisplay(DisplayOn.AUS, CustomFormatters.linkFormatter, checkbox: false, colSize: ConfigValues.NameColumnWidth)]
        //public string Name { get; set; }

        [Display(Name = "Horse", Order = 2)]
        [CustomDisplay(DisplayOn.HK, CustomFormatters.linkFormatter, checkbox: false, colSize: ConfigValues.NameColumnWidth)]
        public string Horse { get; set; } //HK_HorseFullName

        [Display(Name = "Jockey", Order = 99)]
        [CustomDisplay(DisplayOn.ALL, colSize: ConfigValues.NameColumnWidth)]
        public string Jockey { get; set; }

        [Display(Name = "JockeyPoints", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public int jockeypoints { get; set; }

        [Display(Name = "Trainer", Order = 99)]
        [CustomDisplay(DisplayOn.ALL, colSize: ConfigValues.NameColumnWidth)]
        public string Trainer { get; set; }

        //[Display(Name = "Gear", Order = 99)]
        //[CustomDisplay(DisplayOn.ALL)]
        //public string Gear { get; set; }

        //[Display(Name = "NewTr?", Order = 99)]
        //[CustomDisplay(DisplayOn.BOTH)]
        //public string Z_newTrainerSinceLastStart { get; set; }

        //[CustomDisplay(DisplayOn.BOTH, CustomFormatters.percentageFormatter)]
        //[Display(Name = "Barrier Advantage", Order = 0)]
        //public int Z_BPAdvFactor { get; set; }

        [CustomDisplay(DisplayOn.ALL)]
        [Display(Name = "Age", Order = 99)]
        public string Age { get; set; }

        [Display(Name = "Sex", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string Sex { get; set; }

        [Display(Name = "Color", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string Color { get; set; }

        //[Display(Name = "Weight", Order = 99)]
        //[CustomDisplay(DisplayOn.AUS)]
        //public float AUS_HcpWT { get; set; }

        //[Display(Name = "Horse Wt.", Order = 0)]
        //[CustomDisplay(DisplayOn.HK)]
        //public float HK_DeclaredHorseWtLbs { get; set; }

        //[Display(Name = "HcpRtg", Order = 99)]
        //[CustomDisplay(DisplayOn.AUS)]
        //public int AUS_HcpRatingAtJump { get; set; }

        //[Display(Name = "Wt.", Order = 99)]
        //[CustomDisplay(DisplayOn.HK)]
        //public float HK_ActualWtLbs { get; set; }

        //[Display(Name = "Rating", Order = 99)]
        //[CustomDisplay(DisplayOn.HK)]
        //public int HK_Rating { get; set; }

        [Display(Name = "Plc.", Order = 11)]
        [CustomDisplay(DisplayOn.ALL, checkbox:false)]
        public string Place { get; set; }

        //[Display(Name = "SP Win", Order = 0)]
        //[CustomDisplay(DisplayOn.AUS, CustomFormatters.currencyFormatter)] 
        //public int AUS_SPW { get; set; }

        //[Display(Name = "SP Place", Order = 0)]
        //[CustomDisplay(DisplayOn.AUS, CustomFormatters.currencyFormatter)]
        //public int AUS_SPP { get; set; }

        //[Display(Name = "Win Odds", Order = 0)]
        //[CustomDisplay(DisplayOn.HK, CustomFormatters.currencyFormatter)]
        //public int HK_WinOdds { get; set; }

        [Display(Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public int nUp { get; set; } 
        
        [Display(Name = "Class+/-", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string Class { get; set; } 

        [Display(Order = 99)]
        [CustomDisplay(DisplayOn.HK)]
        public int Rtg { get; set; } 
    
        [Display(Name = "Gld?", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string Gld { get; set; } 
    
        [Display(Order = 99)]
        [CustomDisplay(DisplayOn.HK)]
        public float CWt { get; set; } 
    
        [Display(Name = "Cwt%BW", Order = 99)]
        [CustomDisplay(DisplayOn.HK, CustomFormatters.percentageFormatter)]
        public float BW { get; set; }

        [Display(Name = "Wt", Order = 99)]
        [CustomDisplay(DisplayOn.AUSnRSA)]
        public float Wt { get; set; } 

        [Display(Name = "Wt+/-", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public float WtPlusLess { get; set; } 
    
        [Display(Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public int DSLR { get; set; } 

        [Display(Name = "BFAVL?", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string BFAVL { get; set; } 

        [Display(Name = "Mdn?", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string Mdn { get; set; } 

        [Display(Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public float MktRel { get; set; } 
        
        [Display(Order = 99)]
        [CustomDisplay(DisplayOn.HK)]
        public int JmpRnk { get; set; } 
        
        [Display(Order = 99)]
        [CustomDisplay(DisplayOn.HK)]
        public int FinishRnk { get; set; } 
            
        [Display(Name = "NewTr?", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string NewTr { get; set; } 
    
        [Display(Name = "LSW?", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string LSW { get; set; }

        [Display(Name = "FirstSt?", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string FirstSt { get; set; }

        [Display(Name = "KAD?", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string KAD { get; set; }

        [Display(Name = "RanOnL?", Order = 99)]
        [CustomDisplay(DisplayOn.HK)]
        public string RanOnL { get; set; }

        [Display(Name = "LostLeadL?", Order = 99)]
        [CustomDisplay(DisplayOn.HK)]
        public string LostLeadL { get; set; }

        [Display(Name = "ROLast?", Order = 99)]
        [CustomDisplay(DisplayOn.AUSnRSA)] 
        public string ROLast { get; set; }

        [Display(Name = "SwampedLast?", Order = 99)]
        [CustomDisplay(DisplayOn.AUSnRSA)] 
        public string SwampedLast { get; set; }
        
        [Display(Name = "FUP?", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string FUP { get; set; } 
    
        [Display(Name = "LUP?", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string LUP { get; set; } 

        [Display(Name = "QBU?", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string QBU { get; set; } 

        [Display(Name = "GJD?", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string GJD { get; set; } 

        [Display(Name = "DRPD?", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string DRPD { get; set; } 

        [Display(Name = "H4CRSE?", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string H4CRSE { get; set; } 

        [Display(Name = "H&J?", Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string HJ { get; set; } 

        [Display(Name = "1TRICKJ?", Order = 99)]
        [CustomDisplay(DisplayOn.HK)]
        public string TRICKJ { get; set; } 

        [Display(Name = "NewGear?", Order = 99)]
        [CustomDisplay(DisplayOn.HK)]
        public string NewGear { get; set; } 

        [Display(Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public int BeenThere { get; set; } 

        [Display(Order = 99)]
        [CustomDisplay(DisplayOn.RSA)]
        public float SandPts { get; set; }

        [Display(Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public float TurfPts { get; set; } 

        [Display(Order = 99)]
        [CustomDisplay(DisplayOn.RSA)]
        public float PolyPts { get; set; }        

        [Display(Order = 99)]
        [CustomDisplay(DisplayOn.AUSnHK)]
        public float AWTPts { get; set; } 

        [Display(Order = 99)]
        [CustomDisplay(DisplayOn.ALL)]
        public string LAST10 { get; set; } 

        //[CustomDisplay(DisplayOn.NONE)]
        //public DateTime Z_AUS_FinishTime { get; set; }

        //[Display(Name = "Time", Order = 0)]
        //[CustomDisplay(DisplayOn.AUS)]
        //public string Formatted_Z_AUS_FinishTime { get { return Z_AUS_FinishTime.ToString("mm:ss.ff"); } }

        //[CustomDisplay(DisplayOn.NONE)]
        //public DateTime HK_FinishTime { get; set; }

        //[Display(Name = "Time", Order = 0)]
        //[CustomDisplay(DisplayOn.HK)]
        //public string Formatted_HK_FinishTime { get { return HK_FinishTime.ToString("mm:ss.ff"); } }

        //[Display(Name = "Jockey Challenge", Order = 0)]
        //[CustomDisplay(DisplayOn.BOTH)]
        //public int JockeyPoints { get; set; }

        //[CustomDisplay(DisplayOn.NONE)]
        //public float STATISTICS1 { get; set; }

        //[CustomDisplay(DisplayOn.NONE)]
        //public float STATISTICS2 { get; set; }

        //[CustomDisplay(DisplayOn.NONE)]
        //public float STATISTICS3 { get; set; }

        //[CustomDisplay(DisplayOn.NONE)]
        //public float STATISTICS4 { get; set; }

        //[CustomDisplay(DisplayOn.NONE)]
        //public static IList<Runner> DummyRunnerList 
        //{
        //    get 
        //    {
        //        return FizzWare.NBuilder.Builder<Runner>.CreateListOfSize(10).Build();
        //    }
        //}

        internal void Update(RunnerDyn runner)
        {
            this.isScratched = runner.isScratched;
            this.RunnerId = runner.RunnerId;
            //this.CurrentTime = runner.CurrentTime;
            this.RaceId = runner.RaceId;
            this.HorseNumber = runner.HorseNumber;
            //this.HorseId = runner.HorseId;

            this.WinOdds = runner.WinOdds;
            this.PlaceOdds = runner.PlaceOdds;
            this.isWinFavorite = runner.isWinFavorite;
            this.WinDropby20 = runner.WinDropby20;
            this.WinDropby50 = runner.WinDropby50;
            this.isPlaceFavorite = runner.isPlaceFavorite;
            this.PlaceDropby20 = runner.PlaceDropby20;
            this.PlaceDropby50 = runner.PlaceDropby50;
            
            this.ODDSLAST1 = runner.ODDSLAST1;
            this.ODDSLAST2 = runner.ODDSLAST2;
            this.ODDSLAST3 = runner.ODDSLAST3;

            this.Z_WinOddsRank = runner.Z_WinOddsRank;
            this.AVG3WinOddsRank = runner.AVG3WinOddsRank;
        }
    }

}
