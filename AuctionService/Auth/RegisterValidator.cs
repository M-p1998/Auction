using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.DTOs;
using FluentValidation;

namespace AuctionService.Auth
{
    public class RegisterValidator: AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            // RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email format");
            // RuleFor(x => x.Password).NotEmpty().MinimumLength(6);


            // RuleFor(x => x.Password).NotEmpty().MinimumLength(6)
            // .WithMessage("Password must be at least 6 characters long")
            // .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            // .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            // .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");


             RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Invalid email format")
                .Must(HaveMinLocalPartLength)
                .WithMessage("Email must have at least 5 characters");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.");
        }

        private bool HaveMinLocalPartLength(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            // Split before @
            var parts = email.Split('@');
            if (parts.Length != 2) return false;

            var localPart = parts[0];

            return localPart.Length >= 5;
        }
    }
}