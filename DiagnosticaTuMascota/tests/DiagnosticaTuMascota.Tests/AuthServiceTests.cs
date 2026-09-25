using DiagnosticaTuMascota.Core;
using Xunit;

namespace DiagnosticaTuMascota.Tests;

public class AuthServiceTests
{
    private static List<UserAccount> Users() => new()
    {
        new UserAccount { Name = "Ana", Email = "ana@ejemplo.com", Phone = "+1 234 567 890", Password = "clave1234" }
    };

    [Fact]
    public void IsValidCredentials_WithCorrectData_ReturnsTrue()
    {
        Assert.True(AuthService.IsValidCredentials(Users(), "ana@ejemplo.com", "clave1234"));
    }

    [Fact]
    public void IsValidCredentials_WrongPassword_ReturnsFalse()
    {
        Assert.False(AuthService.IsValidCredentials(Users(), "ana@ejemplo.com", "incorrecta"));
    }

    [Fact]
    public void IsValidCredentials_UnknownEmail_ReturnsFalse()
    {
        Assert.False(AuthService.IsValidCredentials(Users(), "nadie@ejemplo.com", "clave1234"));
    }

    [Fact]
    public void IsValidCredentials_EmailIsCaseInsensitive()
    {
        Assert.True(AuthService.IsValidCredentials(Users(), "ANA@EJEMPLO.COM", "clave1234"));
    }

    [Fact]
    public void LoginError_CorrectCredentials_ReturnsNone()
    {
        Assert.Equal(AuthService.LoginFailure.None, AuthService.LoginError(Users(), "ana@ejemplo.com", "clave1234"));
    }

    [Fact]
    public void LoginError_NoAccounts_ReturnsNoAccounts()
    {
        Assert.Equal(AuthService.LoginFailure.NoAccounts, AuthService.LoginError(new List<UserAccount>(), "ana@ejemplo.com", "clave1234"));
    }

    [Fact]
    public void LoginError_NoUsersCollection_ReturnsNoAccounts()
    {
        Assert.Equal(AuthService.LoginFailure.NoAccounts, AuthService.LoginError(null!, "ana@ejemplo.com", "clave1234"));
    }

    [Fact]
    public void LoginError_EmptyEmail_ReturnsEmptyEmail()
    {
        Assert.Equal(AuthService.LoginFailure.EmptyEmail, AuthService.LoginError(Users(), "   ", "clave1234"));
    }

    [Fact]
    public void LoginError_EmptyPassword_ReturnsEmptyPassword()
    {
        Assert.Equal(AuthService.LoginFailure.EmptyPassword, AuthService.LoginError(Users(), "ana@ejemplo.com", ""));
    }

    [Fact]
    public void LoginError_WrongPassword_ReturnsInvalidCredentials()
    {
        Assert.Equal(AuthService.LoginFailure.InvalidCredentials, AuthService.LoginError(Users(), "ana@ejemplo.com", "incorrecta"));
    }

    [Fact]
    public void LoginError_UnknownEmail_ReturnsInvalidCredentials()
    {
        Assert.Equal(AuthService.LoginFailure.InvalidCredentials, AuthService.LoginError(Users(), "nadie@ejemplo.com", "clave1234"));
    }

    [Fact]
    public void IsEmailRegistered_ExistingEmail_ReturnsTrue()
    {
        Assert.True(AuthService.IsEmailRegistered(Users(), "ana@ejemplo.com"));
    }

    [Fact]
    public void IsEmailRegistered_NewEmail_ReturnsFalse()
    {
        Assert.False(AuthService.IsEmailRegistered(Users(), "nueva@ejemplo.com"));
    }

    [Fact]
    public void VerifyIdentity_WithMatchingPhone_ReturnsTrue()
    {
        Assert.True(AuthService.VerifyIdentity(Users(), "ana@ejemplo.com", "+1 234 567 890"));
    }

    [Fact]
    public void VerifyIdentity_WrongPhone_ReturnsFalse()
    {
        Assert.False(AuthService.VerifyIdentity(Users(), "ana@ejemplo.com", "999 999 999"));
    }

    [Theory]
    [InlineData("usuario@dominio.com", true)]
    [InlineData("correo-sin-arroba", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidEmail_Variants(string? email, bool expected)
    {
        Assert.Equal(expected, AuthService.IsValidEmail(email));
    }

    [Theory]
    [InlineData("+1 234 567 890", true)]
    [InlineData("12345", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidPhone_Variants(string? phone, bool expected)
    {
        Assert.Equal(expected, AuthService.IsValidPhone(phone));
    }

    [Theory]
    [InlineData("clave1234", true)]
    [InlineData("123", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidPassword_Variants(string? password, bool expected)
    {
        Assert.Equal(expected, AuthService.IsValidPassword(password));
    }

    [Fact]
    public void PasswordsMatch_EqualStrings_ReturnsTrue()
    {
        Assert.True(AuthService.PasswordsMatch("abc123", "abc123"));
    }

    [Fact]
    public void PasswordsMatch_DifferentStrings_ReturnsFalse()
    {
        Assert.False(AuthService.PasswordsMatch("abc123", "abc456"));
    }

    [Fact]
    public void FindByEmail_ExistingUser_ReturnsUser()
    {
        var user = AuthService.FindByEmail(Users(), "ana@ejemplo.com");

        Assert.NotNull(user);
        Assert.Equal("Ana", user!.Name);
    }

    [Fact]
    public void FindByEmail_UnknownUser_ReturnsNull()
    {
        Assert.Null(AuthService.FindByEmail(Users(), "desconocido@ejemplo.com"));
    }
}