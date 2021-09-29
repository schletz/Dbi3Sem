# Erstellen der Solution

```text
rd /S /Q ExamManager
md ExamManager
cd ExamManager
md ExamManager.Application
md ExamManager.Test
md ExamManager.WebApi
md ExamManager.Wasm
md ExamManager.Dto

cd ExamManager.Application
dotnet new classlib
dotnet add package Bogus
dotnet add package MongoDB.Driver

cd ..\ExamManager.Test
dotnet new xunit
dotnet add reference ..\ExamManager.Application

cd ..\ExamManager.Dto
dotnet new classlib

cd ..\ExamManager.WebApi
dotnet new webapi
dotnet add reference ..\ExamManager.Application
dotnet add reference ..\ExamManager.Dto

cd ..\ExamManager.Wasm
dotnet new blazorwasm --pwa
dotnet add reference ..\ExamManager.Dto

cd ..
dotnet new sln
dotnet sln add ExamManager.Wasm
dotnet sln add ExamManager.WebApi
dotnet sln add ExamManager.Dto
dotnet sln add ExamManager.Test
dotnet sln add ExamManager.Application

start ExamManager.sln
```
