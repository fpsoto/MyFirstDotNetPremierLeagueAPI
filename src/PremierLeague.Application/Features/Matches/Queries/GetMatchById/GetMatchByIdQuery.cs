using MediatR;
using PremierLeague.Application.Common.Models;
using PremierLeague.Application.Contracts.Responses;

namespace PremierLeague.Application.Features.Matches.Queries.GetMatchById;

public record GetMatchByIdQuery(Guid MatchId) : IRequest<Result<MatchResponse>>;
