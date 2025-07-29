# Avalanche
![Avalanche](./icons/avalanche-logo-gray.png)
Loadtesting for Web Applications

## App
### Inmemory
Volumes
- /app/data
- /app/scenarios or /scenarios

### Postgres
Volumes
- /app/scenarios or /scenarios

| Name              | Value       |   | 
|-------------------|-------------|---|
| AV_EVENT_STORE_DB | pgsql       |   |
| AV_READ_MODEL_DB  | pgsql       |   |
| AV_DB             | pgsql       |   |
| AV_DB_SERVER      |             |   |
| AV_DB_PORT        |             |   |
| AV_DB_USERNAME    |             |   |
| AV_DB_PASSWORD    |             |   |
| SCENARIO_PATH     | ./scenarios | Optional path to the scenarios. This has to be same as the volume. |
| DATA_PATH         |             |   |



## Executor
| Short | long         |   |
|-------|--------------|---|
| -f    | --configfile |   |
| -u    | --url        |   |


## Domain
```
Scenario:
  Name: name of the scenario (same as filename)
  TestCases: <- refactor to TestCases
    - Name: name of the test
      Users: amount of threads
      Urls:
      - 'https://host.docker.internal:32770/'
      Iterations: 10
      Duration: 0
      Interval: 0
      RampupTime: 0
      UseCookies: True
      Delay: 1
      Init:
        Url: 'https://host.docker.internal:32770/'
```

```
TestRun:
  TestId: unique id for each testrun
  Scenario: name of the scenario
  TestCases:
    - TestName: <- refactor to TestCase. name of the test. taken from the scenario config
      TestCase: name of the test. taken from the scenario config
      Id: generated id. not relevant...
      ThreadId: Id of the thread that the test war run in
      ThreadNumber: <- refactor to threadid
```
# Docker
### Runner
To run the tests pull the runner and run a scenario from a folder
```
docker pull registry.gitlab.com/wickedflame/avalanche/runner:latest
docker run --rm -i -v ./scenarios:/scenarios registry.gitlab.com/wickedflame/avalanche/runner:latest run -s local -u https://url_to_avalanche_client.com
```

## Docker compose

```
services:
  postgres:
    container_name: avalanche_postgres
    restart: unless-stopped
    image: postgres:17
    ports:
      - 5432:5432
    volumes:
      - ./data/pgdata:/var/lib/postgresql/data
    environment:
      POSTGRES_USER: avl
      POSTGRES_PASSWORD: pGreSsql1

  pgadmin:
    image: dpage/pgadmin4
    restart: always
    ports:
      - "8888:80"
    environment:
      PGADMIN_DEFAULT_EMAIL: avalanche@opacc.ch
      PGADMIN_DEFAULT_PASSWORD: pGadm1n
    volumes:
      - avalanche_postgres_data:/var/lib/pgadmin

  client:
    container_name: client
    restart: unless-stopped
    image: registry.gitlab.com/wickedflame/avalanche/client:latest
    volumes:
      - avalanche_data:/scenarios
    environment:
      - AV_EVENT_STORE_DB=pgsql
      - AV_READ_MODEL_DB=pgsql
      - AV_DB_SERVER=postgres
      - AV_DB_PORT=5432
      - AV_DB_USERNAME=avl
      - AV_DB_PASSWORD=pGreSsql1
      - SCENARIO_PATH=./scenarios # optional parameter
    # ports:
    #   - 8080:8080
    depends_on:
      - postgres
    networks:
      - default
      - traefik_default
    labels:
      - "traefik.http.routers.avalanche.tls=true"
      - "traefik.http.routers.avalanche.rule=Host(`avalanche.was.local`)"
      - "traefik.http.routers.avalanche.entrypoints=websecure"
      - "traefik.http.services.avalanche.loadbalancer.server.port=8080"

#  avalanche:
#    container_name: avalanche
#    restart: unless-stopped
#    image: registry.gitlab.com/wickedflame/avalanche/runner:latest
#    volumes:
#      - ./scenarios:/scenarios

volumes:
  avalanche_postgres_data:
    external: true
  avalanche_data:
    external: true

networks:
  default:
  traefik_default:
    name: traefik_default
    external: true
```

### Add to local docker registry
```
docker build -f dockerfile-runner -t "avalanche_runner_:latest" . --no-cache --force-rm=true
docker build -f dockerfile-client -t "avalanche_client:latest" . --no-cache --force-rm=true
```
```
docker run --rm -i -v ./scenarios:/scenarios avalanche_runner:latest run -s local
```



## What is Load Testing
### Load Testing
Load tests apply an ordinary amount of stress to an application to see how it performs. For example, you may load test an ecommerce application using traffic levels that you've seen during Black Friday or other peak holidays. The goal is to identify any bottlenecks that might arise and address them before new code is deployed.

In the DevOps process, load tests are often run alongside functional tests in a continuous integration and deployment pipeline to catch any issues early. 

### Stress Testing
Stress tests are designed to break the application rather than address bottlenecks. It helps you understand its limits by applying unrealistic or unlikely load scenarios. By deliberately inducing failures, you can analyze the risks involved at various break points and adjust the application to make it break more gracefully at key junctures.

These tests are usually run on a periodic basis rather than within a DevOps pipeline. For example, you may run a stress test after implementing performance improvements.

### Spike Testing
Spike tests apply a sudden change in load to identify weaknesses within the application and underlying infrastructure. These tests are often extreme increments or decrements rather than a build-up in load. The goal is to see if all aspects of the system, including server and database, can handle sudden bursts in demand.

These tests are usually run prior to big events. For instance, an ecommerce website might run a spike test before Black Friday.

### Endurance Testing
Endurance tests, also known as soak tests, keep an application under load over an extended period of time to see how it degrades. Oftentimes, an application might handle a short-term increase in load, but memory leaks or other issues could degrade performance over time. The goal is to identify and address these bottlenecks before they reach production.

These tests may be run parallel to a continuous integration pipeline, but their lengthy runtimes mean they may not be run as frequently as load tests.

### Scalability Testing
Scalability tests measure an application's performance when certain elements are scaled up or down. For example, an e-commerce application might test what happens when the number of new customer sign-ups increases or how a decrease in new orders could impact resource usage. They might run at the hardware, software or database level.

These tests tend to run less frequently since they're designed to diagnose specific issues rather than broadly help identify bottlenecks within the entire application.

### Volume Testing
Also known as flood tests, measure how well an application responds to large volumes of data in the database. In addition to simulating network requests, a database is vastly expanded to see if there's an impact with database queries or accessibility with an increase in network requests. Basically it tries to uncover difficult-to-spot bottlenecks.

These tests are usually run before an application expects to see an increase in database size. For instance, an ecommerce application might run the test before adding new products.
