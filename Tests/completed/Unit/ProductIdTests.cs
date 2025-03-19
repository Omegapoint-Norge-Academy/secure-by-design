using SalesApi.Domain;
using SalesApi.Domain.DomainPrimitives;

namespace Tests.Unit;

public class ProductIdTests
{
    // Each line in the given file is a test case, and the test method runs once for each line in the file.
    public static TheoryData<string> InjectionStrings => new(File.ReadAllLines("blns-injection.txt").AsEnumerable());

    [Theory]
    [MemberData(nameof(InjectionStrings))]
    public void Constructor_Should_Reject_InvalidData(string injectionString)
    {
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ProductId(injectionString));
    }

    [Fact]
    public void Constructor_Should_Reject_EmptyData()
    {
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ProductId(null!));
        Assert.Throws<DomainPrimitiveArgumentException<string>>(() => new ProductId(string.Empty));
    }

    [Theory]
    [InlineData("14asd")]
    [InlineData("qweqweqwe")]
    [InlineData("1")]
    public void Constructor_Accept_ValidData(string validString)
    {
        Assert.Equal(new ProductId(validString).Value, validString);
    }
}