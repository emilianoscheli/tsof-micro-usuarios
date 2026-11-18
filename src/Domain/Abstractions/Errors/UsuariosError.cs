namespace UsuariosAPI.Domain.Abstractions.Errors
{
    using APICore.Responses;

    public record UsuariosError : Error
    {
        public UsuariosError(string code, string description)
            : base(code, description)
        {
        }

        public static Error EntidadNoEncontrada(string entidad)
        {
            return new Error("USUARIOS.Entidad_No_Encontrada", string.Format("La entidad '{0}' no fue encontrada", entidad));
        }

        public static Error ReporteErrorValidacion(string descripcion)
        {
            return new Error("USUARIOS.Reporte_Error_Validacion", descripcion);
        }

        public static readonly Error RequestInvalido = new ("USUARIOS.Request_Invalido", "El request es inválido");

        public static Error DatoExistente(string dato)
        {
            return new Error("USUARIOS.Dato_Existente", string.Format("El dato '{0}' ya existe en el sistema", dato));
        }

        public static Error EntidadDesactivadaNoSePuedeUtilizar(string entidad)
        {
            return new Error("USUARIOS.Entidad_Desactivada_No_Se_Puede_Utilizar", string.Format("La entidad '{0}' está desactivada, por lo que no se puede utilizar", entidad));
        }

        public static Error UsuarioNoEncontrado(string idRed)
        {
            return new Error("USUARIOS.Usuario_No_Encontrado", string.Format("El usuario con id '{0}' no fue encontrado", idRed));
        }

        public static readonly Error ArchivoVacio = new ("USUARIOS.Archivo_Vacio", "Fallo la descarga. Inténtalo de nuevo más tarde.");

        public static Error OperacionNoPermitida(string operacion)
        {
            return new Error("USUARIOS.Operacion_No_Permitida", operacion);
        }

        // Errores específicos para usuarios externos
        public static readonly Error UsuarioNoEsExterno = new ("USUARIOS.Usuario_No_Es_Externo", "Esta operación solo puede realizarse sobre usuarios externos");

        public static readonly Error UsuarioEsExterno = new ("USUARIOS.Usuario_Es_Externo", "Esta operación no puede realizarse sobre usuarios externos");

        public static readonly Error UsuarioExternoYaActivo = new ("USUARIOS.Usuario_Externo_Ya_Activo", "El usuario externo ya se encuentra activo");

        public static readonly Error UsuarioExternoYaInactivo = new ("USUARIOS.Usuario_Externo_Ya_Inactivo", "El usuario externo ya se encuentra inactivo");

        public static readonly Error UsuarioExternoSoloUnRol = new ("USUARIOS.Usuario_Externo_Solo_Un_Rol", "Los usuarios externos solo pueden tener un rol asignado (Contratista o Estudio Auditor)");

        public static readonly Error UsuarioExternoRolInvalido = new ("USUARIOS.Usuario_Externo_Rol_Invalido", "Los usuarios externos solo pueden tener el rol Contratista o Estudio Auditor");

        public static readonly Error UsuarioExternoNoTieneSociedades = new ("USUARIOS.Usuario_Externo_No_Tiene_Sociedades", "Los usuarios externos no pueden tener sociedades asignadas");

        public static readonly Error UsuarioInternoDebeEstarActivo = new ("USUARIOS.Usuario_Interno_Debe_Estar_Activo", "Los usuarios internos deben estar siempre activos");

        public static readonly Error RolesIncompatibles = new ("USUARIOS.Usuario_Rol_Incompatible", "El usuario que se intenta ingresar no es compatible con el rol deseado");
    }
}
