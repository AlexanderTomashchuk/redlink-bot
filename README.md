# redlink-bot

```
docker rm pg_container --force; docker volume rm redlink-bot-postgesql_redlink-db-data;
```

```
docker compose up -d;
```

```
dotnet ef migrations add InitialCreate --verbose -p="./src/Infrastructure" -s="./src/Bot.WebHook" -o="./Persistence/Migrations"; 
```

```
ngrok http 5001
```
