using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;
using Taste.DataAccess.Data.Repository.IRepository;
using Taste.Models;
using Taste.Models.ViewModels;
using Taste.Utility;

namespace Taste.Pages.Customer.Cart
{
    public class SummaryModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public SummaryModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public OrderDetailsCart detailCart { get; set; }

        public IActionResult OnGet()
        {
            detailCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            detailCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<ShoppingCart> cart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                detailCart.listCart = cart.ToList();
            }

            foreach (var cartList in detailCart.listCart)
            {
                cartList.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(m => m.Id == cartList.MenuItemId);
                detailCart.OrderHeader.OrderTotal += (cartList.MenuItem.Price * cartList.Count);
            }

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(c => c.Id == claim.Value);
            detailCart.OrderHeader.PickupName = applicationUser.FullName;
            detailCart.OrderHeader.PickUpTime = DateTime.Now;
            detailCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            return Page();

        }

        public IActionResult OnPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            detailCart.listCart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value).ToList();

            detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            detailCart.OrderHeader.OrderDate = DateTime.Now;
            detailCart.OrderHeader.UserId = claim.Value;
            detailCart.OrderHeader.Status = SD.PaymentStatusPending;
            detailCart.OrderHeader.PickUpTime = Convert.ToDateTime(detailCart.OrderHeader.PickUpDate.ToShortDateString() + " " +detailCart.OrderHeader.PickUpTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _unitOfWork.OrderHeader.Add(detailCart.OrderHeader);
            _unitOfWork.Save();

            foreach(var item in detailCart.listCart)
            {
                item.MenuItem = _unitOfWork.MenuItem.GetFirstOrDefault(m => m.Id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = detailCart.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                detailCart.OrderHeader.OrderTotal += (orderDetails.Count * orderDetails.Price);
                _unitOfWork.OrderDetails.Add(orderDetails);
                
            }
            detailCart.OrderHeader.OrderTotal = Convert.ToDouble(String.Format("{0:.##}", detailCart.OrderHeader.OrderTotal));
            _unitOfWork.ShoppingCart.RemoveRange(detailCart.listCart);
            HttpContext.Session.SetInt32(SD.ShoppingCart, 0);
            _unitOfWork.Save();

            if (stripeToken != null)
            {

                var options = new ChargeCreateOptions
                {
                    //Amount is in cents
                    Amount = Convert.ToInt32(detailCart.OrderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Description = "Order ID : " + detailCart.OrderHeader.Id,
                    Source = stripeToken
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);

                detailCart.OrderHeader.TransactionId = charge.Id;

                if (charge.Status.ToLower() == "succeeded")
                {
                    //email 
                    detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    detailCart.OrderHeader.Status = SD.StatusSubmitted;
                }
                else
                {
                    detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                }
            }
            else
            {
                detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            _unitOfWork.Save();

            return RedirectToPage("/Customer/Cart/OrderConfirmation", new { id = detailCart.OrderHeader.Id });

        }

        

    }
}