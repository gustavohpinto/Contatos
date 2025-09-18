using Contatos.Application.DTOs;
using Contatos.Domain.Abstractions;
using Contatos.Domain.Entities;
using Contatos.Domain.Exceptions;
using Contatos.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contatos.Application.Services
{
    public class ContactService(IContactRepository repo, IClock clock) : IContactService
    {
        public async Task<ContactResponseDTO> CreateAsync(ContactRequestDTO req, CancellationToken ct)
        {
            var sex = ParseSex(req.Sex);

            var entity = Contact.Create(req.Name, req.BirthDate, sex, clock);

            await repo.AddAsync(entity, ct);
            await repo.SaveChangesAsync(ct);

            return Map(entity);
        }

        public async Task<ContactResponseDTO> UpdateAsync(Guid id, ContactRequestDTO req, CancellationToken ct)
        {
            var entity = await repo.GetActiveByIdAsync(id, ct)
                ?? throw new KeyNotFoundException("Contact not found.");

            var sex = ParseSex(req.Sex);

            entity.Update(req.Name, req.BirthDate, sex, clock);

            await repo.SaveChangesAsync(ct);

            return Map(entity);
        }

        public async Task<ContactResponseDTO?> GetAsync(Guid id, CancellationToken ct)
            => (await repo.GetActiveByIdAsync(id, ct)) is { } e ? Map(e) : null;

        public async Task<List<ContactResponseDTO>> ListAsync(CancellationToken ct)
            => (await repo.ListActiveAsync(ct)).Select(Map).ToList();

        public async Task DeactivateAsync(Guid id, CancellationToken ct)
        {
            var entity = await repo.GetActiveByIdAsync(id, ct)
                ?? throw new KeyNotFoundException("Contact not found.");

            entity.Deactivate(clock);
            await repo.SaveChangesAsync(ct);
        }

        public async Task<ContactResponseDTO> ActivateAsync(Guid id, CancellationToken ct)
        {
            var entity = await repo.GetByIdIncludingInactiveAsync(id, ct)
            ?? throw new KeyNotFoundException("Contact not found.");

            entity.Activate(clock);
            await repo.SaveChangesAsync(ct);

            return Map(entity);
        }


        public async Task DeleteAsync(Guid id, CancellationToken ct)
            => await repo.DeleteAsync(id, ct);

        private ContactResponseDTO Map(Contact e)
        => new ContactResponseDTO
        {
            Id = e.Id,
            Name = e.Name,
            BirthDate = e.BirthDate,
            Sex = e.Sex.ToString(),
            Age = e.Age(clock)
        };

        private Sex ParseSex(string value)
        {
            return value.ToLower() switch
            {
                "m" or "male" => Sex.Male,
                "f" or "female" => Sex.Female,
                _ => throw new DomainException("Invalid value for Sex. Use 'M', 'F', 'Male' or 'Female'.")
            };
        }
    }
}
