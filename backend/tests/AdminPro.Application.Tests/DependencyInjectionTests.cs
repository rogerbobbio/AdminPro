using System.Linq;
using AdminPro.Infrastructure.Persistence;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ApplicationDI = AdminPro.Application.DependencyInjection;

namespace AdminPro.Application.Tests;

public class DependencyInjectionTests
{
    public record FakeRequest(string Name) : IRequest<string>;

    [Fact]
    public void AddApplicationServices_RegistersPipelineBehaviorsInOrder()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<AppDbContext>(options => options.UseSqlite("DataSource=:memory:"));

        ApplicationDI.AddApplicationServices(services);

        using var provider = services.BuildServiceProvider();
        var behaviors = provider.GetServices<IPipelineBehavior<FakeRequest, string>>().ToList();

        behaviors.Should().HaveCount(3);
        behaviors[0].Should().BeOfType<AdminPro.Application.Common.Behaviors.ValidationBehavior<FakeRequest, string>>();
        behaviors[1].Should().BeOfType<AdminPro.Application.Common.Behaviors.LoggingBehavior<FakeRequest, string>>();
        behaviors[2].Should().BeOfType<AdminPro.Application.Common.Behaviors.TransactionBehavior<FakeRequest, string>>();
    }
}
