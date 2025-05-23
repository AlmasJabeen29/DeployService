using DeployService.Clients;
using Microsoft.Office.Interop.Outlook;
using NumarisConnectt.Application.DataTransferObjects.RetrievalDtos;
using NumarisConnectt.Application.Services.Interfaces;
using System.Runtime.Versioning;


namespace DeployService.Utilities
{
    public class Mailer
    {
        private readonly ApiClient _ApiClient;
        private readonly RequestProcessor _requestProcessor;

        public Mailer(ApiClient ApiClient, RequestProcessor requestProcessor)
        {
            _ApiClient = ApiClient;
            _requestProcessor = requestProcessor;
        }

        [SupportedOSPlatform("windows")]
        private void SendEmail(string subject, string body, string to, List<string> cc = null)
        {
            try
            {
                Type? outlookType = Type.GetTypeFromProgID("Outlook.Application");
                if (outlookType != null)
                {
                    dynamic outlookApp = Activator.CreateInstance(outlookType)!;
                    if (outlookApp != null)
                    {
                        MailItem mailItem = (MailItem)outlookApp.CreateItem(OlItemType.olMailItem);
                        mailItem.Subject = subject;
                        mailItem.Body = body;
                        mailItem.To = to;
                        if (cc != null)
                        {
                            mailItem.CC = string.Join(";", cc);
                        }
                        mailItem.Send();

                        Console.WriteLine("Email sent successfully!");
                    }
                }
                else
                {
                    Console.WriteLine("Outlook is not installed or the ProgID is incorrect.");
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine($"Error sending email: {e.Message}");
            }
        }

        [SupportedOSPlatform("windows")]
        public async Task SendEmailOnRequestSubmission(RequestDto request)
        {
            var (user, host, scanner, baseline) = await _requestProcessor.RetrieveRequestDataAsync(request);
            var testLabAdmins = await _ApiClient.GetAdminstratorsAsync("TestLab");

            // Email to User and Test Lab Admins
            string subject = $"Numaris Installation request id {request.Id} submitted.";
            string body = $"Dear User, \r\n\r\nYour request for Numaris Installation has been submitted successfully. Your request Id is {request.Id}. \r\n\r\nRegards, \r\n\r\nNumarisConnectt Support";
            SendEmail(subject, body, user.Email, testLabAdmins.Select(a => a.Email).ToList());

            // Email to Factory Admins if applicable
            if (host?.Location == "Factory")
            {
                var factoryAdmins = await _ApiClient.GetAdminstratorsAsync("Factory");
                body = $"Dear Factory Admin, \r\n\r\nA request for Numaris Installation has been submitted successfully. Below are the details: \r\n\r\n" +
                       $"Request ID: {request.Id}  \r\n" +
                       $"Username: {user.UserName} \r\n" +
                       $"User email: {user.Email} \r\n" +
                       $"Scanner Integration: {request.ScannerIntegrationRequested} \r\n" +
                       $"Start date: {host.BlockedFrom} \r\n" +
                       $"System Type: {scanner.Name} \r\n" +
                       $"Baseline: {baseline.Baseline} \r\n" +
                       $"Hostname: {host.HostName} \r\n\r\n" +
                       "Regards, \r\n\r\nNumarisConnectt Support";
                SendEmail(subject, body, string.Join(";", factoryAdmins.Select(a => a.Email).ToList()));
            }
        }

        [SupportedOSPlatform("windows")]
        public async Task SendEmailOnSuccessfulInstallation(RequestDto request)
        {
            var (user, host, scanner, baseline) = await _requestProcessor.RetrieveRequestDataAsync(request);
            var testLabAdmins = await _ApiClient.GetAdminstratorsAsync("TestLab");

            // Email to User and Test Lab Admins
            string subject = $"Numaris Installation with request id {request.Id} is completed";
            string body = $"Dear User, \r\n\r\nThe Numaris Installation with request {request.Id} is completed. \r\n\r\nRegards, \r\n\r\nNumarisConnectt Support";
            SendEmail(subject, body, user.Email, testLabAdmins.Select(a => a.Email).ToList());

            // Email to Factory Admins if applicable
            if (host?.Location == "Factory")
            {
                var factoryAdmins = await _ApiClient.GetAdminstratorsAsync("Factory");
                body = $"Dear Factory Admin, \r\n\r\nThe Numaris Installation with request id {request.Id} is completed. \r\n\r\n" +
                       $"The host {host.HostName} is occupied with the baseline {baseline.Baseline} \r\n\r\n" +
                       "Regards, \r\n\r\nNumarisConnectt Support";
                SendEmail(subject, body, string.Join(";", factoryAdmins.Select(a => a.Email).ToList()));
            }
        }

    }
}
