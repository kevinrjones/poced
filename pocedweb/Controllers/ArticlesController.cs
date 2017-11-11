using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArticlesService;
using ArticlesService.Intrfaces;
using pocedweb.Models;

namespace pocedweb.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly IArticlesService _articlesService;

        public ArticlesController(IArticlesService articlesService)
        {
            _articlesService = articlesService;
        }
        // GET: Articles
        public ActionResult Index()
        {
            var articles = new List<Article>
            {
                new Article { Id = 1 }
            };
            return View(articles);
        }

        /*
         * 	public ModelAndView index(@CurrentUser AuthenticatedUser user) {
		List<GetJacketArticle> serviceEntries = service.getAllArticles(user.getId());
		List<Article> articles = new ArrayList<Article>();
		serviceEntries.forEach(e -> {
			Article entry = new Article(e);
			articles.add(entry);
		});

		ModelAndView mv = new ModelAndView("article/index");
		mv.addObject("articles", articles);
		
		return mv;
	}
         */
        public FileStreamResult Image(int id)
        {
            var imageByteArray = _articlesService.GetImage(id);
            MemoryStream stream = new MemoryStream(imageByteArray);

            return new FileStreamResult(stream, "image/png");

        }
    }
}