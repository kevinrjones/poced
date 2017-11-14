using PocedRepository.Entities;
using Repository;

namespace PocedRepository
{
    public interface IArticlesRepository : IRepository<Article>
    {
        byte[] GetArticle(int articleId);
    }
}