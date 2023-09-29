namespace DaemonsRunner.Domain.Exceptions.Base
{
    public class DomainException : Exception
    {
        public DomainException() { }

        public DomainException(string message) : base(message) { }
    }
}
