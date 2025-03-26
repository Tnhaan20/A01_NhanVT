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
    public class DetailsModel : PageModel
    {
        private readonly ICategoriesRepo _context;

        public DetailsModel(ICategoriesRepo context)
        {
            _context = context;
        }

        public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(short id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _context.GetCategoryId(id);
            if (category == null)
            {
                return NotFound();
            }
            else
            {
                Category = category;
            }
            return Page();
        }
    }
}
