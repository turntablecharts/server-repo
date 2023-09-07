using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Presentation.DTO;
using Presentation.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Presentation.Controllers
{
    [Route("api/Email")]
    public class EmailController : Controller
    {

        private IGenericRepository<SubscribersEmail> _subscribers;
        private IConfiguration _configuration;
        public EmailController(IGenericRepository<SubscribersEmail> subscribers, IConfiguration configuration)
        {
            _subscribers = subscribers;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribersEmail subscriberInfo)
        {
            if (!string.IsNullOrEmpty(subscriberInfo.Email))
            {
                if(!IsValidEmail(subscriberInfo.Email))
                {
                    var response = new ResponseDto<string> { Data = "Invalid Email. Kindly Check your input", StatusCode = 400 };
                    return StatusCode(response.StatusCode, response);
                }
                string alreadyExists = null; // _subscribers.GetWithInclude(m => m.Email == subscriberInfo.Email, string.Empty).FirstOrDefault();
                if (alreadyExists == null)
                {
                    subscriberInfo.SignUpDate = DateTime.UtcNow;
                    //var subscriber = await _subscribers.AddAsync(subscriberInfo);
                    var response = new ResponseDto<string> { Data = "Subscription Successful\n. A welcome email has being sent to you", StatusCode = 200 };

                    await SendEmailResend(subscriberInfo.Name, subscriberInfo.Email, "Welcome To TurnTable Charts");
                    return StatusCode(response.StatusCode, response);
                }
                var res = new ResponseDto<string> { Data = "Seems like you have subscribed already. ", StatusCode = 200 };
                return StatusCode(res.StatusCode, res);
            }

            return Ok();
        }

        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (!System.Text.RegularExpressions.Regex.IsMatch(email, emailPattern))
            {
                return false;
            }

            string[] parts = email.Split('@');
            string domain = parts[1];

            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(domain);
                if (hostEntry != null && hostEntry.AddressList.Length > 0)
                {
                    foreach (IPAddress ip in hostEntry.AddressList)
                    {
                        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            return true;
                        }
                    }
                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine($"ValidateEmail Failed with ex{ex}");
            }
            return false;
        }

        private async Task SendEmailResend(string userName, string userEmail, string title)
        {
            string htmlContent = System.IO.File.ReadAllText(@"Templates/WelcomeEmail.html");

            htmlContent = htmlContent.Replace("[Name]", userName).Replace("[DateTime]", DateTime.Now.Year.ToString());
            var request = new
            {
                from = $"TurnTable <{_configuration.GetValue<string>("ResendDetails:SenderEmail")}>",
                to = new string[] { userEmail },
                subject = title,
                html = htmlContent
            };

            var baseUrl = _configuration.GetValue<string>("ResendDetails:SendEmailUrl");
            string requestContent = JsonConvert.SerializeObject(request);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration.GetValue<string>("ResendDetails:ApiKey")}");
            var response = await client.PostAsync(baseUrl, new StringContent(requestContent, Encoding.Default, "application/json"));
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{nameof(SendEmailResend)} {JsonConvert.SerializeObject(responseContent)}");
        }
    }
}

