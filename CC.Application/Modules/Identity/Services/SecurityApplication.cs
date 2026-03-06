using CC.Application.Common.Bases;
using CC.Application.Common.Helpers;
using CC.Application.DTOs.Security;
using CC.Application.Modules.Identity.Interfaces;
using CC.Domain.Entities;
using CC.Domain.Exceptions;
using CC.Domain.Repositories;

namespace CC.Application.Modules.Identity.Services
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
            // 1. Buscamos el rol cargando explícitamente sus permisos actuales
            var roles = await _unitOfWork.Roles.GetAsync(
                filter: x => x.Id == (request.Id ?? Guid.Empty),
                includeProperties: "RolePermissions"
            );

            var role = roles.FirstOrDefault();

            if (role == null)
            {
                // ===========================
                // CASO: CREAR NUEVO
                // ===========================
                var nameUpper = request.Name.ToUpper().Trim();

                var exists = await _unitOfWork.Roles.GetAsync(x => x.Name == nameUpper);
                if (exists.Any())
                    return _serviceData.CreateResponse(Guid.Empty, "El nombre del rol ya existe.");

                var newRole = new Role(nameUpper, request.ShowName, request.Description ?? string.Empty);

                if (request.PermissionIds != null && request.PermissionIds.Any())
                {
                    foreach (var pId in request.PermissionIds.Distinct())
                    {
                        newRole.RolePermissions.Add(new RolePermission(newRole.Id, pId));
                    }
                }

                await _unitOfWork.Roles.AddAsync(newRole);
                await _unitOfWork.SaveChangesAsync();

                return _serviceData.CreateResponse(newRole.Id, "Rol creado exitosamente.");
            }
            else
            {
                // 1. Actualizar detalles (ShowName, etc.)
                role.UpdateDetails(request.ShowName, request.Description ?? string.Empty);

                // 2. LIMPIEZA DE SEGURIDAD
                // Aunque creas que está vacío, esto asegura que el rastreador de EF 
                // no tenga "fantasmas" de la consulta inicial.
                role.RolePermissions.Clear();

                // 3. ASIGNACIÓN ÚNICA
                if (request.PermissionIds != null && request.PermissionIds.Any())
                {
                    // El .Distinct() evita que si el JSON trae [5, 6, 5], EF intente insertar el '5' dos veces.
                    var uniquePermissionIds = request.PermissionIds.Distinct().ToList();

                    foreach (var pId in uniquePermissionIds)
                    {
                        // Creamos la relación directamente
                        var assignment = new RolePermission(role.Id, pId);
                        role.RolePermissions.Add(assignment);
                    }
                }

                // 4. EL CAMBIO VITAL:
                // NO uses _unitOfWork.Roles.UpdateAsync(role).
                // Al haber hecho el GetAsync al principio, EF ya sabe que 'role' existe. 
                // Solo llama al Save directamente.
                await _unitOfWork.SaveChangesAsync();

                return _serviceData.CreateResponse(role.Id, "Rol actualizado exitosamente.");
            }
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
            if (request.Id.HasValue && request.Id.Value > 0)
            {
                // 1. Buscamos el registro (Asegúrate de que GetAsync devuelva algo que puedas actualizar)
                var features = await _unitOfWork.Features.GetAsync(f => f.Id == request.Id.Value && !f.IsDeleted);
                var featureToUpdate = features.FirstOrDefault();

                if (featureToUpdate == null)
                    throw new DomainException("FEATURE_NOT_FOUND", "Error", "No se encontró el módulo para actualizar.");

                // 2. Aplicamos los cambios usando el método de tu Entidad
                featureToUpdate.UpdateDetails(request.ShowName, request.Path, request.Icon);

                // 3. ¡ESTA ES LA PARTE QUE FALTABA! Notificar cambios y Guardar
                await _unitOfWork.Features.UpdateAsync(featureToUpdate); // Opcional dependiendo de tu repo, pero recomendado
                await _unitOfWork.SaveChangesAsync(); // <--- SIN ESTO NO SE GUARDA NADA

                return _serviceData.CreateResponse(featureToUpdate.Id, "Módulo actualizado correctamente.");
            }
            else
            {
                // MODO CREACIÓN
                var nameUpper = request.Name.ToUpper().Trim();
                var exists = await _unitOfWork.Features.GetAsync(f => f.Name == nameUpper && !f.IsDeleted);

                if (exists.Any())
                    throw new DomainException("FEATURE_EXISTS", "Error", "Ya existe un módulo con el nombre especificado.");

                var newFeature = new Feature(nameUpper, request.ShowName, request.Path, request.Icon);

                await _unitOfWork.Features.AddAsync(newFeature);
                await _unitOfWork.SaveChangesAsync();

                return _serviceData.CreateResponse(newFeature.Id, "Módulo creado correctamente.");
            }
        }

        public async Task<BaseResponse<int>> CreatePermissionAsync(PermissionDto request)
        {
            int resultId;

            if (request.Id.HasValue && request.Id.Value > 0)
            {
                // --- MODO EDICIÓN ---
                var existingPermission = await _unitOfWork.Permissions.GetByIdAsync(request.Id.Value);

                if (existingPermission == null)
                    throw new DomainException("NOT_FOUND", "No encontrado", $"El permiso con ID {request.Id} no existe.");

                // Usamos el método Update de tu clase de dominio (esto valida las reglas de negocio)
                existingPermission.Update(request.Name, request.ShowName);

                // EF Core detecta los cambios automáticamente al estar trackeado
                await _unitOfWork.Permissions.UpdateAsync(existingPermission);
                resultId = existingPermission.Id;
            }
            else
            {
                // --- MODO CREACIÓN ---
                var newPermission = new Permission(request.Name, request.ShowName, request.FeatureId);

                await _unitOfWork.Permissions.AddAsync(newPermission);

                // Guardamos aquí para que Postgres genere el ID y lo asigne a newPermission.Id
                await _unitOfWork.SaveChangesAsync();
                resultId = newPermission.Id;
            }

            // Guardar cambios finales (para el caso de Update)
            await _unitOfWork.SaveChangesAsync();

            return _serviceData.CreateResponse(resultId, "Operación realizada con éxito.");
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
                r.Description,
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
                role.Description,
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
