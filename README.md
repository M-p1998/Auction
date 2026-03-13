# 🏷️ Auction Platform

A full-stack distributed auction system built with microservices architecture using ASP.NET Core, React, and Kubernetes.

🌐 **Live Demo:** [auctionapp.online](https://auctionapp.online)

---

## 📌 Overview

The Auction Platform allows users to create auctions, place bids in real time, search listings, and receive live notifications when outbid. It is designed as a distributed system of 5 independent microservices, each owning its own database and communicating asynchronously via RabbitMQ.

---

## 🏗️ Architecture

```
                        ┌─────────────────┐
                        │   React Frontend │
                        └────────┬────────┘
                                 │ HTTPS
                        ┌────────▼────────┐
                        │  GatewayService  │  ← YARP Reverse Proxy + JWT Validation
                        └──┬──┬──┬──┬────┘
                           │  │  │  │
          ┌────────────────┘  │  │  └─────────────────┐
          │                   │  │                     │
  ┌───────▼──────┐   ┌────────▼──┐   ┌────────────────▼──┐
  │AuctionService│   │BidService  │   │  SearchService     │
  │  PostgreSQL  │   │ PostgreSQL │   │     MongoDB        │
  └──────┬───────┘   └─────┬─────┘   └────────────────────┘
         │                 │
         └────────┬────────┘
                  │ RabbitMQ Events
         ┌────────▼────────┐
         │NotificationService│  ← SignalR WebSocket Push
         └─────────────────┘
```

---

## 🛠️ Tech Stack

| Layer | Technology | Why |
|---|---|---|
| Backend | C# / ASP.NET Core 8 | Strongly typed, enterprise-grade, great DI support |
| Frontend | React + TypeScript | Type safety for complex API responses |
| API Gateway | YARP | Native ASP.NET Core reverse proxy, centralized JWT auth |
| Message Broker | RabbitMQ + MassTransit | Async decoupled communication, retry logic |
| Auction DB | PostgreSQL + EF Core | Relational data, ACID transactions for bid consistency |
| Search DB | MongoDB | Read-optimized, flexible document schema for varied auction types |
| Caching | Redis | Fast reads for frequently accessed auction listings |
| Real-time | SignalR | WebSocket push notifications for live bid updates |
| Auth | JWT | Stateless authentication, validated at the gateway |
| Containers | Docker | Consistent environments across dev, CI, and production |
| Orchestration | Kubernetes (DigitalOcean) | Health checks, rolling deployments, secrets management |
| CI/CD | GitHub Actions | Automated build, push, and deploy on every commit to main |
| Validation | FluentValidation | Clean, declarative input validation |

---

## 📦 Services

### AuctionService (Port 7001)
- Source of truth for all auction data
- Handles auction CRUD, user registration, and JWT issuance
- Runs `AuctionEndingWorker` background service to auto-close expired auctions
- Implements the **Outbox Pattern** to guarantee RabbitMQ event delivery
- **Database:** PostgreSQL

### BidService (Port 7003)
- Validates and records all bids
- Maintains a local cache of auction state (consumed from RabbitMQ events) to avoid tight coupling with AuctionService
- Publishes `BidPlaced` events
- **Database:** PostgreSQL

### SearchService (Port 7002)
- Read-optimized service for searching and filtering auctions
- Stays in sync with AuctionService via RabbitMQ events (`AuctionCreated`, `AuctionUpdated`, `AuctionDeleted`)
- Separates read workload from write workload
- **Database:** MongoDB

### NotificationService (Port 7004)
- Manages real-time WebSocket connections via SignalR
- Consumes `BidPlaced` and `AuctionEnded` events and pushes updates to connected clients
- No database — purely event-driven

### GatewayService (Port 6001)
- Single entry point for all frontend traffic
- Routes requests to downstream services using YARP
- Validates JWT tokens before any request reaches a downstream service
- Handles CORS for the frontend domain

---

## 🔑 Key Design Decisions

### Why Microservices?
Each domain has different characteristics — bidding needs ACID consistency, search needs flexible reads, notifications need real-time push. A monolith would mix these concerns and create scaling bottlenecks.

### Why RabbitMQ over direct HTTP?
Loose coupling and resilience. If SearchService is down, AuctionService continues working — events queue up and are processed on recovery. Direct HTTP would cause cascading failures.

### Why the Outbox Pattern?
Prevents message loss. If AuctionService crashes after writing to PostgreSQL but before publishing to RabbitMQ, events would be lost. The outbox writes events atomically to the DB — a background worker then publishes them, guaranteeing at-least-once delivery.

### Why separate databases per service?
Each service owns its data — no shared database antipattern. AuctionService PostgreSQL is optimized for writes. SearchService MongoDB is optimized for reads. They stay in sync via events.

### Why JWT at the Gateway?
Centralized auth. No downstream service needs to validate tokens independently — if a request reaches BidService, it is already authenticated.

---

## 🚀 Running Locally

### Prerequisites
- Docker Desktop
- .NET 8 SDK
- Node.js 18+

### Start all services

```bash
docker-compose up
```

This starts: PostgreSQL, MongoDB, RabbitMQ, Redis, and all 5 services.

### Start the frontend

```bash
cd auction-client
npm install
npm run dev
```

Frontend runs at `http://localhost:5173`

---

## 🔐 Environment Variables

Each service reads configuration from `appsettings.json` and environment variables. Key variables:

| Variable | Description |
|---|---|
| `JwtSettings__Key` | JWT signing secret |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `RabbitMq__Host` | RabbitMQ hostname |
| `ConnectionStrings__Redis` | Redis connection string |

In production these are stored as **Kubernetes Secrets**.

---

## ☸️ Kubernetes Deployment

All Kubernetes manifests are in the `/k8s` directory.

```
k8s/
├── apps/
│   ├── auction-service.yaml
│   ├── bid-service.yaml
│   ├── gateway-service.yaml
│   ├── search-service.yaml
│   └── notification-service.yaml
└── infra/
    ├── namespace.yaml
    ├── auction-frontend.yaml
    ├── postgres.yaml
    ├── mongo.yaml
    ├── rabbitmq.yaml
    └── redis.yaml
```

### Deploy manually

```bash
kubectl apply -f k8s/infra
kubectl apply -f k8s/apps
```

### Force frontend rollout after deploy

```bash
kubectl rollout restart deployment/auction-frontend -n auction
```

---

## ⚙️ CI/CD Pipeline

GitHub Actions workflows in `.github/workflows/`:

| Workflow | Trigger | What it does |
|---|---|---|
| `ci.yml` | Push to main | Runs tests |
| `cd-docker.yml` | Push to main | Builds and pushes Docker images to DockerHub |
| `deploy.yml` | Push to main | Applies Kubernetes manifests and rolls out new images |
| `frontend-ci.yml` | Push to main | Builds and tests the React frontend |

Every push to `main` triggers the full pipeline — build → push → deploy. No manual steps.

---

## 📡 API Endpoints

### Auth
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register-user` | Register a new user |
| POST | `/api/auth/login-user` | Login and receive JWT |
| POST | `/api/auth/login-admin` | Admin login |

### Auctions
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/auctions` | List all auctions |
| GET | `/api/auctions/{id}` | Get auction by ID |
| POST | `/api/auctions` | Create auction (auth required) |
| PUT | `/api/auctions/{id}` | Update auction (auth required) |
| DELETE | `/api/auctions/{id}` | Delete auction (admin only) |

### Bids
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/bids` | Place a bid (auth required) |
| GET | `/api/bids/{auctionId}` | Get bid history for an auction |

### Search
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/search` | Search auctions with filters |

---

## 🧪 Testing

```bash
cd AuctionTests
dotnet test
```

Unit tests cover auction validation logic, bid validation, and service layer.

---

## 📈 What I'd Improve Next

- **Distributed tracing** with OpenTelemetry + Jaeger — trace a request across all 5 services in one view
- **Replace custom JWT auth** with Keycloak for OAuth2, token refresh, and user management out of the box
- **Rate limiting** at the gateway to prevent bid spamming
- **Event sourcing** for bids — store every bid event as an immutable log for full audit trail

---

## 👤 Author

**Mya Phyu**  
[GitHub](https://github.com/M-p1998) · [LinkedIn](https://linkedin.com/in/mya-phyu) · mya.p.phyu1@gmail.com
