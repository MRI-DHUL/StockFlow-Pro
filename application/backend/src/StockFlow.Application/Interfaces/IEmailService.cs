namespace StockFlow.Application.Interfaces;

public interface IEmailService
{
    /// <summary>
    /// Send an email to a single recipient
    /// </summary>
    Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send an email to multiple recipients
    /// </summary>
    Task SendEmailAsync(IEnumerable<string> toEmails, string subject, string body, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send low stock alert email
    /// </summary>
    Task SendLowStockAlertAsync(string productName, string sku, int currentQuantity, int threshold, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Send order confirmation email
    /// </summary>
    Task SendOrderConfirmationAsync(string customerEmail, string customerName, Guid orderId, decimal totalAmount, CancellationToken cancellationToken = default);
}
