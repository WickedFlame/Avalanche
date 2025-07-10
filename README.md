# Avalanche
Loadtesting for Web Applications



Tool
[x] Rename Parameter ConfigFile to Scenario (-s --scenario)

Scenario config
[x] Rename Tests to TestCases
[x] Rename TestConfig to TestCase (class)
- Refactor URLs to single Url
[x] Rename Threads to Users (config and UI)

- Rename TestName to TestCase
- Rename ThreadNumber to ThreadId
- Rename Name in TestRuns to Scenario




## App
### Postgres
| Name | Value |  | 
|---|---|---|
| AV_EVENT_STORE_DB | pgsql | |
| AV_READ_MODEL_DB  | pgsql | |
| AV_DB             | pgsql | |
| AV_DB_SERVER | | |
| AV_DB_PORT | | |
| AV_DB_USERNAME | | |
| AV_DB_PASSWORD | | |



## Executor
| Short | long         |   |
|-------|--------------|---|
| -f    | --configfile |   |
| -u    | --url        |   |


## Domain
```
Scenario:
  Name: name of the scenario (same as filename)
  Tests: <- refactor to TestCases
    - Name: name of the test
      # Threads: 10
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
  Tests:
    - TestName: <- refactor to TestCase. name of the test. taken from the scenario config
      TestCase: name of the test. taken from the scenario config
      Id: generated id. not relevant...
      ThreadId: Id of the thread that the test war run in
      ThreadNumber: <- refactor to threadid
```
# Docker

```
docker pull registry.gitlab.com/wickedflame/avalanche/avalanche-tool
```

```
docker run --rm -i registry.gitlab.com/wickedflame/avalanche/avalanche-tool run -f local
```

```
docker build -f dockerfile-tool -t "avalanche-tmp:latest" . --no-cache --force-rm=true
```


```
docker run -t -i -v <host_dir>:<container_dir>  ubuntu /bin/bash
docker export registry.gitlab.com/wickedflame/avalanche/avalanche-tool | tar t > avalanche-tool-files.txt
docker image save registry.gitlab.com/wickedflame/avalanche/avalanche-tool > avalanche-tool-files.tar
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

These tests tend to run less frequently since they�re designed to diagnose specific issues rather than broadly help identify bottlenecks within the entire application.

### Volume Testing
Also known as flood tests, measure how well an application responds to large volumes of data in the database. In addition to simulating network requests, a database is vastly expanded to see if there's an impact with database queries or accessibility with an increase in network requests. Basically it tries to uncover difficult-to-spot bottlenecks.

These tests are usually run before an application expects to see an increase in database size. For instance, an ecommerce application might run the test before adding new products.
