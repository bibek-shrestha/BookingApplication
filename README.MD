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


