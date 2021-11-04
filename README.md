# redlink-bot

```
docker rm pg_container --force; docker volume rm redlink-bot-postgesql_redlink-db-data;
```

```
docker compose up;
```

```
dotnet ef migrations add InitialCreate --verbose -p="./src/Infrastructure" -s="./src/Bot.Pooling" -o="./Persistence/Migrations"; 
```