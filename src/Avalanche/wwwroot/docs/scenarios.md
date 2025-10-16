# Scenarios
A Scenario describes a full set of TestCases that are executed with each testrun.  
Each Scenario can contain a Config section, a Authorization section and multiple TestCases.

### Configuration
The Scenario is configured in a *.yml file.  
The name of the file defines the name of the Scneario.  
The Scenariofile has to be stored in the folder 'scenarios'. If the Scenario is created in the application, it is automatically stored in the folder.  


```
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

A Scenario defines at least the Node for [TestCases](testcases).  
Optional nodes are [Config](config) and [Authorization](authorization).  


