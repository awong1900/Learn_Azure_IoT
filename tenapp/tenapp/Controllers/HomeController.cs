using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Azure.Devices.Client;
using tenapp.Models;
using tenapp.Utilities;
using System.Threading.Tasks;

namespace tenapp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            // return null;
            return View("View");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult LabData()
        {
            ViewBag.Message = "Your view page.";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LabData(LabDataModels labDataModel)
        {
            ViewBag.Message = "Your view page.";
            AzureDataStorage azureDataStorage = new AzureDataStorage();
            // azureDataStorage.SendDataToBlob(labDataModel);
            // azureDataStorage.SendDataToEventHub(labDataModel);
            // azureDataStorage.SendDataToServiceBusQueue(labDataModel);
            await azureDataStorage.SendDataToIotHubAsync(labDataModel);
            return View();
        }

        [HttpGet]
        public ActionResult Send(string deviceId, string temp)
        {
            System.Diagnostics.Debug.WriteLine("Have log");
            return View("Index");
        }

    }

}