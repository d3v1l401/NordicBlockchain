# Nordic Blockchain
A blockchain based transaction manager for transaction proofing that require reliability and multi-sided confirmation.
Designed with the intent of not storing the asset value itself but the metadata of such asset with subsequent confirmation delegated to the bank's systems.


# Compile & Run
Nordic Blockchain uses .NET Core, as far as the building environment comprises .NET Core and NuGet Package Management software chain the project is compilable in an easy way.

### Dependencies
* [Newtonsoft JSON](https://www.newtonsoft.com/json) 
* [PemUtils](https://github.com/huysentruitw/pem-utils)

### Compile procedure
1. Compile "NordicBlockchain"
2. Compile "NBService"
3. Compile "NordicMiner"

### Run
Run dotnet on NBService.dll or your custom service application.

## Service definition

Multiple parts of the service exist for different purposes.

| Project Name   |Tipology                       |Definition                   |
|----------------|-------------------------------|-----------------------------|
|NordicBlockchain|`Library [REQUIRED]`           |Core NB code and mechanisms.  |
|NBService       |`Main Service [REQUIRED]`      |Actual blockchain node service, **used for testing**. |
|NBMiner        |`Miner Service [REQUIRED]`     |Miner service / IOperations ad-hoc tester, **used for testing**|

# WARNING

This is a proof of concept idea born by several events during my bachelor project brainstorming sessions, this project has been made by myself only (didn't have a group :c) and it is incomplete from the original full design.

I release this project for the future graduate wanna-bes who might be interested in something like this, you're free to take this code and extend or replace it as much as you wish as far as you of course cite me as the original author; this is an official graduation project that led me a quite good grade.

For all the students: there's an end in this dark tunnel of exams, keep it up and be creative, you'll be rewarded one day.
If you're interested in understanding what are some of the problems with this project, please open an issue or write me on my **protonmail email only, the VIA one has been destroyed**.
