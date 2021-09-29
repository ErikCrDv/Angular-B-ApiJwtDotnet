using ApiClientes.Models;
using ApiClientes.Models.Dto;
using ApiClientes.Repositorio;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClientes.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserRepositorio _userRepositorio;
		protected ResponseDto _responseDto;

		public UsersController(IUserRepositorio userRepositorio)
		{
			_userRepositorio = userRepositorio;
			_responseDto = new ResponseDto();
		}

		[HttpPost("Register")]
		public async Task<ActionResult> Register(UserDto userDto)
		{
			var respuesta = await _userRepositorio.Register(
				new User
				{
					UserName = userDto.UserName
				}, userDto.Password);

			if(respuesta == "existe")
			{
				_responseDto.IsSuccess = false;
				_responseDto.DisplayMessage = "El usuario ya existe";
				return BadRequest(_responseDto);
			}

			if(respuesta == "error")
			{
				_responseDto.IsSuccess = false;
				_responseDto.DisplayMessage = "Error al crear el usuiario";
				return BadRequest(_responseDto);
			}

			_responseDto.DisplayMessage = "Usuario creado con exito";
			//_responseDto.Result = respuesta;

			JwTPackage jtp = new JwTPackage();
			jtp.UserName = userDto.UserName;
			jtp.Token = respuesta;
			_responseDto.Result = jtp;
			return Ok(_responseDto);
		}


		[HttpPost("Login")]
		public async Task<ActionResult> Login(UserDto userDto)
		{
			var respuesta = await _userRepositorio.Login(userDto.UserName, userDto.Password);
			if(respuesta == "noUser")
			{
				_responseDto.IsSuccess = false;
				_responseDto.DisplayMessage = "Usuario no existe";
				return BadRequest(_responseDto);
			}
			if (respuesta == "wrongPassword")
			{
				_responseDto.IsSuccess = false;
				_responseDto.DisplayMessage = "Wrong Password";
				return BadRequest(_responseDto);
			}

			//_responseDto.Result = respuesta;
			_responseDto.DisplayMessage = "Usuario Conectado";
			JwTPackage jtp = new JwTPackage();
			jtp.UserName = userDto.UserName;
			jtp.Token = respuesta;
			_responseDto.Result = jtp;
			return Ok(_responseDto);
		}
	}

	public class JwTPackage
	{
		public string UserName { get; set; }
		public string Token { get; set; }
	}
}
