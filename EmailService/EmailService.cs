using Azure.Communication.Email;
using EmailService.Exceptions;
using EmailService.Services;
using System.Net.Http;

namespace EmailService
{
    public class EmailService : IEmailService
    {
        private EmailClient _client;

        public EmailService(string connectionString) 
        { 
            _client = new EmailClient(connectionString);
        }

        public async Task SendEmailAsync(string senderEmail, string recipientEmail, string subject, string plainTextContent, string htmlContent)
        {
            try
            {
                var recipientEmailAddress = new EmailAddress(recipientEmail);
                var recipientList = new List<EmailAddress> { recipientEmailAddress };
                var recipients = new EmailRecipients(recipientList);
                
                var content = new EmailContent(subject);
                content.Html = htmlContent;
                content.PlainText = plainTextContent;

                var emailMessage = new EmailMessage(senderEmail, recipients, content);

                var response = await _client.SendAsync(Azure.WaitUntil.Completed, emailMessage);

                if (response.Value.Status == EmailSendStatus.Succeeded)
                    Console.WriteLine($"Email sent successfully");
                else
                    throw new EmailServiceException($"Failed to send email, status: {response.Value.Status}");
            }
            catch (Exception ex)
            {
                throw new EmailServiceException($"Exception while sending email: {ex.Message}");
            }
        }
    }
}
