using BookTracker.Api.Storage.Members;

namespace BookTracker.Api.Application.Auth.GetCurrentMember;

public class GetCurrentMemberQueryHandler(IMemberRepository memberRepository) : IHandler
{
    public async Task<CurrentMemberResponse?> Execute(int id)
    {
        var member = await memberRepository.GetByIdAsync(id);

        if (member is null)
        {
            return null;
        }

        return new CurrentMemberResponse
        {
            Id = member.Id,
            Name = member.Name.Value,
            Email = member.Email.Value,
            Role = member.Role.ToString()
        };
    }
}