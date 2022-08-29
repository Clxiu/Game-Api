using System;
using FluentValidation;

namespace GameApi.Model
{
    public class User
    {
        public int UserId { get; set; }

        public string Username { get; set; } = default!;

        // The password should be hashed with SHA256
        // This IS still dangerous, we'll to tell students not use their passwords for other sites.
        public string Password { get; set; } = default!;

        public string Email { get; set; } = default!;
    }

    public class UserValidator: AbstractValidator<User>
    {
        public UserValidator()
        {
            // check username is not null, empty and is beteen 1 and 15 characters
            RuleFor(x => x.Username).NotNull().NotEmpty().Length(1, 15).WithMessage("Please provide a valid user name.");
            // check password is not null, empty and is beteen 6 and 12 characters
            RuleFor(x => x.Password).NotNull().NotEmpty().Length(6, 12).WithMessage("Please provide a valid password.");
            // check email is not null, empty and is valid email address
            RuleFor(x => x.Email).NotNull().NotEmpty().EmailAddress().WithMessage("Please provide a valid email.");
        }
    }
}

