# Running application
Open project in Visual Studio by opening .sln file in root directory. Set startup project to EtteplanMORE.ServiceManual.Web and run EtteplanMORE.ServiceManual.Web with the green arrow and dropbox located next to it. If you ran into some problems check that necessary [NuGet](#dependencies) and correct ASP.NET packages are installed.

# Database
SQLite is used in this implementation. Initialisation queries are included in [SQL](/SQL) folder and should be ran in following order CreateTables->FactoryDeviceData->ServiceTaskData. Database is already initialised so you shouldn't need to use these queries unless you want to create a new database.

There are 30 FactoryDevices and 10 ServiceTasks in database by default.

## SQL diagram
![SQLDiagram](/SQL/SQLUML.jpg)

# Criticality of ServiceTask
Criticality of ServiceTask is stored as integer value in the database. Integer values with corresponding criticality in plain text:

    Critical    = 1
    Important   = 2
    Mild        = 3

In future response could be refactored to use plain text instead of integers to add readability.

# CRUD operations
## C(reate) new ServiceTask
Params that can be provided are: Closed (boolean), Criticality (integer), Description (string), TargetId (integer)

TargetId __must be provided__ and there must be FactoryDevice with provided id in database!!
Any other parameters don't need to be provided.

Default values for parameters are following:

    Criticality = Mild (3)

    Description = No description provided.

    Closed      = False


Route: HTTP __POST__: api/servicetasks?{PARAMS}

Postman examples:

    POST http://localhost:50441/api/servicetasks?TargetId=1&Criticality=1&Description=Change nothing&Closed=True
    
    POST http://localhost:50441/api/servicetasks?TargetId=1&Criticality=1&Closed=True
    
    POST http://localhost:50441/api/servicetasks?TargetId=1
    
    POST http://localhost:50441/api/servicetasks?TargetId=3&Criticality=3&Description=This is decription&Closed=False

## R(ead) ServiceTasks
### Listing all ServiceTasks
Route: HTTP __GET__: api/servicetasks/

Postman example:

    GET http://localhost:50441/api/servicetasks/

### Filter by target device
Route: HTTP __GET__: api/servicetasks/target/{id}

Where id is TargetId (id of target device). NOTE: Returns always at least a null JSON object even if the target device doesn't exist. If target is null in response that means ServiceTask has been given FactoryDevice which no longer exists in database.

Postman examples:

    GET http://localhost:50441/api/servicetasks/target/3

    GET http://localhost:50441/api/servicetasks/target/100    // Doesn't Exist! => Null respond

    GET http://localhost:50441/api/servicetasks/target/11

### Search operation
Route: HTTP __GET__: /api/servicetasks/search?{PARAMS}

Every parameter is optional

Search options:

    TaskId:         NOTE: If this is given all other params are ignored!

    TargetId:       Find only for given target

    MinCriticality: Finds all with higher or same criticality compared to provided (Critical is highest and Mild lowest)

    IssuedBefore:   Finds all issued before given date

    IssuedAfter:    Finds all issued after given date

    Closed:         Finds all with given closed status. 0 is any, 1 is True and -1 is False

    DescContains:   Finds all with description containing given substring

Postman examples:

    Return task with TaskId 2 (other params are ignored) so 1 and 2 are same request

    1. GET http://localhost:50441/api/servicetasks/search?TaskId=2&TargetId=2&MinCriticality=2&IssuedBefore=2020-01-20&IssuedAfter=2015-01-20&Closed=0&DescContains=Fix something

    2. GET http://localhost:50441/api/servicetasks/search?TaskId=2

    Return all tasks with criticality of Important or higher

    GET http://localhost:50441/api/servicetasks/search?MinCriticality=1

    Return all tasks for target with id 11 where criticality is critical

    GET http://localhost:50441/api/servicetasks/search?TargetId=11&MinCriticality=1

    Return all closed tasks issued before 01/01/2019

    GET http://localhost:50441/api/servicetasks/search?Closed=1&IssuedBefore=2019-01-01

    Return all open tasks issued between 01/01/2012 and 01/01/2015

    GET http://localhost:50441/api/servicetasks/search?Closed=-1&IssuedBefore=2015-01-01&IssuedAfter=2012-01-01

    Return all tasks where description contains "Check some"

    GET http://localhost:50441/api/servicetasks/search?DescContains=Check some

    Return all open tasks where description contains "Check"

    GET http://localhost:50441/api/servicetasks/search?Closed=-1&DescContains=Check

## U(pdate) ServiceTask
Params that can be provided are: TargetId (integer), Criticality (integer), Description (string), Closed (boolean)

Meaning that TaskId and DateIssued can not be changed.

If TargetId is provided there must be FactoryDevice with same id in database.
Any parameters don't have to be provided and in that case only Closed status will be updated to False. NOTE: If closed status is not provided it will be set to False by default.
Also it isn't checked that ServiceTask exists in first place so you might update something that doesn't exist.

Route: HTTP __PUT__: api/servicetasks/{id}?{PARAMS}

Where id is TaskId to be updated.

Postman examples:

    Update TaskId 2 with new TargetId(1), new Criticality(3=Mild), new Description(Fixed) and new Closed status(True)

    PUT http://localhost:50441/api/servicetasks/2?TargetId=1&Criticality=3&Description=Fixed&Closed=True

    Update TaskId 2 with new Criticality(1=Critical) and new Closed status(False)

    PUT http://localhost:50441/api/servicetasks/2?Criticality=1

    Update TaskId 3 with new TargetId(1) and new Closed status(True)

    PUT http://localhost:50441/api/servicetasks/3?TargetId=1&Closed=True

    Update TaskId 3 with new Closed status(False)

    PUT http://localhost:50441/api/servicetasks/3

## D(elete) ServiceTask
Route: HTTP __DELETE__: api/servicetasks/{id}

Where id is TaskId to remove. NOTE: this does not check if TaskId exists in database!

Postman examples:

    DELETE http://localhost:50441/api/servicetasks/3

    DELETE http://localhost:50441/api/servicetasks/2

    DELETE http://localhost:50441/api/servicetasks/1

# UnitTests
There are small amount of UnitTests included for FactoryDeviceService and ServiceTaskService. There is no function for resetting database though so you need to grab a fresh SMDB.db file from [SQL](/SQL) folder and place it to [UnitTest](/EtteplanMORE.ServiceManual.UnitTests) folder everytime you want to re-run tests.

# Dependencies
Application core depends on following NuGet packages:

    System.Data.SQLite.Core
  
    Dapper

# Security concerns
Application is currently vulnerable to SQLInjections and user is not authenticated and/or restricted in any way.
