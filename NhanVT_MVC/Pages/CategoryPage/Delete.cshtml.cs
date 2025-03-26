using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AS1_BusinessModel;
using AS1_Repository;
using Microsoft.AspNetCore.SignalR;
using NhanVT_MVC.Pages.Hubs;

namespace NhanVT_Assignment1.Pages.CategoryPage
{
    public class DeleteModel : PageModel
    {
        private readonly ICategoriesRepo _cateContext;
        private readonly INewsArticleRepository _newsContext;
        private readonly IHubContext<FunewsHub> _hubContext;


        public DeleteModel(ICategoriesRepo cateContext, INewsArticleRepository newsContext, IHubContext<FunewsHub> hubContext)
        {
            _cateContext = cateContext;
            _newsContext = newsContext;
            _hubContext = hubContext;
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(short id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _cateContext.GetCategoryId(id);

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

        public async Task<IActionResult> OnPostAsync(short id)
{
    if (id == null)
    {
        return NotFound();
    }

    var category = _cateContext.GetCategoryId(id);
        
    if (category == null)
    {
        return NotFound();
    }

            if (_newsContext.isInUsed(category.CategoryId))
            {
                ModelState.AddModelError(string.Empty, "This category cannot be deleted because it is referenced by other data in the system.");
                Category = category;
                return Page();
            }


    try
    {
        _cateContext.DeleteCategory(id);
                await _hubContext.Clients.All.SendAsync("Change");

                return RedirectToPage("./Index");
    }
    catch (DbUpdateException ex)
    {
        // Handle the database constraint violation
        ModelState.AddModelError(string.Empty, "This category cannot be deleted because it is referenced by other data in the system.");
        Category = category;
        return Page();
    }
}
    }
}
