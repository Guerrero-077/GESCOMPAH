using Entity.Domain.Models.ModelBase;

namespace Entity.Domain.Models.Implements.Utilities
{
    public class Notification : BaseModel
    {
        public enum TypeNotification
        {
            Information,
            Warning,
            Error,
            Critical
        }
        public string Message { get; set; } = null!;
    }
}
