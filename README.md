# Reference

Prerequisite 

 1) Microsoft Visual Studio 2013

 2) RavenDB-Build-2956 not more than this or less (http://hibernatingrhinos.com/downloads/RavenDB%20Installer/2956)

 3) Mongodb running on localhost where worker is deployed
 
 4) Good to have Posh-Git follow the process in https://github.com/dahlbyk/posh-git 
 
     
Steps to build and execute

1) Clone the Repository to Reference

2) Open Reference/Src/Framework.sln and build

3) Open powershell navigate to Reference and execute build.ps1 (May end up in some errors but continue to next step)

4) Open Reference/Src/COSMOS.sln and build

5) Execute the following from the folder Reference after MongoDB is started from the Mogodb localhost using Command Prompt or Powershell


     mongo.exe tools\database\MongoDB\data\stageload.js
     
     
6)

 Debug\Start new instance of HA.COSMOS.Worker in COSMOS.sln
 
 Debug\Start new instance of HA.COSMOS.WebApi in COSMOS.sln
 
 Debug\Start new instance of COSMOSClientConsole in COSMOS.sln
 
 
