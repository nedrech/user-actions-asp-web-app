FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Nedrech.WebApp/Nedrech.WebApp.csproj", "Nedrech.WebApp/"]
RUN dotnet restore "Nedrech.WebApp/Nedrech.WebApp.csproj"
COPY . .
WORKDIR "/src/Nedrech.WebApp"
RUN dotnet build "Nedrech.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Nedrech.WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_ENVIRONMENT Production
ENV Logging__Console__FormatterName=Simple
ENTRYPOINT ["dotnet", "Nedrech.WebApp.dll"]
