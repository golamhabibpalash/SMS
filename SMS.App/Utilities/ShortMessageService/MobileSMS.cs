using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
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

            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            string vendorAPILink = MyConfig.GetValue<string>("PhoneSMSSetup:PhoneSMSVendorAPILink");
            string token = MyConfig.GetValue<string>("PhoneSMSSetup:Token");
            try
            {
                var values = new Dictionary<string, string>
                {
                    //{"token", "449291fd377e6c0d5c72f424c454145a" },
                    {"token", token },
                    {"to", phoneNumber },
                    {"message", message }
                };
                var content = new FormUrlEncodedContent(values);
                //var response = await client.PostAsync("http://api..greenweb.com.bd/api.php?", content);
                var response = await client.PostAsync(vendorAPILink, content);
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
