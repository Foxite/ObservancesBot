FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ObservancesBot/ObservancesBot.csproj", "ObservancesBot/"]
RUN dotnet restore "ObservancesBot/ObservancesBot.csproj"
COPY . .
WORKDIR "/src/ObservancesBot"
RUN dotnet build "ObservancesBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ObservancesBot.csproj" -c Release -o /app/publish

FROM base AS final
RUN apt update && apt -y install cron
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["ObservancesBot/docker/bin/", "/usr/local/bin/"]
RUN bash -c 'chmod +x /usr/local/bin/{run_on_schedule,schedule}'
CMD /usr/local/bin/schedule
