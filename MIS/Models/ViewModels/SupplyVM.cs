using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MIS.Models.ViewModels
{
    public class DeliveryVM
    {
        public Guid DeliveryId { get; set; }
        public DateTime Date { get; set; }
        public string DeliverierName { get; set; }
        public int BookCount { get; set; }
        public string EmployeeName { get; set; }
    }
}