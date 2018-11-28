using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Produces("application/json")]
    [Route("api/Search")]
    public class SearchController : Controller
    {
        //DI our api settings into our controller
        private readonly APISettings _apiSettings;
        public SearchController(IOptions<APISettings> apiSettings)
        {
            _apiSettings = apiSettings.Value;
        }

        [HttpGet]
        public SearchResult Search()
        {
            //create an HTTP Client for the web query
            HttpClient YelpClient = new HttpClient();

            //set the authorization headers
            YelpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", _apiSettings.YelpAPIKey);

            //create our query string
            var uri = new UriBuilder(_apiSettings.YelpBaseUrl + "/businesses/search");
            var queryString = System.Web.HttpUtility.ParseQueryString(String.Empty);

            //Create the name value pairs
            queryString["location"] = "87102";
            queryString["term"] = "Gluten free";
            queryString["limit"] = "5";

            uri.Query = queryString.ToString();

            //make the api request and deal with the response
            var request = YelpClient.GetAsync(uri.ToString());
            var response = request.Result;
            if(response.IsSuccessStatusCode)
            {
                var responseString = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<SearchResult>(responseString);
            }

            return new SearchResult() { total = -1 };
        }
    }
}