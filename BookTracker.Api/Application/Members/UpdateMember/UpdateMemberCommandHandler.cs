
using BookTracker.Api.Domain.Actors;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage.Members;

namespace BookTracker.Api.Application.Members.UpdateMember;

public class UpdateMemberCommandHandler(IMemberRepository memberRepository) : IHandler
{
    public async Task<bool> Execute(
        Actor actor,
        int id,
        UpdateMemberRequest request)
    {
        MemberPermissions.EnsureCanManage(actor, id);
        var mail = new MemberEmail(request.Email);
        if (await memberRepository.EmailExistsAsync(mail, id))
        {
            throw new MemberEmailAlreadyExistsException();
        }
        var member =
                new Member
                {
                    Id = id,
                    Name = new MemberName(request.Name),
                    Email = new MemberEmail(request.Email)
                };

        return await memberRepository.UpdateAsync(member);
    }
}
