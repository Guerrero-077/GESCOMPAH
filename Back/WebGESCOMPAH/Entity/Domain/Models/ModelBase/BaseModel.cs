namespace Entity.Domain.Models.ModelBase
{
    public abstract class BaseModel
    {
        public int Id { get; set; }
        public bool Active { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
    