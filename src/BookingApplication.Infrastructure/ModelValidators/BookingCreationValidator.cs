using System;
using BookingApplication.Infrastructure.Models;
using FluentValidation;

namespace BookingApplication.Infrastructure.ModelValidators;

public class BookingCreationValidator: AbstractValidator<BookingCreationDto>
{
    public BookingCreationValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is Required.");
        RuleFor(x => x.Name).NotNull().WithMessage("Name is Required.");
        RuleFor(x => x.BookingTime).Matches(@"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$").WithMessage("Booking Time must be in the format HH:mm.");
    }

}
