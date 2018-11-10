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

### 1_1_Queries

- ~~Created GraphQL types for each entity.~~
- ~~Created the query and the schema.~~
- ~~Added new types and some utilities to the DI container.~~
- ~~GraphiQL started playing with us.~~

### 1_2_Types_Schemas_Simple_Queries

- ~~Added a general query and one with arguments.~~
- ~~Learned how Fragments and Directives work.~~

### 1_3_Arguments_Fragments_Directives

- ~~Checkpoint for section 1.~~

## 2_1_Mutations

Queries are fun, but having the chance of updating some data is twice as fun. In GraphQL language, any operation that changes data (create, update, delete) is called a mutation. Let's implement a mutation in order to create a new Issue.

1. We're starting by creating a DTO for our data input. Inside our `Models` folder we need a new class file called `IssueCreateInput.cs`, and put this into it:
    ```csharp
    using System;

    namespace Issues.Models
    {
        public class IssueCreateInput
        {
        public string Name { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public DateTime Created { get; set; } 
        }
    }        
    ``` 

2. Now, let's modify our `IIssueService` interface and corresponding implementation on `Services/IssueService.cs`:
    ```csharp
    // IIssueService...
    Task<Issue> CreateAsync(Issue issue);
    ```

    ```csharp
    // IssueService...
    public Task<Issue> CreateAsync(Issue issue)
    {
        _issues.Add(issue);
        return Task.FromResult(issue);
    }
    ```

3. Next we must create the corresponding GraphQL type. Inside `Schemas` add a new file called `IssueCreateInputType.cs`:
    ```csharp
    using GraphQL.Types;

    namespace Issues.Schema
    {
        public class IssueCreateInputType : InputObjectGraphType
        {
            public IssueCreateInputType()
            {
                Name = "IssueInput";
                Field<NonNullGraphType<StringGraphType>>("name");
                Field<NonNullGraphType<StringGraphType>>("description");
                Field<NonNullGraphType<IntGraphType>>("userId");
                Field<NonNullGraphType<DateGraphType>>("created");    
            }
        }
    }    
    ```

4. After this we're creating an `IssuesMutation.cs` file inside `Schemas`. Put the next contents in it:
    ```csharp
    using System;
    using GraphQL.Types;
    using Issues.Models;
    using Issues.Services;

    namespace Issues.Schemas
    {
        public class IssuesMutation : ObjectGraphType<object>
        {
            public IssuesMutation(IIssueService issues)
            {
                Name = "Mutation";
                Field<IssueType>(
                    "createIssue",
                    arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IssueCreateInputType>> { Name = "issue" }),
                    resolve: context => 
                    {
                        var issueInput = context.GetArgument<IssueCreateInput>("issue");
                        var id = Guid.NewGuid().ToString();
                        var issue = new Issue(
                            issueInput.Name,
                            issueInput.Description,
                            issueInput.Created,
                            issueInput.UserId,
                            id);
                        return issues.CreateAsync(issue);
                    }
                );
            }
        }
    }
    ```

5. We need to upgrade our `IssuesSchema` class so that it supports mutations:
    ```csharp
    // Update the constructor
    public IssuesSchema(IssuesQuery query, IssuesMutation mutation, IDependencyResolver resolver)
    {
        Query = query;
        Mutation = mutation;
        DependencyResolver = resolver;
    }
    ```

6. Go register all the new types we created in `Startup.cs`.
    ```csharp
    services.AddSingleton<IssueCreateInputType>();
    services.AddSingleton<IssuesMutation>();    
    ```

7. Build and run the project. Create a new issue with this command:
    ```
    mutation createIssue ($issue: IssueInput!) {
        createIssue (issue: $issue) {
            id
            name
            created
            description
            user {
                id
                username
            }
        }
    }    
    ```

    Use the following query variables:
    ```
    {
        "issue": {
            "name": "Test Issue",
            "description": "A test issue",
            "userId": 1,
            "created": "01-01-2018"
        }
    }    
    ```

    You'll get the data for the fields you defined, including the newly assigned id.

    You even could verify with a query that the new issue is there:
    ```
    query getIssues {
        issues {
            name
            user {
                username
            }
        }
    }    
    ```

# Credits
Gabriel Trujillo C. <joaqt23@gmail.com>