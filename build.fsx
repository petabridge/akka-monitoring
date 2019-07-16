#I @"packages\fake\tools\"
#r "FakeLib.dll"

open System
open System.IO

open Fake.TaskRunnerHelper
open Fake
open Fake.FileUtils

cd __SOURCE_DIRECTORY__

//--------------------------------------------------------------------------------
// Information about the project for Nuget and Assembly info files
//--------------------------------------------------------------------------------

let product = "Akka.NET Monitoring"
let authors = [ "Aaron Stannard" ]
let copyright = "Copyright © Aaron Stannard 2013-2015"
let company = "Akka.net"
let description = "Pluggable monitoring system extension for Akka.NET actor systems"
let tags = ["akka";"actors";"actor";"model";"Akka";"concurrency";"monitoring";"statsd";]
let configuration = "Release"

// Read release notes and version

let release =
    File.ReadLines "RELEASE_NOTES.md"
    |> ReleaseNotesHelper.parseReleaseNotes

//--------------------------------------------------------------------------------
// Directories

let binDir = "bin"
let testOutput = "TestResults"

let nugetDir = binDir @@ "nuget"
let workingDir = binDir @@ "build"
let nugetExe = FullName @".nuget\NuGet.exe"

//--------------------------------------------------------------------------------
// Clean build results

Target "Clean" (fun _ ->
    CleanDir binDir
)

Target "Restore" RestorePackages

//--------------------------------------------------------------------------------
// Generate AssemblyInfo files with the version for release notes 


open AssemblyInfoFile

Target "AssemblyInfo" (fun _ ->
    let version = release.AssemblyVersion + ".0"

    CreateCSharpAssemblyInfoWithConfig "src/SharedAssemblyInfo.cs" [
        Attribute.Company "Akka"
        Attribute.Copyright copyright
        Attribute.Version version
        Attribute.FileVersion version ] <| AssemblyInfoFileConfig(false)
)

//--------------------------------------------------------------------------------
// Build the solution

Target "Build" (fun _ ->
    !!"AkkaMonitoring.sln"
    |> MSBuildRelease "" "Rebuild"
    |> ignore
)

//--------------------------------------------------------------------------------
// Copy the build output to bin directory
//--------------------------------------------------------------------------------

Target "CopyOutput" (fun _ ->    
    let copyOutput project =
        let src = "src" @@ project @@ @"bin/Release/"
        let dst = binDir @@ project
        CopyDir dst src allFiles
    [ "Akka.Monitoring"
      "Akka.Monitoring.StatsD"]
    |> List.iter copyOutput
)

Target "BuildRelease" DoNothing

//--------------------------------------------------------------------------------
// Nuget targets 
//--------------------------------------------------------------------------------

module Nuget = 
    // add Akka dependency for other projects
    let getAkkaDependency project =
        match project with
        | "Akka.Monitoring" -> []
        | _ -> ["Akka.Monitoring", release.NugetVersion]

    // selected nuget description
    let description project =
        match project with
        | "Akka.Monitoring.StatsD" -> "StatsD client for Akka.NET Monitoring"
        | _ -> description

open Nuget

//--------------------------------------------------------------------------------
// Clean nuget directory

Target "CleanNuget" (fun _ ->
    CleanDir nugetDir
)

//--------------------------------------------------------------------------------
// Pack nuget for all projects
// Publish to nuget.org if nugetkey is specified

