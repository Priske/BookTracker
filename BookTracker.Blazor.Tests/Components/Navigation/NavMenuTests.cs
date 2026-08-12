using Bunit;
using BookTracker.Blazor.Layout;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Authorization;

namespace BookTracker.Blazor.Tests.Layout;

public class NavMenuTests : BunitContext
{
    [Fact]
    public void AnonymousUserSeesLogin()
    {
        AddAuthorization();

        var cut = Render<NavMenu>();

        Assert.NotNull(cut.Find("a[href='login']"));
        Assert.Empty(cut.FindAll("a[href='logout']"));
    }

    [Fact]
    public void LoggedInUserSeesLogout()
    {
        var authorization = AddAuthorization();

        authorization.SetAuthorized(
            "Ada Reader",
            AuthorizationState.Authorized);

        var cut = Render<NavMenu>();

        Assert.NotNull(cut.Find("a[href='logout']"));
        Assert.Empty(cut.FindAll("a[href='login']"));
    }

    [Fact]
    public void AdministratorSeesCreateBookAction()
    {
        var authorization = AddAuthorization();

        authorization.SetAuthorized(
            "Ada Reader",
            AuthorizationState.Authorized);

        authorization.SetRoles("Administrator");

        var cut = Render<NavMenu>();

        Assert.NotNull(
            cut.Find("a[href='/books/create']"));
    }

    [Fact]
    public void MemberDoesNotSeeCreateBookAction()
    {
        var authorization = AddAuthorization();

        authorization.SetAuthorized(
            "Ada Reader",
            AuthorizationState.Authorized);

        authorization.SetRoles("Member");

        var cut = Render<NavMenu>();

        Assert.Empty(
            cut.FindAll("a[href='/books/create']"));
    }
}