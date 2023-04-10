using MicroCategory.Domain.Models;
using MicroCategory.Domain.Repositories.Interface;

namespace MicroCategory.Domain.Repositories.Implement
{
    public class CTermmetumRepository : EfRepository<CTermmetum>, ICTermmetumRepository
    {

        /// <summary>
        /// Initialize a new instance of the <see cref="CTermmetumRepository"/> class
        /// </summary>
        /// <param name="dbContext"></param>
        public CTermmetumRepository(DatabaseContext dbContext) : base(dbContext) { }
    }
}
