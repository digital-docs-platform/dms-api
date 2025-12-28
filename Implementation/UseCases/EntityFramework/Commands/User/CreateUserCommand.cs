using Application.Security.Cryptography;
using Application.UseCases.Commands;
using Application.UseCases.DTO.User;
using DataAccess;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace Implementation.UseCases.EntityFramework.Commands.User
{
    public class CreateUserCommand : EFUseCase, ICreateUserCommand
    {
        private readonly IPasswordHasher _hasher;
        public CreateUserCommand(DatabaseContext context, IPasswordHasher hasher) : base(context)
        {
            _hasher = hasher;

        }

        public IEnumerable<string> RequiredPermissions => new List<string> {"User.Create"};

        public int Id => 1;

        public string Name => "Create User";

        public string Description => "Create new user command";

        public async Task ExecuteAsync(CreateUserDto request, CancellationToken ct)
        {
            if (request is null)
                throw new ValidationException("Request is null.");

            var firstName = request.FirstName?.Trim();
            var lastName = request.LastName?.Trim();
            var email = request.Email?.Trim();
            var password = request.Password;

            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException("Email is required.");
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ValidationException("First Name is required.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ValidationException("Last Name is required. ");
            if (string.IsNullOrWhiteSpace(password))
                throw new ValidationException("Password is required.");

            var exists = await _context.Users.AnyAsync(u => u.Email == email, ct);


            //if (exists)
                //throw new ConflictException("User with this email already exists.");


            var user = new Domain.Entities.User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = _hasher.Hash(password),
            };

            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);
        }
    }
}
