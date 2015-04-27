# Reference

Prerequisite 

 1) NserviceBus 4.x (only to have RavenDB the dlls pushed in the repo, this will be changed to get from nuget) 

 2) Mongodb running on localhost where worker is deployed
 
     
Steps to build and execute

1) Clone the Repository to Reference

2) Open Reference/Src/Framework.sln and build

3) Open powershell navigate to Reference and execute build.ps1

4) Open Reference/Src/COSMOS.sln and build

5) Execute the following from the folder Reference after MongoDB is started from the Mogodb localhost using Command Prompt or Powershell


     mongo.exe tools\database\MongoDB\data\stageload.js
     
     
6)

 Debug\Start new instance of HA.COSMOS.Worker in COSMOS.sln
 
 Debug\Start new instance of HA.COSMOS.WebApi in COSMOS.sln
 
 Debug\Start new instance of COSMOSClientConsole in COSMOS.sln
 
 
