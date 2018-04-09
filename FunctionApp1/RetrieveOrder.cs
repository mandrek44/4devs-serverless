using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;

namespace FunctionApp1
{
    public static class RetrieveOrder
    {
        [FunctionName("RetrieveOrder")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "retrieveorder")]HttpRequest req, 
            [Table("Orders", Connection = "OrdersStorage")]CloudTable ordersTable,
            TraceWriter log)
        {
            
            string fileName = req.Query["fileName"];

            log.Info($"Retrieving order by fileName=${fileName}");

            var findByFilenameQuery = new TableQuery<Order>().Where(
                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, fileName));

            var results = await ordersTable.ExecuteQuerySegmentedAsync(findByFilenameQuery, null);
            if (!results.Any())
            {
                return new NotFoundResult();
            }

            var foundOrder = results.First();
            return new JsonResult(new
            {
                foundOrder.FileName,
                foundOrder.CustomerEmail,
                foundOrder.RequiredHeight,
                foundOrder.RequiredWidth
            });
        }
    }
}
