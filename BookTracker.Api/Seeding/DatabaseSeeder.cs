using BookTracker.Api.Domain.Members;
using BookTracker.Api.Security;
using BookTracker.Api.Storage;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Seeding;

public static class DatabaseSeeder
{
    public static void SeedBooks(AppDbContext dbContext, int count = 50)
    {
        if (dbContext.Books.Any())
        {
            return;
        }

        var books = BookFuzzr.Many(count);

        dbContext.Books.AddRange(books);

        dbContext.SaveChanges();
    }



    public static void SeedMembers(AppDbContext dbContext, int count = 50)
    {
        if (dbContext.Members.Any())
        {
            return;
        }

        var members = MemberFuzzr.Many(count);

        dbContext.Members.AddRange(members);

        dbContext.SaveChanges();
    }

    public static void SeedAdministrator(
    AppDbContext dbContext,
    IConfiguration configuration,
    IPasswordHasher<Member> passwordHasher)
    {
        var settings = configuration
           .GetRequiredSection(DevelopmentAdminSettings.SectionName)
           .Get<DevelopmentAdminSettings>()
           ?? throw new InvalidOperationException(
               "DevelopmentAdmin settings are missing.");

        if (settings is null ||
            string.IsNullOrWhiteSpace(settings.Password))
        {
            return;
        }


        if (string.IsNullOrWhiteSpace(settings.Name))
        {
            throw new InvalidOperationException(
                "DevelopmentAdmin:Name is missing.");
        }

        if (string.IsNullOrWhiteSpace(settings.Email))
        {
            throw new InvalidOperationException(
                "DevelopmentAdmin:Email is missing.");
        }

        if (string.IsNullOrWhiteSpace(settings.Password))
        {
            throw new InvalidOperationException(
                "DevelopmentAdmin:Password is missing.");
        }

        var email =
            new MemberEmail(settings.Email);

        var exists =
            dbContext.Members.Any(member =>
                (string)member.Email == email.Value);

        if (exists)
        {
            return;
        }

        var administrator =
            new Member
            {
                Name =
                    new MemberName(settings.Name),
                Email = email,
                PasswordHash = string.Empty,
                Role = MemberRole.Administrator
            };

        administrator.PasswordHash =
            passwordHasher.HashPassword(
                administrator,
                settings.Password);

        dbContext.Members.Add(administrator);
        dbContext.SaveChanges();
    }
}