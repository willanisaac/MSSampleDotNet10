using System.Text.Json;
using System.Text.Json.Serialization;

namespace Users.Data;

public class UsersRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _mutex = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly List<User> _users;

    public UsersRepository(string filePath)
    {
        _filePath = filePath;
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        _users = LoadUsers();
    }

    public Task<List<User>> GetAllAsync()
    {
        return Task.FromResult(_users.Select(u => Clone(u)).ToList());
    }

    public Task<User?> GetByIdAsync(string id)
    {
        var user = _users.FirstOrDefault(u => string.Equals(u.Id, id, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user is null ? null : Clone(user));
    }

    public async Task<User> AddAsync(CreateUserInput input)
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = input.Name,
            Email = input.Email,
            Phone = input.Phone,
            Role = string.IsNullOrWhiteSpace(input.Role) ? "Customer" : input.Role,
            IsActive = input.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await PersistAsync(() => _users.Add(user));
        return Clone(user);
    }

    public async Task<User?> UpdateAsync(string id, UpdateUserInput input)
    {
        User? updated = null;
        await PersistAsync(() =>
        {
            var existing = _users.FirstOrDefault(u => string.Equals(u.Id, id, StringComparison.OrdinalIgnoreCase));
            if (existing is null)
            {
                return;
            }

            existing.Name = string.IsNullOrWhiteSpace(input.Name) ? existing.Name : input.Name;
            existing.Email = string.IsNullOrWhiteSpace(input.Email) ? existing.Email : input.Email;
            existing.Phone = string.IsNullOrWhiteSpace(input.Phone) ? existing.Phone : input.Phone;
            updated = Clone(existing);
        });

        return updated;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var removed = false;
        await PersistAsync(() =>
        {
            var target = _users.FirstOrDefault(u => string.Equals(u.Id, id, StringComparison.OrdinalIgnoreCase));
            if (target is null)
            {
                return;
            }

            _users.Remove(target);
            removed = true;
        });

        return removed;
    }

    private List<User> LoadUsers()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                var content = File.ReadAllText(_filePath);
                var users = JsonSerializer.Deserialize<List<User>>(content, _jsonOptions);
                if (users is not null)
                {
                    return users;
                }
            }
            catch
            {
                // ignore malformed file and fall back to empty
            }
        }

        return new List<User>();
    }

    private async Task PersistAsync(Action mutation)
    {
        await _mutex.WaitAsync();
        try
        {
            mutation();
            var json = JsonSerializer.Serialize(_users, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
        finally
        {
            _mutex.Release();
        }
    }

    private static User Clone(User user)
    {
        return new User
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class CreateUserInput
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer";
    public bool IsActive { get; set; } = true;
}

public class UpdateUserInput
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
