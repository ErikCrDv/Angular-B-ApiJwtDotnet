﻿using ApiClientes.Data;
using ApiClientes.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiClientes.Repositorio
{
	public class UserRepositorio : IUserRepositorio
	{
		private readonly ApplicationDbContext _applicationDbContext;
		private readonly IConfiguration _configuration;

		public UserRepositorio(ApplicationDbContext applicationDbContext, IConfiguration configuration)
		{
			_applicationDbContext = applicationDbContext;
			_configuration = configuration;
		}


		public async Task<string> Login(string userName, string password)
		{
			var user = await _applicationDbContext.Users.FirstOrDefaultAsync(
				x => x.UserName.ToLower().Equals(userName.ToLower()));

			if(user == null)
			{
				return "noUser";
			}
			else if (!VerificarPasswordHash(password, user.PasswordHash, user.PasswordSalt))
			{
				return "wrongPassword";
			}
			else
			{
				return CrearToken(user);
			}


		}

		public async Task<int> Register(User user, string password)
		{
			try
			{
				if (await UserExiste(user.UserName))
				{
					return -1;
				}

				CrearPasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
				user.PasswordHash = passwordHash;
				user.PasswordSalt = passwordSalt;

				await _applicationDbContext.Users.AddAsync(user);
				await _applicationDbContext.SaveChangesAsync();
				return user.Id;
			}
			catch (Exception)
			{
				return -500;
			}
		}

		public async Task<bool> UserExiste(string userName)
		{
			if(await _applicationDbContext.Users.AnyAsync(x => x.UserName.ToLower().Equals(userName.ToLower())))
			{
				return true;
			}

			return false;
		}

		private void CrearPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new System.Security.Cryptography.HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}

		public bool VerificarPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
			{
				var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				var computedHashLength = computedHash.Length;
				for(int i = 0; i < computedHashLength; i++)
				{
					if(computedHash[i] != passwordHash[i])
					{
						return false;
					}
				}
				return true;
			}
		}


		//TOKEN
		private string CrearToken(User user)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.UserName)
			};

			var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var tokenDescriptior = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = System.DateTime.Now.AddDays(1),
				SigningCredentials = creds
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptior);

			return tokenHandler.WriteToken(token);
		}
	}
}