namespace PocedRepository
{
    public class UsersRepository : PocedRepository<User>, IUsersRepository
    {
        public UsersRepository(string connectionString) : base(connectionString)
        {
        }
    }
}