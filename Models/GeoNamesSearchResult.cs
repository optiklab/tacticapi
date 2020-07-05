using System;
using System.Collections.Generic;

namespace TacTicA.Api.Models
{
    public class GeoNamesSearchResult
    {
        public List<GeoName> GeoNames { get; set; }
        public int TotalResultsCount { get; set; }
    }
}