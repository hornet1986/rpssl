using Rpssl.SharedKernel;

namespace Rpssl.Domain.GameResults;
public sealed record GameResultCreatedDomainEvent(Guid GameResultId) : IDomainEvent;
