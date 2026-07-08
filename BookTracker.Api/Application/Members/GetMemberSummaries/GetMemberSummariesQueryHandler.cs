using BookTracker.Api.Storage;
using Microsoft.EntityFrameworkCore;

namespace BookTracker.Api.Application.Members.GetMemberSummaries;


public class GetMemberSummariesQueryHandler(AppDbContext dbContext) : IHandler
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 10;
    private const int MinPage = 1;
    private const int MaxPageSize = 50;

    public async Task<GetMemberSummariesResponse> Execute(GetMemberSummariesRequest request)
    {
        var page = Math.Max(1, request.Page ?? DefaultPage);
        var pageSize = Math.Clamp(request.PageSize ?? DefaultPageSize, MinPage, MaxPageSize);

        var query = dbContext.Members.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = $"%{request.Search.Trim()}%";

            query = query.Where(book =>
                EF.Functions.Like((string)book.Name, search) ||
                EF.Functions.Like((string)book.Email, search));
        }

        var totalItems = await query.CountAsync();

        var members = await query
            .OrderBy(member => member.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(member =>
                new MemberSummary
                {
                    Id = member.Id,
                    Name = member.Name,
                    Email = member.Email
                })
            .ToListAsync();

        return
            new GetMemberSummariesResponse
            {
                Items = members,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
    }
}