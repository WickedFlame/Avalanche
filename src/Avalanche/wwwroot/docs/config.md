# Config

The **Config** section defines global parameters that apply to all **TestCases** within a Scenario.  
These properties control the behavior of test execution, including user load, iteration limits, timing, and session handling.  

The **Config** node is optional, but when defined, its values are applied globally to each TestCase within the Scenario.

## Global Configuration

Defining a `Config` node at the root level of a Scenario applies the specified parameters to all TestCases.  
This allows consistent test execution without redefining the same values per case.

### Example

```yaml
Config:
  Users: 5
  Iterations: 10
  Duration: 0
  Interval: 0
  RampupTime: 0
  UseCookies: True
  Delay: 1
```

## Properties
| Parameter  | Value | Description                                                                       |
| ---------- | ----- | --------------------------------------------------------------------------------- |
| Users      | INT   | Amount of users per testrun.<br />Defaults to 1                                   |
| Iterations | INT   | Amount of iterations per user                                                     | 
| Duration   | INT   | Total duration of the testrun in minutes                                          |
| Interval   | INT   | Interval of each user call in milliseconds                                        |
| Delay      | INT   | Delay between each request in seconds                                             |
| RampupTime | INT   | Rampup-time in seconds                                                            |
| UseCookies | BOOL  | Use same cookies for all requests per user?<br />Defaults to true                 |
  

## Per-TestCase Configuration Overrides

While the global `Config` node defines default values for all TestCases, each **TestCase** can override specific parameters as needed.  
When a property is defined within a TestCase, it takes precedence over the corresponding global configuration.

This allows fine-tuned control for scenarios that require varying loads, timings, or behaviors across individual test cases.

### Example

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
  - Name: Homepage
    Urls:
      - 'https://testsite.com/'
    Config:
      Users: 10
      Delay: 2

  - Name: Privacy Page
    Urls:
      - 'https://testsite.com/Home/Privacy'
```
  
### Explanation
In the example above:
  
- The **Homepage** test case overrides `Users` and `Delay`, running with **10 users** and a **2-second delay** between requests.  
- The **Privacy Page** test case inherits all values from the global configuration.
  
### Override Hierarchy

1. **TestCase Config** — highest priority  
2. **Global Config** — defined at the Scenario root level  
3. **Application Defaults** — lowest priority  

This hierarchy ensures that fine-grained control can be applied per TestCase while maintaining a consistent global baseline across all scenarios.
  