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

### 2_1_Mutations

- Created Input Type for new mutation and added new service method for Issue creation.
- Created corresponding Graph type.
- Created mutation, upgraded schema, and registered new types.
- Ran new mutation using input variables.

## 2_2_Create_Issue

As mentioned before, we can create a mutation for an update operation, like changing the status of the issue.

1. Let's start by modifying our `Issues.cs` class model in order to enable status changes only through helper methods.

    In the constructor
    ```csharp
    public Issue(string name, string description, DateTime created, int userId, string Id) {
        // ...
        Status = IssueStatuses.REPORTED;
    }
    ```

    And at the class level
    ```csharp  
    public IssueStatuses Status { get; private set; }

    public void Start()
    {
        Status = IssueStatuses.IN_PROGRESS;
    }
    ```

2. In the corresponding Graph type, add a field for statuses:
    ```csharp
    // In the constructor of Schemas/IssueType.cs...
    Field<IssueStatusesEnum>(
        "status",
        resolve: context => context.Source.Status);
    ```

3. Now we'll add a new method to start the progress on the issue in the service interface.
    ```csharp
    // In IIssueService
    Task<Issue> StartAsync(string issueId);
    ```

    And we'll implement it as follows:
    ```csharp
    // In IssueService
    private Issue GetById(string id)
    {
        // Helper private method
        var issue = _issues.SingleOrDefault(i => Equals(i.Id, id));
        if (issue == null)
        {
            throw new ArgumentException(string.Format("Issue Id '{0}' is invalid", id));
        }
        return issue;
    }

    public Task<Issue> StartAsync(string issueId)
    {
        // Implement the new contract member
        var issue = GetById(issueId);
        issue.Start();
        return Task.FromResult(issue);    
    }
    ```

4. And next, add the new mutation operation to our `IssuesMutation.cs` class file.
    ```csharp
    FieldAsync<IssueType>(
        "startIssue",
        arguments: new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "issueId"}),
        resolve: async context =>
        {
            var issueId = context.GetArgument<string>("issueId");
            return await context.TryAsyncResolve(
                async c => await issues.StartAsync(issueId));
        }
    );
    ```

5. We'll now be able to start progress on an issue by means of invoking this mutation:
    ```
    mutation startIssue ($issueId: String!) {
        startIssue (issueId: $issueId) {
            id
            name
            status
        }
    }
    ```

    And in the query variables:
    ```
    {
        "issueId" : "issue-guid-here"
    }
    ```

    Try either existing or non-existent issue ids to verify how errors are reported.

# Credits
Gabriel Trujillo C. <joaqt23@gmail.com>