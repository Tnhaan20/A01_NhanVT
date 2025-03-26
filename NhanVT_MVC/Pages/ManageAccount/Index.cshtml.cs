using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AS1_BusinessModel;
using AS1_Repository;

namespace NhanVT_Assignment1.Pages.ManageAccount
{
    public class IndexModel : PageModel
    {

        private readonly IAccountRepository _context;

        public IndexModel(IAccountRepository context)
        {
            _context = context;
        }

        public IList<SystemAccount> SystemAccount { get;set; } = default!;

        public IActionResult OnGet()
        {
            var email = HttpContext.Session.GetString("Email");
            var isAdmin = HttpContext.Session.GetString("IsAdmin");

            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(isAdmin))
            {
                return RedirectToPage("/Login");
            }

            var roleId = HttpContext.Session.GetInt32("RoleId");
            
            if (roleId == 2 && roleId == 1 && !(isAdmin.Equals("true")))
            {
                return RedirectToPage("/Unauthorized");
            }

            SystemAccount = _context.getAllAccounts();
            return Page();
        }
    }
}