let createNugetPackages _ =
    let mutable dirName = 1
    let removeDir dir = 
        let del _ = 
            DeleteDir dir
            not (directoryExists dir)
        runWithRetries del 3 |> ignore

    let getDirName workingDir dirCount =
        workingDir + dirCount.ToString()

    CleanDir workingDir

    ensureDirectory nugetDir
    for nuspec in !! "src/**/*.nuspec" do
        let ourWorkingDir = getDirName workingDir dirName

        printfn "Creating nuget packages for %s" nuspec
        
        ensureDirectory ourWorkingDir

        let project = Path.GetFileNameWithoutExtension nuspec 
        let projectDir = Path.GetDirectoryName nuspec
        let projectFile = (!! (projectDir @@ project + ".*sproj")) |> Seq.head
        let releaseDir = projectDir @@ @"bin/Release"
        let packages = projectDir @@ "packages.config"        
        let packageDependencies = if (fileExists packages) then (getDependencies packages) else []
        let dependencies = packageDependencies @ getAkkaDependency project
        let releaseVersion = release.NugetVersion
        let desc = description project

        let pack outputDir  =
            NuGetHelper.NuGet
                (fun p ->
                    { p with
                        Description = desc
                        Authors = authors
                        Copyright = copyright
                        Project =  project
                        Properties = ["Configuration", "Release"]
                        ReleaseNotes = release.Notes |> String.concat "\n"
                        Version = releaseVersion
                        Tags = tags |> String.concat " "
                        OutputPath = outputDir
                        WorkingDir = ourWorkingDir
                        Dependencies = dependencies })
                nuspec

        // Copy dll, pdb and xml to libdir = workingDir/lib/net45/
        let libDir = ourWorkingDir @@ @"lib\net45\"
        ensureDirectory libDir
        !! (releaseDir @@ project + ".dll")
        ++ (releaseDir @@ project + ".pdb")
        ++ (releaseDir @@ project + ".xml")
        ++ (releaseDir @@ project + ".ExternalAnnotations.xml")
        |> CopyFiles libDir

        // Copy all src-files (.cs and .fs files) to workingDir/src
        let nugetSrcDir = ourWorkingDir @@ @"src/"
        // CreateDir nugetSrcDir

        let isCs = hasExt ".cs"
        let isFs = hasExt ".fs"
        let isAssemblyInfo f = (filename f).Contains("AssemblyInfo")
        let isSrc f = (isCs f || isFs f) && not (isAssemblyInfo f) 
        CopyDir nugetSrcDir projectDir isSrc
        
        //Remove workingDir/src/obj and workingDir/src/bin
        removeDir (nugetSrcDir @@ "obj")
        removeDir (nugetSrcDir @@ "bin")

        // Create both normal nuget package and symbols nuget package. 
        // Uses the files we copied to workingDir and outputs to nugetdir
        pack nugetDir
        
        //removeDir workingDir
        dirName <- dirName + 1

let publishNugetPackages _ = 
    let rec publishPackage url accessKey trialsLeft packageFile =
        let tracing = enableProcessTracing
        enableProcessTracing <- false
        let args p =
            match p with
            | (pack, key, "") -> sprintf "push \"%s\" %s" pack key
            | (pack, key, url) -> sprintf "push \"%s\" %s -source %s" pack key url

        tracefn "Pushing %s Attempts left: %d" (FullName packageFile) trialsLeft
        try 
            let result = ExecProcess (fun info -> 
                    info.FileName <- nugetExe
                    info.WorkingDirectory <- (Path.GetDirectoryName (FullName packageFile))
                    info.Arguments <- args (packageFile, accessKey,url)) (System.TimeSpan.FromMinutes 1.0)
            enableProcessTracing <- tracing
            if result <> 0 then failwithf "Error during NuGet symbol push. %s %s" nugetExe (args (packageFile, "key omitted",url))
        with exn -> 
            if (trialsLeft > 0) then (publishPackage url accessKey (trialsLeft-1) packageFile)
            else raise exn
    let shouldPushNugetPackages = hasBuildParam "nugetkey"
    
    if (shouldPushNugetPackages) then
        printfn "Pushing nuget packages"
        if shouldPushNugetPackages then
            let normalPackages= 
                !! (nugetDir @@ "*.nupkg") 
                -- (nugetDir @@ "*.symbols.nupkg") |> Seq.sortBy(fun x -> x.ToLower())
            for package in normalPackages do
                publishPackage (getBuildParamOrDefault "nugetpublishurl" "") (getBuildParam "nugetkey") 3 package


Target "Nuget" <| fun _ -> 
    createNugetPackages()
    publishNugetPackages()

Target "CreateNuget" <| fun _ -> 
    createNugetPackages()

Target "PublishNuget" <| fun _ -> 
    publishNugetPackages()
//--------------------------------------------------------------------------------
//  Target dependencies
//--------------------------------------------------------------------------------

Target "All" DoNothing

// build dependencies
"Clean" ==> "Restore" ==> "AssemblyInfo" ==> "Build" ==> "CopyOutput" ==> "BuildRelease"

// nuget dependencies
"CleanNuget" ==> "BuildRelease" ==> "Nuget"


"BuildRelease" ==> "All"
"Nuget" ==> "All"

RunTargetOrDefault "NuGet"