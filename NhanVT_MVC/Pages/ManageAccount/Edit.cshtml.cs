using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AS1_BusinessModel;
using AS1_Repository;

namespace NhanVT_Assignment1.Pages.ManageAccount
{
    public class EditModel : PageModel
    {
        private readonly IAccountRepository _context;

        public EditModel(IAccountRepository context)
        {
            _context = context;
        }

        [BindProperty]
        public SystemAccount SystemAccount { get; set; } = default!;

        [TempData]
        public string SuccessMessage { get; set; }

        public IActionResult OnGet(short? id)
        {
            var email = HttpContext.Session.GetString("Email");
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var isAdmin = HttpContext.Session.GetString("IsAdmin");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            // Check authorization - only admin or role 1 can access
            if ((roleId != 1) && (isAdmin != "true"))
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

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _context.UpdateAccount(SystemAccount.AccountId, SystemAccount);
                SuccessMessage = "Account updated successfully!";
                return Page(); // Stay on the same page to show the success message
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return Page();
            }
        }

       
    }
}