using Rpssl.SharedKernel;

namespace Rpssl.Domain.Users;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
