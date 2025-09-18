using Contatos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contatos.Domain.Repositories
{
    public interface IContactRepository
    {
        Task AddAsync(Contact contact, CancellationToken ct);
        Task<Contact?> GetActiveByIdAsync(Guid id, CancellationToken ct);
        Task<Contact?> GetByIdIncludingInactiveAsync(Guid id, CancellationToken ct); // 👈 novo

        Task<List<Contact>> ListActiveAsync(CancellationToken ct);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
        Task<Contact?> GetByIdAsync(Guid id, CancellationToken ct);

    }
}
