# Authorization
The Authorization node is configured at rootlevel of the Scenario or can be set for each TestCase separately.
  
## No Authorization
Requests are unauthorized if no Authorization Node is provided.  
If a Authorization Node is provided in the root of the Configuration, it can be disabled in the configuration of the TestCase by setting the type to 'none'
```
TestCases:
  - Urls:
      - 'https://testsite.com/'
    Name: Startpage
    Authorization:
      Type: none
```
### Properties
| Property     | Value |
| ------------ | ----- |
| Type         | none  |

## OAuth
Avalanche supports the client_credentials and password GrantTypes to authenticate against a OAuth Identity Provider.  

### GrantType client_credentials
```
Authorization:
  Type: oauth
  GrantType: client_credentials
  Authority: https://avalanche.io/api/auth/v1/token
  ClientId: avalancheclient
  ClientSecret: avalanchesecret
  Scope: test
```

### GrantType password
```
Authorization:
  Type: oauth
  GrantType: password
  Authority: https://avalanche.io/api/auth/v1/token
  ClientId: avalancheclient
  ClientSecret: avalanchesecret
  Username: 
  Password: 
  Scope: test
```

### Properties
| Property     | Value                                  |
| ------------ | -------------------------------------- |
| Type         | oauth                                  |
| GrantType    | password                               |
| Authority    | https://avalanche.io/api/auth/v1/token |
| ClientId     | avalancheclient                        |
| ClientSecret | avalanchesecret                        |
| Username     |                                        |
| Password     |                                        |
| Scope        | test                                   |

