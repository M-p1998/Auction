using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.DTOs;
using FluentValidation;

namespace AuctionService.Auth
{
    public class LoginValidator: AbstractValidator<LoginDto>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();;
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}