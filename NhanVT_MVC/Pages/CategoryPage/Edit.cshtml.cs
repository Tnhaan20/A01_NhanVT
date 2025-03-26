using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AS1_BusinessModel;
using AS1_Repository;
using Microsoft.AspNetCore.SignalR;
using NhanVT_MVC.Pages.Hubs;

namespace NhanVT_Assignment1.Pages.CategoryPage
{
    public class EditModel : PageModel
    {
        private readonly ICategoriesRepo _context;
        private readonly IHubContext<FunewsHub> _hubContext;

        public EditModel(ICategoriesRepo context, IHubContext<FunewsHub> hubContext)
        {
            _context = context;
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

            var category =  _context.GetCategoryId(id);
            if (category == null)
            {
                return NotFound();
            }
            Category = category;
           ViewData["ParentCategoryId"] = new SelectList(_context.GetCategoryList(), "Value", "Text");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            
            try
            {
                _context.UpdateCategory(Category);
                await _hubContext.Clients.All.SendAsync("Change");

            }
            catch (Exception e)
            {
                throw;
            }

            return RedirectToPage("./Index");
        }

        
    }
}
