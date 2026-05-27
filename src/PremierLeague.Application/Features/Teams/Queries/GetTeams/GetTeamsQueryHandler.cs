using MediatR;
using PremierLeague.Application.Common.Models;
using PremierLeague.Application.Contracts.Responses;
using PremierLeague.Domain.Interfaces;

namespace PremierLeague.Application.Features.Teams.Queries.GetTeams;

public sealed class GetTeamsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetTeamsQuery, PaginatedResult<TeamResponse>>
{
    public async Task<PaginatedResult<TeamResponse>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
    {
        var teams = await uow.Teams.GetAllAsync(cancellationToken);

        var filtered = teams.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
            filtered = filtered.Where(t =>
                t.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase) ||
                t.ShortName.Contains(request.Search, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.City))
            filtered = filtered.Where(t => t.City.Equals(request.City, StringComparison.OrdinalIgnoreCase));

        filtered = request.SortBy?.ToLower() switch
        {
            "city" => request.Descending ? filtered.OrderByDescending(t => t.City) : filtered.OrderBy(t => t.City),
            "foundedyear" => request.Descending ? filtered.OrderByDescending(t => t.FoundedYear) : filtered.OrderBy(t => t.FoundedYear),
            "stadiumcapacity" => request.Descending ? filtered.OrderByDescending(t => t.StadiumCapacity) : filtered.OrderBy(t => t.StadiumCapacity),
            _ => request.Descending ? filtered.OrderByDescending(t => t.Name) : filtered.OrderBy(t => t.Name)
        };

        var totalCount = filtered.Count();
        var items = filtered
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TeamResponse(t.Id, t.Name, t.ShortName, t.Stadium, t.StadiumCapacity, t.Coach, t.FoundedYear, t.City, t.PrimaryColor))
            .ToList();

        return PaginatedResult<TeamResponse>.Create(items, totalCount, request.PageNumber, request.PageSize);
    }
}
