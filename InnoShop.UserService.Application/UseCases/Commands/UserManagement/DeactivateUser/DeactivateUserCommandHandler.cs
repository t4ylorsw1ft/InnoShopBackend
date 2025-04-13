using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Application.Interfaces.Clients;
using InnoShop.UserService.Domain.Entities;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using MediatR;

namespace InnoShop.UserService.Application.UseCases.Commands.Management.Deactivate
{
    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductServiceClient _productServiceClient;

        public DeactivateUserCommandHandler(
            IUserRepository userRepository,
            IProductServiceClient productServiceClient)
        {
            _userRepository = userRepository;
            _productServiceClient = productServiceClient;
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
            await _productServiceClient.DeactivateProductsByUserIdAsync(user.Id, cancellationToken);
        }
    }
}
