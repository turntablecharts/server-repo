using System;
namespace Common
{
	public class GenericResponse <T>
	{
		public T? Data { get; set; }
		public int StatusCode { get; set; }
		public string? Message { get; set; }
	}
}

