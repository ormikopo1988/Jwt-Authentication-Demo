# Jwt-Authentication-Demo
This demo shows local and linkedIn user registration and login flow using Angular 6 web app generated with Angular CLI, ASP.NET Core 2.1 WebApi and LinkedIn API.

To play with it:

For Client App you run a terminal on the root of the client app and then type: 
  - npm install and then 
  - ng serve

For ASPNET Core 2.1 Web API you run a terminal on the root of the web api project and then type: 
  - dotnet ef migrations add InitialCreate and then 
  - dotnet ef database update

You will also need to register a client application in LinkedIn and take the ClientId and ClientSecret credentials and put inside both the client app and the web api in the corresponding places marked with <your-linkedin-registered-client-app-id/secret>
