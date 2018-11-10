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

- Created GraphQL types for each entity.
- Created the query and the schema.
- Added new types and some utilities to the DI container.
- GraphiQL started playing with us.

## 1_2_Types_Schemas_Simple_Queries

1. We're just starting. How about getting all the users or filtering them by Id? This is easily done by means of adding more queries to our `IssuesQuery` class. 

    1.1. To create a query to get all the users first add a new parameter to the constructor in order to get the users service injected:
    ```csharp
    public IssuesQuery(IIssueService issues, IUserService users)
    {
        // ...
    }
    ``` 

    And now we can add a new query inside the constructor:
    ```csharp
    Field<ListGraphType<UserType>>(
            "users",
            resolve: context => users.GetUsersAsync());
    ``` 

    Build, run, and try this query on GraphiQL:
    ```
    query getAllUsers {
        users {
            id
            username
        }
    }
    ```

    1.2. Now, to get an user by their id we need to utilize the concept of arguments. Add this query after the one we just created:
    ```csharp
    Field<UserType>(
        "user",
        arguments: new QueryArguments(new QueryArgument<IntGraphType> {Name = "id"}),
        resolve: context => 
        {
            var id = context.GetArgument<int>("id");
            return users.GetUserByIdAsync(id);
        }
    );
    ```

    We should now be able to leverage the power of query variables. Try this on GraphiQL:
    ```
    query getUserById ($userId: Int!) {
        user (id: $userId) {
            id
            username
        }
    }
    ```

    And in the *Query Variables* area:
    ```
    {
        "userId": 2
    }
    ```

    You should be able to see the info of the corresponding user in the response area. 
    
    See how the request and response look like in the browser console.

2. But that's not all. We can also use Fragments. They are reusable sets of fields that save you typing time.

    For example, try this:
    ```
    fragment basicIssue on IssueType {
        id
        name
    }

    query getIssuesWithFragments {
        issues {
            ...basicIssue
            user {
                id
            }
        }
    }
    ```
    
3. And furthermore, there's something called directives that you can use to toggle the display of fields or fragments.
    Try this right below your last query:
    ```
    fragment users on UserType {
	    id
        username
    }

    query getIssuesWithDirectives ($withoutBasicData: Boolean!, $withUser: Boolean!) {
        issues {
            ...basicIssue @skip (if: $withoutBasicData)
            description
            user @include (if: $withUser)
            {
                ...users
            }
        }
    }
    ```

    And these variables:
    ```
    {
        "withoutBasicData": false,
        "withUser": false        
    }
    ```

    Go ahead and change the values of the variables and see what happens.


# Credits
Gabriel Trujillo C. <joaqt23@gmail.com>