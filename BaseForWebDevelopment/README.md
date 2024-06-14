## Useful commands in .net console CLI

Install tooling

~~~bash
dotnet tool update -g dotnet-ef
dotnet tool update -g dotnet-aspnet-codegenerator 
~~~

## EF Core migrations

Run from solution folder  

~~~bash
dotnet ef migrations --project App.DAL.EF --startup-project WebApp add FOOBAR
dotnet ef database   --project App.DAL.EF --startup-project WebApp update
dotnet ef database   --project App.DAL.EF --startup-project WebApp drop
~~~


Run from WebApp folder!  

~~~bash
cd WebApp

dotnet aspnet-codegenerator controller -name ContestsController        -actions -m  App.Domain.Contest        -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
# use area
dotnet aspnet-codegenerator controller -name ContestsController        -actions -m  App.Domain.Contest        -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f

cd ..
~~~


~~~bash
cd WebApp
dotnet aspnet-codegenerator controller -name OrdersController -actions -m  Order -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
~~~