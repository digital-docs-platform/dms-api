using Application.Exceptions;
using Application.PermissionHandling;
using Application.Security.Cryptography;
using Application.UseCases;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Requests.User;
using Application.UseCases.DTO.User;
using DataAccess;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace Implementation.UseCases.EntityFramework.Commands.User
{
    public class EFCreateUserCommand : EFUseCase, ICreateUserCommand
    {
        private readonly IPasswordHasher _hasher;
        public EFCreateUserCommand(DatabaseContext context, IPasswordHasher hasher) : base(context)
        {
            _hasher = hasher;

        }

        public int Id => 1;

        public string Name => "Create User";

        public string Description => "Create new user command";

        public string RequiredPermission => PermissionCodes.UsersWrite;

        public PermissionScope Scope => PermissionScope.User;

        public async Task ExecuteAsync(CreateUserRequest request, CancellationToken ct)
        {

            var firstName = request.FirstName.Trim();
            var lastName = request.LastName.Trim();
            var email = request.Email.Trim();
            var password = request.Password;
            var jobTitle = request.JobTitle?.Trim();
            var department = request.Department?.Trim();



            var exists = await _context.Users.AnyAsync(u => u.Email == email, ct);


            if (exists)
                throw new EntityAlreadyExistsException("User with this email already exists.");


            var user = new Domain.Entities.User
            {
                Id = request.Id,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = _hasher.Hash(password),
                JobTitle = jobTitle,
                Department = department,
                IsLocked = false,
                
            };

            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);
        }
    }
}
