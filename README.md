# GraphQL - Let's put REST to rest
This is the repository for the corresponding workshop to be covered during dotNetConf 2018 in Medellin,  CO.

## Where we were?

- Defined a set of prerequisites.
- Created the solution 
- Created the server project and added all the basic files and dependencies we need. 
- Now we can get the GraphiQL interface, though it won't do much at this time.

## 0_2_Server_Basic

1. Let's start creating our `Issues` directory in the solution root.
    
    WINDOWS
    ```
    cd ..
    md Issues
    cd Issues
    ```
    
    UNIX
    ```
    cd ..
    mkdir Issues && cd $_
    ```

2. Now create a class library project.
    ```
    dotnet new classlib
    ```

    Wait until it fetches dependencies and installs everything it needs.

3. Go up one level and add the newly created project to the solution.
    ```
    cd ..
    dotnet sln add Issues/Issues.csproj
    ```

4. Then inside the `server` directory add the corresponding reference to the `Issues` project.
    ```
    cd server
    dotnet add reference ../Issues/Issues.csproj
    ```

5. Let's go to our `Issues`folder and delete the `Class1.cs` sample class file. Then create three folders: `Models`, `Services`, and `Schemas`.
    
    WINDOWS
    ```
    cd ..\Issues
    del Class1.cs
    md Models Services Schemas
    ```

    UNIX 
    ```
    cd ../Issues
    rm Class1.cs
    mkdir Models Services Schemas
    ```

6. Now add the Nuget packages this project will depend on.

    - [GraphQL](https://www.nuget.org/packages/GraphQL/)
        ```
        dotnet add package GraphQL --version 2.1.0
        ```
    
    - [System.Reactive](https://www.nuget.org/packages/System.Reactive/)
        ```
        dotnet add package System.Reactive --version 3.1.1	
        ```
        This will help us with the heavy lifting of Subscriptions over Websockets.

7. Let's create our first models. 

    7.1. Inside `Models` folder create a new class file called `Users.cs`. Add this into it:
    ```csharp
    namespace Issues.Models
    {
        public class User
        {
            public User(int id, string username)
            {
                Id = id;
                Username = username;
            }

            public int Id { get; private set; }
            public string Username { get; private set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Avatar { get; set; }
            public string Email { get; set; }
        }
    }    
    ```

    7.2. And for our `Issue` model, inside of the same `Models` folder add a new class called `Issue.cs` and put the following code into it:
    ```csharp
    using System;

    namespace Issues.Models
    {
        public class Issue
        {
            public Issue(string name, string description, DateTime created, int userId, string Id)
            {
                Name = name;
                Description = description;
                Created = created;
                UserId = userId;
                this.Id = Id;
            }

            public string Name { get; set; }
            public string Description { get; set;}
            public DateTime Created { get; private set; }
            public int UserId { get; set; }
            public string Id { get; private set; }
            public IssueStatuses Status { get; set; }
        }

        [Flags]
        public enum IssueStatuses
        {
            REPORTED = 2,
            IN_PROGRESS = 4,
            FIXED = 8,
            CANCELLED = 16
        }
    }
    ```

    What we have here is a very simplistic model of an Issue and an enumeration for its possible statuses.

8. Next we'll create our `User` and `Issue` services. For the sake of time an in memory implementation will be used, but in order to make it more scalable, we'll use the repository service pattern so you can easily plug EF Core or any other persistence strategy you wish.

    8.1. For the Users service, create under `Services` folder a class file named `UserService.cs` and add this inside it:
    ```csharp
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Issues.Models;

    namespace Issues.Services
    {
        public class UserService : IUserService
        {
            private IList<User> _users;

            public UserService()
            {
                _users = new List<User>();
                _users.Add(new User(1, "fakeuser1"));
                _users.Add(new User(2, "fakeuser2"));
                _users.Add(new User(3, "fakeuser3"));
                _users.Add(new User(4, "fakeuser4"));
            }

            public User GetUserById(int id)
            {
                return GetUserByIdAsync(id).Result;
            }

            public Task<User> GetUserByIdAsync(int id)
            {
                return Task.FromResult(_users.Single(u => Equals(u.Id, id)));
            }

            public Task<IEnumerable<User>> GetUsersAsync()
            {
                return Task.FromResult(_users.AsEnumerable());
            }
        }

        public interface IUserService
        {
            User GetUserById(int id);
            Task<User> GetUserByIdAsync(int id);
            Task<IEnumerable<User>> GetUsersAsync();
        }
    }
    ```

    This file contains both the service interface and its implementation for some basic operations like getting users by their id and get all users. 

    8.2. And for the Issues services, we'll do something similar as we did for Users. Create an `IssueService.cs` class file inside `Services` and add this into it:
    ```csharp
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Issues.Models;

    namespace Issues.Services
    {
        public class IssueService : IIssueService
        {
            private IList<Issue> _issues;

            public IssueService()
            {
                _issues = new List<Issue>();
                _issues.Add(new Issue("Can't login", "The login breaks", DateTime.Now, 1, "C1DFA804-5A4B-405A-B6FA-12ABD0AD09FE"));
                _issues.Add(new Issue("Main panel misaligned", "Main panel is not centered", DateTime.Now.AddHours(1), 2, "241C58BB-56FD-48BB-88D4-379DA547B6C2"));
                _issues.Add(new Issue("Tests failing", "Tests for services are failing", DateTime.Now.AddHours(2), 3, "9282CBA7-0499-4769-919C-B9E35437008A"));
                _issues.Add(new Issue("Wrong font size", "The basic font size should be 12px", DateTime.Now.AddHours(2), 4, "8902E024-E54D-430F-B600-3B3CB22F591B")); 
            }
            public Task<Issue> GetIssueByIdAsync(string id)
            {
                return Task.FromResult(_issues.Single(o => Equals(o.Id, id)));
            }
            public Task<IEnumerable<Issue>> GetIssuesAsync()
            {
                return Task.FromResult(_issues.AsEnumerable());
            }
        }    

        public interface IIssueService
        {
            Task<Issue> GetIssueByIdAsync(string id);
            Task<IEnumerable<Issue>> GetIssuesAsync();
        }
    }    
    ```

9. And finally, we are going to leverage the power of the DI Container included in .NET Core to register our newly created services.

    Let's make in `server/Startup.cs` the  `ConfigureServices` method look like this:
    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {   
        services.AddSingleton<IIssueService, IssueService>();
        services.AddSingleton<IUserService, UserService>();
    }
    ```

10. Let's rebuild to verify everything is fine.
    ```
    dotnet clean 
    dotnet build
    ```

# Credits
Gabriel Trujillo C. <joaqt23@gmail.com>
