# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DrPioMuseumSurveyAPI/DrPioMuseumSurveyAPI.csproj", "DrPioMuseumSurveyAPI/"]
RUN dotnet restore "DrPioMuseumSurveyAPI/DrPioMuseumSurveyAPI.csproj"
COPY . .
WORKDIR "/src/DrPioMuseumSurveyAPI"
RUN dotnet publish "DrPioMuseumSurveyAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "DrPioMuseumSurveyAPI.dll"]