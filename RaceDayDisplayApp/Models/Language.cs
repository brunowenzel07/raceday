using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceDayDisplayApp.Models
{
    public class Language
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }

        public string EnName { get; set; }

        public string LocalName { get; set; }

        public string InUse { get; set; }
    }
}