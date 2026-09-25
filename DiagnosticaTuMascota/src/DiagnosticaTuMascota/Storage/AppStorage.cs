using System.Text.Json;
using DiagnosticaTuMascota.Core;

namespace DiagnosticaTuMascota.Storage;

/// <summary>Abstracción de un almacén clave→valor persistente en JSON.</summary>
public interface IJsonStore
{
    T? Load<T>(string key);
    void Save<T>(string key, T value);
    void Delete(string key);
    bool Exists(string key);
}

/// <summary>Implementación de <see cref="IJsonStore"/> con archivos .json en un directorio.</summary>
public sealed class JsonFileStore : IJsonStore
{
    private readonly string _directory;
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonFileStore(string directory)
    {
        _directory = directory;
        Directory.CreateDirectory(directory);
    }

    private string PathFor(string key) => Path.Combine(_directory, Sanitize(key) + ".json");

    private static string Sanitize(string key)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return new string(key.Select(c => invalid.Contains(c) ? '_' : c).ToArray());
    }

    public bool Exists(string key) => File.Exists(PathFor(key));

    public T? Load<T>(string key)
    {
        try
        {
            var path = PathFor(key);
            if (!File.Exists(path)) return default;
            return JsonSerializer.Deserialize<T>(File.ReadAllText(path), Options);
        }
        catch
        {
            return default;
        }
    }

    public void Save<T>(string key, T value)
    {
        try
        {
            File.WriteAllText(PathFor(key), JsonSerializer.Serialize(value, Options));
        }
        catch
        {
            // Ignorar errores de escritura (misma estrategia que localStorage).
        }
    }

    public void Delete(string key)
    {
        try
        {
            var path = PathFor(key);
            if (File.Exists(path)) File.Delete(path);
        }
        catch
        {
            // Ignorar.
        }
    }
}

/// <summary>
/// Almacenamiento de la aplicación con "sandbox por usuario": las claves del
/// sistema (users, currentUser) son globales; el resto se prefijan con el
/// email del usuario logueado, igual que el scoping de la versión web.
/// </summary>
public sealed class AppStorage
{
    private static readonly IReadOnlySet<string> SystemKeys = new HashSet<string> { "users", "currentUser", "uiSettings" };
    private readonly IJsonStore _store;
    private string? _currentUserEmail;

    public AppStorage(IJsonStore store)
    {
        _store = store;
        _currentUserEmail = GetCurrentUser()?.Email;
    }

    public static AppStorage CreateDefault() => new(
        new JsonFileStore(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "DiagnosticaTuMascota")));

    // ---- Autenticación ----

    public UserAccount? GetCurrentUser() => _store.Load<UserAccount>("currentUser");

    public void SetCurrentUser(UserAccount? user)
    {
        if (user is null)
        {
            _store.Delete("currentUser");
            _currentUserEmail = null;
        }
        else
        {
            _store.Save("currentUser", user);
            _currentUserEmail = user.Email;
        }
    }

    public List<UserAccount> GetUsers() => _store.Load<List<UserAccount>>("users") ?? new List<UserAccount>();

    public void SaveUsers(List<UserAccount> users) => _store.Save("users", users);

    // ---- Datos con sandbox por usuario ----

    private string Scope(string key)
    {
        if (SystemKeys.Contains(key)) return key;
        return string.IsNullOrEmpty(_currentUserEmail) ? key : $"{key}_{SanitizeEmail(_currentUserEmail)}";
    }

    private static string SanitizeEmail(string email) => email.Trim().Replace('@', '_').Replace('.', '_');

    public T GetData<T>(string key, T defaultValue)
    {
        var scoped = Scope(key);
        var data = _store.Load<T>(scoped);
        return data is null ? defaultValue : data;
    }

    public void SetData<T>(string key, T value) => _store.Save(Scope(key), value);

    /// <summary>Email usado actualmente para el sandbox (útil en tests).</summary>
    public string? CurrentUserEmail => _currentUserEmail;
}