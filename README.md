# Entity Guardian

#### In your projects developed with EntityFramework, it keeps track of all the changes that take place in your database and records them wherever you want.

![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/byerlikaya/EntityGuardian/dotnet.yml)
[![EntityGuardian Nuget](https://img.shields.io/nuget/v/EntityGuardian)](https://www.nuget.org/packages/EntityGuardian)
[![EntityGuardian Nuget](https://img.shields.io/nuget/dt/EntityGuardian)](https://www.nuget.org/packages/EntityGuardian)

It is very simple to use and ready in just 4 steps.

1. Install **EntityGuardian** NuGet package from [here](https://www.nuget.org/packages/EntityGuardian/).

````
PM> Install-Package EntityGuardian
````

2. Add the `EntityGuardianInterceptor` interceptor.

```csharp
builder.Services.AddDbContext<MemoryDbContext>(
    (serviceProvider, options) =>
        options.AddInterceptors(
            serviceProvider.GetRequiredService<EntityGuardianInterceptor>()));
```

3. Add and configure builder.Services.AddEntityGuardian().

```csharp
builder.Services.AddEntityGuardian(
   "your_sql_server_connection_string",
    option =>
    {
         option.StorageType = StorageType.SqlServer;
         option.DataSynchronizationTimeout = 30;
         option.ClearDataOnStartup = false;
         option.RoutePrefix = "entity-guardian";
         option.EntityGuardianSchemaName = "EntityGuardian";
    });
```

`StorageType` The type of database where changes will be saved. Only Sql Server is supported for now.	

`DataSynchronizationTimeout` The time in seconds that changes will be reflected. Default 30 seconds.

`ClearDataOnStartup` Determines the deletion of previous records since the project is Startup.

`RoutePrefix` Access link prefix for the dashboard. Default "entity-guardian".

`EntityGuardianSchemaName` Schema of tables belonging to EntityGuardian in the database. Default "EntityGuardian".
	
4. Add app.UseEntityGuardian()

```csharp
app.UseEntityGuardian();
```


Stand up your project, go to /entity-guardian and keep track of all changes.

### Transactions
![1](https://github.com/byerlikaya/EntityGuardian/assets/9534517/e1bd4ce2-fd67-4ebf-862a-ab2d618fc45f)

### Transaction Details
![2](https://github.com/byerlikaya/EntityGuardian/assets/9534517/3aeba852-92bf-4ce3-a6ba-a115abcfcf38)

### Transaction Detail
![3](https://github.com/byerlikaya/EntityGuardian/assets/9534517/8be01a74-a958-4a04-bf9f-704398a138c7)

Future planned developments.

* Login page.
* Multiple database support.

Give a star ⭐, fork and stay tuned.
