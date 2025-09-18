using Contatos.Application.DTOs;
using Contatos.Application.Services;
using Contatos.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Contatos.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _service;

        public ContactController(IContactService service) => _service = service;


        [HttpGet]
        public async Task<ActionResult<List<ContactResponseDTO>>> List(CancellationToken ct)
        {
            return await _service.ListAsync(ct);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ContactResponseDTO>> Get(Guid id, CancellationToken ct)
        {
            var contact = await _service.GetAsync(id, ct);
            if (contact is null) return NotFound();
            return contact;
        }

        [HttpPost]
        public async Task<ActionResult<ContactResponseDTO>> Create(ContactRequestDTO dto, CancellationToken ct)
        {
            var result = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ContactResponseDTO>> Update(Guid id, ContactRequestDTO dto, CancellationToken ct)
        {
            var result = await _service.UpdateAsync(id, dto, ct);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        { await _service.DeactivateAsync(id, ct); return NoContent(); }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }

        [HttpPut("{id}/activate")]
        public async Task<ActionResult<ContactResponseDTO>> Activate(Guid id, CancellationToken ct)
        {
            try
            {
                var result = await _service.ActivateAsync(id, ct);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DomainException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
