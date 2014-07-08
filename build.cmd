@echo off

.nuget\nuget.exe update -self

.nuget\nuget.exe install FAKE -OutputDirectory packages -ExcludeVersion

if not exist packages\SourceLink.Fake\tools\SourceLink.fsx ( 
  .nuget\nuget.exe install SourceLink.Fake -OutputDirectory packages -ExcludeVersion
)
cls

set encoding=utf-8
packages\FAKE\tools\FAKE.exe build.fsx %*
