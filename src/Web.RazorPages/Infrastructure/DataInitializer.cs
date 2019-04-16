using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StartupCreativeAgency.Infrastructure;
using StartupCreativeAgency.Domain.Entities;
using System.Threading;
using Bogus;

namespace StartupCreativeAgency.Web.RazorPages.Infrastructure
{
    /// <summary>
    /// Генератор начальных данных приложения.
    /// </summary>
    public static class DataInitializer
    {
        /// <summary>
        /// Заполняет хранилища данных приложения начальными данными.
        /// </summary>
        /// <param name="db">Контекст базы данных приложения.</param>
        /// <param name="userManager">Репозиторий для хранения идентичностей пользователей доменной модели в виде объекта типа 
        /// <see cref="UserManager{UserIdentity}"/>.</param>
        /// <param name="roleManager">Репозиторий для хранения ролей пользователей в виде объекта типа 
        /// <see cref="RoleManager{IdentityRole}"/>.</param>
        public static void Initialize(ApplicationDbContext db, UserManager<UserIdentity> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            db.Database.Migrate();

            InitializeUsers(userManager, roleManager, db);
            InitializeData(db);
        }

        private static void InitializeUsers(UserManager<UserIdentity> userManager, RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            CreateRoles(roleManager);
            CreateUsers(userManager, db);
        }

        private static void CreateRoles(RoleManager<IdentityRole> roleManager)
        {
            var roleNames = new List<string> { "Administrator", "User" };
            IdentityResult identityResult;
            roleNames.ForEach(roleName =>
            {
                if (roleManager.RoleExistsAsync(roleName).Result == false)
                {
                    identityResult = roleManager.CreateAsync(new IdentityRole(roleName)).Result;
                    if (!identityResult.Succeeded)
                        throw new DataInitializeException($"Unable to create role '{roleName}'.");
                }
            });
        }

        private static void CreateUsers(UserManager<UserIdentity> userManager, ApplicationDbContext db)
        {
            var socialLinks = new SocialLink[]
            {
                new SocialLink("Facebook", "http://www.facebook.com"),
                new SocialLink("Twitter", "http://www.twitter.com"),
                new SocialLink("GooglePlus", "http://www.plus.google.com"),
                new SocialLink("Linkedin", "http://www.linkedin.com")
            };
            var adminUser = new DomainUser(new UserIdentity("admin", "admin@example.com"),
                    new UserProfile("Admin", "", "Administrator", ""));
            var users = new List<DomainUser>
            {
                adminUser,
                new DomainUser(new UserIdentity("user1", "khalil.uddin@example.com"),
                    new UserProfile("MD. Khalil", "Uddin", "Head of Ideas", "~/images/member-1.jpeg", socialLinks), adminUser),
                new DomainUser(new UserIdentity("user2", "rubel.miah@example.com"),
                    new UserProfile("Rubel", "Miah", "Lead WordPress Developer", "~/images/member-2.jpeg", socialLinks), adminUser),
                new DomainUser(new UserIdentity("user3", "shamim.mia@example.com"),
                    new UserProfile("Shamim", "Mia", "Sr. Web Developer", "~/images/member-3.jpeg", socialLinks), adminUser),
                new DomainUser(new UserIdentity("user4", "john.doe@example.com"),
                    new UserProfile("John", "Doe", "Front-end Developer", "~/images/member-4.jpeg", socialLinks), adminUser)
            };
            IdentityResult identityResult;
            users.ForEach(user =>
            {
                if (userManager.FindByNameAsync(user.Identity.UserName).Result == null)
                {
                    string password = user.Identity.UserName == adminUser.Identity.UserName ? "Admin123" : "User123";
                    string role = user.Identity.UserName == adminUser.Identity.UserName ? "Administrator" : "User";
                    try
                    {
                        if (user.Identity.UserName != adminUser.Identity.UserName)
                            user.Profile.ChangeDisplayStatus(true);
                    }
                    catch (InvalidOperationException)
                    {
                        // Ничего не делать. Оставить значение DisplayAsTeamMember = false.
                    }
                    identityResult = userManager.CreateAsync((UserIdentity)user.Identity, password).Result;
                    if (!identityResult.Succeeded)
                        throw new DataInitializeException($"Unable to create identity for '@{user.Identity.UserName}'.");
                    identityResult = userManager.AddToRoleAsync((UserIdentity)user.Identity, role).Result;
                    if (!identityResult.Succeeded)
                        throw new DataInitializeException($"Unable to add user '@{user.Identity.UserName}' to role '{role}'.");
                }
                if (db.DomainUsers.FirstOrDefault(x => x.Identity.UserName == user.Identity.UserName) == null)
                {
                    try
                    {
                        db.DomainUsers.Add(user);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw new DataInitializeException($"Unable to add 'DomainUser' for '@{user.Identity.UserName}'.", ex);
                    }
                }
            });
        }

