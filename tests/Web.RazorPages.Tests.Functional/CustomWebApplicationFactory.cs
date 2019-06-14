using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StartupCreativeAgency.Domain.Abstractions.Repositories;
using StartupCreativeAgency.Domain.Abstractions.Services;
using StartupCreativeAgency.Domain.Entities;
using StartupCreativeAgency.Domain.Logic.Services;
using StartupCreativeAgency.Infrastructure;

namespace StartupCreativeAgency.Web.RazorPages.Tests.Functional
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>, IDisposable
    {
        // Каждая фабрика пусть будет использовать одно подключение к БД. От InMemoryDb пришлось отказаться,
        // ввиду некоторых особенностей SQLite, из-за которых тесты с InMemoryDb проходят успешно, а с
        // SQLite фейлятся.
        private SqliteConnection _connection = new SqliteConnection("DataSource=:memory:");
        private string _dataDirectory;
        private string _imagesDirectory;
        private bool _seedData;

        private string _contactsFileName = "read-contacts.json";
        public string ContactsFileName
        {
            get { return _contactsFileName; }
            set
            {
                _contactsFileName = value;
                ContactsFilePath = Path.Combine(_dataDirectory, _contactsFileName);
            }
        }

        private string _socialLinksFileName = "read-social-links.json";
        public string SocialLinksFileName
        {
            get { return _socialLinksFileName; }
            set
            {
                _socialLinksFileName = value;
                SocialLinksFilePath = Path.Combine(_dataDirectory, _socialLinksFileName);
            }
        }

        public string ContactsFilePath { get; private set; }
        public string SocialLinksFilePath { get; private set; }

        public CustomWebApplicationFactory(bool seedData = true)
        {
            _seedData = seedData;
            _dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            _imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "images");

            if (!Directory.Exists(_dataDirectory))
                Directory.CreateDirectory(_dataDirectory);
            if (!Directory.Exists(_imagesDirectory))
                Directory.CreateDirectory(_imagesDirectory);

            ContactsFilePath = Path.Combine(_dataDirectory, ContactsFileName);
            SocialLinksFilePath = Path.Combine(_dataDirectory, SocialLinksFileName);

            _connection.Open();
        }

        protected override void Dispose(bool disposing)
        {
            _connection?.Close();

            if (Directory.Exists(_dataDirectory))
                Directory.Delete(_dataDirectory, true);
            if (Directory.Exists(_imagesDirectory))
                Directory.Delete(_imagesDirectory, true);

            base.Dispose(disposing);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            // Метод будет вызван до вызова ConfigureServices класса Startup.
            builder.ConfigureServices(services =>
            {
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkProxies()
                    .AddEntityFrameworkSqlite()
                    .BuildServiceProvider();
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseLazyLoadingProxies()
                        .UseSqlite(_connection)
                        .UseInternalServiceProvider(serviceProvider);
                });
            });
            // Метод будет вызван после вызова ConfigureServices класса Startup.
            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<IFileRepository, FileRepository>(serviceProvider =>
                {
                    var environment = serviceProvider.GetRequiredService<IHostingEnvironment>();
                    return new FileRepository(environment)
                    {
                        DataDirectory = _dataDirectory,
                        ImagesDirectory = _imagesDirectory
                    };
                });
                services.AddScoped<IContactsService, ContactsService>(serviceProvider =>
                {
                    var fileService = serviceProvider.GetRequiredService<IFileService>();
                    return new ContactsService(fileService)
                    {
                        ContactsFileName = ContactsFileName,
                        SocialLinksFileName = SocialLinksFileName
                    };
                });
                if (_seedData)
                    CreateTestDataAsync(services.BuildServiceProvider()).Wait();
            });
        }

        private async Task CreateTestDataAsync(IServiceProvider services)
        {
            using (var scope = services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
                var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scopedServices.GetRequiredService<UserManager<UserIdentity>>();

                await AddRolesAsync(roleManager);
                await AddUsersAsync(db, userManager);
                await AddServicesAsync(db);
                await AddWorksAsync(db);
                await AddBlogPostsAsync(db);
                await AddBrandsAsync(db);
                await AddTestimonialsAsync(db);
                await AddMessagesAsync(db);
                await AddContactsAsync();
            }
        }

        private Task AddRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Administrator", "User" };
            Array.ForEach(roleNames, async roleName => await roleManager.CreateAsync(new IdentityRole(roleName)));
            return Task.CompletedTask;
        }

        private async Task AddUsersAsync(ApplicationDbContext db, UserManager<UserIdentity> userManager)
        {
            GetUserCollection().ForEach(async user =>
            {
                string role = user.Identity.UserName == "admin" ? "Administrator" : "User";
                string password = user.Identity.UserName == "admin" ? "Admin123" : "User123";
                await userManager.CreateAsync(user.Identity as UserIdentity, password);
                await userManager.AddToRoleAsync(user.Identity as UserIdentity, role);
                if (user.Identity.UserName != "admin")
                    user.Profile.ChangeDisplayStatus(true);
                await db.DomainUsers.AddAsync(user);
            });
            await db.SaveChangesAsync();
        }

        private List<DomainUser> GetUserCollection()
        {
            var socialLinks = new SocialLink[]
            {
                new SocialLink("Facebook", "Link #1"),
                new SocialLink("Twitter", "Link #2"),
                new SocialLink("GooglePlus", "Link #3"),
                new SocialLink("Linkedin", "Link #4")
            };
            return new List<DomainUser>
            {
                new DomainUser(1, new UserIdentity("user1", "user1@example.com"),
                    new UserProfile("FirstName #1", "LastName #1", "Job #1", "Path #1", socialLinks)),
                new DomainUser(2, new UserIdentity("user2", "user2@example.com"),
                    new UserProfile("FirstName #2", "LastName #2", "Job #2", "Path #2", socialLinks)),
                new DomainUser(3, new UserIdentity("admin", "admin@example.com"),
                    new UserProfile("Admin", "Admin", "Administrator", string.Empty)),
                new DomainUser(4, new UserIdentity("user4", "user4@example.com"),
                    new UserProfile("FirstName #4", "LastName #4", "Job #4", "Path #4", socialLinks)),
                new DomainUser(5, new UserIdentity("user5", "user5@example.com"),
                    new UserProfile("FirstName #5", "LastName #5", "Job #5", "Path #5", socialLinks))
            };
        }

        private async Task AddServicesAsync(ApplicationDbContext db)
        {
            var creator = GetUserCollection().First();
            db.Services.AddRange(new ServiceInfo[]
            {
                new ServiceInfo(1, creator)
                {
                    Caption = "Caption #1",
                    IconClass = "Class #1",
                    Description = "Description #1"
                },
                new ServiceInfo(2, creator)
                {
                    Caption = "Caption #2",
                    IconClass = "Class #2",
                    Description = "Description #2"
                },
                new ServiceInfo(3, creator)
                {
                    Caption = "Caption #3",
                    IconClass = "Class #3",
                    Description = "Description #3"
                }
            });
            await db.SaveChangesAsync();
        }

        private async Task AddWorksAsync(ApplicationDbContext db)
        {
            var users = GetUserCollection();
            db.Works.AddRange(new WorkExample[]
            {
                new WorkExample(1, users[0]) { Name = "Name #1", Category = "Category #1", ImagePath = "Path #1", Description = "Description #1" },
                new WorkExample(2, users[1]) { Name = "Name #2", Category = "Category #2", ImagePath = "Path #2", Description = "Description #2" },
                new WorkExample(3, users[2]) { Name = "Name #3", Category = "Category #4", ImagePath = "Path #3", Description = "Description #3" },
                new WorkExample(4, users[3]) { Name = "Name #4", Category = "Category #1", ImagePath = "Path #4", Description = "Description #4" },
                new WorkExample(5, users[4]) { Name = "Name #5", Category = "Category #3", ImagePath = "Path #5", Description = "Description #5" },
                new WorkExample(6, users[0]) { Name = "Name #6", Category = "Category #5", ImagePath = "Path #6", Description = "Description #6" },
                new WorkExample(7, users[1]) { Name = "Name #7", Category = "Category #1", ImagePath = "Path #7", Description = "Description #7" },
                new WorkExample(8, users[2]) { Name = "Name #8", Category = "Category #4", ImagePath = "Path #8", Description = "Description #8" },
                new WorkExample(9, users[3]) { Name = "Name #9", Category = "Category #2", ImagePath = "Path #9", Description = "Description #9" }
            });
            await db.SaveChangesAsync();
        }

        private async Task AddBlogPostsAsync(ApplicationDbContext db)
        {
            var users = GetUserCollection();
            for (int index = 1; index <= 4; index++)
            {
                db.BlogPosts.Add(new BlogPost(index, users[index - 1])
                {
                    ImagePath = $"Path #{index}",
                    Title = $"Title #{index}",
                    Category = $"Category #{index}",
                    Content = $"Content #{index}"
                });
                Thread.Sleep(100);
            }
            await db.SaveChangesAsync();
        }

        private async Task AddBrandsAsync(ApplicationDbContext db)
        {
            var users = GetUserCollection();
            db.Brands.AddRange(new Brand[]
            {
                new Brand(1, users[0]) { Name = "Name #1", ImagePath = "Path #1" },
                new Brand(2, users[1]) { Name = "Name #2", ImagePath = "Path #2" },
                new Brand(3, users[2]) { Name = "Name #3", ImagePath = "Path #3" },
                new Brand(4, users[3]) { Name = "Name #4", ImagePath = "Path #4" },
                new Brand(5, users[4]) { Name = "Name #5", ImagePath = "Path #5" }
            });
            await db.SaveChangesAsync();
        }

        private async Task AddTestimonialsAsync(ApplicationDbContext db)
        {
            var users = GetUserCollection();
            db.Testimonials.AddRange(new Testimonial[]
            {
                new Testimonial(1, users[0])
                {
                    Author = "Author #1",
                    Company = "Company #1",
                    Text = "Text #1"
                },
                new Testimonial(2, users[1])
                {
                    Author = "Author #2",
                    Company = "Company #2",
                    Text = "Text #2"
                },
                new Testimonial(3, users[2])
                {
                    Author = "Author #3",
                    Company = "Company #3",
                    Text = "Text #3"
                }
            });
            await db.SaveChangesAsync();
        }

        private async Task AddMessagesAsync(ApplicationDbContext db)
        {
            for (int index = 1; index <= 3; index++)
            {
                db.Messages.Add(new Message
                {
                    Name = $"Name #{index}",
                    Company = $"Company #{index}",
                    Email = $"Email #{index}",
                    Subject = $"Subject #{index}",
                    Text = $"Text #{index}",
                    IPAddress = "127.0.0.1",
                    IsRead = index != 3 ? true : false
                });
            }
            await db.SaveChangesAsync();
        }

        private async Task AddContactsAsync()
        {
            await File.WriteAllTextAsync(ContactsFilePath, JsonConvert.SerializeObject(new ContactInfo[]
            {
                new ContactInfo("Address") { Caption = "Caption #1", Values = new string[] { "Value #1", "Value #2", "Value #3" }},
                new ContactInfo("Phone") { Caption = "Caption #2", Values = new string[] { "Value #1", "Value #2", "Value #3" }},
                new ContactInfo("Email") { Caption = "Caption #3", Values = new string[] { "Value #1", "Value #2" }}
            }));
            await File.WriteAllTextAsync(SocialLinksFilePath, JsonConvert.SerializeObject(new Dictionary<string, string>
            {
                ["Facebook"] = "Link #1",
                ["Twitter"] = "Link #2",
                ["GooglePlus"] = "Link #3",
                ["Linkedin"] = "Link #4",
            }));
        }

        public async Task<HttpClient> CreateClientWithAuthCookiesAsync(string userName, 
            WebApplicationFactoryClientOptions options = null)
        {
            var httpClient = options == null ? CreateClient() : CreateClient(options);
            using (var scope = Server.Host.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserIdentity>>();
                var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<UserIdentity>>();
                var user = await userManager.FindByNameAsync(userName);
                signInManager.Context = new DefaultHttpContext
                {
                    RequestServices = scope.ServiceProvider
                };
                if (await signInManager.CanSignInAsync(user))
                    await signInManager.SignInAsync(user, true);
                var cookies = signInManager.Context.Response.Headers["Set-Cookie"];
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Cookie", cookies.ToArray());
            }
            return httpClient;
        }
    }
}
