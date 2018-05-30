using System.Linq;
using Poced.Repository.Bare.Contexts;
using Poced.Repository.Bare.Entities;

namespace Poced.Repository.Bare
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