using Microsoft.AspNetCore.Mvc;

namespace CC.Api.Middleware
{
    public class PermissionAttribute : TypeFilterAttribute
    {
        public PermissionAttribute(string feature, string action) : base(typeof(PermissionFilter))
        {
            Arguments = new object[] { feature, action };
        }
    }
}
