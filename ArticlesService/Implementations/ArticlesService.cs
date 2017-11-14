using DataInterfaces;
using PocedRepository;

namespace ArticlesService.Intrfaces
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