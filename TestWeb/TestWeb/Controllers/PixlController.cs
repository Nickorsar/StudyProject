using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;
using RestSharp;
using RestSharp.Authenticators;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWeb.Models;

namespace TestWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PixlController : ControllerBase
    {
        private static string REFRESHTOKEN;
        private static string ACCESSTOKEN;
        private const string BASE_URL = "http://api.pixlpark.com/";
        private const string PUBLIC_KEY = "38cd79b5f2b2486d86f562e3c43034f8";
        private const string PRIVATE_KEY = "8e49ff607b1f46e1a5e8f6ad5d312a80";
        
        [Route("api/oauth/requesttoken")]
        private string GetRequestToken()
        {
            RestClient client = new RestClient(BASE_URL+"/oauth/requesttoken");
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            var response = client.Get(request);
            if (response.IsSuccessful)
            {
                string stream = response.Content;
                dynamic d = JsonConvert.DeserializeObject(stream);
                return d.RequestToken;
            }
            return "";
        }
        [Route("api/oauth/accesstoken")]
        private string GetAccessToken()
        {

            var requestToken = GetRequestToken();
            if (requestToken.Length == 0) return "";
            RestClient client = new RestClient(BASE_URL+"/oauth/accesstoken");
            
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");    
            request.AddParameter("oauth_token",requestToken );
            request.AddParameter("grant_type", "api");
            request.AddParameter("username", PUBLIC_KEY);
            request.AddParameter("password", "5f73b6e9098df19a060476feab51f65c1b8ba1f5");

            var response = client.Get(request);
            if (response.IsSuccessful)
            {
                string stream = response.Content;
                dynamic d = JsonConvert.DeserializeObject(stream);
                return d.AccessToken;
            }
            return "";
        }
        [Route("api/orders")]
        public List<Order> GetOrders()
        {
            var oauth = GetAccessToken();
            if (oauth.Length == 0) return null;
            List<Order> orders = new();
            RestClient client = new RestClient(BASE_URL + "orders");
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            request.AddParameter("oauth_token", oauth);

            IRestResponse<List<Order>> response = client.Get<List<Order>>(request);
            if (response.IsSuccessful) return (List<Order>)response;
            return null;
        }
    }
}
