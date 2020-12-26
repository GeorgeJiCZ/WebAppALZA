# WebAppALZA
Product Web API

This solution WebAppALZA has two projects - WebAppALZA.API is REST Web API project and WebAppALZA.xUnitTest is Unit Test project.
ASP.NET Core 3.1 and C# 8 are used for programming in MS Visual Studio 2019.

## Project WebAppALZA.API

Description:
- the basis is Empty project of ASP.NET Core Application
- used patterns: MVC, Repository, Dependency Injection
- dependencis: EF Core (SQL Server), Versioning, Swashbuckle (Swagger)
- Swagger is used for testing and documentation
- Controllers/ProductController: action methods for requests by HTTP verbs
- Data/DbInitializer: created database and seed data
- Data/products.json: init data for database
- Extension/SwaggerExtension: init method for Swagger
- folder Migrations: EF first code files for create/update database
- folder Models: product model and context classes for ORM 
- folder Repositories: files are layer between data access layer and business logic layer of an application
- class Startup: application configuration at runtime and init database 

Running:
- the database is created and the data is inserted when the application is started for the first time - class DbInitializer
- if the database does not exist, then creating a database is automatically a process without user intervention - Database.Migrate()
- the seed data is also entered automatically if there is no data in the table

Deployment:
- deployed on Azure 
- https://webappalzaapijv.azurewebsites.net/swagger/index.html
- https://webappalzaapijv.azurewebsites.net/api/v2/product


## Project WebAppALZA.xUnitTest

Description:
- the basis is xUnit Test Project (.NET Core)
- dependencis: xUnit, Moq, Configuration and WebAppALZA.API project
- testing ProductController
- data of products: fake data (property TestData) or from database 

Running:
- testing in Visual Studio
- input data is used by parametr DataType of appsettings.json (1 -> fake data, 2 -> data from DB)
- DB connection string is in appsettings.json too

