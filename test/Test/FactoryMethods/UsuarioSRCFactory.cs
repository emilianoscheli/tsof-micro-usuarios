namespace UsuariosAPI.Test.FactoryMethods
{
    using UsuariosAPI.Domain.Rol;
    using UsuariosAPI.Domain.Usuario;
    using UsuariosAPI.Domain.UsuarioRol;
    using UsuariosAPI.Domain.UsuarioSociedad;
    using UsuariosAPI.SDK.Contracts.DTOs.Contratista;
    using UsuariosAPI.SDK.Contracts.DTOs.EstudioAuditor;
    using UsuariosAPI.SDK.Contracts.DTOs.Sede;
    using UsuariosAPI.SDK.Contracts.DTOs.Sociedad;
    using UsuariosAPI.SDK.Contracts.DTOs.Usuario;
    using UsuariosAPI.SDK.Contracts.DTOs.UsuarioRol;
    using UsuariosAPI.SDK.Contracts.DTOs.UsuarioSociedad;

    public static class UsuarioSRCFactory
    {
        public static ContratistaUsuarioDTO CreateContratistaDTO(int id = 1, string nombre = "Contratista 1")
        {
            return new ContratistaUsuarioDTO
            {
                Id = id,
                RazonSocial = nombre,
            };
        }

        public static EstudioAuditorUsuarioDTO CreateEstudioAuditorDTO(int id = 1, string nombre = "estudio")
        {
            return new EstudioAuditorUsuarioDTO
            {
                Id = id,
                Nombre = nombre,
            };
        }

        public static SedeUsuarioDTO CreateSedeDTO(int id = 1, string nombre = "sede")
        {
            return new SedeUsuarioDTO
            {
                Id = id,
                Nombre = nombre,
            };
        }

        public static SociedadUsuarioDTO CreateSociedadDTO(int id = 1, string nombre = "sociedad")
        {
            return new SociedadUsuarioDTO
            {
                Id = id,
                Nombre = nombre,
            };
        }

        public static Rol CreateRol(int id = 1, string nombre = "Rol 1")
        {
            return new Rol
            {
                Id = id,
                Nombre = nombre,
                Descripcion = string.Empty,
                AsignaSociedad = true,
                EsManual = true,
            };
        }

        public static UsuarioRol CreateUsuarioRol(int id = 1, int usuario = 1, int rol = 1)
        {
            return new UsuarioRol
            {
                Id = id,
                UsuarioSRCId = usuario,
                UsuarioSRC = Create(id: usuario),
                RolId = rol,
                Rol = CreateRol(id: rol),
            };
        }

        public static UsuarioSociedad CreateUsuarioSociedad(int id = 1, int usuario = 1, int sociedad = 1)
        {
            return new UsuarioSociedad
            {
                Id = id,
                UsuarioSRCId = usuario,
                UsuarioSRC = Create(id = usuario),
                SociedadFkId = sociedad,
            };
        }

        public static UsuarioSRC Create(int id = 1, string nombreApellido = "Maria Lopez", string idRed = "SE12345", string correo = "maria@mail.com")
        {
            return new UsuarioSRC
            {
                Id = id,
                NombreCompleto = nombreApellido,
                CorreoElectronico = correo,
                IdRed = idRed,
            };
        }

        public static UsuarioSRC CreateAllData(int id = 1, string nombreApellido = "Maria Lopez", string idRed = "SE12345", string correo = "maria@mail.com", int? estudioAuditorId = 1, int? sedeId = 1, int? contratistaId = 1)
        {
            var usuario = new UsuarioSRC
            {
                Id = id,
                NombreCompleto = nombreApellido,
                CorreoElectronico = correo,
                IdRed = idRed,
                Roles = [CreateUsuarioRol()],
                Sociedades = [CreateUsuarioSociedad()],
                EstudioAuditorFkId = estudioAuditorId,
                ContratistaFkId = contratistaId,
                SedeFkId = sedeId,
            };

            return usuario;
        }

        public static UsuarioDTO CreateUsuarioDetalleDTO(
            int id = 1,
            string nombreApellido = "Maria Lopez",
            string idRed = "SE12345",
            string correo = "maria@mail.com",
            int estudioAuditorId = 1,
            int sedeId = 1,
            int contratistaId = 1,
            string nombreEstudio = "estudio",
            string nombreSede = "sede",
            string nombreContratista = "contratista",
            string nombreSociedad = "sociedad")
        {
            var usuarioSRC = CreateAllData(id, nombreApellido, idRed, correo, estudioAuditorId, sedeId, contratistaId);

            return new UsuarioDTO
            {
                Codigo = usuarioSRC.Id,
                NombreCompleto = usuarioSRC.NombreCompleto,
                CorreoElectronico = usuarioSRC.CorreoElectronico,
                IdRed = usuarioSRC.IdRed,
                Roles = string.Join(',', usuarioSRC.Roles!.Select(x => $"{x.Rol!.Nombre} {(x.Rol.EsManual ? "- M" : "- A")}")),
                Sociedades = nombreSociedad,
                RolesList = usuarioSRC.Roles!.Select(x => new UsuariosRolesDTO
                {
                    Rol = new RolDetailsDTO
                    {
                        Nombre = x.Rol!.Nombre,
                        Descripcion = x.Rol.Descripcion,
                        AsignaSociedad = x.Rol.AsignaSociedad,
                        EsManual = x.Rol.EsManual,
                    },
                    UsuarioSRCId = x.UsuarioSRCId,
                }).ToList(),
                SociedadesList = usuarioSRC.Sociedades!.Select(x => new UsuarioSociedadDTO
                {
                    Sociedad = new SociedadUsuarioDTO
                    {
                        Nombre = nombreSociedad,
                    },
                    UsuarioSRCId = x.UsuarioSRCId,
                }).ToList(),
                EstudioAuditor = new EstudioAuditorUsuarioDTO
                {
                    Id = 1,
                    Nombre = nombreEstudio,
                },
                Sede = new SedeUsuarioDTO
                {
                    Id = 1,
                    Nombre = nombreSede,
                },
                Contratista = new ContratistaUsuarioDTO
                {
                    Id = 1,
                    RazonSocial = nombreContratista,
                },
            };
        }
    }
}
