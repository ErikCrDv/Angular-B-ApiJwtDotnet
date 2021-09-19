using ApiClientes.Data;
using ApiClientes.Models;
using ApiClientes.Models.Dto;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClientes.Repositorio
{
	public class ClienteRepositorio : IClienteRepositorio
	{
		private readonly ApplicationDbContext _applicationDbContext;
		private IMapper _mapper;

		public ClienteRepositorio(ApplicationDbContext applicationDbContext, IMapper mapper)
		{
			_applicationDbContext = applicationDbContext;
			_mapper = mapper;
		}


		public async Task<ClienteDto> CreateUpdate(ClienteDto clienteDto)
		{
			Cliente cliente = _mapper.Map<ClienteDto, Cliente>(clienteDto);
			if(cliente.Id > 0)
			{
				_applicationDbContext.Clientes.Update(cliente);
			}
			else
			{
				await _applicationDbContext.Clientes.AddAsync(cliente);
			}
			await _applicationDbContext.SaveChangesAsync();

			return _mapper.Map<Cliente, ClienteDto>(cliente);
		}

		public async Task<bool> DeleteCliente(int id)
		{
			try
			{
				Cliente cliente = await _applicationDbContext.Clientes.FindAsync(id);
				if (cliente == null)
				{
					return false;
				}

				_applicationDbContext.Clientes.Remove(cliente);
				await _applicationDbContext.SaveChangesAsync();

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<ClienteDto> GetClienteById(int id)
		{
			Cliente cliente = await _applicationDbContext.Clientes.FindAsync(id);
			return _mapper.Map<ClienteDto>(cliente);
		}

		public async Task<List<ClienteDto>> GetClientes()
		{
			List<Cliente> lista = await _applicationDbContext.Clientes.ToListAsync();
			return _mapper.Map<List<ClienteDto>>(lista);
		}
	}
}
