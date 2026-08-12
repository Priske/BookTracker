using Bunit;
using BookTracker.Blazor.Auth;
using BookTracker.Blazor.Pages.Auth;
using Microsoft.AspNetCore.Components;
using BookTracker.Blazor.Tests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace BookTracker.Blazor.Tests.Components.Pages.Auth;

public class LogoutTests : BunitContext
{
    [Fact]
    public void LogoutRemovesTokenAndNavigatesToBookTracker()
    {
        var authSession = new FakeAuthSession
        {
            Token = "test-token"
        };

        Services.AddSingleton<IAuthSession>(authSession);
        Services.AddScoped<BookTrackerAuthenticationStateProvider>();

        var navigationManager =
            Services.GetRequiredService<NavigationManager>();

        Render<Logout>();

        Assert.Null(authSession.Token);

        Assert.EndsWith(
            "/booktracker",
            navigationManager.Uri);
    }


}