using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiClientes.Data;
using ApiClientes.Models;
using ApiClientes.Repositorio;
using ApiClientes.Models.Dto;
using Microsoft.AspNetCore.Authorization;

namespace ApiClientes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientesController : ControllerBase
    {
		private readonly IClienteRepositorio _clienteRepositorio;
        protected ResponseDto _responseDto;

		public ClientesController(IClienteRepositorio clienteRepositorio)
        {
			_clienteRepositorio = clienteRepositorio;
            _responseDto = new ResponseDto();
		}

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
			try
			{
                var lista = await _clienteRepositorio.GetClientes();
                _responseDto.Result = lista;
                _responseDto.DisplayMessage = "Lista de Clientes";
			}
			catch (Exception ex)
			{
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string> { ex.ToString() };
			}

            return Ok(_responseDto);
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _clienteRepositorio.GetClienteById(id);
            if(cliente == null)
			{
                _responseDto.IsSuccess = false;
                _responseDto.DisplayMessage = "Cliente no existe";
                return NotFound(_responseDto);
			}

            _responseDto.Result = cliente;
            _responseDto.DisplayMessage = "Informacion del Cliente";
            return Ok(_responseDto);
        }

        // PUT: api/Clientes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, ClienteDto clienteDto)
        {
			try
			{
                ClienteDto model = await _clienteRepositorio.CreateUpdate(clienteDto);
                _responseDto.Result = model;
                return Ok(_responseDto);
			}
			catch (Exception ex)
			{
                _responseDto.IsSuccess = false;
                _responseDto.DisplayMessage = "Error al Actualizar el registro";
                _responseDto.ErrorMessage = new List<string> { ex.ToString() };
                return BadRequest(_responseDto);
			}
        }

        // POST: api/Clientes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(ClienteDto clienteDto)
        {
			try
			{
                ClienteDto model = await _clienteRepositorio.CreateUpdate(clienteDto);
                _responseDto.Result = model;

                return CreatedAtAction("GetCliente", new { id = model.Id }, _responseDto);
            }
			catch (Exception ex)
			{
                _responseDto.IsSuccess = false;
                _responseDto.DisplayMessage = "Error al crear el registro";
                _responseDto.ErrorMessage = new List<string> { ex.ToString() };
                return BadRequest(_responseDto);
            }

            
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
			try
			{
                bool estadoEliminado = await _clienteRepositorio.DeleteCliente(id);
                if (estadoEliminado)
				{
                    _responseDto.Result = estadoEliminado;
                    _responseDto.DisplayMessage = "Cliente eliminado con exito";
                    return Ok(_responseDto);
				}
				else
				{
                    _responseDto.IsSuccess = false;
                    _responseDto.DisplayMessage = "Error al eliminar el cliente";
                    return BadRequest(_responseDto);
				}
			}
			catch (Exception ex)
			{
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessage = new List<string> { ex.ToString() };
                return BadRequest(_responseDto);
			}
        }

    }
}
