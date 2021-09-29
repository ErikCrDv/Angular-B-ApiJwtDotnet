using ApiClientes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClientes.Repositorio
{
	public interface IUserRepositorio
	{
		Task<string> Register(User user, string password);
		Task<string> Login(string userName, string password);
		Task<bool> UserExiste(string userName);
	}
}
