using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Members;

namespace BookTracker.Api.Tests.IntegrationTests.Domain;

public class MemberPermissionsTests
{
    [Fact]
    public void AdministratorCanViewMemberList()
    {

        var actor =
            new Actor(
                1,
                MemberRole.Administrator);

        MemberPermissions.EnsureCanViewDirectory(actor);
    }

    [Fact]
    public void MEmberCanNotViewMemberList()
    {

        var actor =
            new Actor(
                1,
                MemberRole.Member);

        Assert.Throws<ForbiddenOperationException>(
            () => MemberPermissions.EnsureCanViewDirectory(actor));
    }

    [Fact]
    public void MemberCanManageItself()
    {

        var actor =
            new Actor(
                1,
                MemberRole.Member);

        MemberPermissions.EnsureCanManage(actor, 1);
    }

    [Fact]
    public void MemberCanNotManageOtherMembers()
    {

        var actor =
            new Actor(
                1,
                MemberRole.Member);

        Assert.Throws<ForbiddenOperationException>(
            () => MemberPermissions.EnsureCanManage(actor, 2));
    }

    [Fact]
    public void AdministratorCannanageOtherMEmbers()
    {

        var actor =
            new Actor(
                1,
                MemberRole.Administrator);

        MemberPermissions.EnsureCanManage(actor, 2);


    }
}