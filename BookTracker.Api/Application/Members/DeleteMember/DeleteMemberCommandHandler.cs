using BookTracker.Api.Storage.Members;

namespace BookTracker.Api.Application.Members.DeleteMember;

public class DeleteMemberCommandHandler(IMemberRepository memberRepositoryRepository) : IHandler
{
    public async Task<bool> Execute(int id)
    {
        return await memberRepositoryRepository.DeleteAsync(id);
    }
}