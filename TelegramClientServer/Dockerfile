#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8008
EXPOSE 44333

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TelegramClientServer/TelegramClientServer.csproj", "TelegramClientServer/"]
COPY ["DemoServiceProto/DemoServiceProto.csproj", "DemoServiceProto/"]
RUN dotnet restore "TelegramClientServer/TelegramClientServer.csproj"
COPY . .
WORKDIR "/src/TelegramClientServer"
RUN dotnet build "TelegramClientServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelegramClientServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelegramClientServer.dll"]