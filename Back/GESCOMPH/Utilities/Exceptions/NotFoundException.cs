namespace Utilities.Exceptions
{
    public class NotFoundException : Exception
    {
        public string EntityName { get; }

        public NotFoundException(string entityName, string message)
            : base(message)
        {
            EntityName = entityName;
        }
    }

}
