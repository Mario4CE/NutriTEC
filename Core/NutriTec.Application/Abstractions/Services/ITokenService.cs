using NutriTec.Application.Autenticacion;

namespace NutriTec.Application.Abstractions.Services;

public interface ITokenService
{
    TokenAutenticacion GenerarToken(UsuarioTokenAutenticacion usuario);
}
