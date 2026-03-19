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
cd client   # ou le dossier de votre UI
npm install
```

### Backend

```bash
cd server   # ou le dossier de votre backend
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

## Structure du projet

```
calculatrice/
├── client/          # Interface React
│   ├── src/
│   └── package.json
└── server/          # API .NET
    ├── Controllers/
    └── *.csproj
```

## Contribuer

Les pull requests sont les bienvenues. Pour des changements majeurs, ouvrez d'abord une issue pour discuter de ce que vous souhaitez modifier.

## Licence

[MIT](LICENSE)
