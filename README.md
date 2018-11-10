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

- Added logic for initial state and a state change inside the issues model.
- Added the corresponding field in the Graph type.
- Put new methods to deal with that state change on issues service interface and implementation class. 
- Added the new operation to mutations class.
- Verified we can start progress by executing the mutation.

## 2_3_Update_Status

This is the end of section 2. Off to section 3. Best is yet to come.

# Credits
Gabriel Trujillo C. <joaqt23@gmail.com>