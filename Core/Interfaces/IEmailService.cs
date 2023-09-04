using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using static Common.AppEnums;
//using Core.DTOs;

namespace Core.Interfaces
{
	public interface IEmailService
	{
		Task<GenericResponse<string>> SendEmail(List<string> emails, EmailTypeEnum emailType);
	}
}

