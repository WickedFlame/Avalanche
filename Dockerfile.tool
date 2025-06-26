FROM registry-git01-lab.opacc.ch/was/server-configuration/was-nuke-build as build-env

WORKDIR /src

COPY . .
COPY .* .

ARG VERSION

RUN dotnet restore
RUN dotnet restore ./build/_build.csproj
RUN nuke deploy -version $VERSION

FROM mcr.microsoft.com/dotnet/aspnet:8.0 as runtime

WORKDIR /app

COPY --from=build-env /src/!Build/tool/ /app/

VOLUME /app/Config
EXPOSE 8080

ENTRYPOINT ["dotnet", "/app/Avalanche.Tool.exe"]