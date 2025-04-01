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

namespace NhanVT_Assignment1.Pages.ManageAccount
{
    public class DeleteModel : PageModel
    {
        private readonly IAccountRepository _context;
        private readonly IHubContext<FunewsHub> _hubContext;

        public DeleteModel(IAccountRepository context, IHubContext<FunewsHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [BindProperty]
        public SystemAccount SystemAccount { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(short id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemaccount = _context.GetAccountById(id);

            if (systemaccount == null)
            {
                return NotFound();
            }
            else
            {
                SystemAccount = systemaccount;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(short id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
            _context.DeleteAccount(id);
                await _hubContext.Clients.All.SendAsync("Change");
            }
            catch (Exception ex) { 
                ex.ToString();
            }

            return RedirectToPage("./Index");
        }
    }
}
