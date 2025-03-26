using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AS1_BusinessModel;
using AS1_Repository;
using Microsoft.AspNetCore.Http;

namespace NhanVT_Assignment1.Pages.ProfilePage
{
    public class IndexModel : PageModel
    {
        private readonly IAccountRepository _accountRepository;
        private readonly INewsArticleRepository _newsRepository;

        public IndexModel(IAccountRepository accountRepository, INewsArticleRepository newsRepository)
        {
            _accountRepository = accountRepository;
            _newsRepository = newsRepository;
        }

        [BindProperty]
        public SystemAccount UserAccount { get; set; }

        public List<NewsArticle> UserArticles { get; set; } = new List<NewsArticle>();

        [TempData]
        public string SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            // Check if user is logged in
            var email = HttpContext.Session.GetString("Email");
            var accountId = HttpContext.Session.GetInt32("AccountId");

            if (string.IsNullOrEmpty(email) || !accountId.HasValue)
            {
                return RedirectToPage("/Login");
            }

            // Get the user account information
            UserAccount = _accountRepository.GetAccountById((short)accountId);
            if (UserAccount == null)
            {
                return RedirectToPage("/Login");
            }

            // Get the user's news articles
            var allArticles = _newsRepository.GetNewsArticles();
            UserArticles = allArticles
                .Where(a => a.CreatedById == accountId.Value)
                .OrderByDescending(a => a.CreatedDate)
                .ToList();

            return Page();
        }

        public IActionResult OnPost()
        {
            // Check if user is logged in
            var email = HttpContext.Session.GetString("Email");
            var accountId = HttpContext.Session.GetInt32("AccountId");

            if (string.IsNullOrEmpty(email) || !accountId.HasValue)
            {
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid)
            {
                // Get the user's news articles for display even if validation fails
                var allArticles = _newsRepository.GetNewsArticles();
                UserArticles = allArticles
                    .Where(a => a.CreatedById == accountId.Value)
                    .OrderByDescending(a => a.CreatedDate)
                    .ToList();

                return Page();
            }

            try
            {
                // Get existing account to preserve role and other fields
                var existingAccount = _accountRepository.GetAccountById((short)accountId);
                if (existingAccount == null)
                {
                    return RedirectToPage("/Login");
                }

                // Update only the allowed fields
                existingAccount.AccountName = UserAccount.AccountName;
                existingAccount.AccountPassword = UserAccount.AccountPassword;

                // Save changes
                _accountRepository.UpdateAccount((short)accountId, existingAccount);

                SuccessMessage = "Your profile has been updated successfully!";

                // Refresh the user articles list
                var allArticles = _newsRepository.GetNewsArticles();
                UserArticles = allArticles
                    .Where(a => a.CreatedById == accountId.Value)
                    .OrderByDescending(a => a.CreatedDate)
                    .ToList();

                // Update the model with the latest data
                UserAccount = existingAccount;

                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");

                // Get the user's news articles for display even if there's an error
                var allArticles = _newsRepository.GetNewsArticles();
                UserArticles = allArticles
                    .Where(a => a.CreatedById == accountId.Value)
                    .OrderByDescending(a => a.CreatedDate)
                    .ToList();

                return Page();
            }
        }
    }
}