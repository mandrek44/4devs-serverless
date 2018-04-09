
using System;
using System.Text;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace FunctionApp1
{
    public static class SaveAnOrder
    {
        public class Order
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public string FileName { get; set; }
            public string CustomerEmail { get; set; }
            public int RequiredWidth { get; set; }
            public int RequiredHeight { get; set; }

            public static Order From(PostOrder postedOrder)
            {
                return new Order()
                {
                    PartitionKey = "Order",
                    RowKey = Guid.NewGuid().ToString(),
                    FileName = postedOrder.FileName,
                    CustomerEmail = postedOrder.CustomerEmail,
                    RequiredHeight = postedOrder.RequiredHeight,
                    RequiredWidth = postedOrder.RequiredWidth
                };
            }
        }

        public class PostOrder
        {
            public string FileName { get; set; }
            public string CustomerEmail { get; set; }
            public int RequiredWidth { get; set; }
            public int RequiredHeight { get; set; }
        }

        [FunctionName("SaveAnOrder")]
        [return: Table("Orders")]
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
