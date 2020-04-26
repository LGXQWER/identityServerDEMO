using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MVC_ClientDemo.Models;

namespace MVC_ClientDemo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task< IActionResult> Index()
        {
            //从service服务器地址获取信息
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }
            //向IdentityServer请求令牌以访问api1，请求token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "mvc client",
                ClientSecret = "mvc secret",
                Scope = "api1"
            });
            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }
            //访问资源
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("http://localhost:5001/WeatherForecast");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ReasonPhrase);
            }           
            var content = await response.Content.ReadAsStringAsync();

            return View("Index", content);
        }

        public async Task< IActionResult> Privacy()
        {
            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var idToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            var refreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            ViewData["accessToken"] = accessToken;
            ViewData["idToken"] = idToken;
            ViewData["refreshToken"] = refreshToken;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
