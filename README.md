# Steam Login

## Project Overview
This project is a Windows application that automates the login process for Steam accounts. It utilizes the Steam Web API and various libraries to handle user authentication, including support for two-factor authentication (2FA). The application is designed to streamline the login experience for users, allowing them to manage multiple accounts efficiently.

## Prerequisites
- .NET SDK (version 8.0 or higher)
- Visual Studio or any compatible IDE

## Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/steam-login.git
   ```
2. Navigate to the project directory:

   ```bash
   cd steam-login
   ```
3. Open the solution file (`steam-login.csproj`) in Visual Studio.

## Running the Project
To start the application, run the project in your IDE or use the following command in the terminal:

```bash
# login flow
dotnet run login abc xyz
```

## Build
```bash
dotnet build

# Locate the executable
./bin/Debug/netx.x-platform/steam-login.exe
```

## Usage
- The application will attempt to log in to the specified Steam account.
- If two-factor authentication is enabled, the application will handle the 2FA process automatically.

## License
This project is licensed under the MIT License.