using DataBase.Entities;
using DataBase.Enums;

namespace DataBase.Repository
{
    public interface IConfigRepository: IRepository<Config>
    {
        string this[string key] { get; set; }
        string this[ConfigValues key] { get; set; }

    }
}
