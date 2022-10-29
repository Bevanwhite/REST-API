dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Swashbuckle.AspNetCore
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```
dotnet list package
Project 'PokemonReviewApp' has the following package references
   [net6.0]: 
   Top-level Package                              Requested   Resolved
   > Microsoft.EntityFrameworkCore.Design         6.0.10      6.0.10  
   > Microsoft.EntityFrameworkCore.SqlServer      6.0.10      6.0.10  
   > Microsoft.EntityFrameworkCore.Tools          6.0.10      6.0.10  
   > Swashbuckle.AspNetCore                       6.2.3       6.2.3   
```

### database code

dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef

dotnet ef migrations add InitializeCode

dotnet ef database update

