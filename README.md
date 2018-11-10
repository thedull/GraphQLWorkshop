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

- ~~Created Input Type for new mutation and added new service method for Issue creation.~~
- ~~Created corresponding Graph type.~~
- ~~Created mutation, upgraded schema, and registered new types.~~
- ~~Ran new mutation using input variables.~~

### 2_2_Create_Issue

- ~~Added logic for initial state and a state change inside the issues model.~~
- ~~Added the corresponding field in the Graph type.~~
- ~~Put new methods to deal with that state change on issues service interface and implementation class. ~~
- ~~Added the new operation to mutations class.~~
- ~~Verified we can start progress by executing the mutation.~~

### 2_3_Update_Status

- Checkpoint for section 2.

## 3_1_Subscriptions

In order to get live notifications of changes in our models GraphQL offers the **Subscriptions** functionality. Let's get an notification when we create an issue.

NOTE: This is a bit long process, since there's a lot of classes involved. For the sake of time, I'd suggest copying and pasting the snippets directly.

1. Create an `IssueEvent.cs` file inside our `Models` directory:
    ```csharp
    using System;

    namespace Issues.Models
    {
        public class IssueEvent
        {
            public IssueEvent(string issueId, string name, IssueStatuses status, DateTime timestamp)
            {
                IssueId = issueId;
                Name = name;
                Status = status;
                Timestamp = timestamp;
                Id = Guid.NewGuid().ToString();
            }
            public string Id { get; private set; }
            public string IssueId { get; set; }
            public string Name { get; set; }
            public IssueStatuses Status { get; set; }
            public DateTime Timestamp { get; private set; }
        }
    } 
    ```

2. Next create the corresponding GraphType in `Schemas/IssueEventType.cs`:
    ```csharp
    using GraphQL.Types;
    using Issues.Models;

    namespace Issues.Schemas
    {
        public class IssueEventType : ObjectGraphType<IssueEvent>
        {
            public IssueEventType()
            {
                Field(e => e.Id);
                Field(e => e.Name);
                Field(e => e.IssueId);
                Field<IssueStatusesEnum>("status",
                    resolve: context => context.Source.Status);
                Field(e => e.Timestamp);
            }
        }
    } 
    ```

3. Now we need to create a service interface and the corresponding implementation for our Issue event.

    In a new file `Services/IssueEventService.cs`:
    ```csharp
    using System;
    using System.Collections.Concurrent;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Issues.Models;

    namespace Issues.Services
    {
        public class IssueEventService : IIssueEventService
        {
            private readonly ISubject<IssueEvent> _eventStream = new ReplaySubject<IssueEvent>(1);
            public IssueEventService()
            {
                AllEvents = new ConcurrentStack<IssueEvent>();
            }
            public ConcurrentStack<IssueEvent> AllEvents { get; }
            public void AddError(Exception exception)
            {
                _eventStream.OnError(exception);
            }
            public IssueEvent AddEvent(IssueEvent orderEvent)
            {
                AllEvents.Push(orderEvent);
                _eventStream.OnNext(orderEvent);
                return orderEvent;
            }
            public IObservable<IssueEvent> EventStream()
            {
                return _eventStream.AsObservable();
            }
        }
        public interface IIssueEventService
        {
            ConcurrentStack<IssueEvent> AllEvents { get; }
            void AddError(Exception exception);
            IssueEvent AddEvent(IssueEvent orderEvent);
            IObservable<IssueEvent> EventStream();
        }
    } 
    ```

4. Now we'll update our `Services/IssueService.cs` in order to utilize the newly created service.
    
    4.1. At the class level add:
    ```csharp
    private readonly IIssueEventService _events;
    ```

    4.2. Modify the constructor so we inject the event service and populate the backing field:
    ```csharp
    public IssueService(IIssueEventService events) {
        // ...
        this._events = events;
    }
    ```

    4.3. Update the `CreateAsync` method to make it trigger the event on issue creation:
    ```csharp
    public Task<Issue> CreateAsync(Issue issue)
    {
        _issues.Add(issue);
        var issueEvent = new IssueEvent(
            issue.Id, 
            issue.Name, 
            IssueStatuses.REPORTED, 
            DateTime.Now);
        _events.AddEvent(issueEvent);
        return Task.FromResult(issue);
    }    
    ``` 

