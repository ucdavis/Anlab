using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using AnlabMvc.Models.Analysis;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnlabMvc.Controllers
{
    public class AnalysisController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AnalysisController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // show the main content for methods of analysis
        [Route("/methods-of-analysis")]
        public async Task<IActionResult> Index()
        {
            var allMethods = await _dbContext.AnalysisMethods
                .Select(x => new AnalysisMethod {Id = x.Id, Title = x.Title, Category = x.Category}).ToListAsync();

            return View(allMethods);
        }
        
        // ex: /analysis/soil/200
        [Route("/analysis/{category}/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            // ignore category since the ID is all we need
            var analysis = await _dbContext.AnalysisMethods.SingleOrDefaultAsync(x => x.Id == id);

            var content = Markdown.ToHtml(analysis.Content);
            
            return View(new AnalysisMethodViewModel { AnalysisMethod = analysis, HtmlContent = content });
        }
    }
}