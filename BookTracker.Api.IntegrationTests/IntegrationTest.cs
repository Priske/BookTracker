using BookTracker.Api.Domain.Members;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BookTracker.Api.Application.Auth.Login;
using BookTracker.Api.IntegrationTests;

namespace BookTracker.Api.IntegrationTests;

public abstract class IntegrationTest : IAsyncLifetime
{
    private readonly PostgreSqlFixture database;
    private readonly CustomWebApplicationFactory factory;

    protected HttpClient Client { get; }
    protected EfReader Reader { get; }
    protected EfWriter Writer { get; }


    protected IntegrationTest(PostgreSqlFixture database)
    {
        this.database = database;
        factory = new CustomWebApplicationFactory(database);
        Client = factory.CreateClient();
        Reader = factory.GetReader();
        Writer = factory.GetWriter();
    }
    public Task InitializeAsync()
    {
        return database.ResetAsync();
    }
    public Task DisposeAsync()
    {
        Client.Dispose();
        factory.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Client.Dispose();
        factory.Dispose();
    }

    protected void SeedMember(
      string password = "analytical-engine")
    {
        var member = new Member
        {
            Name = new MemberName("Ada Lovelace"),
            Email = new MemberEmail("ada@example.com"),
            PasswordHash = string.Empty
        };

        var passwordHasher = new PasswordHasher<Member>();

        member.PasswordHash =
            passwordHasher.HashPassword(member, password);

        Writer.Seed(db => db.Members.Add(member));
    }

    protected async Task<int> AuthenticateAsMember(
     MemberRole role = MemberRole.Member,
     string name = "Ada Lovelace",
     string email = "ada@example.com",
     string password = "analytical-engine")
    {
        var member =
            new Member
            {
                Name = new MemberName(name),
                Email = new MemberEmail(email),
                PasswordHash = string.Empty,
                Role = role
            };

        var passwordHasher =
            new PasswordHasher<Member>();

        member.PasswordHash =
            passwordHasher.HashPassword(
                member,
                password);

        Writer.Seed(db =>
            db.Members.Add(member));

        var request =
            new LoginRequest
            {
                Email = email,
                Password = password
            };

        var response =
            await Client.PostAsJsonAsync(
                "/auth/login",
                request);

        var login =
            await response.ReadJsonAs<LoginResponse>(
                HttpStatusCode.OK);

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                login.AccessToken);

        return member.Id;
    }
}