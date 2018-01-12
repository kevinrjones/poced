using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poced.Logging;
using Poced.Logging.Web;
using Poced.Services.Intrfaces;
using Poced.Web.Models;

namespace Poced.Web.Controllers
{
    [Authorize]
    public class ArticlesController : Controller
    {
        private readonly IPocedWebLogger _logger;
        private readonly IArticlesService _articlesService;

        public ArticlesController(IPocedWebLogger logger, IArticlesService articlesService)
        {
            _logger = logger;
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

        public ActionResult Image(int id)
        {
            var imageByteArray = _articlesService.GetImage(id);
            if (imageByteArray != null)
            {
                MemoryStream stream = new MemoryStream(imageByteArray);

                return new FileStreamResult(stream, "image/png");
            }
            return NotFound();
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