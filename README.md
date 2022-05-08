# Minimal JWT Auth Example

This is a minimal example implementation of .Net-identity with JSON Web Token
based access control for REST-APIs. 

The code is fairly extensively commented, and some possible suggestions for
extensions and possible useage are also in comments.

For a step-by-step overview of how to create JWT-based auth, see section
[Overview of creating JWT-based auth](#Overview-of-creating-JWT-based-auth)
below.

Adding functionality for creating administrators and managing users and claims
is left as an exercise for the reader.

## In depth

One of the simpler ways to secure an API in dotNet is to use the Identity package
with JSON Web Token bearers.

JWTs are Base64-encoded strings with a Header, Payload, and Signature.

The header contains information such as the algorithm used for signing the JWT.
The signature is generated from the base64-encoded header and payload in combination
with a secret key. This allows the issuer/server to verify that the token hasn't
been tampered with.
The payload contains the data we wish to send to the user. In the case of
using JWTs for auth purposes the payload may contain such data as username,
assigned claims and roles, and token expiry date.

To access a protected resource, first the user must login. Upon successful login
a ViewModel is returned to the user, containing at least a JWT.
The second step for the user is to then attach the JWT string as a header in
an API call to the protected resource, in the format:

```
Authorization: Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJVc2VyIjoidHJ1ZSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJqb2huQGV4YW1wbGUuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoiam9obkBleGFtcGxlLmNvbSIsIm5iZiI6MTY1MjAzMzY2NSwiZXhwIjoxNjUyMTIwMDY1fQ.65PwkhVizr79EU425V5h496cUos-IoZPHCDn516VOAlyRJNFWoP9p-q6Uw8eQpW4BPqsKRy699iM0V0-WPy6gw
```

Where "Authorization" is the header key, and the value is the entire string
"Bearer eyJhb...Py6gw".

If the user has access to the requested resource, and the header has been sent
correctly, the call should return a successful response.

Try copying and pasting the string eyJhb...Py6gw from above into the JWT debugger
at [JWT.io](https://jwt.io/) to see how the undecoded data is formatted.

## Example useage using swagger

To try out the functionality of this project, follow these steps.

First clone and run the project using the instructions in
[Getting Started](#Getting-Started) below.

Once you have the project running, if it doesn't automatically open your browser
at the built-in swagger page, open https://localhost:PORT/swagger/index.html

You can find the correct port number in the console where you ran
"dotnet watch run". Look for a locahost URL starting with **https** and
copy-paste that into your browser.

You should arrove at a page similar to this:

![Swagger User Interface](https://raw.githubusercontent.com/mikosken/MinimalJwtAuth/master/readme_images/Swagger_UI_1.png)

Expand the section **/api/auth/register**, click the button **Try it out**, and
fill out the text box similar to image below before clicking **Execute**.

<details>
    <summary><b>Show image: API request to register a new user</b></summary>
    ![API request to register a new user](https://raw.githubusercontent.com/mikosken/MinimalJwtAuth/master/readme_images/Swagger_UI_2_register_request.png)
</details>


You should receive a response 201: Created. At this point you can try to log in
by expanding the section **/api/auth/login**, click the button **Try it out**,
and fill out the text box as before, and finally clicking **Execute**.

<details>
    <summary><b>Show image: Response upon successful login request</b></summary>
    ![Response upon successful login request](https://raw.githubusercontent.com/mikosken/MinimalJwtAuth/master/readme_images/Swagger_UI_3_login_response.png)
</details>


You should receive a response 200: Success with a token in the request body.
Copy the long string inside the citation marks, this is the JWT you need to
access the weather API.

First attempt to access the weather API by expanding **/api/weather**, clicking
**Try it out**, and **Execute**. You should get response 401: Unauthorized.

Next, tell swagger to attach your authentication token to API requests. Scroll
to the top of the page and click on the button **Authorize**.

<details>
    <summary><b>Show image: Adding a bearer token to Swagger requests</b></summary>
![Adding a bearer token to Swagger requests](https://raw.githubusercontent.com/mikosken/MinimalJwtAuth/master/readme_images/Swagger_UI_4_authorize.png)
</details>


An input prompt should appear asking you for the JWT you copied earlier. In the
text box enter without quotes: "**Bearer eyJhb...Py6gw**" where *eyJhb...Py6gw*
is *your copied token*. Then click **Authorize** and **Close**.

Now try accessing the weather API as before. This time you should receive a code
200: Success, with some random wather data in the body.

<details>
    <summary><b>Show image: Successful response from JWT protected API endpoint</b></summary>
![Successful response from JWT protected API endpoint](https://raw.githubusercontent.com/mikosken/MinimalJwtAuth/master/readme_images/Swagger_UI_5_weather_success.png)
</details>


## Getting Started

To compile or continue development try this:

Install **git**, **dotnet SDK** and **Visual Studio Code**, then open a git console:

```
cd .\suitable\project\folder
git clone <address_to_this_repo>
cd ".\Minimal JWT Auth"
code .
```

To build and run enter in console:
```
cd .\AuthAPI
dotnet ef database update
dotnet build
dotnet watch run
```

## Overview of creating JWT-based auth

A micro-tutorial of how to use JSON Web Token based authorization with a dotNet
Core Web API.

1. Create new dotNet Core Web API project.

   ```
   dotnet new webapi -n <ProjectName>
   ```

2. Add required Nuget packages:

   * Microsoft.AspNetCore.Authentication.JwtBearer: "Middleware that enables an application to receive an OpenID Connect bearer token."
   * Microsoft.AspNetCore.Identity: "ASP.NET Core Identity allows you to add login features to your application and makes it easy to customize data about the logged in user."
   * Microsoft.AspNetCore.Identity.EntityFrameworkCore: "Identity provider that uses Entity Framework Core."
   * Microsoft.EntityFrameworkCore: "Entity Framework Core is a modern object-database mapper for .NET."
   * Microsoft.EntityFrameworkCore.Sqlite: "SQLite database provider for Entity Framework Core."
   * Microsoft.EntityFrameworkCore.Tools: "Entity Framework Core Tools for the NuGet Package Manager Console."
   * System.IdentityModel.Tokens.Jwt: "Includes types that provide support for creating, serializing and validating JSON Web Tokens."


3. Create IdentityDbContext: /Data/ApplicationContext.cs
3. Inject database context and configure to use SQLite: Program.cs
3. Add connection string for SQLite to development appsettings: appsettings.Development.json

4. Create initial database for identity:

   ```
   dotnet ef migrations add "Initial identity db" -o "./Migrations"
   dotnet ef database update
   ```

5. Add Identity service and configure password and user requirements: Program.cs
5. Add and configure Authentication service with JWTBearer: Program.cs
5. Add Autorization service and add policies etc: Program.cs
5. Add JWT signing key to development appsettings: appsettings.Development.json
5. Add UseAuthentication to request pipeline: Program.cs

6. Add empty Auth controller with constructor injection for config, roleManager, signInManager, and userManager: /Controllers/AuthController.cs

7. Create ViewModels for Login, Registration, and User data: /ViewModels/
7. Add JwtValidForSeconds to developer appsettings: appsettings.Development.json
7. Add API methods for RegisterUser, and Login to AuthController. Helper method CreateJwtToken.: /Controllers/AuthController.cs

8. Restrict your APIs to require a valid JWT for authorization.
8. (Optional) Add function to use bearer tokens for authorization in Swagger: Program.cs


## References

Inspiration, code snippets, etc.

* [Configure_Swagger_to_accept_Header_Authorization](https://www.freecodespot.com/blog/use-jwt-bearer-authorization-in-swagger/#VIII_Configure_Swagger_to_accept_Header_Authorization)
