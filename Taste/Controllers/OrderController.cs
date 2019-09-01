using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Models;
using Taste.Models.ViewModels;
using Taste.Utility;

namespace Taste.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get(string status = null)
        {
            List<OrderDetailsViewModel> orderListVM = new List<OrderDetailsViewModel>();

            IEnumerable<OrderHeader> OrderHeaderList;

            if (User.IsInRole(SD.CustomerRole))
            {
                //retrieve all orders for that customer
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                OrderHeaderList = _unitOfWork.OrderHeader.GetAll(u => u.UserId == claim.Value, null, "ApplicationUser");
            }
            else
            {
                OrderHeaderList = _unitOfWork.OrderHeader.GetAll(null, null, "ApplicationUser");
            }

            if (status == "cancelled")
            {
                OrderHeaderList = OrderHeaderList.Where(o => o.Status == SD.StatusCancelled || o.Status == SD.StatusRefunded || o.Status == SD.PaymentStatusRejected);
            }
            else
            {
                if (status == "completed")
                {
                    OrderHeaderList = OrderHeaderList.Where(o => o.Status == SD.StatusCompleted);
                }
                else
                {
                    OrderHeaderList = OrderHeaderList.Where(o => o.Status == SD.StatusReady || o.Status == SD.StatusInProcess || o.Status == SD.StatusSubmitted || o.Status == SD.PaymentStatusPending);
                }
            }

            foreach(OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderId == item.Id).ToList()
                };
                orderListVM.Add(individual);
            }

            return Json(new { data = orderListVM });
        }

    }
}