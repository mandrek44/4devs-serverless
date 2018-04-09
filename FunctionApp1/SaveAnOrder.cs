using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace FunctionApp1
{
    public class PostOrder
    {
        public string FileName { get; set; }
        public string CustomerEmail { get; set; }
        public int RequiredWidth { get; set; }
        public int RequiredHeight { get; set; }
    }
    public static class SaveAnOrder
    {

        [FunctionName("SaveAnOrder")]
        [return: Table("Orders", Connection = "OrdersStorage")]
        public static Order Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "order")]HttpRequest req, TraceWriter log)
        {
            log.Info("SaveAnOrder: C# HTTP trigger function processed a request.");

            var bodyString = new HttpRequestStreamReader(req.Body, Encoding.UTF8).ReadToEnd();
            var postedOrder = JsonConvert.DeserializeObject<PostOrder>(bodyString);

            log.Info(bodyString);

            return Order.From(postedOrder);
        }
    }
}
