using Contatos.Domain.Entities;
using Contatos.Domain.Exceptions;
using Contatos.Tests.Helper;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contatos.Tests.Domain
{
    public class ContactTests
    {
        private readonly Clock _clock = new(new DateTime(2025, 09, 18, 12, 0, 0, DateTimeKind.Utc));

        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var birth = new DateOnly(2000, 9, 18);
            var c = Contact.Create("Maria", birth, Sex.Female, _clock);

            c.Name.Should().Be("Maria");
            c.Sex.Should().Be(Sex.Female);
            c.IsActive.Should().BeTrue();
            c.Age(_clock).Should().Be(25);
        }

        [Fact]
        public void Create_WithFutureBirthDate_ShouldThrow()
        {
            var future = new DateOnly(3000, 1, 1);
            Action act = () => Contact.Create("Ana", future, Sex.Female, _clock);
            act.Should().Throw<DomainException>().WithMessage("*future*");
        }

        [Fact]
        public void Create_WithAgeZero_ShouldThrow()
        {
            var today = DateOnly.FromDateTime(_clock.UtcNow());
            Action act = () => Contact.Create("Bebê", today, Sex.Male, _clock);
            act.Should().Throw<DomainException>().WithMessage("*zero*");
        }

        [Fact]
        public void Create_With17Years_ShouldThrow()
        {
            var seventeen = DateOnly.FromDateTime(_clock.UtcNow()).AddYears(-17);
            Action act = () => Contact.Create("João", seventeen, Sex.Male, _clock);
            act.Should().Throw<DomainException>().WithMessage("*adult*");
        }

        [Fact]
        public void Update_ShouldChangeFields()
        {
            var c = Contact.Create("Maria", new DateOnly(2000, 1, 1), Sex.Female, _clock);
            c.Update("Maria Silva", new DateOnly(1999, 12, 31), Sex.Female, _clock);

            c.Name.Should().Be("Maria Silva");
            c.BirthDate.Should().Be(new DateOnly(1999, 12, 31));
        }

        [Fact]
        public void Deactivate_ShouldSetIsActiveFalse()
        {
            var c = Contact.Create("Maria", new DateOnly(2000, 1, 1), Sex.Female, _clock);
            c.Deactivate(_clock);
            c.IsActive.Should().BeFalse();
        }
    }
}
