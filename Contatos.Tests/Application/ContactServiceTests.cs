using Contatos.Application.DTOs;
using Contatos.Application.Services;
using Contatos.Domain.Abstractions;
using Contatos.Domain.Entities;
using Contatos.Domain.Exceptions;
using Contatos.Domain.Repositories;
using Contatos.Tests.Helper;
using FluentAssertions;
using Moq;

namespace Contatos.Tests.Application;

public class ContactServiceTests
{
    private readonly Clock _clock = new(new DateTime(2025, 09, 18, 12, 0, 0, DateTimeKind.Utc));
    private readonly Mock<IContactRepository> _repo = new();

    private ContactService CreateService() => new(_repo.Object, _clock);

    [Fact]
    public async Task CreateAsync_ShouldPersistAndReturnDto()
    {
        var req = new ContactRequestDTO
        {
            Name = "John",
            BirthDate = new DateOnly(2000, 9, 18),
            Sex = "M"
        };
        _repo.Setup(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var svc = CreateService();

        var resp = await svc.CreateAsync(req, CancellationToken.None);

        resp.Name.Should().Be("John");
        resp.Age.Should().Be(25);
        resp.Sex.Should().Be("Male");

        _repo.Verify(r => r.AddAsync(It.IsAny<Contact>(), It.IsAny<CancellationToken>()), Times.Once);
        _repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        var existing = Contact.Create("Old", new DateOnly(2000, 1, 1), Sex.Female, _clock);
        _repo.Setup(r => r.GetActiveByIdAsync(existing.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(existing);
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var req = new ContactRequestDTO
        {
            Name = "New",
            BirthDate = new DateOnly(1999, 12, 31),
            Sex = "F"
        };

        var svc = CreateService();

        var resp = await svc.UpdateAsync(existing.Id, req, CancellationToken.None);

        resp.Name.Should().Be("New");
        resp.Sex.Should().Be("Female");
        _repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenNotFound()
    {
        _repo.Setup(r => r.GetActiveByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Contact?)null);

        var svc = CreateService();
        var result = await svc.GetAsync(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ListAsync_ShouldReturnOnlyActive()
    {
        var a = Contact.Create("A", new DateOnly(1990, 1, 1), Sex.Male, _clock);
        var b = Contact.Create("B", new DateOnly(1990, 1, 1), Sex.Female, _clock);
        b.Deactivate(_clock);

        _repo.Setup(r => r.ListActiveAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(new List<Contact> { a });

        var svc = CreateService();
        var list = await svc.ListAsync(CancellationToken.None);

        list.Should().HaveCount(1);
        list[0].Name.Should().Be("A");
    }

    [Fact]
    public async Task DeactivateAsync_ShouldCallSaveChanges()
    {
        var c = Contact.Create("A", new DateOnly(1990, 1, 1), Sex.Male, _clock);
        _repo.Setup(r => r.GetActiveByIdAsync(c.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(c);
        _repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
             .Returns(Task.CompletedTask);

        var svc = CreateService();
        await svc.DeactivateAsync(c.Id, CancellationToken.None);

        c.IsActive.Should().BeFalse();
        _repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenContactNotFound()
    {
        _repo.Setup(r => r.GetActiveByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Contact?)null);

        var svc = CreateService();
        var req = new ContactRequestDTO
        {
            Name = "Teste",
            BirthDate = new DateOnly(2000, 1, 1),
            Sex = "M"
        };

        Func<Task> act = async () => await svc.UpdateAsync(Guid.NewGuid(), req, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeactivateAsync_ShouldThrow_WhenContactNotFound()
    {
        _repo.Setup(r => r.GetActiveByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Contact?)null);

        var svc = CreateService();

        Func<Task> act = async () => await svc.DeactivateAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenSexIsInvalid()
    {
        var req = new ContactRequestDTO
        {
            Name = "InvalidSex",
            BirthDate = new DateOnly(2000, 1, 1),
            Sex = "X"
        };

        var svc = CreateService();

        Func<Task> act = async () => await svc.CreateAsync(req, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
                 .WithMessage("*Invalid value for Sex*");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenBirthDateInFuture()
    {
        var futureDate = new DateOnly(3000, 1, 1);
        var req = new ContactRequestDTO
        {
            Name = "FuturePerson",
            BirthDate = futureDate,
            Sex = "M"
        };

        var svc = CreateService();

        Func<Task> act = async () => await svc.CreateAsync(req, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>()
                 .WithMessage("*future*");
    }
}
