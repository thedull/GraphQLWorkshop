# GraphQL - Let's put REST to rest
This is the repository for the corresponding workshop to be covered during dotNetConf 2018 in Medellin,  CO.

## Where we were?

### 0_1_Initial
- ~~Defined a set of prerequisites.~~
- ~~Created the solution.~~ 
- ~~Created the server project and added all the basic files and dependencies we need.~~ 
- ~~Got a dumb GraphiQL interface.~~

### 0_2_Server_Basic
- ~~Created a class library project to handle our Issues logic.~~
- ~~Created two models: `User` and `Issue`.~~
- ~~Following a service repository pattern, we created the corresponding interfaces for those entities and implemented them.~~
- ~~Registered all the services in the .NET Core DI container.~~

### 0_3_Library_Basic

- ~~Checkpoint for section 0~~

## 1_1_Queries

1. In order to enable querying we'll start by creating our GraphQL types for each entity inside the `Schemas` directory.

    1.1. Let's create a class file named `UserType.cs` and write this into it:
    ```csharp
    using GraphQL.Types;
    using Issues.Models;

    namespace Issues.Schemas
    {
        public class UserType : ObjectGraphType<User>
        {
            public UserType()
            {
                Field(f => f.Id)
                    .Description("User Id");
                Field(f => f.Username)
                    .Description("User handle");
            }
        }
    }
    ```

    1.2. Now, for our issues entity we're going to write the corresponding type specification inside `IssueType.cs`:
    ```csharp
    using GraphQL.Types;
    using Issues.Models;
    using Issues.Services;

    namespace Issues.Schemas
    {
        public class IssueType : ObjectGraphType<Issue>
        {
            public IssueType(IUserService users)
            {
                Field(f => f.Id)
                    .Description("Issue Id");
                Field(f => f.Name)
                    .Description("Issue summary");
                Field(f => f.Description)
                    .Description("Brief issue description");
                Field<UserType>(
                    "user",
                    resolve: context => users.GetUserByIdAsync(context.Source.UserId));
                Field(f => f.Created)
                    .Description("Creation datetime");
            }
        }
    }
    ```

    1.3. Remember we have an enumeration type for our issue status. Create a class file with the name `IssueStatusesEnum.cs` with this content:
    ```csharp
    using GraphQL.Types;

    namespace Issues.Schemas
    {
        public class IssueStatusesEnum : EnumerationGraphType
        {
            public IssueStatusesEnum()
            {
                Name = "IssueStatuses";
                AddValue("REPORTED", "Issue was reported", 2);
                AddValue("IN_PROGRESS", "Issue is in progress", 4);
                AddValue("FIXED", "Issue is fixed", 8);
                AddValue("CANCELLED", "Issue was cancelled", 1);
            }
        }
    }
    ```  

2. Next we need to create the corresponding query. Inside `Schemas` directory create a class file named `IssuesQuery.cs` and set this as its content:
    ```csharp
    using GraphQL.Types;
    using Issues.Services;

    namespace Issues.Schemas
    {
        public class IssuesQuery : ObjectGraphType<object>
        {
            public IssuesQuery(IIssueService issues)
            {
                Name = "Query";
                Field<ListGraphType<IssueType>>(
                    "issues",
                    resolve: context => issues.GetIssuesAsync());
            }
        }
    }
    ```

3. And then, we'll create the schema class. In a new file called `IssuesSchema.cs` type this:
    ```csharp
    using GraphQL;
    using GraphQL.Types;

    namespace Issues.Schemas
    {
        public class IssuesSchema : Schema
        {
            public IssuesSchema(IssuesQuery query, IDependencyResolver resolver)
            {
                Query = query;
                DependencyResolver = resolver;
            }
        }
    }
    ```

4. At this point, we're going to register some new types inside the DI container. 

    4.1. For our entity types add these lines to the `ConfigureServices` method in your `Startup.cs` file:
    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // ...
        services.AddSingleton<IssueType>();
        services.AddSingleton<UserType>();
        services.AddSingleton<IssueStatusesEnum>();
        services.AddSingleton<IssuesQuery>();
        services.AddSingleton<IssuesSchema>();
    }
    ```
    
    Keep in mind adding the corresponding reference to `Issues.Schemas` in the usings area at the top.

    ```csharp
    // ...
    using Issues.Schemas
    ```

    4.2. After that, let's register our dependency resolver as follows:
    ```csharp
    // inside your ConfigureServices method...
    services.AddSingleton<IDependencyResolver>(
                c => new FuncDependencyResolver(type => c.GetRequiredService(type)));
    ```

    This will need the usage of the GraphQL library at the top:
    ```csharp
    using GraphQL;
    ```

    4.3. And to finish with the DI container we need a couple more lines to make our transports work as intended.
    ```csharp
    // inside ConfigureServices...
    services.AddGraphQLHttp();
    services.AddGraphQLWebSocket<IssuesSchema>();

    // ... and inside Configure method...
    app.UseWebSockets();
    app.UseGraphQLWebSocket<IssuesSchema>(new GraphQLWebSocketsOptions());
    app.UseGraphQLHttp<IssuesSchema>(new GraphQLHttpOptions());
    ```

    This requires adding these usings:
    ```csharp
    using GraphQL.Server.Transports.AspNetCore;
    using GraphQL.Server.Transports.WebSockets;
    ```

5. Build and run your project. Now you should be able to test in GraphiQL a query like:
    ```
    query getIssues {
        issues {
            id
            name
            description
            created
            user {
                id
                username
            }
        }
    }
    ```

    Try removing some fields or bringing them back. It's the power of GraphQL! 

# Credits
Gabriel Trujillo C. <joaqt23@gmail.com>
