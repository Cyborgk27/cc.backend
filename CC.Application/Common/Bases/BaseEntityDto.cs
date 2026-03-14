namespace CC.Application.Common.Bases
{
    public abstract class BaseEntityDto<TKey>
    {
        public TKey? Id { get; set; }
        public string AuditCreateUser { get; set; } = string.Empty;
        public DateTime AuditCreateDate { get; set; }
        public string? AuditUpdateUser { get; set; }
        public DateTime? AuditUpdateDate { get; set; }
        public string? AuditDeleteUser { get; set; }
        public DateTime? AuditDeleteDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
