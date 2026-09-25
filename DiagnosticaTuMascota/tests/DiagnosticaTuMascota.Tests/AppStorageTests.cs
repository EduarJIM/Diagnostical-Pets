using DiagnosticaTuMascota.Core;
using DiagnosticaTuMascota.Storage;
using Xunit;

namespace DiagnosticaTuMascota.Tests;

public class AppStorageTests : IDisposable
{
    private readonly string _tempDir;

    public AppStorageTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "DtmTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        try { Directory.Delete(_tempDir, recursive: true); } catch { }
    }

    private AppStorage NewStorage() => new(new JsonFileStore(_tempDir));

    [Fact]
    public void JsonFileStore_SaveAndLoad_Roundtrips()
    {
        var store = new JsonFileStore(_tempDir);
        var pet = new PetData { Id = 7, Name = "Rex", Breed = "Beagle" };

        store.Save("pet", pet);
        var loaded = store.Load<PetData>("pet");

        Assert.NotNull(loaded);
        Assert.Equal(7, loaded!.Id);
        Assert.Equal("Rex", loaded.Name);
        Assert.Equal("Beagle", loaded.Breed);
    }

    [Fact]
    public void JsonFileStore_MissingKey_ReturnsDefault()
    {
        var store = new JsonFileStore(_tempDir);

        var loaded = store.Load<List<string>>("no-existe");

        Assert.Null(loaded);
    }

    [Fact]
    public void GetData_MissingKey_ReturnsDefaultValue()
    {
        var storage = NewStorage();

        var pets = storage.GetData("petsData", new List<PetData> { new() { Name = "Default" } });

        Assert.Single(pets);
    }

    [Fact]
    public void SetData_And_GetData_Roundtrips()
    {
        var storage = NewStorage();
        var pets = new List<PetData> { new() { Id = 1, Name = "Max" } };

        storage.SetData("petsData", pets);
        var loaded = storage.GetData("petsData", new List<PetData>());

        Assert.Single(loaded);
        Assert.Equal("Max", loaded[0].Name);
    }

    [Fact]
    public void Data_IsSandboxedPerUser()
    {
        var storage = NewStorage();
        storage.SetCurrentUser(new UserAccount { Email = "ana@ejemplo.com", Name = "Ana" });
        storage.SetData("petsData", new List<PetData> { new() { Name = "Mascota de Ana" } });

        storage.SetCurrentUser(new UserAccount { Email = "luis@ejemplo.com", Name = "Luis" });
        var luisPets = storage.GetData("petsData", new List<PetData>());

        Assert.Empty(luisPets);
    }

    [Fact]
    public void Data_SameKeyDifferentUsers_DoesNotCollide()
    {
        var storage = NewStorage();

        storage.SetCurrentUser(new UserAccount { Email = "ana@ejemplo.com", Name = "Ana" });
        storage.SetData("petsData", new List<PetData> { new() { Name = "Max" } });

        storage.SetCurrentUser(new UserAccount { Email = "luis@ejemplo.com", Name = "Luis" });
        storage.SetData("petsData", new List<PetData> { new() { Name = "Luna" } });

        storage.SetCurrentUser(new UserAccount { Email = "ana@ejemplo.com", Name = "Ana" });
        var anaPets = storage.GetData("petsData", new List<PetData>());

        Assert.Single(anaPets);
        Assert.Equal("Max", anaPets[0].Name);
    }

    [Fact]
    public void Users_AreStoredGlobally_NotSandboxed()
    {
        var storage = NewStorage();
        var users = new List<UserAccount>
        {
            new() { Email = "a@x.com", Name = "A" },
            new() { Email = "b@x.com", Name = "B" }
        };

        storage.SaveUsers(users);
        storage.SetCurrentUser(users[0]);

        var loaded = storage.GetUsers();

        Assert.Equal(2, loaded.Count);
    }

    [Fact]
    public void CurrentUser_SetAndClear_Works()
    {
        var storage = NewStorage();
        var user = new UserAccount { Email = "a@x.com", Name = "A" };

        storage.SetCurrentUser(user);
        Assert.NotNull(storage.GetCurrentUser());
        Assert.Equal("a@x.com", storage.GetCurrentUser()!.Email);

        storage.SetCurrentUser(null);
        Assert.Null(storage.GetCurrentUser());
    }

    [Fact]
    public void DemoData_InitialPets_MatchWebReference()
    {
        var pets = DemoData.CreateInitialPets();

        Assert.Equal(2, pets.Count);
        Assert.Equal("Max", pets[0].Name);
        Assert.Equal("Luna", pets[1].Name);
    }

    [Fact]
    public void DemoData_InitialAlerts_UseTodayDate()
    {
        var alerts = DemoData.CreateInitialAlerts();

        Assert.Single(alerts);
        Assert.Equal(DateTime.Today.ToString("yyyy-MM-dd"), alerts[0].Date);
    }
}