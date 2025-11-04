# Scenarios

A **Scenario** defines a complete set of **TestCases** that are executed during each test run.  
Each Scenario can include the following sections:

- **Configuration**
- **Authorization**
- **TestCases** (one or more)

## Configuration

Scenarios are defined in `.yml` configuration files.  
The **filename** determines the **Scenario name**.

Scenario files must be located in the `scenarios` directory.  
When a Scenario is created within the application interface, it is automatically stored in this directory.

## Example Scenario Definition

Below is an example of a typical Scenario configuration:

```yaml
Config:
  Users: 5
  Iterations: 10
  Duration: 0
  Interval: 0
  RampupTime: 0
  UseCookies: True
  Delay: 1

TestCases:
  - Urls:
      - 'https://testsite.com/'
    Name: Startpage

  - Urls:
      - 'https://testsite.com/Home/Privacy'
    Name: Privacy
    Init:
      Url: 'https://testsite.com/Home/Privacy'

Authorization:
  Type: none
```
### Explanation
**Config**: Defines execution parameters such as user count, iteration limits, delays, and session behavior.  
**TestCases**: Specifies one or more test steps, each containing URLs and optional initialization logic.  
**Authorization**: Defines authentication type or credentials to be used during execution (e.g., `none`, `basic`, `bearer`).  
  
A Scenario defines at least the Node for [TestCases](testcases).  
Optional nodes are [Config](config) and [Authorization](authorization).  


