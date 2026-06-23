namespace NutriTec.Application.Abstractions.Services;

public interface IPasswordHasher
{
    string GenerarHash(string contrasena);

    bool Verificar(string contrasena, string hashAlmacenado);
}
