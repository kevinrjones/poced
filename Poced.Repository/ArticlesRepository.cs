using System.Linq;
using Poced.Repository.Contexts;
using Poced.Repository.Entities;

namespace Poced.Repository
{
    public class ArticlesRepository : PocedRepository<Article>, IArticlesRepository
    {
        public ArticlesRepository(PocedDbContext dbContext) : base(dbContext)
        {
        }

        public byte[] GetArticle(int articleId)
        {
            var data = from e in Entities
                where e.ArticleId == articleId
                select e;

            return data.FirstOrDefault()?.Image;
        }
    }
}