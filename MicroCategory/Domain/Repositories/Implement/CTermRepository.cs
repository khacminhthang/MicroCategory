using MicroCategory.Domain.Models;
using MicroCategory.Domain.Repositories.Interface;

namespace MicroCategory.Domain.Repositories.Implement
{
    public class CTermRepository: EfRepository<CTerm>, ICTermRepository
    {

        /// <summary>
        /// Initialize a new instance of the <see cref="CTermRepository"/> class
        /// </summary>
        /// <param name="dbContext"></param>
        public CTermRepository(DatabaseContext dbContext) : base(dbContext) { }
    }
}
