using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using rgvlee.Study.Serilog.Models;

namespace rgvlee.Study.Serilog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _logger.LogDebug(1, "Logger injected into HomeController");
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
                _logger.LogInformation("Null string {key1}", (string) null);
            }
            catch (Exception ex)
            {
                _logger.LogError("Null string failed", ex);
            }

            try
            {
                _logger.LogInformation("Params with null item {key1}", new object[] { null });
            }
            catch (Exception ex)
            {
                _logger.LogError("Params with null item failed", ex);
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