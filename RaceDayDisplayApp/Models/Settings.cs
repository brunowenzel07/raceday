using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace RaceDayDisplayApp.Models
{    
    /// <summary>
    /// Each of these boolean properties will be rendered as checkboxes in the view.
    /// In order to be properly linked to the columns of the grid, their names should match the names of the Runner class properties
    /// </summary>
    public class UserSettings
    {
        [CustomDisplay(DisplayOn.NONE)]
        public int Id { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public string Name { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public string Z_TableType { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public int UserId { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public int CountryId { get; set; }

        public bool Barrier { get; set; }

        [Display(Name = "Horse", Order = 0)]
        [LinkedTo("Name")]
        public bool HorseName { get; set; }

        [Display(Name = "Jockey", Order = 1)]
        [LinkedTo("Jockey")]
        public bool JockeyName { get; set; }

        [Display(Name = "Trainer", Order = 2)]
        [LinkedTo("Trainer")]
        public bool TrainerName { get; set; }

        [Display(Name = "Weight", Order = 3)]
        [LinkedTo("AUS_HcpWT")]
        public bool AUSHcpWT { get; set; }

        [Display(Name = "HcpRtg", Order = 4)]
        [LinkedTo("AUS_HcpRatingAtJump")]
        public bool HcpRatingAtJump { get; set; }

        [Display(Name = "Carried Wt.", Order = 5)]
        [CustomDisplay(DisplayOn.BOTH)]
        public bool CarriedWt { get; set; }

        //[Display(Name = "SP Win", Order = 5)]
        //[CustomDisplay(DisplayOn.AUS)]
        //[LinkedTo("AUS_SPW")]
        //public bool Aus_SPW { get; set; }

        //[Display(Name = "SP Place", Order = 6)]
        //[CustomDisplay(DisplayOn.AUS)]
        //[LinkedTo("AUS_SPP")]
        //public bool AUS_SPP { get; set; }

        [Display(Name = "Gear", Order = 7)]
        [CustomDisplay(DisplayOn.AUS)]
        public bool Gear { get; set; }

        //[Display(Name = "Time", Order = 8)]
        //[CustomDisplay(DisplayOn.AUS)]
        //[LinkedTo("Formatted_Z_AUS_FinishTime")]
        //public bool Z_AUS_FinishTime { get; set; }

        [Display(Name = "Age", Order = 11)]
        [LinkedTo("Age")]
        public bool HorseAge { get; set; }

        [Display(Name = "Sex", Order = 12)]
        [LinkedTo("Sex")]
        public bool HorseSex { get; set; }

        [Display(Name = "Color", Order = 13)]
        [LinkedTo("Color")]
        public bool HorseColor { get; set; }

        //the statistical fields are displayed depending on what is in the RaceStatistics table

        //[CustomDisplay(DisplayOn.NONE)]
        //public bool Statistical1 { get; set; }

        //[CustomDisplay(DisplayOn.NONE)]
        //public bool Statistical2 { get; set; }

        //[CustomDisplay(DisplayOn.NONE)]
        //public bool Statistical3 { get; set; }

        //[CustomDisplay(DisplayOn.NONE)]
        //public bool Statistical4 { get; set; }

        [Display(Order = 19)]
        [LinkedTo("ODDSLAST1")]
        public bool OddsLast1 { get; set; }
        
        [Display(Order = 20)]
        [LinkedTo("ODDSLAST2")]
        public bool OddsLast2 { get; set; }

        [Display(Order = 21)]
        [LinkedTo("ODDSLAST3")]
        public bool OddsLast3 { get; set; }

        [Display(Name = "Wt.", Order = 23)]
        [CustomDisplay(DisplayOn.HK)]
        public bool HK_ActualWtLbs { get; set; }

        [Display(Name = "Horse Wt.", Order = 24)]
        [CustomDisplay(DisplayOn.HK)]
        public bool HK_DeclaredHorseWtLbs { get; set; }

        [Display(Name = "Rating", Order = 25)]
        [CustomDisplay(DisplayOn.HK)]
        public bool HK_Rating { get; set; }

        //[Display(Name = "Current odds", Order = 26)]
        //[CustomDisplay(DisplayOn.HK)]
        //public bool HK_WinOdds { get; set; }

        [Display(Name = "Time", Order = 27)]
        [CustomDisplay(DisplayOn.HK)]
        [LinkedTo("Formatted_HK_FinishTime")]
        public bool HK_FinishTime { get; set; }

        [Display(Order = 27)]
        [CustomDisplay(DisplayOn.BOTH)]
        [LinkedTo("LSW")]
        public bool isLSW { get; set; }

        [Display(Order = 27)]
        [CustomDisplay(DisplayOn.BOTH)]
        [LinkedTo("Wt")]
        public bool WtDiffLast { get; set; }
    
        [Display(Order = 27)]
        [CustomDisplay(DisplayOn.BOTH)]
        [LinkedTo("Class")]
        public bool ClassDiffLast { get; set; }
    
        //[Display(Order = 27)]
        //[CustomDisplay(DisplayOn.BOTH)]
        //[LinkedTo("")]
        //public bool isranOnLast { get; set; }

        //[Display(Order = 27)]
        //[CustomDisplay(DisplayOn.BOTH)]
        //[LinkedTo("LostLeadlast")]
        //public bool lostLeadAtLast { get; set; }                        

        [Display(Order = 27)]
        [CustomDisplay(DisplayOn.BOTH)]
        [LinkedTo("Rtg")]
        public bool HK_RtgDiffLast { get; set; }                        
    
        //[Display(Order = 27)]
        //[CustomDisplay(DisplayOn.BOTH)]
        //[LinkedTo("")]
        //public bool isBackFromSpell { get; set; }                       
    
        //[Display(Order = 27)]
        //[CustomDisplay(DisplayOn.BOTH)]
        //[LinkedTo("")]
        //public bool carriedWtOfHorseWt { get; set; }                    

        [Display(Order = 27)]
        [CustomDisplay(DisplayOn.BOTH)]
        [LinkedTo("KAD")]
        public bool isKAD { get; set; }

        //[Display(Order = 27)]
        //[CustomDisplay(DisplayOn.BOTH)]
        //[LinkedTo("CD_Winner")]
        //public bool isCDWinner { get; set; }                            

        [Display(Order = 27)]
        [CustomDisplay(DisplayOn.BOTH)]
        [LinkedTo("FirstSt")]
        public bool isFirstStarter { get; set; }                        
    
        [Display(Order = 27)]
        [CustomDisplay(DisplayOn.BOTH)]
        public bool nUp { get; set; }                                   
    
        [Display(Order = 27)]
        [CustomDisplay(DisplayOn.BOTH)]
        [LinkedTo("Gld")]
        public bool isGeldedSinceLast { get; set; }                     

        //[Display(Name = "New trainer", Order = 9)]
        //[LinkedTo("NewTr")]
        //public bool Z_NewTrainerSinceLastStart { get; set; }

        [CustomDisplay(DisplayOn.NONE)]
        public static UserSettings DEFAULT { 
            get 
            {
                return new UserSettings { HorseName = true }; 
            } 
        }

    }

    public class ViewUserSetting
    {
        public string PropertyName;
        public string DisplayName;
        public bool Checked;
    }

}
