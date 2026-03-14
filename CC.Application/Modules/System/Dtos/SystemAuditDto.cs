namespace CC.Application.Modules.System.Dtos
{
    public class SystemAuditDto
    {
        public Guid Id { get; set; }
        public string? UserEmail { get; set; }
        public string Operation { get; set; } = null!;
        public string? Module { get; set; }
        public string? Action { get; set; }
        public string Endpoint { get; set; } = null!;
        public int ResponseCode { get; set; }
        public int ExecutionTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UserIp { get; set; }
        public string? RequestData { get; set; }
    }
}
