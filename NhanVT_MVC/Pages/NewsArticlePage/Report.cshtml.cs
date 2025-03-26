using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AS1_BusinessModel;
using AS1_Repository;

namespace NhanVT_Assignment1.Pages.NewsArticlePage
{
    public class ReportModel : PageModel
    {
        private readonly INewsArticleRepository _newsArticleRepository;

        public ReportModel(INewsArticleRepository newsArticleRepository)
        {
            _newsArticleRepository = newsArticleRepository;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public List<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();

        public bool ShowReport { get; set; } = false;

        public ChartData CategoryData { get; set; }
        public ChartData StatusData { get; set; }
        public ChartData DailyArticlesData { get; set; }

        public IActionResult OnGet()
        {
            // Check authorization
            var email = HttpContext.Session.GetString("Email");
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var isAdmin = HttpContext.Session.GetString("IsAdmin");

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToPage("/Login");
            }

            // Set default dates if not provided
            if (!StartDate.HasValue)
            {
                StartDate = DateTime.Today.AddMonths(-1);
            }

            if (!EndDate.HasValue)
            {
                EndDate = DateTime.Today;
            }

            // If we have date parameters, generate the report
            if (StartDate.HasValue && EndDate.HasValue)
            {
                ShowReport = true;

                // Adjust EndDate to include the entire day
                var endDateAdjusted = EndDate.Value.AddDays(1).AddTicks(-1);

                // Get all news articles
                var allArticles = _newsArticleRepository.GetNewsArticles();

                // Filter by date range and sort by created date in descending order
                NewsArticles = allArticles
                    .Where(a => a.CreatedDate.HasValue &&
                                a.CreatedDate.Value >= StartDate.Value &&
                                a.CreatedDate.Value <= endDateAdjusted)
                    .OrderByDescending(a => a.CreatedDate)
                    .ToList();

                // Prepare chart data
                PrepareChartData();
            }

            return Page();
        }

        private void PrepareChartData()
        {
            // Prepare category data for chart
            var categoryGroups = NewsArticles
                .Where(a => a.Category != null)
                .GroupBy(a => a.Category.CategoryDesciption)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .ToList();

            CategoryData = new ChartData
            {
                Labels = categoryGroups.Select(g => g.Name).ToArray(),
                Data = categoryGroups.Select(g => g.Count).ToArray()
            };

            // Prepare status data for chart
            var activeCount = NewsArticles.Count(a => Convert.ToInt32(a.NewsStatus) == 1);
            var inactiveCount = NewsArticles.Count(a => Convert.ToInt32(a.NewsStatus) == 0);

            StatusData = new ChartData
            {
                Labels = new[] { "Active", "Inactive" },
                Data = new[] { activeCount, inactiveCount }
            };

            // Prepare daily articles data for chart
            var dailyGroups = NewsArticles
                .Where(a => a.CreatedDate.HasValue)
                .GroupBy(a => a.CreatedDate.Value.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(g => g.Date)
                .ToList();

            DailyArticlesData = new ChartData
            {
                Labels = dailyGroups.Select(g => g.Date.ToString("MM/dd/yyyy")).ToArray(),
                Data = dailyGroups.Select(g => g.Count).ToArray()
            };
        }
    }

    public class ChartData
    {
        public string[] Labels { get; set; }
        public int[] Data { get; set; }
    }
}