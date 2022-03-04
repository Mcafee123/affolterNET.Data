using affolterNET.Data.TestHelpers;
using Xunit;

namespace Example.Data.IntegrationTest
{
    public class ExampleFixture : DbFixture
    {
        public ExampleFixture() : base("CONNSTRING", "83694dd8-458d-4674-af47-af19a35a4527")
        {
        }
    }

    [CollectionDefinition(nameof(ExampleFixture))]
    public class IntegrationTestCollection : ICollectionFixture<ExampleFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
        // https://xunit.github.io/docs/shared-context.html
    }
}