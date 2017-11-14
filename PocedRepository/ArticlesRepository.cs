using System.Linq;
using PocedRepository.Entities;

namespace PocedRepository
{
    public class ArticlesRepository : PocedRepository<Article>, IArticlesRepository
    {
        public ArticlesRepository(string connectionString) : base(connectionString)
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