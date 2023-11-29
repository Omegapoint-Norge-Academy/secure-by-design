namespace Tests;

[TestFixture]
public class ProductIdTests
{

    // Each line in the given file is a test case
    public static string[] InjectionStrings => File.ReadAllLines("blns-injection.txt");
    [TestCaseSource(nameof(InjectionStrings))]
    public void Constructor_Should_Reject_InvalidData(string injectionString)
    {
        // Implement this test
    }

    [Test]
    public void Constructor_Should_Reject_EmptyData()
    {
        // Implement this test
    }

    [TestCase("14asd")]
    [TestCase("qweqweqwe")]
    [TestCase("1")]
    public void Constructor_Accept_ValidData(string validString)
    {
        // Implement this test
    }
}