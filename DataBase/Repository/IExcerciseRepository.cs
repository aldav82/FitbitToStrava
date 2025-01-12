using DataBase.Entities;
using DataBase.Enums;

namespace DataBase.Repository
{
    public interface IExcerciseRepository : IRepository<Excercise>
    {
        IQueryable<Excercise> GetByIds(long[] ids);
    }
}
