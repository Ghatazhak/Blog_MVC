using Blog_MVC.Data;
using System.Threading.Tasks;

namespace Blog_MVC.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;

        public DataService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ManageDataAsync()
        {
            // Task 1: Seed a few roles into the system
            await SeedRolesAsync();

            // Task 2: Seed a few user in to the system
            await SeedUsersAsync();

        }

        private async Task SeedRolesAsync()
        {

        }

        private async Task SeedUsersAsync()
        {

        }







    }
}
