using Poced.Repository.Bare.Entities;
using Poced.RepositoryInterfaces;

namespace Poced.Repository.Bare
{
    public interface IArticlesRepository : IRepository<Article>
    {
        byte[] GetArticle(int articleId);
    }
}