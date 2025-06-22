# SmartO!rder

SmartO!rder is a sample ASP.NET Core MVC application demonstrating an online store and a café ordering system.

## Features

- **Stores**
  - Access stores via a slug-based URL: `/store/{slug}`
  - View products and initiate purchases by article number
- **Cafés**
  - Access café menus by slug and table number: `/cafe/{slug}/menu/{table}`
  - Guests can call a waiter from the menu page

Both systems share the same data context and use Entity Framework Core with Identity for authentication.

### Administration

- **Merchants** can manage their own stores and cafés, add products with quantities, create tables and see orders in the `/merchant/dashboard` area.
- **Administrators** manage user roles, create merchants, stores and cafés via `/admin/dashboard`.
  Authentication pages are available at `/auth-PK`. A default administrator
  account is created on startup with login `chakylbekov` and password
  `141221Ch!`.

## Development

1. Ensure [.NET 8 SDK](https://dotnet.microsoft.com/) is installed.
2. Restore packages and run migrations if necessary.
3. Start the application:
   ```bash
   dotnet run --project "SmartO!rder"
   ```

After running, navigate to `https://localhost:5001` and use the slugged URLs to browse stores and café menus.

