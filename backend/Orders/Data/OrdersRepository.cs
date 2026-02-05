using System.Text.Json;
using System.Text.Json.Serialization;

namespace Orders.Data;

public class OrdersRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _mutex = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly List<Order> _orders;

    public OrdersRepository(string filePath)
    {
        _filePath = filePath;
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_filePath)!);
        _orders = LoadOrders();
    }

    public Task<Order?> GetByIdAsync(string id)
    {
        var order = _orders.FirstOrDefault(o => string.Equals(o.Id, id, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(order is null ? null : Clone(order));
    }

    public Task<List<Order>> GetByUserAsync(string userId)
    {
        var orders = _orders
            .Where(o => string.Equals(o.UserId, userId, StringComparison.OrdinalIgnoreCase))
            .Select(Clone)
            .ToList();

        return Task.FromResult(orders);
    }

    public async Task<Order> AddAsync(CreateOrderInput input)
    {
        var items = input.Items.Select(i => new OrderItem
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            Quantity = i.Quantity,
            Price = i.Price,
            Subtotal = Math.Round(i.Price * i.Quantity, 2)
        }).ToList();

        var order = new Order
        {
            Id = Guid.NewGuid().ToString("N"),
            UserId = input.UserId,
            Status = "Pending",
            Items = items,
            TotalAmount = Math.Round(items.Sum(x => x.Subtotal), 2),
            CreatedAt = DateTime.UtcNow
        };

        await PersistAsync(() => _orders.Add(order));
        return Clone(order);
    }

    public async Task<Order?> CancelAsync(string id)
    {
        Order? updated = null;
        await PersistAsync(() =>
        {
            var existing = _orders.FirstOrDefault(o => string.Equals(o.Id, id, StringComparison.OrdinalIgnoreCase));
            if (existing is null)
            {
                return;
            }

            if (!string.Equals(existing.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
            {
                existing.Status = "Cancelled";
            }

            updated = Clone(existing);
        });

        return updated;
    }

    private List<Order> LoadOrders()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                var content = File.ReadAllText(_filePath);
                var orders = JsonSerializer.Deserialize<List<Order>>(content, _jsonOptions);
                if (orders is not null)
                {
                    return orders;
                }
            }
            catch
            {
                // ignore malformed file and fall back to empty
            }
        }

        return new List<Order>();
    }

    private async Task PersistAsync(Action mutation)
    {
        await _mutex.WaitAsync();
        try
        {
            mutation();
            var json = JsonSerializer.Serialize(_orders, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
        finally
        {
            _mutex.Release();
        }
    }

    private static Order Clone(Order order)
    {
        return new Order
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            Items = order.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                Price = i.Price,
                Subtotal = i.Subtotal
            }).ToList()
        };
    }
}

public class Order
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class OrderItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Subtotal { get; set; }
}

public class CreateOrderInput
{
    public string UserId { get; set; } = string.Empty;
    public List<OrderItemInput> Items { get; set; } = new();
}

public class OrderItemInput
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
