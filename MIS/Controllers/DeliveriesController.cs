using MIS.Models.Entities;
using MIS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIS.Controllers
{
    public class DeliveriesController : Controller
    {
        public ActionResult ShowDeliveries(string period = "all", string search = "")
        {
            var today = DateTime.Today;
            DateTime? periodStart = null;

            if (period == "month")
            {
                periodStart = today.AddMonths(-1);
            }
            else if (period == "year")
            {
                periodStart = today.AddYears(-1);
            }

            using (var db = new MISEntities())
            {
                var deliveriesQuery = db.Delivery
                    .Join(db.Deliverier, d => d.DeliverierId, dr => dr.DeliverierId, (d, dr) => new { d, dr })
                    .Join(db.User, temp => temp.d.UserId, u => u.UserId, (temp, u) => new
                    {
                        Delivery = temp.d,
                        DeliverierName = temp.dr.Name,
                        Employee = u.FirstName + " " + u.LastName + " " + (u.Patronymic ?? "")
                    });

                if (periodStart.HasValue)
                {
                    deliveriesQuery = deliveriesQuery.Where(d => d.Delivery.Date >= periodStart.Value);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    deliveriesQuery = deliveriesQuery.Where(d =>
                        d.DeliverierName.Contains(search) || d.Employee.Contains(search));
                }

                var deliveries = deliveriesQuery
                    .OrderByDescending(d => d.Delivery.Date)
                    .Select(d => new
                    {
                        DeliveryId = d.Delivery.DeliveryId,
                        Date = d.Delivery.Date,
                        DeliverierName = d.DeliverierName,
                        EmployeeName = d.Employee,
                        BookCount = db.Delivery
                            .Where(bd => bd.DeliveryId == d.Delivery.DeliveryId)
                            .Count()
                    })
                    .ToList()
                    .Select(d => new DeliveryVM
                    {
                        DeliveryId = d.DeliveryId,
                        Date = d.Date,
                        DeliverierName = d.DeliverierName,
                        BookCount = d.BookCount,
                        EmployeeName = d.EmployeeName
                    })
                    .ToList();

                return View(deliveries);
            }
        }

    }
}