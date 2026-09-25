namespace DiagnosticaTuMascota.Core;

/// <summary>
/// Validaciones de autenticación: login, registro y recuperación de contraseña
/// (flujo de 3 pasos igual que la versión web).
/// </summary>
public static class AuthService
{
    /// <summary>Compara dos contraseñas (la validación de coincidencia del registro).</summary>
    public static bool PasswordsMatch(string a, string b) => string.Equals(a, b, StringComparison.Ordinal);

    /// <summary>Indica si ya existe una cuenta con ese correo (case-insensitive).</summary>
    public static bool IsEmailRegistered(IEnumerable<UserAccount> users, string email) =>
        users.Any(u => string.Equals(u.Email, email?.Trim(), StringComparison.OrdinalIgnoreCase));

    /// <summary>Valida credenciales completas (correo + contraseña).</summary>
    public static bool IsValidCredentials(IEnumerable<UserAccount> users, string email, string password) =>
        users.Any(u => string.Equals(u.Email, email?.Trim(), StringComparison.OrdinalIgnoreCase)
                       && string.Equals(u.Password, password, StringComparison.Ordinal));

    /// <summary>Busca un usuario por correo (case-insensitive).</summary>
    public static UserAccount? FindByEmail(IEnumerable<UserAccount> users, string email) =>
        users.FirstOrDefault(u => string.Equals(u.Email, email?.Trim(), StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Valida el inicio de sesión y devuelve el motivo del fallo (o
    /// <see cref="LoginFailure.None"/> si las credenciales son correctas).
    /// Mantiene la lógica testeable sin UI y permite mensajes claros en la pantalla.
    /// </summary>
    public static LoginFailure LoginError(IEnumerable<UserAccount> users, string? email, string? password)
    {
        if (users is null || !users.Any()) return LoginFailure.NoAccounts;
        if (string.IsNullOrWhiteSpace(email)) return LoginFailure.EmptyEmail;
        if (string.IsNullOrEmpty(password)) return LoginFailure.EmptyPassword;
        return IsValidCredentials(users, email, password) ? LoginFailure.None : LoginFailure.InvalidCredentials;
    }

    /// <summary>Verifica identidad por correo + teléfono (paso 2 de recuperación).</summary>
    public static bool VerifyIdentity(IEnumerable<UserAccount> users, string email, string phone)
    {
        var user = FindByEmail(users, email);
        return user is not null && string.Equals(user.Phone, phone?.Trim(), StringComparison.Ordinal);
    }

    /// <summary>Validación básica de formato de correo.</summary>
    public static bool IsValidEmail(string? email) =>
        !string.IsNullOrWhiteSpace(email)
        && email.Contains('@')
        && email.Contains('.')
        && email.IndexOf('@') < email.LastIndexOf('.');

    /// <summary>Validación básica de teléfono: al menos 7 caracteres con algún dígito.</summary>
    public static bool IsValidPhone(string? phone) =>
        !string.IsNullOrWhiteSpace(phone)
        && phone.Replace(" ", "").Length >= 7
        && phone.Any(char.IsDigit);

    /// <summary>Validación de contraseña: al menos 4 caracteres.</summary>
    public static bool IsValidPassword(string? password) =>
        !string.IsNullOrWhiteSpace(password) && password.Length >= 4;

    /// <summary>Motivos de fallo al iniciar sesión.</summary>
    public enum LoginFailure
    {
        /// <summary>Credenciales válidas: el login debe proceder.</summary>
        None,

        /// <summary>No existe ninguna cuenta registrada.</summary>
        NoAccounts,

        /// <summary>Falta el correo electrónico.</summary>
        EmptyEmail,

        /// <summary>Falta la contraseña.</summary>
        EmptyPassword,

        /// <summary>El correo o la contraseña no coinciden con una cuenta.</summary>
        InvalidCredentials
    }
}