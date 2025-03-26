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

namespace NhanVT_Assignment1.Pages.NewsArticlePage
{
    public class EditModel : PageModel
    {
        private readonly INewsArticleRepository _context;
        private readonly IAccountRepository _accountContext;

        private readonly ICategoriesRepo _categoryContext;

        public EditModel(INewsArticleRepository context, ICategoriesRepo categoryContext, IAccountRepository accountContext)
        {
            _context = context;
            _categoryContext = categoryContext;
            _accountContext = accountContext;
        }

        [BindProperty]
        public NewsArticle NewsArticle { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsarticle =  _context.getNewsById(id);
            if (newsarticle == null)
            {
                return NotFound();
            }
            NewsArticle = newsarticle;
            if (!ModelState.IsValid)
            {
            }
                ViewData["CategoryId"] = new SelectList(_categoryContext.GetList(), "Value", "Text");
            ViewData["CreatedById"] = new SelectList(_accountContext.GetSelectAccountList(), "Value", "Text");
            return Page();
        }

         public async Task<IActionResult> OnPostAsync()
        {
           
            try
            {
                _context.UpdateNews(NewsArticle);
            }
            catch (DbUpdateConcurrencyException)
            {
                if ((_context.getNewsById(NewsArticle.NewsArticleId)) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        
    }
}
