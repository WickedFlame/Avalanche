# Config
Some properties of the TestCases can be configured in a global scope that is applied to each TestCase. 
The required properties are defined in the Config node in the root of the Scenario.
The Config node is optional.  

## Global configuration
Defining the Config Node in the the root of the Scenario, applies the defined values to each TestCase.

```
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