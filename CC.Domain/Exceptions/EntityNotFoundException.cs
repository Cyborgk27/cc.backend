namespace CC.Domain.Exceptions
{
    public class EntityNotFoundException : DomainException
    {
        // Esta excepción hereda de tu DomainException base
        public EntityNotFoundException(string entityName, object key)
            : base(
                "NOT_FOUND",
                "Recurso no encontrado",
                $"La entidad '{entityName}' con el ID '{key}' no existe o fue eliminada.")
        {
        }
    }
}
