using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using ArticlesService.Intrfaces;
using PocedWeb.Models;

namespace PocedWeb.Controllers
{
    [Authorize]

    public class ArticlesController : Controller
    {
        private readonly IArticlesService _articlesService;

        public ArticlesController(IArticlesService articlesService)
        {
            _articlesService = articlesService;
        }
        // GET: Articles
        [Route("Articles")]
        [Route("")]
        public ActionResult Index()
        {
            var articles = new List<Article>
            {
                new Article { Id = 1, Title = "BBC Sport", Url = "http://news.bbc.co.uk/sport"}
            };
            return View(articles);
        }


        // calculate $($("#queue .item")[0]).height()
        [Route("Articles/Image/{id}")]

        public FileStreamResult Image(int id)
        {
            var imageByteArray = _articlesService.GetImage(id);
            MemoryStream stream = new MemoryStream(imageByteArray);

            return new FileStreamResult(stream, "image/png");

        }

        [Route("Articles/New")]
        public ActionResult New()
        {
            return View();
        }

        [Route("Articles/Create")]
        public ActionResult Create(NewArticle article)
        {
            if (ModelState.IsValid)
            {
                // add model to database
                return RedirectToAction("Index");
            }
            else
            {
                return View("New", new Article { Title = article.Title, Url = article.Url, Tags = article.Tags });
            }
        }

    }
}