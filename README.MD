# Booking Application
## Prerequisites
To successfully build and run this project, ensure your environment meets the following version requirements:
### .NET Version
* Required Version: .NET 8.0
* Installation:
  * Download and install the .NET 8.0 SDK from the official [.NET website](https://dotnet.microsoft.com/).
  * Verify the installation by running the following command in your terminal or command prompt:
    >$ dotnet --version
    
    _This should display 8.0.x_

### C# Version
* Required Version: C# 12
* C# 12 is supported by .NET 8.0.

>_Please use required IDE version to support these versions._



## Build Commands
To build the project, change directory to root directory.
* Clean the Build Artifacts:
Run the following command to remove any previous build outputs:

  >`$ dotnet clean`

* Build the Project:
Run the following command to compile the project:
  >`$ dotnet build`

## Running the project
* Run the Project (from root folder):
Use the dotnet run command to start the server:
  >`$ dotnet run --project src/BookingApplication.API`

## Running the tests
* Run the tests:
Use the dotnet test command to run tests:
  >`$ dotnet test`

>_This runs the application using HTTP server. Please read the official documentation to run the project using HTTPS server._

>_The application is configured with both servers. If you are using and IDE choose the HTTPS configuration to run the project._

### Configured ports
* HTTP port: 5244
* HTTPS port: 7125

## Project Structure
The application's source code is located within the `src` folder, while the test project resides in the `test` folder.

## Postman Collection
A Postman collection is included in the root folder of the project.
>`Booking.postman_collection.json`

The requests are configured for HTTPS server and port. If you are running the application in HTTP server, please change the protocol and port you are using.

## Application Configurations / Assumptions
Following are some of the **assumptions** made during the application development.
* Business hours are configurable.
* Booking durataions are configurable.
* Latest booking time is configurable.
* A booking buffer is required which is configurable, so that the user cannot make immediate bookings. _i.e. User has to book time that is at least 15 minutes after current time._
* Maximum number of simultaneous bookings are also configurable.
* Bookings cannot be made for a time slot in past.

Following table shows properties in `appsettings.json` under `Booking` property to configure the configurable properties.

| Properties        | Description           | DataType           | Default  |
| ------------- |:-------------:|:-------------:| -----:|
| BusinessHoursStartTime    | Start of the business hours | string | "09:00" |
| BusinessHoursEndTime      | End of the business hours      | string      |   "17:00" |
| TimeBeforeBusinessHoursForLastBooking | How many minutes before the end of business hours the last booking can be done      | int (minutes)      |    60 |
| BufferForBooking      | Amount of buffer time required after current time for new booking.      | int (minutes)      |   15 |
| BookingDuration      | Duration of a booking      | int (minutes)      |  59 |
| MaximumSimultaneousBookings      | Maximum number of simultaneous bookings alllowed      | int      |   4 |

>_Please change these settings according to your needs._

>_Application might always give an error saying **Booking cannot be made for a time in the past. Please select a future time for your booking.** based on the time of the days the applciation is used as it runs based on current time._

>_Please change the request body in the postman collection to suit the time of day or tweak the `Timeprovider` in `Program.cs`_

## Database Used
The application is configured with Sqlite. A new .db file is created and migration is run everytime the application is restarted. The testing is done using in-memory version of Sqlite.

