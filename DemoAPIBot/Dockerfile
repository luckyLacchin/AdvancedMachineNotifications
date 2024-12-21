#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8443
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DemoAPIBot/DemoAPIBot.csproj", "DemoAPIBot/"]
COPY ["DemoServiceProto/DemoServiceProto.csproj", "DemoServiceProto/"]
RUN dotnet restore "DemoAPIBot/DemoAPIBot.csproj"
COPY . .
VOLUME /ServerApiDb
WORKDIR "/src/DemoAPIBot"
RUN dotnet build "DemoAPIBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DemoAPIBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoAPIBot.dll"]