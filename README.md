# ClearSettle Backoffice

<div align="center">

[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-19.0-61DAFB?style=flat-square&logo=react)](https://reactjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.x-3178C6?style=flat-square&logo=typescript)](https://www.typescriptlang.org/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3.12-FF6600?style=flat-square&logo=rabbitmq)](https://www.rabbitmq.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=flat-square&logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![License: MIT](https://img.shields.io/badge/License-MIT-22C55E?style=flat-square)](https://opensource.org/licenses/MIT)

**Plataforma de liquidação financeira de alta performance que simula o ambiente de Backoffice de uma corretora.**

Processa operações de ativos seguindo o modelo de liquidação D+2, com arquitetura orientada a eventos, mensageria assíncrona e atualizações em tempo real via WebSockets.

</div>

---

## Visão Geral

O ClearSettle implementa o ciclo completo de vida de uma ordem de compra/venda no mercado financeiro: da entrada da ordem via API REST até a liquidação final no modelo D+2, passando por filas de mensageria, workers de processamento em background e notificação em tempo real para o dashboard.

### Fluxo da Informação

```
Cliente HTTP
    │
    ▼
┌─────────────┐     trade_pending_queue     ┌──────────────┐
│   REST API  │ ──────────────────────────► │  TradeWorker │
│  (SignalR)  │                             │  (Consumer)  │
└─────────────┘                             └──────┬───────┘
       ▲                                           │ persiste
       │                                           ▼
       │  trade_settled_queue              ┌──────────────┐
       │ ◄─────────────────────────────── │  SQL Server  │
       │                                  └──────────────┘
       │ SignalR push                             ▲
       ▼                                          │ consulta
┌─────────────┐                          ┌────────┴───────┐
│  Dashboard  │                          │SettlementWorker│
│   (React)   │                          │  (a cada 60s)  │
└─────────────┘                          └────────────────┘
```

1. A **API** recebe um Trade e o publica na fila `trade_pending_queue`
2. O **TradeWorker** consome a fila e persiste a operação no **SQL Server**
3. O **SettlementWorker** verifica ordens pendentes a cada 60 segundos
4. Ao liquidar, publica na fila `trade_settled_queue`
5. A **API** escuta essa fila e dispara um evento via **SignalR**
6. O **Dashboard** atualiza a linha com efeito visual e recarrega os gráficos

---

## Funcionalidades

- **API REST** para recebimento e consulta de ordens de compra/venda
- **Processamento assíncrono** via RabbitMQ — desacoplamento total entre API e workers
- **Liquidação automática** D+2 por Worker Service em background
- **Dashboard em tempo real** com atualização via WebSockets (sem refresh manual)
- **Gráficos de volume** financeiro consolidados por ticker
- **Clean Architecture** — separação clara entre Domain, Application, Infrastructure e API

---

## Stack Tecnológica

### Backend — .NET 9

| Tecnologia | Uso |
|---|---|
| C# / .NET 9 | Web API & Worker Services |
| Entity Framework Core | ORM e migrações |
| SQL Server 2022 | Persistência relacional |
| RabbitMQ 3.12 | Message broker (filas de entrada e liquidação) |
| SignalR | Push notifications via WebSocket |
| Clean Architecture | Organização em camadas |

### Frontend — React 19

| Tecnologia | Uso |
|---|---|
| React 19 + Vite | SPA e build tool |
| TypeScript | Tipagem estrita |
| Tailwind CSS v4 | Estilização utilitária |
| Recharts | Gráficos interativos |
| Lucide React | Biblioteca de ícones |

---

## Estrutura do Projeto

```
ClearSettle/
├── src/
│   ├── SettlementService.Api/          # Web API + SignalR Hub
│   │   ├── Controllers/
│   │   ├── Hubs/
│   │   └── Program.cs
│   ├── SettlementService.Worker/       # TradeWorker + SettlementWorker
│   │   ├── Workers/
│   │   └── Program.cs
│   ├── SettlementService.Application/  # Casos de uso, interfaces, DTOs
│   ├── SettlementService.Domain/       # Entidades e regras de negócio
│   └── SettlementService.Infrastructure/ # EF Core, RabbitMQ, repositórios
├── frontend/                           # React 19 + Vite
│   ├── src/
│   │   ├── components/
│   │   ├── services/
│   │   └── types/
│   └── package.json
├── docker-compose.yml
└── README.md
```

---

## Como Executar

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js v18+](https://nodejs.org/)
- [Docker](https://www.docker.com/)

### 1. Infraestrutura (Docker)

Suba o RabbitMQ e o SQL Server com os comandos abaixo:

```bash
# RabbitMQ com painel de administração em localhost:15672
docker run -d --name rabbitmq \
  -p 5672:5672 -p 15672:15672 \
  -e RABBITMQ_DEFAULT_USER=admin \
  -e RABBITMQ_DEFAULT_PASS=admin123 \
  rabbitmq:3-management

# SQL Server 2022
docker run -d --name sqlserver \
  -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=YourStrongPassword123" \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest
```

> **Painel RabbitMQ:** acesse `http://localhost:15672` com `admin / admin123`

### 2. Backend — API

```bash
cd src/SettlementService.Api
dotnet run
```

A API ficará disponível em `http://localhost:5000`. O Swagger estará em `http://localhost:5000/swagger`.

### 3. Backend — Worker

Em um novo terminal:

```bash
cd src/SettlementService.Worker
dotnet run
```

O worker iniciará os consumidores das filas e o robô de liquidação automática.

### 4. Frontend

```bash
cd frontend
npm install
npm run dev
```

O dashboard estará disponível em `http://localhost:5173`.

---

## Uso da API

### Registrar uma Nova Ordem

```http
POST /api/Trades
Content-Type: application/json
```

```json
{
  "tickerSymbol": "WEGE3",
  "quantity": 100,
  "price": 38.50,
  "buyerAccountId": "550e8400-e29b-41d4-a716-446655440000",
  "sellerAccountId": "550e8400-e29b-41d4-a716-446655440001"
}
```

**Resposta `201 Created`:**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "tickerSymbol": "WEGE3",
  "quantity": 100,
  "price": 38.50,
  "status": "Pending",
  "tradeDate": "2025-07-10T14:32:00Z",
  "settlementDate": "2025-07-12T00:00:00Z"
}
```

### Consultar Todas as Ordens

```http
GET /api/Trades
```

### Consultar Ordem por ID

```http
GET /api/Trades/{id}
```

Acesse `http://localhost:5000/swagger` para explorar todos os endpoints com documentação interativa.

---

## Status de Liquidação

| Status | Descrição |
|---|---|
| `Pending` | Ordem recebida, aguardando processamento |
| `Settled` | Liquidada com sucesso no prazo D+2 |
| `Failed` | Falha no processo de liquidação — requer intervenção |

---

## Variáveis de Ambiente

Crie um arquivo `appsettings.Development.json` na pasta da API com:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=ClearSettle;User Id=sa;Password=YourStrongPassword123;TrustServerCertificate=True"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "admin",
    "Password": "admin123"
  }
}
```

---

## Contribuindo

Contribuições são bem-vindas! Para contribuir:

1. Faça um fork do repositório
2. Crie uma branch para sua feature (`git checkout -b feature/minha-feature`)
3. Commit suas alterações (`git commit -m 'feat: adiciona minha feature'`)
4. Push para a branch (`git push origin feature/minha-feature`)
5. Abra um Pull Request

---

## Licença

Distribuído sob a licença MIT. Consulte o arquivo [LICENSE](LICENSE) para mais informações.

---

<div align="center">

Desenvolvido por **Juliano**

</div>
