using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using StockFlow.Application.Interfaces;

namespace StockFlow.Infrastructure.Services;

public class GmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GmailService> _logger;
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _senderEmail;
    private readonly string _senderName;
    private readonly string _senderPassword;

    public GmailService(IConfiguration configuration, ILogger<GmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        var gmailSettings = _configuration.GetSection("GmailSettings");
        _smtpServer = gmailSettings["SmtpServer"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(gmailSettings["SmtpPort"] ?? "587");
        _senderEmail = gmailSettings["SenderEmail"] ?? throw new InvalidOperationException("Gmail SenderEmail not configured");
        _senderName = gmailSettings["SenderName"] ?? "StockFlow Pro";
        _senderPassword = gmailSettings["SenderPassword"] ?? throw new InvalidOperationException("Gmail SenderPassword not configured");
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default)
    {
        await SendEmailAsync(new[] { toEmail }, subject, body, cancellationToken);
    }

    public async Task SendEmailAsync(IEnumerable<string> toEmails, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_senderName, _senderEmail));
            
            foreach (var toEmail in toEmails)
            {
                message.To.Add(MailboxAddress.Parse(toEmail));
            }
            
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls, cancellationToken);
            await client.AuthenticateAsync(_senderEmail, _senderPassword, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent successfully to {Recipients}. Subject: {Subject}", 
                string.Join(", ", toEmails), subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipients}. Subject: {Subject}", 
                string.Join(", ", toEmails), subject);
            throw;
        }
    }

    public async Task SendLowStockAlertAsync(string productName, string sku, int currentQuantity, int threshold, CancellationToken cancellationToken = default)
    {
        var subject = $"⚠️ Low Stock Alert: {productName}";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #d32f2f;'>Low Stock Alert</h2>
                <p>The following product has fallen below the minimum stock threshold:</p>
                <table style='border-collapse: collapse; margin: 20px 0;'>
                    <tr>
                        <td style='padding: 8px; font-weight: bold;'>Product:</td>
                        <td style='padding: 8px;'>{productName}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px; font-weight: bold;'>SKU:</td>
                        <td style='padding: 8px;'>{sku}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px; font-weight: bold;'>Current Quantity:</td>
                        <td style='padding: 8px; color: #d32f2f;'>{currentQuantity}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px; font-weight: bold;'>Threshold:</td>
                        <td style='padding: 8px;'>{threshold}</td>
                    </tr>
                </table>
                <p style='color: #666;'>Please reorder stock to avoid stockouts.</p>
                <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>
                <p style='color: #999; font-size: 12px;'>This is an automated alert from StockFlow Pro</p>
            </body>
            </html>
        ";

        var adminEmails = _configuration.GetSection("GmailSettings:AdminEmails").Get<List<string>>() 
            ?? new List<string> { "admin@stockflowpro.com" };

        await SendEmailAsync(adminEmails, subject, body, cancellationToken);
        _logger.LogInformation("Low stock alert sent for product {ProductName} ({SKU})", productName, sku);
    }

    public async Task SendOrderConfirmationAsync(string customerEmail, string customerName, Guid orderId, decimal totalAmount, CancellationToken cancellationToken = default)
    {
        var subject = $"Order Confirmation - #{orderId.ToString().Substring(0, 8)}";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2 style='color: #2e7d32;'>Order Confirmation</h2>
                <p>Dear {customerName},</p>
                <p>Thank you for your order! Your order has been successfully placed.</p>
                <table style='border-collapse: collapse; margin: 20px 0;'>
                    <tr>
                        <td style='padding: 8px; font-weight: bold;'>Order ID:</td>
                        <td style='padding: 8px;'>{orderId}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px; font-weight: bold;'>Total Amount:</td>
                        <td style='padding: 8px; color: #2e7d32; font-weight: bold;'>${totalAmount:F2}</td>
                    </tr>
                    <tr>
                        <td style='padding: 8px; font-weight: bold;'>Order Date:</td>
                        <td style='padding: 8px;'>{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</td>
                    </tr>
                </table>
                <p>We'll notify you when your order is ready for shipping.</p>
                <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>
                <p style='color: #999; font-size: 12px;'>Thank you for choosing StockFlow Pro!</p>
            </body>
            </html>
        ";

        await SendEmailAsync(customerEmail, subject, body, cancellationToken);
        _logger.LogInformation("Order confirmation email sent to {CustomerEmail} for order {OrderId}", 
            customerEmail, orderId);
    }
}
