using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Newtonsoft;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

using TestWeb.Models;

namespace TestWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        //private static string REFRESHTOKEN;
        //private static string ACCESSTOKEN;
        private const string BASE_URL = "http://api.pixlpark.com/";
        private const string PUBLIC_KEY = "38cd79b5f2b2486d86f562e3c43034f8";
        private const string PRIVATE_KEY = "8e49ff607b1f46e1a5e8f6ad5d312a80";

        private string GetRequestToken()
        {
            RestClient client = new RestClient(BASE_URL + "/oauth/requesttoken");
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            var response = client.Get(request);
            Debug.WriteLine(response.Content);
            if (response.IsSuccessful)
            {
                string stream = response.Content;
                dynamic d = JsonConvert.DeserializeObject(stream);
                return d.RequestToken;
            }
            return "";
        }
        private string GetAccessToken()
        {

            var requestToken = GetRequestToken();

            
            var bytes = Encoding.UTF8.GetBytes(requestToken+PRIVATE_KEY);
            var sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach(var b in hash)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            var password = sb.ToString();
            Debug.WriteLine(requestToken);
            Debug.WriteLine(password);
            if (requestToken.Length == 0) return "";
            RestClient client = new RestClient(BASE_URL + "/oauth/accesstoken");

            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddParameter("oauth_token", requestToken);
            request.AddParameter("grant_type", "api");
            request.AddParameter("username", PUBLIC_KEY);
            request.AddParameter("password", password);

            var response = client.Get(request);
            Debug.WriteLine(response.Content);
            if (response.IsSuccessful)
            {
                string stream = response.Content;
                dynamic d = JsonConvert.DeserializeObject(stream);
                return d.AccessToken;
            }
            return "";
        }
        public IActionResult Index()
        {
            var oauth = GetAccessToken();
            if (oauth == null) return View();
            Debug.WriteLine(oauth);
            
            RestClient client = new RestClient(BASE_URL + "orders");
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddParameter("oauth_token", oauth);

            //IRestResponse<List<Order>> response = client.Get<List<Order>>(request);
            var response = client.Get(request);
            
            ResponseOrder result = JsonConvert.DeserializeObject<ResponseOrder>(response.Content);
            
            Debug.WriteLine(response.Content);
           // Order[] orders = result.Result;
           foreach(var order in result.Result)
            {
                Debug.WriteLine($"{order.Id} {order.Price}");
            }
            //Debug.WriteLine(Date(1630393560000));
            if (response.IsSuccessful) return View(result);

            return View();
        }

        
        
    }
}
