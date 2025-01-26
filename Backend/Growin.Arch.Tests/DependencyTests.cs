namespace Growin.Arch.Tests;

using FluentAssertions;
using System.Reflection;

public class DependencyTests
{
    [Test]
    public void GrowinDomain_ShouldNotDependOnOtherProjectsInSolution()
    {
        var domainAssembly = Assembly.Load("Growin.Domain");

        var solutionProjects = new[]
        {
            "Growin.ApplicationService",
            "Growin.Infra",
            "Growin.Api"
        };

        var referencedAssemblies = domainAssembly.GetReferencedAssemblies();

        foreach (var reference in referencedAssemblies)
        {
            solutionProjects.Should().NotContain(reference.Name);
        }
    }

    [Test]
    public void GrowinApplicationService_ShouldNotDependOnOtherProjectsInSolution()
    {
        var domainAssembly = Assembly.Load("Growin.ApplicationService");

        var solutionProjects = new[]
        {
            "Growin.Infra",
            "Growin.Api"
        };

        var referencedAssemblies = domainAssembly.GetReferencedAssemblies();

        foreach (var reference in referencedAssemblies)
        {
            solutionProjects.Should().NotContain(reference.Name);
        }
    }

    [Test]
    public void GrowinInfra_ShouldNotDependOnOtherProjectsInSolution()
    {
        var domainAssembly = Assembly.Load("Growin.Infra");

        var solutionProjects = new[]
        {
            "Growin.ApplicationService",
            "Growin.Api"
        };

        var referencedAssemblies = domainAssembly.GetReferencedAssemblies();

        foreach (var reference in referencedAssemblies)
        {
            solutionProjects.Should().NotContain(reference.Name);
        }
    }
}