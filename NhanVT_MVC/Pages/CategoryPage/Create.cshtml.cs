using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using AS1_BusinessModel;

namespace NhanVT_Assignment1.Pages.CategoryPage
{
    public class CreateModel : PageModel
    {
        private readonly AS1_BusinessModel.FunewsManagementContext _context;

        public CreateModel(AS1_BusinessModel.FunewsManagementContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["ParentCategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryDesciption");
            return Page();
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Categories.Add(Category);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
