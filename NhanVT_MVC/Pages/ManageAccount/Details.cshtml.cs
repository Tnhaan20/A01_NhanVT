using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AS1_BusinessModel;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using AS1_Repository;

namespace NhanVT_Assignment1.Pages.ManageAccount
{
    public class DetailsModel : PageModel
    {
        

        private readonly IAccountRepository _context;

        public DetailsModel(IAccountRepository context)
        {
            _context = context;
        }

      

        public SystemAccount SystemAccount { get; set; } = default!;

       public IActionResult OnGet(short? id)
{
    var email = HttpContext.Session.GetString("Email");
    var roleId = HttpContext.Session.GetInt32("Role");
    var isAdmin = HttpContext.Session.GetString("IsAdmin");
    if (string.IsNullOrEmpty(email))
    {
        return RedirectToPage("/Login");
    }

    // Check authorization - only admin or role 1 can access
    if (roleId != 1 && roleId != 2 && !(isAdmin.Equals("true")))
    {
        return RedirectToPage("/Unauthorized");
    }

    if (id == null)
    {
        return NotFound();
    }

    var systemaccount = _context.GetAccountById(id.Value);
    if (systemaccount == null)
    {
        return NotFound();
    }
    
    SystemAccount = systemaccount;
    return Page();
}

       
    }
}
