using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Threading;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VkPoster
{
    class Program
    {
        static void Main(string[] args)
        {
            var api = new VkApi();
            api.Authorize(new ApiAuthParams()
            {
                Login = ConfigurationManager.AppSettings["login"],
                Password = ConfigurationManager.AppSettings["password"],
                ApplicationId = ulong.Parse(ConfigurationManager.AppSettings["appId"]),
                Settings = Settings.All
            });

            while (true)
            {
                Process.Start(ConfigurationManager.AppSettings["pathToApplication"]);

                Thread.Sleep(int.Parse(ConfigurationManager.AppSettings["sleepTime"]) * 1000 * 60);

                string text = "";

                using (StreamReader reader = new StreamReader(ConfigurationManager.AppSettings["pathToOut"]))
                {
                    text = reader.ReadToEnd();
                }

                api.Wall.Post(new WallPostParams
                {
                    OwnerId = int.Parse(ConfigurationManager.AppSettings["ownerId"]),
                    FromGroup = true,
                    Message = text,
                });

            }
        }
    }
}
