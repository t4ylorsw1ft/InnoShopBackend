using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Domain.Entities;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using MediatR;

namespace InnoShop.UserService.Application.UseCases.Queries.IsUserActive
{
    public class IsUserActiveCommandHandler : IRequestHandler<IsUserActiveCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public IsUserActiveCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(IsUserActiveCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
                throw new NotFoundException(typeof(User), request.UserId);

            return user.IsActive;
        }
    }
}
