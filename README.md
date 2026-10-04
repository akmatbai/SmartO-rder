# SmartO!rder

SmartO!rder is a sample ASP.NET Core MVC application demonstrating an online store and a café ordering system.

## Features

- **Stores**
  - Access stores via a slug-based URL: `/store/{slug}`

  - View products and initiate purchases by article number or by entering the article code manually
- **Cafés**
  - Access café menus by slug and table number: `/cafe/{slug}/menu/{table}`
  - Guests can call a waiter from the menu page and browse items with images

Both systems share the same data context and use Entity Framework Core with Identity for authentication.

### Roles

The application defines several roles used throughout the dashboards:

- `Administrator`
- `CafeMerchant`
- `StoreMerchant`
- `Accountant`
- `StoreOperator`
- `CafeOperator`
- `Waiter`
- `Cook`

### Administration

- **Merchants** can manage their own stores and cafés, add products with quantities, create tables and see orders in the `/merchant/dashboard` area.
- **Administrators** manage user roles, create merchants, stores and cafés via `/admin/dashboard`.
  Authentication pages are available at `/auth-PK`.

On startup the application applies pending EF Core migrations and creates the roles. An administrator
account is created only when its credentials are configured (user secrets or environment variables), e.g.:

```bash
cd "SmartO!rder"
dotnet user-secrets set "SeedAdmin:UserName" "admin"
dotnet user-secrets set "SeedAdmin:Password" "<strong password>"
dotnet user-secrets set "SeedAdmin:Email" "admin@example.com"   # optional
```

or `SeedAdmin__UserName` / `SeedAdmin__Password` environment variables in production.

## Development

1. Ensure [.NET 8 SDK](https://dotnet.microsoft.com/) is installed.
2. Configure the `SeedAdmin` credentials (see above). Migrations are applied automatically on startup.
3. Start the application:
   ```bash
   dotnet run --project "SmartO!rder"
   ```

After running, navigate to `https://localhost:5001` and use the slugged URLs to browse stores and café menus.

