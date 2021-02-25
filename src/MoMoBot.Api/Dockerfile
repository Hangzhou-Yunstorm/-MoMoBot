FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["MoMoBot.Api/MoMoBot.Api.csproj", "MoMoBot.Api/"]
RUN dotnet restore "MoMoBot.Api/MoMoBot.Api.csproj"
COPY . .
WORKDIR "/src/MoMoBot.Api"
RUN dotnet build "MoMoBot.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MoMoBot.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MoMoBot.Api.dll"]