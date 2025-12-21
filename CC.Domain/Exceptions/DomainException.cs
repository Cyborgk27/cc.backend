namespace CC.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public string Name { get; }
        public string ShowName { get; }

        public DomainException(string name, string showName, string message)
            : base(message)
        {
            Name = name;
            ShowName = showName;
        }

        // Sobrecarga opcional por si no quieres pasar un mensaje técnico distinto al showName
        public DomainException(string name, string showName)
            : base(showName)
        {
            Name = name;
            ShowName = showName;
        }
    }
}