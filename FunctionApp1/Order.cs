using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace FunctionApp1
{
    public class Order : TableEntity
    {
        public string FileName { get; set; }
        public string CustomerEmail { get; set; }
        public int RequiredWidth { get; set; }
        public int RequiredHeight { get; set; }

        public static Order From(PostOrder postedOrder)
        {
            return new Order()
            {
                PartitionKey = "Order",
                RowKey = postedOrder.FileName,
                FileName = postedOrder.FileName,
                CustomerEmail = postedOrder.CustomerEmail,
                RequiredHeight = postedOrder.RequiredHeight,
                RequiredWidth = postedOrder.RequiredWidth
            };
        }
    }
}