using RestSharp;
using System.Net;

namespace tps_apps.service
{
    public class CustomFunction
    {
        public async void SendMessageNotif(string message)
        {
            try
            {
                var url = "https://api.telegram.org/bot6188748965:AAHlc7MP2C-MWt0_ZHfA-JI4UQzw95bTZ74/sendMessage?chat_id=-946291736&text=" + message;
                var client = new RestClient(url);
                var request = new RestRequest("", Method.Get);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var response = await client.ExecuteAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (!string.IsNullOrEmpty(response.Content))
                    {
                        //eventLog1.WriteEntry(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + messsage);
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + message);
                    }
                }
                else
                {
                    //Console.WriteLine("Send message failed, check your both");
                    //eventLog1.WriteEntry(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + messsage);
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + message);
                }
            }
            catch (Exception ex)
            {
                //eventLog1.WriteEntry(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ex.Message);
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ex.Message);
            }
        }
    }
}
