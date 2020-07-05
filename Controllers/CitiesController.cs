using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Http;
using TacTicA.Api.Models;
using Newtonsoft.Json;

namespace TacTicApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private static readonly HttpClient HttpClient = new HttpClient();
        private IMemoryCache _cache;
        private readonly ILogger<CitiesController> _logger;

        public CitiesController(ILogger<CitiesController> logger, IMemoryCache memoryCache, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _cache = memoryCache;
            _clientFactory = clientFactory;
        }

        // Request: https://localhost:5001/Cities/GetCityInfo/Taganrog
        // Response:
        // {"woeid":"484907","country":"RU","cityName":"Taganrog","long":"38.89688","lat":"47.23617","gmtOffset":3}
        [HttpGet("{action}/{cityName}")]
        public async Task<IActionResult> GetCityInfo(string cityName)
        {
            City cityFound = null;
            var client = _clientFactory.CreateClient();
            using(var response = await client.GetAsync("http://api.geonames.org/searchJSON?username=optiklab&maxRows=1&q=" + cityName))
            {
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int) response.StatusCode);

                var responseContent = await response.Content.ReadAsStringAsync();
                var deserializedSearchResults = JsonConvert.DeserializeObject<GeoNamesSearchResult>(responseContent);

                // TODO Rework. As simple as possible.
                if (deserializedSearchResults != null && deserializedSearchResults.TotalResultsCount > 0 && deserializedSearchResults.GeoNames != null)
                {
                    var geoName = deserializedSearchResults.GeoNames.FirstOrDefault();

                    if (geoName != null)
                    {
                        using(var tzResponse = await client.GetAsync("http://api.geonames.org/timezoneJSON?username=optiklab&lng=" + geoName.lng + "&lat=" + geoName.lat))
                        {
                            if (!tzResponse.IsSuccessStatusCode)
                                return StatusCode((int) tzResponse.StatusCode);

                            responseContent = await tzResponse.Content.ReadAsStringAsync();
                            var deserializedTz = JsonConvert.DeserializeObject<Timezone>(responseContent);

                            // TODO Rework. As simple as possible.
                            if (deserializedTz != null)
                            {
                                cityFound = new City
                                {
                                    Woeid = geoName.geonameId,
                                    Country = geoName.countryCode,
                                    CityName = geoName.name,
                                    Lat = geoName.lat,
                                    Long = geoName.lng,
                                    GmtOffset = deserializedTz.gmtOffset
                                };
                            }
                        }
                    }
                }
            }

            return Ok(cityFound);
        }

        // Request: https://localhost:5001/Cities/GetCityByIp/178_76_221_84
        // Response: {"woeid":null,"country":"RU","cityName":"Taganrog","long":"38.8969","lat":"47.2362","gmtOffset":0}
        [HttpGet("{action}/{ipAddress}")]
        public async Task<IActionResult> GetCityByIp(string ipAddress)
        {
            City cityFound = null;
            var client = _clientFactory.CreateClient();
            ipAddress = ipAddress.Replace("_", ".");
            using(var ipInfoResponse = await client.GetAsync("https://api.ipinfodb.com/v3/ip-city/?key=debfc7c448e8b9d818084949fa23db2382f2488fbfd52e805a3e059091c65d8b&ip=" + ipAddress + "&format=json"))
            {
                if (!ipInfoResponse.IsSuccessStatusCode)
                    return StatusCode((int) ipInfoResponse.StatusCode);

                var responseContent = await ipInfoResponse.Content.ReadAsStringAsync();
                var deserializedIpInfoResponse = JsonConvert.DeserializeObject<IpInfoResponse>(responseContent);

                // TODO Rework. As simple as possible.
                if (deserializedIpInfoResponse != null)
                {
                    cityFound = new City
                    {
                        //Woeid = geoName.geonameId,
                        Country = deserializedIpInfoResponse.countryCode,
                        CityName = deserializedIpInfoResponse.cityName,
                        Lat = deserializedIpInfoResponse.latitude,
                        Long = deserializedIpInfoResponse.longitude,
                        //GmtOffset = deserializedTz.gmtOffset
                    };
                }
            }

            return Ok(cityFound);
        }

        [HttpGet("{action}/{letters}")]
        public async Task<IActionResult> GetCitySuggested(string letters)
        {
            List<City> cities = new List<City>();

            var client = _clientFactory.CreateClient();
            using(var response = await client.GetAsync("http://api.geonames.org/searchJSON?username=optiklab&maxRows=10&q=" + letters))
            {
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int) response.StatusCode);

                var responseContent = await response.Content.ReadAsStringAsync();
                var deserializedSearchResults = JsonConvert.DeserializeObject<GeoNamesSearchResult>(responseContent);

                // TODO Rework. As simple as possible.
                if (deserializedSearchResults != null && deserializedSearchResults.TotalResultsCount > 0 && deserializedSearchResults.GeoNames != null)
                {
                    foreach (var geoName in deserializedSearchResults.GeoNames)
                    {
                        if (geoName != null)
                        {
                            City cityFound = new City
                            {
                                Woeid = geoName.geonameId,
                                Country = geoName.countryCode,
                                CityName = geoName.name,
                                Lat = geoName.lat,
                                Long = geoName.lng,
                            };

                            using(var tzResponse = await client.GetAsync("http://api.geonames.org/timezoneJSON?username=optiklab&lng=" + geoName.lng + "&lat=" + geoName.lat))
                            {
                                if (!tzResponse.IsSuccessStatusCode)
                                    return StatusCode((int) tzResponse.StatusCode);

                                responseContent = await tzResponse.Content.ReadAsStringAsync();
                                var deserializedTz = JsonConvert.DeserializeObject<Timezone>(responseContent);

                                // TODO Rework. As simple as possible.
                                if (deserializedTz != null)
                                {
                                    cityFound.GmtOffset = deserializedTz.gmtOffset;
                                }
                            }

                            cities.Add(cityFound);
                        }
                    }
                }
            }

            return Ok(cities);
        }
    }
}
