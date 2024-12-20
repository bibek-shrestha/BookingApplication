using System;
using BookingApplication.Infrastructure.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BookingApplication.Test.Infrastructure.Helpers;

public class BookingConfigOptionHelper
{
    public static IOptions<BookingConfigOption> configureBookingOption()
    {
        var environment = "Test";
        var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.{environment}.json").Build();
        var bookingConfigOption = new BookingConfigOption();
        configuration.GetSection(BookingConfigOption.BookingConfig).Bind(bookingConfigOption);
        var options = Options.Create(bookingConfigOption);
        return options;
    }

}
