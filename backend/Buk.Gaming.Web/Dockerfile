#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app


RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
        libc6-dev \
        libgdiplus \
        libx11-dev \
     && rm -rf /var/lib/apt/lists/*
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["src/Buk.Gaming.Web/Buk.Gaming.Web.csproj", "src/Buk.Gaming.Web/"]
COPY ["src/Buk.Gaming.Toornament/Buk.Gaming.Toornament.csproj", "src/Buk.Gaming.Toornament/"]
COPY ["src/Buk.Gaming.Images/Buk.Gaming.Images.csproj", "src/Buk.Gaming.Images/"]
COPY ["src/Buk.Gaming/Buk.Gaming.csproj", "src/Buk.Gaming/"]
COPY ["src/Buk.Gaming.Sanity/Buk.Gaming.Sanity.csproj", "src/Buk.Gaming.Sanity/"]
COPY ["src/Buk.Gaming.Store/Buk.Gaming.Store.csproj", "src/Buk.Gaming.Store/"]
RUN dotnet restore "src/Buk.Gaming.Web/Buk.Gaming.Web.csproj"
COPY . .
WORKDIR "/src/src/Buk.Gaming.Web"
RUN dotnet build "Buk.Gaming.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Buk.Gaming.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Buk.Gaming.Web.dll"]