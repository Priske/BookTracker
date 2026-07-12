

namespace BookTracker.Api.Domain.Members;

public class MemberEmailAlreadyExistsException()
    : DomainException("A member with this email already exists.");