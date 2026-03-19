# 🧮 Calculatrice

Une application de calculatrice avec une interface frontend et un backend API.

## Stack technique

- **Frontend** — React (Vite)
- **Backend** — .NET (ASP.NET Core)

## Prérequis

- [Node.js](https://nodejs.org/) (v18+)
- [.NET SDK](https://dotnet.microsoft.com/download) (v8+)

## Installation

### Cloner le dépôt

```bash
git clone https://github.com/votre-utilisateur/calculatrice.git
cd calculatrice
```

### Frontend

```bash
cd CalculatriceUi
npm install
```

### Backend

```bash
cd CalculatriceApi/CalculatriceApi
dotnet restore
```

## Démarrage

Lancer les deux serveurs **dans des terminaux séparés** :

**Frontend**
```bash
npm run dev
```

**Backend**
```bash
dotnet run
```

L'application sera disponible par défaut sur `http://localhost:5173` (Vite) et l'API sur `http://localhost:5000` (ou le port configuré dans `launchSettings.json`).

