using DataBase.Entities;
using DataBase.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Repository
{

    public class ExcerciseRepository : Repository<Excercise>, IExcerciseRepository
    {
        public ExcerciseRepository(AppDbContext context) : base(context)
        {
        }

        public IQueryable<Excercise> GetByIds(long[] ids)
        {
            return this._context.Excercises.Where(c => ids.Any(id => id == c.ExcerciseId));
        }
    }
}