5. Create a new class file inside `Schemas` named `IssuesSubscription.cs` and add this into it (I know it's a bit long, just 🐻  with me):
    ```csharp
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using GraphQL.Types;
    using GraphQL.Subscription;
    using GraphQL.Resolvers;
    using Issues.Models;
    using Issues.Services;

    namespace Issues.Schemas
    {
        public class IssuesSubscription : ObjectGraphType<object>
        {
            private readonly IIssueEventService _events;

            public IssuesSubscription(IIssueEventService events)
            {
                _events = events;
                Name = "Subscription";
                AddField(new EventStreamFieldType
                {
                    Name = "issueEvent",
                    Arguments = new QueryArguments(new QueryArgument<ListGraphType<IssueStatusesEnum>>
                    {
                        Name = "statuses"
                    }),
                    Type = typeof(IssueEventType),
                    Resolver = new FuncFieldResolver<IssueEvent>(ResolveEvent),
                    Subscriber = new EventStreamResolver<IssueEvent>(Subscribe)
                });
            }

            private IObservable<IssueEvent> Subscribe(ResolveEventStreamContext context)
            {
                var statusList = context.GetArgument<IList<IssueStatuses>>("statuses", new List<IssueStatuses>());
                if (statusList.Count > 0)
                {
                    IssueStatuses statuses = 0;
                    foreach (var status in statusList)
                    {
                        statuses = statuses | status;    
                    }
                    return _events.EventStream().Where(e => (e.Status & statuses) == e.Status);
                }
                else
                {
                    return _events.EventStream();
                }
            
            }

            private IssueEvent ResolveEvent(ResolveFieldContext context)
            {
                var issueEvent = context.Source as IssueEvent;
                return issueEvent;
            }
        }
    } 
    ```

6. We should now be able to upgrade our schema in `IssuesSchema.cs` and add the subscription functionality.

    The constructor should look like this:
    ```csharp
    public IssuesSchema(IssuesQuery query, IssuesMutation mutation, IssuesSubscription subscription, IDependencyResolver resolver)
    {
        Query = query;
        Mutation = mutation;
        Subscription = subscription;
        DependencyResolver = resolver;
    }
    ```

7. And after all that, we only need to register the new types in the DI container:
    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // ... after registering the mutation
        services.AddSingleton<IssuesSubscription>();
        services.AddSingleton<IssueEventType>();
        services.AddSingleton<IIssueEventService, IssueEventService>();
    }
    ```

8. Build and run. If everything is OK we should be able to test the subscriptions.

    8.1. Open two incognito windows pointing to our server (typically http://localhost:5000).

    8.2. In the first incognito window add this:
    ```
    subscription {
        issueEvent(statuses: [REPORTED]) {
            issueId
            name
            status
        }
    }
    ```
    And run it. You should get a message like: *"Your subscription data will appear here after server publication!"* 

    8.3. Do the same in the second, but with an IN_PROGRESS status:
    ```
    subscription {
        issueEvent(statuses: [IN_PROGRESS]) {
            issueId
            name
            status
        }
    }
    ```
    And run it. You should get the same message.

    8.4. In the main window execute a mutation to create an issue, like:
    ```
    mutation createIssue ($issue: IssueInput!) {
        createIssue (issue: $issue) {
            id
            name
            status
        }
    }
    ```

    And include in the query variables:
    ```
    {
        "issue": {
            "name": "Test issue 1",
            "description": "Test issue 1",
            "userId": 1,
            "created": "01-01-2018"
        } 
    }    
    ```

    When you run the mutation, you'll get the notification in the first incognito window.

    8.5. Get the id of the issue you just created and in the main window add a new mutation to start progress:
    ```
    mutation startProgress ($issueId: String!) {
        startIssue(issueId: $issueId) {
            id
            name
            status
        }
    }    
    ```

    And add a new query variable named `issueId` with the id of the issue you just copied.

    When you run the mutation, you'll see the notification in the second incognito window.

# Credits
Gabriel Trujillo C. <joaqt23@gmail.com>