﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BuildAutomation.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Task = System.Threading.Tasks.Task;

namespace BuildAutomation.Controllers
{
    public class BuildController : ApiController
    {

        public void Post()
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<ReleaseEvent>(Request.Content.ReadAsStringAsync().Result);
                var validKey = ConfigurationManager.AppSettings.Get("VSTSSubId");
                if (obj.subscriptionId == validKey) //can't have random people triggering this!
                {
                    string TelegramAPIKey = ConfigurationManager.AppSettings.Get("TelegramAPIToken");

                    var msg =
                        "Woot!  New build has been released, and is staged on the server.  Do you want me to copy the files and update?";

                    var bot = new Telegram.Bot.Client(TelegramAPIKey, System.Environment.CurrentDirectory);
                    var result = bot.SendTextMessage(-1001094155678, msg,
                        replyMarkup:
                            new InlineKeyboardMarkup(new[]
                            {new InlineKeyboardButton("Yes", "update|yes"), new InlineKeyboardButton("No", "update|no")}))
                        .Result;
                }
            }
            catch (Exception e)
            {
                string path = HttpContext.Current.Server.MapPath("~/App_Data/error.log");
                StreamWriter writer = new StreamWriter(path);
                writer.WriteLine(e.Message);
            }
            

        }
    }
}
