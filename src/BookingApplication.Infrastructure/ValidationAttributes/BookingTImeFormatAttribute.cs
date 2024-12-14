using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace BookingApplication.Infrastructure.ValidationAttributes;

public class BookingTImeFormatAttribute : ValidationAttribute
{
    private const string BookingTimeFormat = "HH:mm";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }
        if (value is string bookingTimeString)
        {
            if (DateTime.TryParseExact(bookingTimeString, BookingTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult($"The field {validationContext.DisplayName} must be in the format {BookingTimeFormat}.");
        }
        return new ValidationResult($"The field {validationContext.DisplayName} must be a string.");
    }

}
