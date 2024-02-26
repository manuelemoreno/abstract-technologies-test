# Getting Started

This test includes two .NET 7 Web APIs. One for the main application and one for creating users and generating access tokens.

The applications can run either locally or using docker: 

For running locally, simply adjust the connectionString in the appSettings.json and/or appSettings.Development.json

For running using docker. Go to the folder **AwesomeFruits** and run docker compose in console
```bash
 docker-compose up -d
```

Three containers will be created. Two web apis and one for the sql server.

Example of a request in the WebApi Users Use __*POST*__ on /api/Auth/register to generate an bearer token:
```bash
{
  "userName": "testuser",
  "password": "testpass",
  "firstName": "mm",
  "lastName": "last",
  "email": "test@test.com"
}
```
If all is success, the response should return the bearer token
```bash
{
    "accessToken": [Bearer Token]
}
```

Use this token and add it to the Authorization header in the operations of the WebAPI.

Swagger is also available for both APIs. 

**Swagger for the WebAPI Users**:
https://localhost:9021/swagger/index.html

**Swagger for the WebAPI**:
https://localhost:9022/swagger/index.html

Run docker compose down to release resources after reviewing

```bash
 docker-compose down
```

