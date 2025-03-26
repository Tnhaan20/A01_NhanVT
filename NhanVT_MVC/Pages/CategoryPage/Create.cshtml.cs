using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AS1_BusinessModel;
using AS1_Repository;
using Microsoft.AspNetCore.SignalR;
using NhanVT_MVC.Pages.Hubs;

namespace NhanVT_Assignment1.Pages.CategoryPage
{
    public class CreateModel : PageModel
    {
        private readonly ICategoriesRepo _context;
        private readonly IHubContext<FunewsHub> _hubContext;

        public CreateModel(ICategoriesRepo context, IHubContext<FunewsHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public IActionResult OnGet()
        {
            var email = HttpContext.Session.GetString("Email");
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (string.IsNullOrEmpty(email))
            {
                RedirectToPage("/Login");
            }
            var staffRole = 1;
            if (roleId.Value != staffRole)
            {
                return Redirect("/Unauthorized");
            }
            Category = new Category { IsActive = true }; // Set IsActive to true by default

            ViewData["ParentCategoryId"] = new SelectList(_context.GetCategoryList(), "Value", "Text");
            return Page();
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {

            try
            {
                _context.AddCategory(Category);
                await _hubContext.Clients.All.SendAsync("Change");


            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToPage("./Index");
        }
    }
}
