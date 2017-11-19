using Poced.Repository.Entities;
using Poced.RepositoryInterfaces;

namespace Poced.Repository
{
    public interface IArticlesRepository : IRepository<Article>
    {
        byte[] GetArticle(int articleId);
    }
}