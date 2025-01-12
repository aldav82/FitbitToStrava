using DataBase.Entities;
using DataBase.Enums;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Repository
{

    public class ConfigRepository : Repository<Config>, IConfigRepository
    {
        public ConfigRepository(AppDbContext context) : base(context)
        {
        }

        public string this[string key] {
            get  {
                var configs = _context.Set<Config>();
                var entry = configs.FirstOrDefault(c=>c.Name == key);
                return entry?.Data;
            } set {
                var configs = _context.Set<Config>();
                var entry = configs.FirstOrDefault(c => c.Name == key);
                if (entry != null)
                {
                    entry.Data = value;
                    _context.Update(entry);
                } else
                {
                    _context.Add(new Config { Name = key, Data = value });  
                }
                _context.SaveChanges();
            } 
        }

        public string this[ConfigValues key] { 
            get => this[key.ToString()]; 
            set => this[key.ToString()] = value; 
        }
    }
}
