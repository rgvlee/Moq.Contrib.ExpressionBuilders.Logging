using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using rgvlee.Study.NLog.Models;

namespace rgvlee.Study.NLog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _logger.LogDebug(1, "NLog injected into HomeController");
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Hello, this is the index!");
            
            try
            {
                _logger.LogInformation("Placeholder with no value {key1}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Placeholder with no value failed", ex);
            }

            try
            {
                _logger.LogInformation("More placeholders than values {key1}, {key2}", "value1");
            }
            catch (Exception ex)
            {
                _logger.LogError("More placeholders than values failed", ex);
            }

            try
            {
                _logger.LogInformation("Null params {key1}", null);
            }
            catch (Exception ex)
            {
                _logger.LogError("Null params failed", ex);
            }

            try
            {
                _logger.LogInformation("One null value {key1}, {key2}", null, "value2");
            }
            catch (Exception ex)
            {
                _logger.LogError("One null value failed", ex);
            }

            try
            {
                _logger.LogInformation("Two null values {key1}, {key2}", null, null);
            }
            catch (Exception ex)
            {
                _logger.LogError("Two null values failed", ex);
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}