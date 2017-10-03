using System.Threading.Tasks;
using Anlab.Core.Data;
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
        
        // ex: /analysis/soil/200
        [Route("/analysis/{category}/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            // ignore category since the ID is all we need
            var analysis = await _dbContext.AnalysisMethods.SingleOrDefaultAsync(x => x.Id == id);

            var content = Markdown.ToHtml(analysis.Content);
            
            return View(new AnalysisMethodViewModel { AnalysisMethod = analysis, HtmlContent = content });
        }
    }
}