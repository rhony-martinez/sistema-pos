using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Infrastructure.Data;
using SistemaPOS.Domain.Entities;
using SistemaPOS.Application.DTOs;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly SistemaPosContext _context;
        private readonly IUserService _userService;

        public UsuarioController(SistemaPosContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // Get user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound(new { message = "User not found." }) : Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = usuario.UsuId }, usuario);
        }

        // Update user
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest dto)
        {
            try
            {
                var actualizado = await _userService.UpdateUserAsync(id, dto);

                if (!actualizado)
                    return NotFound(new { message = "Usuario no encontrado." });

                return Ok(new { message = "Usuario actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al actualizar el usuario.",
                    error = ex.Message
                });
            }
        }




        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
