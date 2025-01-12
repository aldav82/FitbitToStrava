namespace DataBase.Entities
{
    public class Config
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; } = string.Empty; // Required
        public string Data { get; set; } = string.Empty; // Required
    }
}
