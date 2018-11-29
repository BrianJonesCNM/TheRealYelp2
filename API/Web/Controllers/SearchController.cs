using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;

namespace Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly APISettings _apiSettings;
        public SearchController(IOptions<APISettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }
        [HttpGet]
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public SearchResult Search(string location)
        {
            //create an HTTP Client for the web query
            HttpClient YelpClient = new HttpClient();
            //ViewData.Model.ToString();
            //create our query string
            var uri = new UriBuilder(_apiSettings.APIBaseUrl + "/api/search");
            var queryString = System.Web.HttpUtility.ParseQueryString(String.Empty);

            ////Create the name value pairs
            if (!String.IsNullOrEmpty(location))
            {
                queryString["location"] = location;
            }

            //queryString["term"] = "Gluten free";
            //queryString["limit"] = "5";

            if (queryString.Count > 0)
            {
                uri.Query = queryString.ToString();
            }
            //make the api request and deal with the response
            var request = YelpClient.GetAsync(uri.ToString());
            var response = request.Result;
            if (response.IsSuccessStatusCode)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<SearchResult>(responseString);
            }

            return new SearchResult() { total = -1 };
        }
    }
}