using SalesApi.Domain;
using SalesApi.Domain.DomainPrimitives;

namespace Tests;

[TestFixture]
public class ProductIdTests
{

    // Each line in the given file is a test case, and the test method runs once for each line in the file.
    public static string[] InjectionStrings => File.ReadAllLines("blns-injection.txt");
    [TestCaseSource(nameof(InjectionStrings))]
    public void Constructor_Should_Reject_InvalidData(string injectionString)
    {
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ProductId(injectionString));
    }

    [Test]
    public void Constructor_Should_Reject_EmptyData()
    {
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ProductId(null!));
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ProductId(string.Empty));
    }

    [TestCase("14asd")]
    [TestCase("qweqweqwe")]
    [TestCase("1")]
    public void Constructor_Accept_ValidData(string validString)
    {
        Assert.That(new ProductId(validString).Value, Is.EqualTo(validString));
    }
}