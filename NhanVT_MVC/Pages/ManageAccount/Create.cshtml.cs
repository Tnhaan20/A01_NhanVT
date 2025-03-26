using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AS1_BusinessModel;
using AS1_Repository;

namespace NhanVT_Assignment1.Pages.ManageAccount
{
    public class CreateModel : PageModel
    {
        private readonly IAccountRepository _context;

        public CreateModel(IAccountRepository context)
        {
            _context = context;
        }

        public IActionResult OnGet()
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

            SystemAccount = new SystemAccount();

            return Page();
        }

        [BindProperty]
        public SystemAccount SystemAccount { get; set; } = default!;
        [TempData]
        public string SuccessMessage { get; set; }
        public IActionResult OnPostAsync()
        {
            if (string.IsNullOrEmpty(SystemAccount.AccountName) ||
                string.IsNullOrEmpty(SystemAccount.AccountEmail) ||
                string.IsNullOrEmpty(SystemAccount.AccountPassword) ||
                !SystemAccount.AccountRole.HasValue)
            {
                ModelState.AddModelError("", "All fields are required.");
                return Page();
            }

            var existingAccount = _context.GetAccountByEmail(SystemAccount.AccountEmail);
            if (existingAccount != null)
            {
                ModelState.AddModelError("SystemAccount.AccountEmail", "This email is already registered.");
                return Page();
            }

            try
            {
                var allAccounts = _context.getAllAccounts();

                // Find the maximum AccountId
                short maxId = 0;
                if (allAccounts != null && allAccounts.Any())
                {
                    maxId = allAccounts.Max(a => a.AccountId);
                }

                // Set the new AccountId to maxId + 1
                SystemAccount.AccountId = (short)(maxId + 1);

                _context.AddAccount(SystemAccount);
                SuccessMessage = "Account added successfully!";
                return Page();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return Page();
            }
                return RedirectToPage("./Index");
        }
    }
}


