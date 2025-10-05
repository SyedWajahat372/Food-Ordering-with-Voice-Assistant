using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Voice_API_OpenAI.Controllers
{
    public class HomeController : Controller
    {
        private static List<JObject> menuItemsData = null;
        private static readonly object lockObj = new object();

        // Load menu.json once
        private void InitializeMenu()
        {
            if (menuItemsData != null) return;

            lock (lockObj)
            {
                if (menuItemsData == null)
                {
                    string path = Server.MapPath("~/App_Data/menu.json");
                    string jsonData = System.IO.File.ReadAllText(path);
                    JObject menu = JObject.Parse(jsonData);

                    menuItemsData = new List<JObject>();

                    foreach (var itemName in menu["menu_items"])
                    {
                        string name = itemName.ToString().Trim();
                        var priceToken = menu["prices"]?[name];
                        var imageToken = menu["images"]?[name];
                        var descToken = menu["descriptions"]?[name];

                        JObject obj = new JObject
                        {
                            ["name"] = name,
                            ["price"] = priceToken != null ? priceToken.Value<decimal>() : 0,
                            ["description"] = descToken != null ? descToken.Value<string>() : "No description",
                            ["image"] = imageToken != null ? imageToken.Value<string>() : "/Content/Menu/default.png"
                        };

                        menuItemsData.Add(obj);
                    }
                }
            }
        }

        public ActionResult Voice() => View();

        [HttpPost]
        public async Task<ActionResult> Session()
        {
            InitializeMenu();

            string apiKey = System.Web.Configuration.WebConfigurationManager.AppSettings["OpenAI_API_Key"];

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

                string menuJson = Newtonsoft.Json.JsonConvert.SerializeObject(menuItemsData);

                string systemPrompt =
                    "You are a friendly restaurant assistant. " +
                    "You must ONLY use the following menu JSON as the single source of truth for prices, descriptions, and images. " +
                    "Never guess or make up information. If an item is not in the JSON, say it's unavailable. " +
                    "When asked for an item, respond with its exact 'name', 'price', 'description', and 'image' from the JSON. " +
                    "If asked for recommendations, pick a random item from this JSON. " +
                    "Here is the menu JSON: " + menuJson;

                var body = new JObject
                {
                    ["model"] = "gpt-4o-mini-realtime-preview",
                    ["voice"] = "alloy",
                    ["instructions"] = systemPrompt
                };

                var resp = await client.PostAsync(
                    "https://api.openai.com/v1/realtime/sessions",
                    new StringContent(body.ToString(), Encoding.UTF8, "application/json")
                );

                string respStr = await resp.Content.ReadAsStringAsync();
                return Content(respStr, "application/json");
            }
        }

        // Optional: expose menuItemsData to frontend via JSON
        [HttpGet]
        public ActionResult GetMenu()
        {
            InitializeMenu();
            return Json(menuItemsData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test()
        {
            return View();
        }
    }
}



