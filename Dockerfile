FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Presentation.Web/Presentation.Web.csproj", "Presentation.Web/"]
RUN dotnet restore "Presentation.Web/Presentation.Web.csproj"
COPY . .
WORKDIR "/src/Presentation.Web"
RUN dotnet build "Presentation.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Presentation.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Presentation.Web.dll"]