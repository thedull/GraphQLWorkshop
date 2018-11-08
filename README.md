# GraphQL - Let's put REST to rest
This is the repository for the corresponding workshop to be covered during dotNetConf 2018 in Medellin,  CO.

## Prerequisites

You'll need:

- DotNetCore SDK: You can get it here https://www.microsoft.com/net/download. Follow the instructions aplicable to your OS.
- Git: https://git-scm.com/downloads
- An editor such as [Visual Studio Code](https://code.visualstudio.com/download) or [Visual Studio Community Edition](https://visualstudio.microsoft.com/vs/community/).
- A GitHub account, in case you want to fork or clone the repository in your local machine for fast-forwarding.
- An HTML5 Websockets capable browser like Chrome.

NOTE: If you're going to use VSCode, I recommend adding the C# extension (https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp), since it provides a lot of goodies and utilities available on Visual Studio.

Finally, verify you have the `dotnet` CLI  available:

```
dotnet --version
```

Anything above 2.0.0 will suffice.


## 0_1_Initial : Setup the solution and server project

0. You can either create the directory where the solution is going to reside or clone the repository in a directory at your best convenience.

1. Let's start by creating the solution file. Inside the work directory, open a terminal console and enter:
    ```
    dotnet new sln
    ```

2. Now create a `nuget.config` file at the solution root and put this into it:
    ```
    <configuration>
        <packageSources>
            <add key="graphql-dotnet" value="https://myget.org/F/graphql-dotnet/api/v3/index.json" />
        </packageSources>
    </configuration>
    ```
    This adds the corresponding nuget source for the GraphQL packages we're going to use.  

3. Add the server that's going to host our GraphiQL.

    3.1. Create a directory named `server` and cd into it.

    WINDOWS
    ```
    md server
    cd server
    ```
    
    UNIX
    ```
    mkdir server && cd $_
    ``` 

    3.2. Create a web project inside it.
    
    ```
    dotnet new web
    ```

    Wait until it finishes downloading and restoring dependencies.

    3.3. Extract the contents of the file `wwwroot.zip` inside the folder `wwwroot`. These are the files our GraphiQL needs in order to run.

    3.4. Add the nuget packages we require for this project. (Make sure you're inside the `server` folder)

    The first two are some transports to enable GraphQL on our MVC project both through HTTP and Websockets.

    - [GraphQL.Server.Transports.AspNetCore](https://www.nuget.org/packages/GraphQL.Server.Transports.AspNetCore/)
        ```
        dotnet add package GraphQL.Server.Transports.AspNetCore --version 2.0.0
        ```

    - [GraphQL.Server.Transports.WebSockets](https://www.nuget.org/packages/GraphQL.Server.Transports.WebSockets/)
        ```
        dotnet add package GraphQL.Server.Transports.WebSockets --version 2.0.0
        ```

    And the last one is needed in order to serve the static files we have in our `wwwroot` directory.
    
    - [Microsoft.AspNetCore.StaticFiles](https://www.nuget.org/packages/Microsoft.AspNetCore.StaticFiles)
        ```
        dotnet add package Microsoft.AspNetCore.StaticFiles --version 2.1.1
        ```

    3.5. Inside `Startup.cs` change
    ```
    app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
    ```
    By
    ```
    app.UseDefaultFiles();
    app.UseStaticFiles();
    ```

    3.6. And finally, add the newly created project to the solution file.
    
    ```
    cd ..
    dotnet sln add server/server.csproj
    ```

4. When you build and run the project, you'll be able to get the GraphiQL interface, though it won't do much at this time.

    ```
    dotnet build
    dotnet run
    ```
    Or press F5 key if you're using VSCode or Visual Studio.

    **NOTE for VSCode users**: If you just installed the C# extension, pressing F5 will open a list with two configurations. Choose `.NET Core`, which will open the `launch.json` file with run cofigurations. Save it and press F5 again. 

# Credits
Gabriel Trujillo C. <joaqt23@gmail.com>
