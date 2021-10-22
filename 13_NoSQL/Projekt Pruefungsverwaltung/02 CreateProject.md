# Erstellen der Solution

Die nachfolgenden Befehle erstellen in Windows eine .NET Solution mit mehreren Teilprojekten.
Unter macOS oder Linux müssen die Befehle adaptiert werden (*rm -ef ExamManager*
bzw. *mkdir* statt *md*). Der Befehl *start ExamManager.sln* ist ein Windowsbefehl, um die
erstellte Solution mit dem Default Programm (meist Visual Studio) zu starten.

Prüfe vor dem Anlegen mit *dotnet --info*, ob die neueste .NET SDK auf dem Rechner installiert
ist. Die Ausgabe muss *.NET SDK* und dann die aktuelle Version beinhalten.

```text
rd /S /Q ExamManager
md ExamManager
cd ExamManager
md ExamManager.Application
md ExamManager.Test
md ExamManager.WebApi
md ExamManager.Wasm
md ExamManager.Dto

cd ExamManager.Dto
dotnet new classlib

cd ..\ExamManager.Application
dotnet new classlib
dotnet add package Bogus
dotnet add package MongoDB.Driver
dotnet add reference ..\ExamManager.Dto

cd ..\ExamManager.Test
dotnet new xunit
dotnet add reference ..\ExamManager.Application


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
