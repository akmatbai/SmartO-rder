# SmartO!rder

SmartO!rder is a sample ASP.NET Core MVC application demonstrating an online store and a café ordering system.

## Features

- **Stores** — `/store/{slug}`
  - Browse products or find one by its article number
  - Checkout: quantity, customer name and phone, pickup or delivery (with address)
  - Stock is reserved atomically, so two buyers can never take the last item twice
  - Payment step through `IPaymentService` (`Services/PaymentService.cs`). The bundled
    `TestPaymentService` approves everything without charging — replace it with a real provider before going live
  - Order page `/store/{slug}/order/{id}` with status: AwaitingPayment → Paid → Completed (or Cancelled)
- **Cafés** — guests scan the table's QR code and land on `/cafe/{slug}/menu/{table}`
  - The café's own menu, grouped by category; unavailable dishes are hidden
  - Cart with quantities and a comment for the kitchen; the order is linked to the table
  - Order status page that refreshes itself: being prepared → ready → served
  - "Call waiter" button

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

- **Administrators** (`/admin/dashboard`) create store and café merchants, stores and cafés.
- **Store merchants** (`/merchant/dashboard`, `/merchant/products`) add, edit and delete products,
  see store orders with delivery details, complete paid orders and cancel unpaid ones (stock is returned).
- **Café merchants** (`/merchant/cafe/...`) manage the menu, tables (with QR codes as PNG and a printable sheet)
  and staff: they create Cook and Waiter accounts bound to one of their cafés.
- **Cooks** (`/cook/orders`) see new orders of their café and mark them ready.
- **Waiters** (`/waiter/calls`, `/waiter/orders`) see calls and ready orders of their café.
- Staff pages refresh automatically every 15 seconds.

Authentication pages are available at `/auth-PK`.

Data is stored in a local SQLite file (`smartorder.db`, see `ConnectionStrings:DefaultConnection`).
On startup the application applies pending EF Core migrations and creates the roles. An administrator
account is created only when its credentials are configured (user secrets or environment variables), e.g.:

```bash
cd "SmartO!rder"
dotnet user-secrets set "SeedAdmin:Email" "admin@example.com"
dotnet user-secrets set "SeedAdmin:Password" "<strong password>"
```

or `SeedAdmin__Email` / `SeedAdmin__Password` environment variables in production. Log in with that e-mail.

## Development

1. Ensure [.NET 8 SDK](https://dotnet.microsoft.com/) is installed.
2. Configure the `SeedAdmin` credentials (see above). Migrations are applied automatically on startup.
3. Start the application:
   ```bash
   dotnet run --project "SmartO!rder"
   ```

After running, navigate to `https://localhost:5001` and use the slugged URLs to browse stores and café menus.

