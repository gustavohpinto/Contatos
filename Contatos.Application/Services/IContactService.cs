using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contatos.Application.DTOs;

namespace Contatos.Application.Services
{
    public interface IContactService
    {
        Task<ContactResponseDTO> CreateAsync(ContactRequestDTO req, CancellationToken ct);
        Task<ContactResponseDTO> UpdateAsync(Guid id, ContactRequestDTO req, CancellationToken ct);
        Task<ContactResponseDTO?> GetAsync(Guid id, CancellationToken ct);
        Task<List<ContactResponseDTO>> ListAsync(CancellationToken ct);
        Task DeactivateAsync(Guid id, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
        Task<ContactResponseDTO> ActivateAsync(Guid id, CancellationToken ct);
    }
}
