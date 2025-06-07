# TeaShop

A modern tea shop web application built with ASP.NET Core Razor Pages.

## Prerequisites

- **.NET SDK 8.0 or later**  
  [Download .NET SDK](https://dotnet.microsoft.com/download)
- **SQL Server** (Express or full, local or remote)
- **Git** (to clone the repository)

## Getting Started

1. **Clone the repository:**
   ```sh
   git clone https://github.com/yourusername/TeaShop.git
   cd TeaShop
   ```

2. **Restore dependencies:**
   ```sh
   dotnet restore
   ```

3. **Set up the database:**
   - Make sure SQL Server is running.
   - The default connection string is in `appsettings.json`:
     ```
     "DefaultConnection": "Data Source=localhost;Initial Catalog=Vervain;Integrated Security=True;TrustServerCertificate=True"
     ```
   - If needed, update the connection string to match your SQL Server setup.

4. **Apply database migrations:**
   ```sh
   dotnet ef database update
   ```
   > If you don't have the EF Core tools, install them with:  
   > `dotnet tool install --global dotnet-ef`

5. **Run the application:**
   ```sh
   dotnet run
   ```
   - The app will be available at the URL shown in the console (e.g., http://localhost:5202).

## Default Admin Account

- **Email:** `admin@teashop.com`
- **Password:** `admin1234`

## Notes

- **Product images** uploaded via the admin panel are not tracked in git and will not be present after cloning. Only main page images are included.
- If you want to reset the database, you can delete it from SQL Server and re-run migrations.

## Project Structure

- `Pages/` — Razor Pages (UI)
- `Models/` — Entity models
- `Migrations/` — EF Core migrations
- `wwwroot/pictures/` — Main page images (included)
- `wwwroot/shop-pictures/` — Product images (excluded from git) 