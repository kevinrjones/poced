using Poced.Repository;
using Poced.Services.Intrfaces;

namespace Poced.Services.Implementations
{
    public class ArticlesService : IArticlesService
    {
        private readonly IArticlesRepository _data;

        public ArticlesService(IArticlesRepository data)
        {
            _data = data;
        }

        public byte[] GetImage(int articleId)
        {
            return _data.GetArticle(articleId);
        }
    }
}