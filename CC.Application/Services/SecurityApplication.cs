using CC.Application.Common.Bases;
using CC.Application.Common.Helpers;
using CC.Application.DTOs.Security;
using CC.Application.Interfaces;
using CC.Domain.Entities;
using CC.Domain.Exceptions;
using CC.Domain.Repositories;

namespace CC.Application.Services
{
    public class SecurityApplication : ISecurityApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ServiceData _serviceData;

        public SecurityApplication(IUnitOfWork unitOfWork, ServiceData serviceData)
        {
            _unitOfWork = unitOfWork;
            _serviceData = serviceData;
        }

        // --- GESTIÓN DE ROLES ---
        public async Task<BaseResponse<Guid>> CreateRoleAsync(RoleDto request)
        {
            // El rol usa Guid y tiene name/showName
            var newRole = new Role(request.Name.ToUpper(), request.ShowName);

            if (request.PermissionIds != null && request.PermissionIds.Any())
            {
                foreach (var pId in request.PermissionIds)
                {
                    // Usamos tu constructor con lógica de validación
                    var assignment = new RolePermission(newRole.Id, pId);
                    newRole.RolePermissions.Add(assignment);
                }
            }

            await _unitOfWork.Roles.AddAsync(newRole);
            await _unitOfWork.SaveChangesAsync();
            return _serviceData.CreateResponse(newRole.Id, "Rol creado exitosamente.");
        }

        // --- ASIGNACIÓN DE PERMISOS (Sincronización) ---
        public async Task<BaseResponse<bool>> AssignPermissionsToRoleAsync(Guid roleId, List<int> permissionIds)
        {
            var role = (await _unitOfWork.Roles.GetAsync(
                filter: r => r.Id == roleId,
                includeProperties: "RolePermissions"
            )).FirstOrDefault();

            if (role == null)
                throw new DomainException("ROLE_NOT_FOUND", "No encontrado", "El rol no existe.");

            // 1. Limpiamos las asignaciones actuales
            role.RolePermissions.Clear();

            // 2. Agregamos las nuevas usando tu constructor validado
            foreach (var pId in permissionIds)
            {
                role.RolePermissions.Add(new RolePermission(roleId, pId));
            }

            await _unitOfWork.Roles.UpdateAsync(role);
            await _unitOfWork.SaveChangesAsync();

            return _serviceData.CreateResponse(true, "Permisos sincronizados correctamente.");
        }

        // --- GESTIÓN DE FEATURES ---
        public async Task<BaseResponse<int>> CreateFeatureAsync(FeatureDto request)
        {
            var feature = new Feature(request.Name, request.ShowName, request.Path, request.Icon);
            await _unitOfWork.Features.AddAsync(feature);
            await _unitOfWork.SaveChangesAsync();
            return _serviceData.CreateResponse(feature.Id, "Módulo creado.");
        }
        public async Task<BaseResponse<int>> CreatePermissionAsync(PermissionDto request) 
        {
            var featureExists = await _unitOfWork.Features.AnyAsync(f => f.Id == request.FeatureId && !f.IsDeleted);
            if (!featureExists)
                throw new DomainException("FEATURE_NOT_FOUND", "Error", "La feature especificada no existe.");

            // 2. Crear la entidad Permiso (usando tu regla de name y showName)
            // El constructor de tu entidad Permission debería recibir estos datos
            var newPermission = new Permission(
                request.Name.ToUpper().Trim(),
                request.ShowName,
                request.FeatureId
            );

            // 3. Persistencia
            await _unitOfWork.Permissions.AddAsync(newPermission);
            await _unitOfWork.SaveChangesAsync();

            return _serviceData.CreateResponse(newPermission.Id, "Permiso creado correctamente.");
        }

        public async Task<BaseResponse<IEnumerable<RoleDto>>> GetAllRolesAsync()
        {
            var roles = await _unitOfWork.Roles.GetAsync(
                filter: r => !r.IsDeleted,
                includeProperties: "RolePermissions"
            );

            var response = roles.Select(r => new RoleDto(
                r.Id,
                r.Name,
                r.ShowName,
                r.RolePermissions.Select(rp => rp.PermissionId).ToList()
            ));

            return _serviceData.CreateResponse(response, "Roles recuperados correctamente.");
        }

        public async Task<BaseResponse<RoleDto>> GetRoleByIdAsync(Guid id)
        {
            var role = (await _unitOfWork.Roles.GetAsync(
                filter: r => r.Id == id && !r.IsDeleted,
                includeProperties: "RolePermissions"
            )).FirstOrDefault();

            if (role == null)
                throw new DomainException("ROLE_NOT_FOUND", "No encontrado", "El rol solicitado no existe.");

            var dto = new RoleDto(
                role.Id,
                role.Name,
                role.ShowName,
                role.RolePermissions.Select(rp => rp.PermissionId).ToList()
            );

            return _serviceData.CreateResponse(dto, "Rol encontrado.");
        }

        public async Task<BaseResponse<IEnumerable<FeatureDto>>> GetAllFeaturesAsync()
        {
            var features = await _unitOfWork.Features.GetAsync(filter: f => !f.IsDeleted);

            var response = features.Select(f => new FeatureDto(
                f.Id,
                f.Name,
                f.ShowName,
                f.Path,
                f.Icon
            ));

            return _serviceData.CreateResponse(response, "Módulos (Features) recuperados.");
        }

        public async Task<BaseResponse<IEnumerable<PermissionDto>>> GetAllPermissionsAsync()
        {
            var permissions = await _unitOfWork.Permissions.GetAsync(filter: p => !p.IsDeleted);

            var response = permissions.Select(p => new PermissionDto(
                p.Id,
                p.Name,
                p.ShowName,
                p.FeatureId
            ));

            return _serviceData.CreateResponse(response, "Catálogo de permisos recuperado.");
        }
    }
}
