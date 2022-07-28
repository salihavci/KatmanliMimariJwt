FROM mcr.microsoft.com/dotnet/sdk:5.0 as build
WORKDIR /app

COPY ./KatmanliMimariJwt.Core/*.csproj ./KatmanliMimariJwt.Core/
COPY ./KatmanliMimariJwt.Data/*.csproj ./KatmanliMimariJwt.Data/
COPY ./KatmanliMimariJwt.Service/*.csproj ./KatmanliMimariJwt.Service/
COPY ./KatmanliMimariJwt.API/*.csproj ./KatmanliMimariJwt.API/
COPY ./MiniApp1.API/*.csproj ./MiniApp1.API/
COPY ./MiniApp2.API/*.csproj ./MiniApp2.API/
COPY ./MiniApp3.API/*.csproj ./MiniApp3.API/
COPY ./SharedLibrary/*.csproj ./SharedLibrary/
COPY *.sln .
RUN dotnet restore

COPY . .
RUN dotnet publish ./KatmanliMimariJwt.API/*.csproj -o /publish/

FROM mcr.microsoft.com/dotnet/aspnet:5.0 as runtime
WORKDIR /app
COPY --from=build /publish .
ENV ASPNETCORE_URLS="http://*:5000"
ENTRYPOINT [ "dotnet","KatmanliMimariJwt.API.dll" ]