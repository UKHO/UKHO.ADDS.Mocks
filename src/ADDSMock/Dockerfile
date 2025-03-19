FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ADDSMock.csproj .
RUN dotnet restore ./ADDSMock.csproj
COPY . .
RUN dotnet build ./ADDSMock.csproj -c $BUILD_CONFIGURATION  -o /app/build
 
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/build .
 
 
ENTRYPOINT ["dotnet", "ADDSMock.dll"]
EXPOSE 8080 
