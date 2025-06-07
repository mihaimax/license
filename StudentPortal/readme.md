# StudentPortal

A web application built with C# 12 and .NET 8, using Entity Framework Core and ASP.NET Core Identity.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
- SQL Server (local or remote instance)

## Required NuGet Packages

Before building the solution, ensure the following NuGet packages are installed with the specified versions:
dotnet add package EntityFramework --version 6.5.1 
dotnet add package MailKit --version 4.12.1 
dotnet add package Microsoft.AspNetCore.DataProtection --version 9.0.5 
dotnet add package Microsoft.AspNetCore.Identity --version 2.3.1 
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 9.0.5 
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.5 
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.5 
dotnet add package Microsoft.Extensions.Identity.Core --version 9.0.5 
dotnet add package MimeKit --version 4.12.0

You can install all packages by running the above commands in the project directory.

## Build & Run Instructions

1. **Clone the repository** 
- git clone https://github.com/mihaimax/license.git 

- cd StudentPortal

3. **Apply database migrations**  
- Ensure your connection string is set in `appsettings.json` and run the following command to apply migrations:
 **dotnet ef database update**

4. **Build the solution** 
- dotnet build

5. **Run the application**
- dotnet run


## Additional Notes

- Make sure your SQL Server instance is running and accessible.
- If you encounter issues with migrations, verify that the `Microsoft.EntityFrameworkCore.Tools` package is installed.
- For development, Visual Studio 2022 is recommended for the best experience.

---

**Happy coding!**