using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AS1_BusinessModel;
using AS1_Repository;

namespace NhanVT_Assignment1.Pages.CategoryPage
{
    public class IndexModel : PageModel
    {
        private readonly ICategoriesRepo _context;

        public IndexModel(ICategoriesRepo context)
        {
            _context = context;
        }

        public IList<Category> Category { get;set; } = default!;

        public async Task OnGetAsync()
        {
            var email = HttpContext.Session.GetString("Email");
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (string.IsNullOrEmpty(email))
            {
                RedirectToPage("/Login");
            }
            var staffRole = 1;
            if (roleId.Value == staffRole)
            {
            Category = _context.getAllCategories();

            }
            else
                Response.Redirect("/Unauthorized");

        }
    }
}
