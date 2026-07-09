using BookTracker.Api.Domain;
using BookTracker.Api.Domain.Members;
using BookTracker.Api.Storage.Members;
using Microsoft.AspNetCore.Identity;

namespace BookTracker.Api.Application.Members.CreateMember;

public class CreateMemberCommandHandler(
    IMemberRepository memberRepository,
    IPasswordHasher<Member> passwordHasher) : IHandler
{

    public async Task<CreateMemberResponse> Execute(CreateMemberRequest request)
    {

        var mail = new MemberEmail(request.Email);
        if (await memberRepository.EmailExistsAsync(mail))
        {
            throw new MemberEmailAlreadyExistsException();
        }

        var member =
            new Member
            {
                Name = new MemberName(request.Name),
                Email = mail

            };
        //Whitespace still legal???
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new DomainException("Password is required.");
        }

        if (request.Password.Length < 8)
        {
            throw new DomainException("Password must contain at least 8 characters.");
        }
        member.PasswordHash = passwordHasher.HashPassword(member, request.Password);

        var savedMember = await memberRepository.AddAsync(member);
        return
            new CreateMemberResponse
            {
                Id = savedMember.Id,
                Name = savedMember.Name,
                Email = savedMember.Email,

            };
    }

}
