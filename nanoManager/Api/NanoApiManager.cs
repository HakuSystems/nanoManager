using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;

namespace nanoManager.Api
{
    public static class NanoApiManager
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        public static NanoUserData User;
        public static Embed CREATEEMBED;
        public static string SERVERVERSION;
        public static string SERVERURL;
        private const string BASE_URL = "https://api.nanosdk.net";

        private static readonly Uri UserSelfUri = new Uri(BASE_URL + "/user/self");
        private static readonly Uri RedeemCreateUri = new Uri(BASE_URL + "/admin/redeemables/create");
        private static readonly Uri LoginUri = new Uri(BASE_URL + "/user/login");
        

        public static bool IsLoggedInAndVerified() => IsUserLoggedIn() && User.IsVerified;

        public static bool IsUserLoggedIn()
        {
            if (User == null && !string.IsNullOrEmpty(NanoApiConfig.Config.AuthKey))
            {
                Login("nanoManager", "GvUdzL8K47tweqn6+");
            }
            return User != null;
        }

        private static void ClearLogin()
        {
            Log("Clearing login data");
            User = null;
            NanoApiConfig.Config.AuthKey = null;
            NanoApiConfig.Save();
            Login("nanoManager", "GvUdzL8K47tweqn6+");
        }

        private static async Task<HttpResponseMessage> MakeApiCall(HttpRequestMessage request)
        {
            if (!string.IsNullOrEmpty(NanoApiConfig.Config.AuthKey))
            {
                var baseAddress = new Uri(BASE_URL);
                var cookieContainer = new CookieContainer();
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                {
                    cookieContainer.Add(baseAddress, new Cookie("Auth-Key", NanoApiConfig.Config.AuthKey));
                }
            }

            var response = await HttpClient.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Log("Got 401 from api, please reauthenticate.");
                ClearLogin();
                EmbedBuilder embed = new EmbedBuilder();
                embed.AddField("Error", "API Call could not be completed, bot Trying to to reconnect, try again.")
                .WithFooter(footer => footer.Text = "{nanoSDK development team}")
                .WithColor(Color.Red);
                await Task.Run(async () =>
                {
                    CREATEEMBED = embed.Build();
                });
                Login("nanoManager", "GvUdzL8K47tweqn6+");
                // throw new Exception("API Call could not be completed.");
            }

            return response;
        }

        public static async void Login(string username, string password)
        {
            Log("Trying to login");
            var content = new StringContent(JsonConvert.SerializeObject(new APILoginData
            {
                Username = username,
                Password = password
            }));
            var request = new HttpRequestMessage
            {
                RequestUri = LoginUri,
                Content = content,
                Method = HttpMethod.Post
            };

            var response = await MakeApiCall(request);
            string result = await response.Content.ReadAsStringAsync();
            var properties = JsonConvert.DeserializeObject<BaseResponse<LoginResponse>>(result);

            if (!response.IsSuccessStatusCode)
            {
                Log("Login failed");
                ClearLogin();
                return;
            }

            NanoApiConfig.Config.AuthKey = properties.Data.AuthKey;
            NanoApiConfig.Save();
            Log("Successfully logged in");
        }
        public static async Task CreateRedeem(int type, int amount)
        {

            Console.WriteLine("Trying to Create Redeemable");
            var content = new StringContent(JsonConvert.SerializeObject(new CreateRedeemData
            {
                Type = type,
                Amount = amount
            }));
            var request = new HttpRequestMessage
            {
                RequestUri = RedeemCreateUri,
                Content = content,
                Method = HttpMethod.Post
            };

            var response = await MakeApiCall(request);
            string result = await response.Content.ReadAsStringAsync();
            var properties = JsonConvert.DeserializeObject<BaseResponse<CreateRedeemResponse>>(result);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Redeem Creation Failed");

                EmbedBuilder embed = new EmbedBuilder();
                embed.AddField("Error", properties.Message)
                .WithFooter(footer => footer.Text = "{nanoSDK development team}")
                .WithColor(Color.Red);
                await Task.Run(async () =>
                {
                    CREATEEMBED = embed.Build();
                });

            }
            if (properties.Message.Contains("Successfully created code"))
            {
                var sb = new StringBuilder();
                string codeArray = null;
                foreach (var item in properties.Data.Codes)
                {
                    sb.AppendLine(item);
                    codeArray = sb.ToString();
                }

                sb.Clear();
                Console.WriteLine("Successfully Created Redeem");

                EmbedBuilder embed = new EmbedBuilder();
                embed.Title = properties.Message;
                embed.AddField("LicenseKey", codeArray)
                .WithFooter(footer => footer.Text = "{nanoSDK development team}")
                .WithColor(Color.Green);
                await Task.Run(async () =>
                {
                    CREATEEMBED = embed.Build();
                });


            }
        }

        public static void Logout() => ClearLogin();


        private static void Log(string msg)
        {
            Console.WriteLine("[nanoAPI] " + msg);
        }
    }
}