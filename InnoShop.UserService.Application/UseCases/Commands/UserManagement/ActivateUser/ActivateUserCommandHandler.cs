using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Application.Interfaces.Clients;
using InnoShop.UserService.Domain.Entities;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using MediatR;

namespace InnoShop.UserService.Application.UseCases.Commands.Management.Activate
{
    public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductServiceClient _productServiceClient;

        public ActivateUserCommandHandler(
            IUserRepository userRepository,
            IProductServiceClient productServiceClient)
        {
            _userRepository = userRepository;
            _productServiceClient = productServiceClient;
        }
        public async Task Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
                throw new NotFoundException(typeof(User), request.UserId);

            if (user.IsActive)
                throw new AlreadyExistsException();

            user.IsActive = true;
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _productServiceClient.ActivateProductsByUserIdAsync(user.Id, cancellationToken);
        }
    }
}
