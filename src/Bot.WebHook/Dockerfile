FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Bot.WebHook/Bot.WebHook/Bot.WebHook.csproj", "Bot.WebHook/"]
RUN dotnet restore "Bot.WebHook/Bot.WebHook/Bot.WebHook.csproj"
COPY . .
WORKDIR "/src/Bot.WebHook"
RUN dotnet build "Bot.WebHook.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bot.WebHook.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bot.WebHook.dll"]
