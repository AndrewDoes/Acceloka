# Acceloka: Ticket Booking Integration

Acceloka is a full-stack platform for event ticket management, featuring a .NET 8 Backend (Vertical Slice Architecture) and a Next.js 15 Frontend with **OpenID Connect (Google Auth)** integration.

---

## 🚀 How to Try the Program

### 1. Backend Setup (The API)

1. **Navigate:** `cd Backend/Acceloka.Api`
2. **Find Your Actual Port:** - Open `Properties/launchSettings.json`.
* Under the `"https"` profile, find the `"applicationUrl"`.
* **Crucial:** Your frontend needs this exact URL (e.g., `https://localhost:7289`) to talk to the database.


3. **Launch:** Run `dotnet run`.
4. **Verify:** Go to `https://localhost:[YOUR_PORT]/swagger` to see the live API docs.

### 2. Frontend Setup (The UI)

1. **Navigate:** `cd Frontend`
2. **Install:** `npm install`
3. **Configure API:** Open `app/services/api.ts`. Change the `baseURL` to match the Backend port you found in Step 1.
4. **Launch:** `npm run dev`
5. **Open Browser:** Go to `http://localhost:3000`.

---

## 🔐 Authentication (OpenID Connect / Google)

The app is configured to use **OpenID Connect** via Google. This means:

* You don't manage passwords locally; Google verifies the user.
* The `AuthContext.tsx` captures the ID Token/JWT from the login flow.
* Every API request automatically includes the `Authorization: Bearer <token>` header via an Axios interceptor in `api.ts`.

**How to Test Auth:**

* Click **Login** in the Navbar.
* Use your Google Account.
* Once authenticated, you can access "My Bookings" and checkout your cart.

---

## 📖 User Tutorial: Using Acceloka

### Step 1: Browse & Filter

* On the home page, use the **Filter Bar** to search for tickets.
* You can filter by **Category** (e.g., Concert, Sport, etc.), **Price**, and **Date**.
* *Example:* Enter "Music" and set Max Price to "500000" to see relevant concerts.

### Step 2: Booking a Ticket

1. Click the **Cart Icon** on any ticket card.
2. The **Booking Drawer** will slide out from the right.
3. Adjust your quantities.
4. Click **Book Ticket**. The app will group your tickets by category and show you a summary.

### Step 3: Managing History

1. Navigate to the **My Bookings** page.
2. Here you can see every ticket you've bought.
3. **Edit:** Change the number of people attending (updates the database instantly).
4. **Revoke:** Click the trash icon to cancel a booking and free up the ticket quota.

### Step 4: Admin Access
Admin Access offers 'Add Ticket' feature. However, to become an admin, you must change it manually in the database. Here are the steps:
1. Identify your User ID: Log in via Google first so your user record is created in the database.
2. Open your Database Tool: Use SQL Server Management Studio (SSMS), Azure Data Studio, or the dotnet ef CLI.
3. Locate the Users Table: Look for the Users table (likely in the Acceloka database).
4. Execute Update SQL: Run the following query, replacing the email with your Google account email:
```
SQL
UPDATE Users 
SET Role = 'Admin' 
WHERE Email = 'your-google-email@gmail.com';
```
5. Restart/Refresh: Once the database is updated, log out and log back in on the frontend. You will now see the "Add Ticket" button in the Navbar/Hero section.

---

## 📂 Architecture for Developers

### Backend (Vertical Slices)

Instead of giant controllers, logic is split into "Features" in `Acceloka.Api/Features/Tickets/`:

* **`AddTicket`**: Admin logic for creating events.
* **`BookTicket`**: The main transaction engine.
* **`RevokeTicket`**: Logic for handling refunds/cancellations.

### Frontend (Next.js 15)

* **`app/context/`**: Contains the global `AuthContext` for OIDC state.
* **`app/services/api.ts`**: The central hub for all API communication.
* **`app/components/`**: Modular components like `TicketGrid`, `BookingDrawer`, and `AddTicketModal`.

---

## 🛠 Troubleshooting

* **Port Mismatch:** If the frontend shows "Network Error," double-check that the port in `api.ts` matches the port running in your terminal for the Backend.
* **CORS:** Ensure `app.UseCors()` in `Program.cs` is allowing traffic from `http://localhost:3000`.
* **Database:** If no tickets appear, run `dotnet ef database update` to seed your local database.
