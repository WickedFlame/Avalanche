# Running Scenarios in an External Runner
  
## Overview
  
The **Runner** is a standalone component used to automate the execution of test scenarios in a containerized environment, independent of the Avalanche Client Application.  
It is ideal for CI/CD pipelines or distributed performance testing setups.
  
## Runner Parameters

The Runner accepts several command-line parameters for controlling scenario execution:

| Short | Long         | Description |
|-------|--------------|--------------|
| `-s`  | `--scenario` | Name of the Scenario or Scenario file. |
| `-u`  | `--url`      | URL of the Avalanche Client Application. If omitted, results are only logged to the console. |
| `-i`  | `--input`    | Reads the Scenario definition from **STDIN**. The scenario must be provided using `< filename.yml`. This option blocks the thread if no data is received via STDIN. |

## Running with Scenarios in a Volume

To execute Scenarios within a Docker container, follow these steps:

1. **Create a folder** named `scenarios` and place your Scenario file inside it.  
   The file name must match the Scenario name.

```bash
scenarios/
  scenario_1.yml
```
2. **Pull the Avalanche Runner image** from the container registry:
```
docker pull registry.gitlab.com/wickedflame/avalanche/runner:latest
```
3. **Run the Scenario** inside the container, mounting the scenarios folder as a volume:
```
docker run --rm -i \
  -v ./scenarios:/scenarios \
  registry.gitlab.com/wickedflame/avalanche/runner:latest \
  run -s scenario_1 -u https://url_to_avalanche_client.com
```
  
## Running the Scenario via STDIN Input

Alternatively, you can provide the Scenario definition directly through STDIN.
This eliminates the need to mount a volume, as the scenario data is streamed into the container.  
Ensure the -i argument is included.  
Redirect the Scenario file into the container’s standard input using <.
```
docker pull registry.gitlab.com/wickedflame/avalanche/runner:latest
docker run --rm -i \
  registry.gitlab.com/wickedflame/avalanche/runner:latest \
  run -s scenario_1 -u https://url_to_avalanche_client.com -i < scenarios/scenario_1.yml
```

# Notes
* When the `--url` parameter is not provided, test results are only displayed in the console output.  
* When connected to an Avalanche Client instance, results are sent back to the Client Application for storage and analysis.  
* The Runner can be integrated into CI/CD pipelines for automated load testing workflows.
