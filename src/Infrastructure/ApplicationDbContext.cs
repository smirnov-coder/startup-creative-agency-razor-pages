using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StartupCreativeAgency.Domain.Entities;

namespace StartupCreativeAgency.Infrastructure
{
    /// <summary>
    /// Контекст базы данных приложения.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<UserIdentity>
    {
        /// <summary>
        /// Создаёт новый экземпляр объекта контекста базы данных приложения с настройками по умолчанию.
        /// </summary>
        public ApplicationDbContext() { }
        
        /// <summary>
        /// Создаёт новый экземпляр объекта контекста базы данных с настройками, определёнными в объекте 
        /// типа <see cref="DbContextOptions{ApplicationDbContext}"/>.
        /// </summary>
        /// <param name="options">Предварительно сконфигурированные настройки контекста базы данных приложения.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Свойства объявлены виртуальными для работы Moq.

        /// <summary>
        /// Коллекция пользователей доменной модели.
        /// </summary>
        public virtual DbSet<DomainUser> DomainUsers { get; set; }
        
        /// <summary>
        /// Коллекция услуг, предлагаемых компанией.
        /// </summary>
        public virtual DbSet<ServiceInfo> Services { get; set; }
        
        /// <summary>
        /// Коллекция примеров выполненных работ.
        /// </summary>
        public virtual DbSet<WorkExample> Works { get; set; }
        
        /// <summary>
        /// Коллекция блог постов.
        /// </summary>
        public virtual DbSet<BlogPost> BlogPosts { get; set; }
        
        /// <summary>
        /// Коллекция брендов (корпоративных клиентов компании).
        /// </summary>
        public virtual DbSet<Brand> Brands { get; set; }
        
        /// <summary>
        /// Коллекция отзывов индивидуальных клиентов компании.
        /// </summary>
        public virtual DbSet<Testimonial> Testimonials { get; set; }
        
        /// <summary>
        /// Коллекция сообщений от пользователей.
        /// </summary>
        public virtual DbSet<Message> Messages { get; set; }

        // Настройка отношений базы данных производится здесь, в контексте базы данных, чтобы
        // не засорять сущности доменной модели ненужными навигационными свойстами и максимально
        // отделить доменную модель от инфраструктуры (согласно DDD).
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<BlogPost>(blogPost =>
            {
                blogPost.HasOne(x => x.CreatedBy)
                    .WithMany()
                    .OnDelete(DeleteBehavior.SetNull);

                blogPost.HasOne(x => x.LastUpdatedBy)
                    .WithMany()
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Brand>(brand =>
            {
                brand.HasOne(x => x.CreatedBy)
                    .WithMany()
                    .OnDelete(DeleteBehavior.SetNull);

                brand.HasOne(x => x.LastUpdatedBy)
                    .WithMany()
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<ServiceInfo>(serviceInfo =>
            {
                serviceInfo.HasOne(x => x.CreatedBy)
                    .WithMany()
                    .OnDelete(DeleteBehavior.SetNull);

                serviceInfo.HasOne(x => x.LastUpdatedBy)
                    .WithMany()
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Testimonial>(testimonial =>
            {
                testimonial.HasOne(x => x.CreatedBy)
                    .WithMany()
                    .OnDelete(DeleteBehavior.SetNull);

                testimonial.HasOne(x => x.LastUpdatedBy)
                    .WithMany()
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<WorkExample>(workExample =>
            {
                workExample.HasOne(x => x.CreatedBy)
                    .WithMany()
                    .OnDelete(DeleteBehavior.SetNull);

                workExample.HasOne(x => x.LastUpdatedBy)
                    .WithMany()
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<DomainUser>(domainUser =>
            {
                domainUser.HasOne(x => x.CreatedBy)
                    .WithMany()
                    .OnDelete(DeleteBehavior.SetNull);

                domainUser.HasOne(x => x.Identity as UserIdentity)
                    .WithOne()
                    .HasForeignKey(typeof(DomainUser))
                    .OnDelete(DeleteBehavior.Cascade);

                domainUser.HasOne(x => x.Profile)
                    .WithOne()
                    .HasForeignKey(typeof(UserProfile), "DomainUserId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<UserProfile>(userProfile =>
            {
                userProfile.HasMany(x => x.SocialLinks)
                    .WithOne()
                    .HasForeignKey("UserProfileId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
