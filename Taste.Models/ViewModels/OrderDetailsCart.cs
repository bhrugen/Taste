using System;
using System.Collections.Generic;
using System.Text;

namespace Taste.Models.ViewModels
{
    public class OrderDetailsCart
    { 
        public List<ShoppingCart> listCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
