using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace TacTicApi
{
    public class CitiesMemoryCache
    {
        public MemoryCache Cache { get; set; }

        public CitiesMemoryCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                //SizeLimit = 1024
            });

            // Read file line by line

            //City cacheEntry;
            //if (!_cache.TryGetValue(cityName+":"+countryName, out cacheEntry))
            //{
            // cacheEntry = new City { };
            // _cache.Set(cityName+":"+countryName, obj, cacheEntryOptions);
            //}
        }
    }
}
