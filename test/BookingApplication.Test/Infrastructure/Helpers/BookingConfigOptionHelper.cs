using System;
using BookingApplication.Infrastructure.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BookingApplication.Test.Infrastructure.Helpers;

public class BookingConfigOptionHelper
{
    public static IOptions<BookingConfigOption> configureBookingOption()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var bookingConfigOption = new BookingConfigOption();
        configuration.GetSection(BookingConfigOption.BookingConfig).Bind(bookingConfigOption);
        var options = Options.Create(bookingConfigOption);
        return options;
    }

}
