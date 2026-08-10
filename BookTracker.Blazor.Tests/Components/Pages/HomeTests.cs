using Bunit;
using BookTracker.Blazor.Pages;

namespace BookTracker.Blazor.Tests.Components.Pages;

public class HomeTests : BunitContext
{
    [Fact]
    public void HidesAuthorsWhenToggleIsClicked()
    {
        var cut = Render<Home>();

        cut.Find("button").Click();

        Assert.DoesNotContain("Frank Herbert", cut.Markup);
        Assert.DoesNotContain("Raymond Chandler", cut.Markup);
    }
    [Fact]
    public void ShowsSelectedBookIdWhenBookIsSelected()
    {
        var cut = Render<Home>();

        var buttons = cut.FindAll("article button");

        buttons[0].Click();

        Assert.Contains("Geselecteerd boek: 1", cut.Markup);
    }
}