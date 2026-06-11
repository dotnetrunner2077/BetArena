# 🎰 BetArena

**BetArena** is a simulated online betting/casino platform built for learning and demonstration purposes. Players can place bets on classic casino games (Roulette, Blackjack, Slots, Poker, Baccarat) and consult a live statistics dashboard that tracks each game's RTP (Return To Player) and each user's total wagered and won amounts.

---

## 📐 Architecture

The solution follows a **Clean Architecture** layering pattern, split into 5 projects:

```
Solution1/
├── Api/                    → ASP.NET Core 8 Web API (controllers, Swagger, DI setup)
├── Applications.Layer/     → Business logic (services, interfaces, DTOs)
├── Domain/                 → EF Core entities, BettingDbContext, Migrations
├── BetArena.Web/           → React 19 + Vite frontend (SPA dashboard)
└── Test/                   → NUnit unit/integration tests
```

### Layer responsibilities

| Project | Role |
|---|---|
| `Api` | HTTP entry point. Exposes REST endpoints, configures DI, CORS, and Swagger. |
| `Applications.Layer` | Orchestrates use cases (`BetService`, `StatsService`) against the domain. |
| `Domain` | EF Core models (`User`, `Game`, `Bet`), `BettingDbContext`, and all migrations. |
| `BetArena.Web` | React SPA that lets users select a game, enter a stake/win amount, and view live stats. |
| `Test` | NUnit tests covering controllers and services with mocked dependencies. |

---

## 🗄️ Domain model

```
User  ─┐
       ├──< Bet >── Game
```

| Entity | Key fields |
|---|---|
| `User` | `Id`, `Email` |
| `Game` | `Id`, `Description`, `Rtp` (calculated, decimal) |
| `Bet`  | `Id`, `UserId`, `GameId`, `Amount`, `WinAmount`, `Result` (Win / Lose) |

**Seeded data (applied via migrations)**

| Games | Users |
|---|---|
| Roulette | alice@betarena.com (Id 1) |
| Blackjack | bob@betarena.com (Id 2) |
| Slots | carol@betarena.com (Id 3) |
| Poker | dave@betarena.com (Id 4) |
| Baccarat | eve@betarena.com (Id 5) |

---

## 🔌 API Endpoints

Base URL (development): `http://localhost:5045`

### `POST /bets` — Place a bet

Places a bet for a user on a named game. Updates the game's live RTP after each bet.

**Request body**
```json
{
  "userId":    1,
  "gameName":  "Roulette",
  "stake":     50.00,
  "winAmount": 90.00
}
```

**Responses**

| Status | Description |
|---|---|
| `201 Created` | Bet placed successfully. Returns `PlaceBetResponse`. |
| `400 Bad Request` | `stake` ≤ 0 or `winAmount` < 0. |
| `404 Not Found` | `gameName` or `userId` does not exist. |

**201 response example**
```json
{
  "betId":     42,
  "userId":    1,
  "gameName":  "Roulette",
  "stake":     50.00,
  "winAmount": 90.00,
  "result":    "Win",
  "gameRtp":   76.50
}
```

---

### `GET /stats` — Global statistics

Returns the current RTP for every game and the cumulative stake/win per user.

**Response example**
```json
{
  "games": [
    { "game": "Roulette",  "rtp": 76.50 },
    { "game": "Blackjack", "rtp": 0.00  }
  ],
  "users": [
    { "userId": 1, "totalStake": 50.00, "totalWin": 90.00 },
    { "userId": 2, "totalStake": 30.00, "totalWin": 0.00  }
  ]
}
```

---

## ▶️ How to run

### Prerequisites

| Tool | Version |
|---|---|
| [.NET SDK](https://dotnet.microsoft.com/download) | 8.0+ |
| [MySQL Server](https://dev.mysql.com/downloads/) | 8.0+ |
| [Node.js](https://nodejs.org/) | 18+ |

---

### 1. Configure the database connection

Edit `Api/appsettings.json` with your MySQL credentials:

```json
{
  "ConnectionStrings": {
    "BettingDb": "server=localhost;port=3306;database=BettingDb;user=root;password=YOUR_PASSWORD"
  }
}
```

---

### 2. Run EF Core migrations

From the solution root, apply all migrations to create and seed the database:

```powershell
dotnet ef database update --project Domain --startup-project Api
```

This runs the three migrations in order:

| Migration | What it does |
|---|---|
| `InitialCreate` | Creates `Users`, `Games`, and `Bets` tables. |
| `AddWinAmountAndGameRtp` | Adds `WinAmount` to `Bets` and `Rtp` to `Games`. |
| `SeedGamesAndUsers` | Inserts 5 games and 5 users. |

> **Tip:** To start fresh, run `dotnet ef database drop --project Domain --startup-project Api` first.

---

### 3. Start the API

```powershell
cd Api
dotnet run
```

The API listens on `http://localhost:5045`.  
Swagger UI is available at `http://localhost:5045/swagger` in Development mode.

---

### 4. Start the frontend

```powershell
cd BetArena.Web
npm install
npm run dev
```

The React app runs on `http://localhost:50062` and proxies `/bets` and `/stats` to the API automatically (configured in `vite.config.js`).

---

### 5. Run the tests

```powershell
dotnet test
```

Tests cover:

| File | What is tested |
|---|---|
| `BetsControllerTests` | HTTP responses for valid and invalid bet requests. |
| `StatsControllerTests` | HTTP response for the stats endpoint. |
| `BetServiceTests` | Stake validation, RTP calculation, error cases. |
| `StatsServiceTests` | Stats aggregation by game and user. |

---

## 🛠️ Tech stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 8, C# 12 |
| ORM | Entity Framework Core 8 (Pomelo MySQL driver) |
| Database | MySQL 8 |
| Frontend | React 19, Vite 8 |
| API docs | Swashbuckle / Swagger |
| Tests | NUnit, Moq |

---

## 📁 Key files reference

```
Api/
  Program.cs                     ← DI registration, CORS, middleware pipeline
  Controllers/BetsController.cs  ← POST /bets
  Controllers/StatsController.cs ← GET /stats

Applications.Layer/
  Services/BetService.cs         ← Bet placement logic & RTP update
  Services/StatsService.cs       ← Stats aggregation
  DTOs/                          ← PlaceBetRequest, PlaceBetResponse, StatsResponse

Domain/
  Entities/                      ← Bet, Game, User
  Data/BettingDbContext.cs        ← EF Core context with seed data
  Migrations/                    ← 3 ordered migrations

BetArena.Web/
  src/components/BetForm.jsx     ← Bet placement form
  src/components/StatsPanel.jsx  ← Live stats dashboard
  vite.config.js                 ← Dev server + API proxy
```
