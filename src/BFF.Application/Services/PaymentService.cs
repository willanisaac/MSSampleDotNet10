using AutoMapper;
using BFF.Application.DTOs.Requests;
using BFF.Application.DTOs.Responses;
using BFF.Application.Interfaces;
using BFF.Application.Services.Interfaces;

namespace BFF.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentsGraphQLClient _graphQLClient;
    private readonly IMapper _mapper;

    public PaymentService(IPaymentsGraphQLClient graphQLClient, IMapper mapper)
    {
        _graphQLClient = graphQLClient;
        _mapper = mapper;
    }

    public async Task<PaymentResponse?> GetPaymentByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _graphQLClient.GetPaymentByIdAsync(id, cancellationToken);
        
        if (response?.Data?.Payment == null)
        {
            return null;
        }

        return _mapper.Map<PaymentResponse>(response.Data.Payment);
    }

    public async Task<List<PaymentResponse>> GetPaymentsByOrderAsync(string orderId, CancellationToken cancellationToken = default)
    {
        var response = await _graphQLClient.GetPaymentsByOrderAsync(orderId, cancellationToken);
        
        if (response?.Data?.Payments == null || response.Data.Payments.Count == 0)
        {
            return new List<PaymentResponse>();
        }

        return _mapper.Map<List<PaymentResponse>>(response.Data.Payments);
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(ProcessPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var input = new
        {
            orderId = request.OrderId,
            amount = request.Amount,
            paymentMethod = request.PaymentMethod
        };

        var response = await _graphQLClient.ProcessPaymentAsync(input, cancellationToken);
        
        if (response?.Data?.Payment == null)
        {
            throw new InvalidOperationException("Failed to process payment");
        }

        return _mapper.Map<PaymentResponse>(response.Data.Payment);
    }

    public async Task<PaymentResponse?> RefundPaymentAsync(string id, CancellationToken cancellationToken = default)
    {
        var response = await _graphQLClient.RefundPaymentAsync(id, cancellationToken);
        
        if (response?.Data?.Payment == null)
        {
            return null;
        }

        return _mapper.Map<PaymentResponse>(response.Data.Payment);
    }
}
