namespace CC.Domain.Common
{
    public abstract class BaseEntity<TKey>
    {
        public TKey? Id { get; protected set; }
        public Guid  AuditCreateUser { get; protected set; }
        public DateTime AuditCreateDate { get; protected set; }
        public Guid? AuditUpdateUser { get; protected set; }
        public DateTime? AuditUpdateDate { get; protected set; }
        public Guid? AuditDeleteUser { get; protected set; }
        public DateTime? AuditDeleteDate { get; protected set; }
        public bool IsDeleted { get; protected set; }

        #region Reglas de negocio comunes
        public void MarkAsDeleted(Guid userId)
        {
            IsDeleted = true;
            AuditDeleteUser = userId;
            AuditDeleteDate = DateTime.UtcNow;
        }

        public void MarkAsUpdated(Guid userId)
        {
            AuditUpdateUser = userId;
            AuditUpdateDate = DateTime.UtcNow;
        }

        public void MarkAsCreated(Guid userId)
        {
            AuditCreateUser = userId;
            AuditCreateDate = DateTime.UtcNow;
            IsDeleted = false;
        }
        #endregion
    }
}
