using System;

namespace TacTicA.Api.Models
{
    public class City
    {
        public string Woeid { get; set; }
        public string Country { get; set; }
        public string CityName { get; set; }
        public string Long { get; set; }
        public string Lat { get; set; }
        public decimal GmtOffset { get; set; }
    }
}