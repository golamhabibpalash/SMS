using System;
using System.Collections.Generic;
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
                    {"token", "6a856059febeaab128231564cdace6ff" },
                    {"to", phoneNumber },
                    {"message", message }
                };
                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("http://api.greenweb.com.bd/api.php?", content);
                var responseString = await response.Content.ReadAsStringAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
