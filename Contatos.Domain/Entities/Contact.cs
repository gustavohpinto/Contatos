using Contatos.Domain.Abstractions;
using Contatos.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Sex {
    Male, Female, Other
}

namespace Contatos.Domain.Entities
{
    public class Contact
    {
        private Contact() { }

        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; } = default!;
        public DateOnly BirthDate { get; private set; }
        public Sex Sex { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        public static Contact Create(string name, DateOnly birthDate, Sex sex, IClock clock)
        {
            Validate(name, birthDate, clock);
            return new Contact { Name = name.Trim(), BirthDate = birthDate, Sex = sex };
        }

        public void Update(string name, DateOnly birthDate, Sex sex, IClock clock)
        {
            Validate(name, birthDate, clock);
            Name = name.Trim();
            BirthDate = birthDate;
            Sex = sex;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate(IClock clock)
        {
            if (!IsActive)
                throw new DomainException("Contact is already inactive.");

            IsActive = false;
            UpdatedAt = clock.UtcNow();
        }

        public void Activate(IClock clock)
        {
            if (IsActive)
                throw new DomainException("Contact is already active.");

            IsActive = true;
            UpdatedAt = clock.UtcNow();
        }


        public int Age(IClock clock)
        {
            var today = DateOnly.FromDateTime(clock.UtcNow());
            int age = today.Year - BirthDate.Year;
            if (BirthDate > today.AddYears(-age)) age--;
            return age;
        }

        private static void Validate(string name, DateOnly birthDate, IClock clock)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name is required.");

            var today = DateOnly.FromDateTime(clock.UtcNow());
            if (birthDate > today) throw new DomainException("BirthDate cannot be in the future.");

            int age = CalculateAge(birthDate, today);
            if (age == 0) throw new DomainException("Age cannot be zero.");
            if (age < 18) throw new DomainException("Contact must be an adult (>= 18).");
        }

        private static int CalculateAge(DateOnly birth, DateOnly today)
        {
            int age = today.Year - birth.Year;
            if (birth > today.AddYears(-age)) age--;
            return age;
        }
    }
}
