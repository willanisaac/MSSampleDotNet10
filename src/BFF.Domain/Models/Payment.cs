using BFF.Domain.Models.Common;

namespace BFF.Domain.Models;

public class Payment : BaseEntity
{
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
}
