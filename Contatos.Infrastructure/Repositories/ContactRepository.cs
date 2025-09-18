using Contatos.Domain.Entities;
using Contatos.Domain.Repositories;
using Contatos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contatos.Infrastructure.Repositories
{
    public class ContactRepository: IContactRepository
    {
        private readonly AppDBContext _db;

        public ContactRepository(AppDBContext db) => _db = db;

        public Task AddAsync(Contact c, CancellationToken ct)
        => _db.Contacts.AddAsync(c, ct).AsTask();

        public Task<Contact?> GetActiveByIdAsync(Guid id, CancellationToken ct)
            => _db.Contacts.FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<Contact?> GetByIdIncludingInactiveAsync(Guid id, CancellationToken ct)
        => _db.Contacts.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<List<Contact>> ListActiveAsync(CancellationToken ct)
            => _db.Contacts.OrderBy(x => x.Name).ToListAsync(ct);

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
            => await _db.Contacts.IgnoreQueryFilters().AnyAsync(x => x.Id == id, ct);

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            var e = await _db.Contacts.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct);
            if (e is null) return;
            _db.Remove(e);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<Contact?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _db.Contacts.FirstOrDefaultAsync(x => x.Id == id, ct);


        public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
    }
}
