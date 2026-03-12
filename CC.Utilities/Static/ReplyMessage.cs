namespace CC.Utilities.Static
{
    public class ReplyMessage
    {
        // --- Mensajes de Éxito ---
        public const string MESSAGE_QUERY = "Consulta exitosa.";
        public const string MESSAGE_QUERY_EMPTY = "No se encontraron registros.";
        public const string MESSAGE_SAVE = "Se registró correctamente.";
        public const string MESSAGE_UPDATE = "Se actualizó correctamente.";
        public const string MESSAGE_DELETE = "Se eliminó correctamente.";
        public const string MESSAGE_ACTIVATE = "El registro ha sido activado.";
        public const string MESSAGE_TOKEN = "Token generado correctamente.";

        // --- Mensajes de Error (Para UserFriendlyException) ---
        public const string MESSAGE_EXISTS = "El registro ya existe.";
        public const string MESSAGE_NOT_FOUND = "El recurso solicitado no existe.";
        public const string MESSAGE_VALIDATE = "Existen errores de validación en los datos enviados.";
        public const string MESSAGE_FAILED = "La operación no pudo completarse.";
        public const string MESSAGE_UNAUTHORIZED = "No tienes permisos para realizar esta acción.";
        public const string MESSAGE_FORBIDDEN = "Acceso denegado al recurso.";
        public const string MESSAGE_INTERNAL_ERROR = "Ocurrió un error inesperado en el servidor.";

        // -- Mensajes de autenticación ---
        public const string MESSAGE_AUTH_SUCCESS = "Autenticación exitosa.";
        public const string MESSAGE_AUTH_FAILED = "Credenciales inválidas.";
        public const string MESSAGE_TOKEN_EXPIRED = "El token ha expirado.";
    }
}