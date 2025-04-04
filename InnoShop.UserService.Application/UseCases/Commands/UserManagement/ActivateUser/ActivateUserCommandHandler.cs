using InnoShop.UserService.Application.Common.Exceptions;
using InnoShop.UserService.Application.UseCases.Commands.Management.Deactivate;
using InnoShop.UserService.Domain.Entities;
using InnoShop.UserService.Domain.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnoShop.UserService.Application.UseCases.Commands.Management.Activate
{
    public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        public ActivateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
        }
    }
}
