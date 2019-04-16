using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional
{
    public class CustomWebAppFactories : IDisposable
    {
        // 4 разные фабрики нужны, чтобя хоть немного сократить время на выполнение тестов.
        public CustomWebApplicationFactory<Startup> ForRead { get; private set; }
        public CustomWebApplicationFactory<Startup> ForAdd { get; private set; }
        public CustomWebApplicationFactory<Startup> ForUpdate { get; private set; }
        public CustomWebApplicationFactory<Startup> ForDelete { get; private set; }

        public CustomWebAppFactories()
        {
            ForRead = new CustomWebApplicationFactory<Startup>(true);
            ForAdd = new CustomWebApplicationFactory<Startup>(true);
            ForUpdate = new CustomWebApplicationFactory<Startup>(true)
            {
                ContactsFileName = "write-contacts.json",
                SocialLinksFileName = "write-social-links.json"
            };
            ForDelete = new CustomWebApplicationFactory<Startup>(true);
        }

        public void Dispose()
        {
            ForRead?.Dispose();
            ForAdd?.Dispose();
            ForUpdate?.Dispose();
            ForDelete?.Dispose();
        }
    }

    [CollectionDefinition("Factories")]
    public class SharedTestResource : ICollectionFixture<CustomWebAppFactories>
    {

    }
}
