using System;
using AdminPro.Domain.Interfaces;
using FluentAssertions;
using Xunit;

namespace AdminPro.Application.Tests.Domain;

public class IAuditableEntityTests
{
    private sealed class FakeAuditableEntity : IAuditableEntity
    {
        public int Id { get; set; }
        public bool Activo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    [Fact]
    public void ExposesAuditFields()
    {
        var createdAt = new DateTime(2026, 1, 1);
        var updatedAt = new DateTime(2026, 1, 2);

        var entity = new FakeAuditableEntity
        {
            Id = 42,
            Activo = true,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        entity.Id.Should().Be(42);
        entity.Activo.Should().BeTrue();
        entity.CreatedAt.Should().Be(createdAt);
        entity.UpdatedAt.Should().Be(updatedAt);
    }
}
