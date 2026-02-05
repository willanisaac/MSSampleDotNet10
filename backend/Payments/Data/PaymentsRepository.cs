using System.Text.Json;
using System.Text.Json.Serialization;

namespace Payments.Data;

public class PaymentsRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _mutex = new(1, 1);
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly List<Payment> _payments;

    public PaymentsRepository(string filePath)
    {
        _filePath = filePath;
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_filePath)!);
        _payments = LoadPayments();
    }

    public Task<Payment?> GetByIdAsync(string id)
    {
        var payment = _payments.FirstOrDefault(p => string.Equals(p.Id, id, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(payment is null ? null : Clone(payment));
    }

    public Task<List<Payment>> GetByOrderAsync(string orderId)
    {
        var payments = _payments
            .Where(p => string.Equals(p.OrderId, orderId, StringComparison.OrdinalIgnoreCase))
            .Select(Clone)
            .ToList();

        return Task.FromResult(payments);
    }

    public async Task<Payment> ProcessAsync(ProcessPaymentInput input)
    {
        var payment = new Payment
        {
            Id = Guid.NewGuid().ToString("N"),
            OrderId = input.OrderId,
            Amount = input.Amount,
            Status = "Completed",
            PaymentMethod = input.PaymentMethod,
            TransactionId = $"txn-{Guid.NewGuid():N}"[..12],
            CreatedAt = DateTime.UtcNow
        };

        await PersistAsync(() => _payments.Add(payment));
        return Clone(payment);
    }

    public async Task<Payment?> RefundAsync(string id)
    {
        Payment? updated = null;
        await PersistAsync(() =>
        {
            var existing = _payments.FirstOrDefault(p => string.Equals(p.Id, id, StringComparison.OrdinalIgnoreCase));
            if (existing is null)
            {
                return;
            }

            existing.Status = "Refunded";
            updated = Clone(existing);
        });

        return updated;
    }

    private List<Payment> LoadPayments()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                var content = File.ReadAllText(_filePath);
                var payments = JsonSerializer.Deserialize<List<Payment>>(content, _jsonOptions);
                if (payments is not null)
                {
                    return payments;
                }
            }
            catch
            {
                // ignore malformed file and fall back to empty
            }
        }

        return new List<Payment>();
    }

    private async Task PersistAsync(Action mutation)
    {
        await _mutex.WaitAsync();
        try
        {
            mutation();
            var json = JsonSerializer.Serialize(_payments, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
        finally
        {
            _mutex.Release();
        }
    }

    private static Payment Clone(Payment payment)
    {
        return new Payment
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Status = payment.Status,
            PaymentMethod = payment.PaymentMethod,
            TransactionId = payment.TransactionId,
            CreatedAt = payment.CreatedAt
        };
    }
}

public class Payment
{
    public string Id { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending";
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ProcessPaymentInput
{
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
}
