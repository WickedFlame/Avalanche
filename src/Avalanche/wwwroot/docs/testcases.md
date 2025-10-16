# TestCases

## Configuration


```
TestCases:
  - Urls:
      - 'https://testsite.com/'
    Name: Startpage
    Init:
      Url: 'https://testsite.com/Home/Privacy'
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
| Name       |       | Name of the Testrun                                                               |
| Urls       |       | List of URL to call per Testrun                                                   |
| Init       |       | Properties for the initialization.<br />Currently only the Url has to be provided |