        private static void InitializeData(ApplicationDbContext db)
        {
            var adminUser = db.DomainUsers.FirstOrDefault(x => x.Identity.UserName == "admin");
            AddServices(db, adminUser);
            AddWorks(db, adminUser);
            AddBrands(db, adminUser);
            AddTestimonials(db, adminUser);
            AddBlogPosts(db, adminUser);
        }

        private static void AddServices(ApplicationDbContext db, DomainUser creator)
        {
            if (!db.Services.Any())
            {
                string description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit. " +
                    "Minima maxime quam architecto quo inventore harum ex magni, dicta impedit. Lorem ipsum.";
                var serviceInfos = new List<ServiceInfo>
                {
                    new ServiceInfo(creator)
                    {
                        IconClass = "fa fa-font",
                        Caption = "Clean Typography",
                        Description = description
                    },
                    new ServiceInfo(creator)
                    {
                        IconClass = "fa fa-code",
                        Caption = "Rock Solid Code",
                        Description = description
                    },
                    new ServiceInfo(creator)
                    {
                        IconClass = "fa fa-support",
                        Caption = "Expert Support",
                        Description = description
                    }
                };
                try
                {
                    serviceInfos.ForEach(serviceInfo =>
                    {
                        db.Services.Add(serviceInfo);
                        db.SaveChanges();
                    });
                }
                catch (Exception ex)
                {
                    throw new DataInitializeException("Unable to add 'Services'.", ex);
                }
            }
        }

        private static void AddWorks(ApplicationDbContext db, DomainUser creator)
        {
            if (!db.Works.Any())
            {
                var faker = new Faker();
                var workExamples = new List<WorkExample>
                {
                    new WorkExample(creator)
                    {
                        Name = "Hair Dresser",
                        ImagePath = "~/images/work-1-1.jpeg",
                        Category = "Branding",
                        Description = faker.Lorem.Sentences(faker.Random.Number(5, 15), " ")
                    },
                    new WorkExample(creator)
                    {
                        Name = faker.Commerce.ProductName(),
                        ImagePath = "~/images/work-1-2.jpeg",
                        Category = "Design",
                        Description = faker.Lorem.Sentences(faker.Random.Number(5, 15), " ")
                    },
                    new WorkExample(creator)
                    {
                        Name = faker.Commerce.ProductName(),
                        ImagePath = "~/images/work-1-3.jpeg",
                        Category = "Branding",
                        Description = faker.Lorem.Sentences(faker.Random.Number(5, 15), " ")
                    },
                    new WorkExample(creator)
                    {
                        Name = faker.Commerce.ProductName(),
                        ImagePath = "~/images/work-2-1.jpeg",
                        Category = "Development",
                        Description = faker.Lorem.Sentences(faker.Random.Number(5, 15), " ")
                    },
                    new WorkExample(creator)
                    {
                        Name = faker.Commerce.ProductName(),
                        ImagePath = "~/images/work-2-2.jpeg",
                        Category = "Strategy",
                        Description = faker.Lorem.Sentences(faker.Random.Number(5, 15), " ")
                    },
                    new WorkExample(creator)
                    {
                        Name = faker.Commerce.ProductName(),
                        ImagePath = "~/images/work-2-3.jpeg",
                        Category = "Branding",
                        Description = faker.Lorem.Sentences(faker.Random.Number(5, 15), " ")
                    },
                    new WorkExample(creator)
                    {
                        Name = faker.Commerce.ProductName(),
                        ImagePath = "~/images/work-3-1.jpeg",
                        Category = "Development",
                        Description = faker.Lorem.Sentences(faker.Random.Number(5, 15), " ")
                    },
                    new WorkExample(creator)
                    {
                        Name = faker.Commerce.ProductName(),
                        ImagePath = "~/images/work-3-2.jpeg",
                        Category = "Design",
                        Description = faker.Lorem.Sentences(faker.Random.Number(5, 15), " ")
                    },
                    new WorkExample(creator)
                    {
                        Name = faker.Commerce.ProductName(),
                        ImagePath = "~/images/work-3-3.jpeg",
                        Category = "Branding",
                        Description = faker.Lorem.Sentences(faker.Random.Number(5, 15), " ")
                    }
                };
                try
                {
                    workExamples.ForEach(workExample =>
                    {
                        db.Works.Add(workExample);
                        db.SaveChanges();
                    });
                }
                catch (Exception ex)
                {
                    throw new DataInitializeException("Unable to add 'Works'.", ex);
                }
            }
        }

