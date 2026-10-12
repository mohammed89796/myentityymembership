
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MyEntityMembershipService.Data.Membership.Entities;
using MyEntityMembershipService.DTOs.Membership.EmailsDtos;

namespace MyEntityMembershipService.Services.Membership.EmailServ
{
    public class EmailService : IEmailService
    {
        private EmailSettings _options;
        public EmailService(IOptions<EmailSettings> options)
        {
            _options = options.Value;
        }
        
        public void SendInvoiceConfirmationEmail(PaymentInvoice paymentInvoice)
        {
            var mail = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_options.Email),
                Subject = "Invoice Confirmation",
            };

            mail.To.Add(MailboxAddress.Parse(paymentInvoice.Email));

            var builder = new BodyBuilder
            {
                HtmlBody = CreateInvoiceConfirmationBody(paymentInvoice),
                TextBody = "Your invoice details are attached."
            };


            mail.Body = builder.ToMessageBody();
            mail.From.Add(new MailboxAddress(_options.DisplayName, _options.Email));

            using var smtp = new SmtpClient();
            smtp.Connect(_options.Host, _options.Port, MailKit.Security.SecureSocketOptions.StartTls);

            smtp.Authenticate(_options.Email, _options.Password);

            smtp.Send(mail);

            smtp.Disconnect(true);
        }


        private string CreateInvoiceConfirmationBody(PaymentInvoice paymentInvoice)
        {
            var itemsHtml = string.Empty;

            // Generate HTML for each payment item
            foreach (var item in paymentInvoice.Items)
            {
                itemsHtml += $@"
                            <tr>
                                <td style='padding: 8px; border: 1px solid #ddd;'>{item.name}</td>
                                <td style='padding: 8px; border: 1px solid #ddd; text-align: right;'>{item.amount:C2}</td>
                            </tr>";
            }

            // Generate the full email body
            var emailBody = $@"
                            <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                                <h2 style='color: #4CAF50;'>Invoice Confirmation</h2>
                                <p>Dear {paymentInvoice.FirstName},</p>
                                <p>Thank you for your purchase! Here are the details of your invoice:</p>
        
                                <table style='border-collapse: collapse; width: 100%; margin: 20px 0;'>
                                    <thead>
                                        <tr>
                                            <th style='padding: 8px; border: 1px solid #ddd; text-align: left; background-color: #f4f4f4;'>Item Name</th>
                                            
                                            <th style='padding: 8px; border: 1px solid #ddd; text-align: right; background-color: #f4f4f4;'>Amount</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {itemsHtml}
                                    </tbody>
                                </table>
        
                                <p><strong>Total Amount:</strong> {paymentInvoice.Amount:C2}</p>
                                <p><strong>Currency:</strong> {paymentInvoice.Currency}</p>
                                <p><strong>Payment Method:</strong> {paymentInvoice.PaymentMethod}</p>
                                <p><strong>Status:</strong> {paymentInvoice.Status}</p>
                                <p><strong>Payment Date:</strong> {paymentInvoice.PaymentDate:MMMM dd, yyyy}</p>

                                <p>If you have any questions about your invoice, feel free to contact us.</p>
                                <p>Thank you for choosing our service!</p>
        
                                <p>Best regards,<br>Codid</p>
                            </div>";

            return emailBody;
        }

    }
}
