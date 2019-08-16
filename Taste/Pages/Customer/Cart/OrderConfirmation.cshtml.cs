using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Taste.Pages.Customer.Cart
{
    public class OrderConfirmationModel : PageModel
    {
        [BindProperty]
        public int orderId { get; set; }
        public void OnGet(int id)
        {
            orderId = id;
        }
    }
}