        private static void AddBrands(ApplicationDbContext db, DomainUser creator)
        {
            if (!db.Brands.Any())
            {
                var brands = new List<Brand>
                {
                    new Brand(creator)
                    {
                        ImagePath = "~/images/client-1.png",
                        Name = "Deorham Networks"
                    },
                    new Brand(creator)
                    {
                        ImagePath = "~/images/client-2.png",
                        Name = "Ratings"
                    },
                    new Brand(creator)
                    {
                        ImagePath = "~/images/client-3.png",
                        Name = "Malik Media"
                    },
                    new Brand(creator)
                    {
                        ImagePath = "~/images/client-4.png",
                        Name = "Bcause"
                    },
                    new Brand(creator)
                    {
                        ImagePath = "~/images/client-5.png",
                        Name = "Wompify"
                    }
                };
                try
                {
                    brands.ForEach(brand =>
                    {
                        db.Brands.Add(brand);
                        db.SaveChanges();
                    });
                }
                catch (Exception ex)
                {
                    throw new DataInitializeException("Unable to add 'Brands'.", ex);
                }
            }
        }

        private static void AddTestimonials(ApplicationDbContext db, DomainUser creator)
        {
            if (!db.Testimonials.Any())
            {
                var faker = new Faker();
                var testimonials = new List<Testimonial>
                {
                    new Testimonial(creator)
                    {
                        Author = "John Doe",
                        Company = "Google Inc.",
                        Text = "Lorem ipsum dolor sit amet consectetur adipisicing elit. Saepe fugit perferendis ratione "
                            + "ducimus, modi recusandae eos aperiam blanditiis asperiores autem nulla sint laborum tenetur, "
                            + "amet iste nisi nam adipisci aliquid?"
                    },
                    new Testimonial(creator)
                    {
                        Author = faker.Person.FullName,
                        Company = faker.Company.CompanyName(),
                        Text = faker.Lorem.Sentences(5, " ")
                    },
                    new Testimonial(creator)
                    {
                        Author = faker.Person.FullName,
                        Company = faker.Company.CompanyName(),
                        Text = faker.Lorem.Sentences(7, " ")
                    }
                };
                try
                {
                    testimonials.ForEach(testimonial =>
                    {
                        db.Testimonials.Add(testimonial);
                        db.SaveChanges();
                    });
                }
                catch (Exception ex)
                {
                    throw new DataInitializeException("Unable to add 'Testimonials'.", ex);
                }
            }
        }

        private static void AddBlogPosts(ApplicationDbContext db, DomainUser creator)
        {
            if (!db.BlogPosts.Any())
            {
                var faker = new Faker();
                var categories = new string[]
                {
                    "Branding", "Design", "Development", "Strategy"
                };
                var blogPosts = new List<BlogPost>();
                for (int i = 0; i < 5; i++)
                {
                    blogPosts.Add(new BlogPost(creator)
                    {
                        Title = faker.Lorem.Sentence(faker.Random.Number(3, 7)),
                        Category = faker.PickRandom(categories),
                        ImagePath = faker.Image.PicsumUrl(faker.Random.Number(500, 900), faker.Random.Number(500, 900)),
                        Content = faker.Lorem.Paragraphs(faker.Random.Number(5, 20), "<br /><br />")
                    });
                    Thread.Sleep(100);
                }
                blogPosts.Add(new BlogPost(creator)
                {
                    Title = "User Interface Designing Elements",
                    Category = "Design",
                    ImagePath = "~/images/blog-2.jpeg",
                    Content = faker.Lorem.Paragraphs(faker.Random.Number(5, 20), "<br /><br />")
                });
                Thread.Sleep(100);
                blogPosts.Add(new BlogPost(creator)
                {
                    Title = "Startup ideas needs to be funded",
                    Category = "Development",
                    ImagePath = "~/images/blog-1.jpeg",
                    Content = faker.Lorem.Paragraphs(faker.Random.Number(5, 20), "<br /><br />")
                });
                try
                {
                    blogPosts.ForEach(blogPost =>
                    {
                        db.BlogPosts.Add(blogPost);
                        db.SaveChanges();
                    });
                }
                catch (Exception ex)
                {
                    throw new DataInitializeException("Unable to add 'BlogPosts'.", ex);
                }
            }
        }
    }

    [Serializable]
    public class DataInitializeException : Exception
    {
        public DataInitializeException() { }
        
        public DataInitializeException(string message) : base(message) { }
        
        public DataInitializeException(string message, Exception inner) : base(message, inner) { }
        
        protected DataInitializeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
