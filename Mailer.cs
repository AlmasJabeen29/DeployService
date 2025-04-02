using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Outlook;
using NumarisConnectt.Api.Controllers;
using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;
using NumarisConnectt.Application.Services.Interfaces;

namespace DeployService
{
    public static class Mailer
    {
        public static void SendEmail(string userEmail)
        {
            try
            {
                Type? outlookType = Type.GetTypeFromProgID("Outlook.Application");
                dynamic outlookApp = Activator.CreateInstance(outlookType);
                if (outlookApp != null)
                {
                    // Create a new mail item
                    MailItem mailItem = (MailItem)outlookApp.CreateItem(OlItemType.olMailItem);

                    // Set up the email properties
                    mailItem.Subject = "Task Completed";
                    mailItem.Body = "The task has been completed successfully.";
                    mailItem.To = userEmail;
                    mailItem.Send();

                    Console.WriteLine("Email sent successfully!");
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Error sending email: " + e.Message);
            }
        }
    }
}
