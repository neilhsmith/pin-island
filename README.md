# Pin Island

Join or form a community to enrich any website with additional insights by placing pins, visible to fellow users. Explore pins naturally as you surf the web, or utilize the Pinformative app to browse a group's feed to view all pins and their discussions in a filterable and sortable format.

## Auth Server

- **URL**: http://localhost:5454
- **Realm**: pin-island
- **Clients**:
  - **Api Swagger**: pin-island.api.swagger
  - **Web Public**: pin-island.web.public
  - **Web Admin**: pin-island.web.admin
- **Scopes**:
  - pin-island_api_read
  - pin-island_api_write
- **Test Users**:
  - **super_admin**: theadmin / Password123$
  - **admin**: admin1 / Password123$
  - **super_user**: superuser1 / Password123$
  - **user**: bob / bob
  - **user**: alice / alice

## API

- **URL**: https://localhost:5050
- **Roles**:
  - SuperAdmin
  - Admin
  - SuperUser
  - User

To start the project from the root dir:
```
dotnet run --project .\server\src\PinIsland.Api\
```

From the project's dir:
```
cd server/src/PinIsland.Api && dotnet run
```

Or debug with the vscode launch config.