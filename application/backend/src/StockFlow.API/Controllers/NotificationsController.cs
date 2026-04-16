using Microsoft.AspNetCore.Mvc;
using StockFlow.Application.Interfaces;

namespace StockFlow.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IEmailService _emailService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        IEmailService emailService,
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _emailService = emailService;
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpPost("test-email")]
    public async Task<IActionResult> TestEmail([FromBody] TestEmailRequest request)
    {
        try
        {
            var emailBody = $@"<html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #2e7d32;'>✅ Email Service Test Successful!</h2>
                    <p>This is a test email from StockFlow Pro API.</p>
                    <p><strong>Gmail SMTP Configuration:</strong> Working correctly!</p>
                    <p><strong>Sent at:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>
                    <p style='color: #999; font-size: 12px;'>StockFlow Pro Notification System</p>
                </body>
            </html>";

            await _emailService.SendEmailAsync(request.ToEmail, "StockFlow Pro - Email Test", emailBody);

            _logger.LogInformation("Test email sent successfully to {Email}", request.ToEmail);
            return Ok(new { message = "Test email sent successfully", recipient = request.ToEmail });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send test email to {Email}", request.ToEmail);
            return StatusCode(500, new { message = "Failed to send test email", error = ex.Message });
        }
    }

    [HttpPost("test-pusher")]
    public async Task<IActionResult> TestPusher()
    {
        try
        {
            var testData = new
            {
                type = "test-notification",
                message = "Pusher WebSocket connection test successful!",
                timestamp = DateTime.UtcNow,
                status = "success"
            };

            await _notificationService.SendNotificationAsync("stockflow-notifications", "test-event", testData);

            _logger.LogInformation("Test Pusher notification sent successfully");
            return Ok(new 
            { 
                message = "Test Pusher notification sent successfully",
                channel = "stockflow-notifications",
                eventName = "test-event",
                data = testData
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send test Pusher notification");
            return StatusCode(500, new { message = "Failed to send Pusher notification", error = ex.Message });
        }
    }

    [HttpPost("test-all")]
    public async Task<IActionResult> TestAll([FromBody] TestEmailRequest request)
    {
        var emailSuccess = false;
        var emailMessage = "";
        var emailError = "";
        var pusherSuccess = false;
        var pusherMessage = "";
        var pusherError = "";

        try
        {
            var emailBody = $@"<html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #2e7d32;'>✅ Complete Notification System Test</h2>
                    <p>Both Email (Gmail) and Pusher services are working correctly!</p>
                    <ul>
                        <li><strong>Email Service:</strong> ✅ Active</li>
                        <li><strong>Pusher Service:</strong> ✅ Active</li>
                        <li><strong>Real-time Notifications:</strong> ✅ Enabled</li>
                    </ul>
                    <p><strong>Tested at:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>
                    <p style='color: #999; font-size: 12px;'>StockFlow Pro Notification System</p>
                </body>
            </html>";

            await _emailService.SendEmailAsync(request.ToEmail, "StockFlow Pro - Full System Test", emailBody);
            
            emailSuccess = true;
            emailMessage = $"Email sent to {request.ToEmail}";
        }
        catch (Exception ex)
        {
            emailError = ex.Message;
        }

        try
        {
            await _notificationService.SendNotificationAsync("stockflow-notifications", "system-test",
                new
                {
                    type = "full-system-test",
                    message = "Email and Pusher services tested successfully!",
                    timestamp = DateTime.UtcNow,
                    emailSent = emailSuccess
                });

            pusherSuccess = true;
            pusherMessage = "Pusher notification sent successfully";
        }
        catch (Exception ex)
        {
            pusherError = ex.Message;
        }

        var results = new
        {
            email = new { success = emailSuccess, message = emailMessage, error = emailError },
            pusher = new { success = pusherSuccess, message = pusherMessage, error = pusherError }
        };

        var overallSuccess = emailSuccess && pusherSuccess;
        return overallSuccess 
            ? Ok(new { message = "All notification services working correctly!", results })
            : StatusCode(500, new { message = "Some notification services failed", results });
    }

    [HttpPost("test-low-stock-alert")]
    public async Task<IActionResult> TestLowStockAlert()
    {
        try
        {
            var productName = "Test Product XYZ";
            var sku = "TEST-SKU-001";
            var currentQuantity = 5;
            var threshold = 10;

            await _emailService.SendLowStockAlertAsync(productName, sku, currentQuantity, threshold);
            await _notificationService.SendLowStockNotificationAsync(productName, sku, currentQuantity, threshold);

            _logger.LogInformation("Test low stock alert sent for {Product}", productName);

            return Ok(new
            {
                message = "Low stock alert test successful!",
                product = productName,
                sku,
                currentQuantity,
                threshold,
                emailSent = true,
                pusherSent = true
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send test low stock alert");
            return StatusCode(500, new { message = "Failed to send low stock alert", error = ex.Message });
        }
    }
}

public class TestEmailRequest
{
    public string ToEmail { get; set; } = string.Empty;
}
