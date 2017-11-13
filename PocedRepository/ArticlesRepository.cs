namespace PocedRepository
{
    public class ArticlesRepository : PocedRepository<Article>, IArticlesRepository
    {
        public ArticlesRepository(string connectionString) : base(connectionString)
        {
        }
    }
}