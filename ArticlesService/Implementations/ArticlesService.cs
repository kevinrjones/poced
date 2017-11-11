using DataInterfaces;

namespace ArticlesService.Intrfaces
{
    public class ArticlesService : IArticlesService
    {
        private readonly IArticlesData _data;

        public ArticlesService(IArticlesData data)
        {
            _data = data;
        }

        public byte[] GetImage(int imageId)
        {
            return _data.GetImage(imageId);
        }
    }
}