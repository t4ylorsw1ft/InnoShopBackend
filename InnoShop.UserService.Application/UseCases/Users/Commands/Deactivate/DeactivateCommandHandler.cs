using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Domain.Entities;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using MediatR;

namespace InnoShop.UserService.Application.UseCases.Users.Commands.Deactivate
{
    public class DeactivateCommandHandler : IRequestHandler<DeactivateCommand>
    {
        private readonly IUserRepository _userRepository;
        public DeactivateCommandHandler(IUserRepository userRepository) 
        { 
            _userRepository = userRepository;
        }

        public async Task Handle(DeactivateCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null) 
            {
                throw new NotFoundException(typeof(User), request.UserId);
            }
            user.IsActive = false;
            await _userRepository.UpdateAsync(user, cancellationToken);
        }
    }
}
