using AS1_Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NhanVT_Assignment1.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IConfiguration _configuration;
        public LoginModel(IAccountRepository accountRepository, IConfiguration configuration)
        {
            _accountRepository = accountRepository;
            _configuration = configuration;
        }

        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public IActionResult OnGet()
        {

            HttpContext.Session.Clear();
            return Page();
        }

        public IActionResult OnPost()
        {
            var account = _accountRepository.GetAccount(Email, Password);
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];
            

            if (Email == adminEmail && Password == adminPassword)
            {
                HttpContext.Session.SetString("Email", Email);
                HttpContext.Session.SetString("IsAdmin", "true");
                
                return RedirectToPage("/ManageAccount/Index"); 
            }

            
            

            if (account == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return Page();
            }

            
            


            HttpContext.Session.SetString("Email", account.AccountEmail);
            HttpContext.Session.SetInt32("RoleId", account.AccountRole.Value);
            HttpContext.Session.SetInt32("AccountId", account.AccountId);

            if (account.AccountRole.Value == 1)
                return RedirectToPage("/CategoryPage/Index");
            else if (account.AccountRole.Value == 2)
                return RedirectToPage("/NewsArticlePage/Index");
            else
                return RedirectToPage("/Unauthorized");

        }

        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Login");
        }
    }

}
