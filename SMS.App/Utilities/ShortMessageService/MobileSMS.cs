using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SMS.App.Utilities.ShortMessageService
{
    public class MobileSMS
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<bool> SendSMS(string phoneNumber, string message)
        {
            try
            {
                var values = new Dictionary<string, string>
                {
                    {"token", "449291fd377e6c0d5c72f424c454145a" },
                    {"to", phoneNumber },
                    {"message", message }
                };
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("http://api..greenweb.com.bd/api.php?", content);
                var responseString = await response.Content.ReadAsStringAsync();

                string responseStatus = responseString.Split(' ').FirstOrDefault();
                if (responseStatus == "Error:")
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
