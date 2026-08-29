namespace MusicPlayer.ArchitectureTests;

public sealed class LayerDependencyTests
{
    [Fact]
    public void DomainDoesNotReferenceOuterLayers()
    {
        AssertDoesNotReference(
            typeof(Domain.AssemblyReference).Assembly,
            "MusicPlayer.Application",
            "MusicPlayer.Infrastructure",
            "MusicPlayer.Api");
    }

    [Fact]
    public void ApplicationDoesNotReferenceInfrastructureOrApi()
    {
        AssertDoesNotReference(
            typeof(Application.AssemblyReference).Assembly,
            "MusicPlayer.Infrastructure",
            "MusicPlayer.Api");
    }

    [Fact]
    public void InfrastructureDoesNotReferenceApi()
    {
        AssertDoesNotReference(
            typeof(Infrastructure.AssemblyReference).Assembly,
            "MusicPlayer.Api");
    }

    private static void AssertDoesNotReference(
        System.Reflection.Assembly assembly,
        params string[] forbiddenAssemblies)
    {
        var references = assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name)
            .Where(name => name is not null)
            .ToHashSet(StringComparer.Ordinal);

        foreach (var forbiddenAssembly in forbiddenAssemblies)
        {
            Assert.DoesNotContain(forbiddenAssembly, references);
        }
    }
}
