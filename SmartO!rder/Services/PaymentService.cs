using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartO_rder.Services
{
    public record PaymentResult(bool Success, string? Reference, string? Error);

    // Integration point for a real payment provider (acquiring bank, Stripe, etc.).
    public interface IPaymentService
    {
        bool IsTestMode { get; }
        // idempotencyKey lets the provider recognise a repeated request for the same order and not charge twice.
        Task<PaymentResult> ChargeAsync(decimal amount, string description, string idempotencyKey, CancellationToken cancellationToken = default);
    }

    // Approves every payment without charging anything. Replace with a real provider before going live.
    public class TestPaymentService : IPaymentService
    {
        public bool IsTestMode => true;

        public Task<PaymentResult> ChargeAsync(decimal amount, string description, string idempotencyKey, CancellationToken cancellationToken = default)
        {
            if (amount <= 0)
                return Task.FromResult(new PaymentResult(false, null, "Amount must be positive"));
            return Task.FromResult(new PaymentResult(true, "TEST-" + Guid.NewGuid().ToString("N")[..12].ToUpperInvariant(), null));
        }
    }
}
