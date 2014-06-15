using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceDayDisplayApp.Models
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }
        public string Name { get; set; }
        public IList<MeetingBase> Meetings { get; set; }

        static public CountryEnum GetEnum(string code)
        {
            switch (code)
            {
                case "AUS":
                    return CountryEnum.AUS;
                case "HKG":
                    return CountryEnum.HK;
                case "RSA":
                    return CountryEnum.RSA;
                default:
                    return CountryEnum.OTHER;
            }
        }

        static public bool Match(CountryEnum country, DisplayOn display)
        {
            switch (display)
            {
                case DisplayOn.NONE:
                    return false;
                case DisplayOn.AUS:
                    return country == CountryEnum.AUS;
                case DisplayOn.HK:
                    return country == CountryEnum.HK;
                case DisplayOn.RSA:
                    return country == CountryEnum.RSA;
                case DisplayOn.AUSnRSA:
                    return country == CountryEnum.RSA || country == CountryEnum.AUS;
                case DisplayOn.AUSnHK:
                    return country == CountryEnum.HK || country == CountryEnum.AUS;
                case DisplayOn.ALL:
                    return true;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public enum CountryEnum
    {
        AUS,
        HK,
        RSA,
        OTHER
    }

}