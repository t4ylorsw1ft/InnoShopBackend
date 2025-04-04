using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Domain.Entities;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using MediatR;

namespace InnoShop.UserService.Application.UseCases.Commands.Management.Deactivate
{
    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        public DeactivateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
                throw new NotFoundException(typeof(User), request.UserId);

            if (!user.IsActive)
                throw new AlreadyExistsException();

            user.IsActive = false;
            await _userRepository.UpdateAsync(user, cancellationToken);
        }
    }
}
