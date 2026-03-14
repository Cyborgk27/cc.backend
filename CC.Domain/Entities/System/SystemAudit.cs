using CC.Domain.Common;

namespace CC.Domain.Entities.System
{
    public class SystemAudit : BaseEntity<Guid>
    {

        // Quién realizó la acción
        public Guid? UserId { get; set; }
        public string? UserEmail { get; set; }
        public string? UserIp { get; set; }

        // Qué y Dónde
        public string Operation { get; set; } = null!;
        public string? Module { get; set; }
        public string? Action { get; set; }
        public string Endpoint { get; set; } = null!;

        // Datos y Rendimiento
        public string? RequestData { get; set; }
        public int ResponseCode { get; set; }
        public int ExecutionTime { get; set; }

        // Cuándo
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Extra: Por si hubo error, guardamos una referencia rápida
        public string? ErrorMessage { get; set; }
    }
}
