#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base

ENV ExposePortForHttp=80
ENV ExposePortForHttps=443
ENV GroupNm=appuser
ENV UsrNm=appuser

ENV ASPNETCORE_URLS=http://*:${ExposePortForHttp}
ENV ASPNETCORE_ENVIRONMENT="Development"

ENV ApiProjectConfig__ConnectionString="Server=tcp:172.20.181.247,1433;Database=TestDB;User ID=sa;Password=ad13!min;"
ENV ApiProjectConfig__JwtAudience="PersonProfile_DF_Api_users"
ENV ApiProjectConfig__JwtIssuer="PersonProfile_DF_Api.com"
ENV ApiProjectConfig__JwtSecret="personprofile_637673205977617256"

WORKDIR /app

# For Local development with root user: use 80/443; For Production deployment with non-root user: use 8080/4443
EXPOSE ${ExposePortForHttp}
EXPOSE ${ExposePortForHttps}

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PersonProfile_DF.Api/PersonProfile_DF.Api.csproj", "PersonProfile_DF.Api/"]
COPY ["PersonProfile_DF.Utilities/PersonProfile_DF.Utilities.csproj", "PersonProfile_DF.Utilities/"]
COPY ["PersonProfile_DF.Business/PersonProfile_DF.Business.csproj", "PersonProfile_DF.Business/"]
RUN dotnet restore "PersonProfile_DF.Api/PersonProfile_DF.Api.csproj"
COPY . .
WORKDIR "/src/PersonProfile_DF.Api"
RUN dotnet build "PersonProfile_DF.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PersonProfile_DF.Api.csproj" -c Release -o /app/publish

FROM base AS final
#Creating new non-root group and user. Delete the following line in case of Local development where root user could be used
RUN groupadd -r ${GroupNm} && useradd -r -s /bin/false -g ${GroupNm} ${UsrNm}
WORKDIR /app
#Changing ownership of main directory and using the newly created user. Delete the following 2 lines in case of Local development where root user could be used
RUN chown -R ${UsrNm}:${GroupNm} /app
USER ${UsrNm}
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PersonProfile_DF.Api.dll"]


