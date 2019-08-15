using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Taste.Utility;

namespace Taste.Pages.Admin.FoodType
{
    [Authorize(Roles = SD.ManagerRole)]
    public class IndexModel : PageModel
    {
        
        public void OnGet()
        {

        }
    }
}