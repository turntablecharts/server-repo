using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Core.Interfaces;
using static Common.AppEnums;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task<GenericResponse<string>> SendEmail(List<string> emails, EmailTypeEnum emailType)
        {
            throw new NotImplementedException();
        }
    }
}

