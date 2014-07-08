#I @"packages\fake\tools\"
#r "FakeLib.dll"
#r "System.Xml.Linq"

open System
open System.IO
open Fake
open Fake.FileUtils

cd __SOURCE_DIRECTORY__

//--------------------------------------------------------------------------------
// Information about the project for Nuget and Assembly info files
//--------------------------------------------------------------------------------


let product = "Akka.NET Monitoring"
let authors = [ "Aaron Stannard" ]
let copyright = "Copyright © Aaron Stannard 2013-2014"
let company = "Akka.net"
let description = "Pluggable monitoring system extension for Akka.NET actor systems"
let tags = ["akka";"actors";"actor";"model";"Akka";"concurrency";"monitoring";"statsd";]
let configuration = "Release"

// Read release notes and version

let release = ReleaseNotesHelper.LoadReleaseNotes "RELEASE_NOTES.md"

    //--------------------------------------------------------------------------------
// Directories

let binDir = "bin"
let testOutput = "TestResults"

let nugetDir = binDir @@ "nuget"
let workingDir = binDir @@ "build"
let libDir = workingDir @@ @"lib\net45\"

//--------------------------------------------------------------------------------
// Clean build results

Target "Clean" (fun _ ->
    DeleteDir binDir
)

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
        let src = "src" @@ project @@ @"bin\release\"
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

Target "Nuget" (fun _ ->
    for nuspec in !! "src/**/*.nuspec" do
        CleanDir workingDir

        let project = Path.GetFileNameWithoutExtension nuspec 
        let projectDir = Path.GetDirectoryName nuspec
        let releaseDir = projectDir @@ @"bin\Release"
        let packages = projectDir @@ "packages.config"

        let pack outputDir =
            NuGetHelper.NuGet
                (fun p ->
                    { p with
                        Description = description project
                        Authors = authors
                        Copyright = copyright
                        Project =  project
                        Properties = ["Configuration", "Release"]
                        ReleaseNotes = release.Notes |> String.concat "\n"
                        Version = release.NugetVersion
                        Tags = tags |> String.concat " "
                        OutputPath = outputDir
                        WorkingDir = workingDir
                        AccessKey = getBuildParamOrDefault "nugetkey" ""
                        Publish = hasBuildParam "nugetkey"
                        
                        Dependencies = getDependencies packages @ getAkkaDependency project })
                nuspec
        // pack nuget (with only dll and xml files)

        ensureDirectory libDir
        !! (releaseDir @@ project + ".dll")
        ++ (releaseDir @@ project + ".xml")
        |> CopyFiles libDir

        pack nugetDir

        // pack symbol packages (adds .pdb and sources)

        !! (releaseDir @@ project + ".pdb")
        |> CopyFiles libDir

        let nugetSrcDir = workingDir @@ @"src\"
        CreateDir nugetSrcDir

        let isCs = hasExt ".cs"
        let isFs = hasExt ".fs"
        let isAssemblyInfo f = (filename f).Contains("AssemblyInfo")
        let isSrc f = (isCs f || isFs f) && not (isAssemblyInfo f) 

        CopyDir nugetSrcDir projectDir isSrc
        DeleteDir (nugetSrcDir @@ "obj")
        DeleteDir (nugetSrcDir @@ "bin")

        // pack in working dir
        pack workingDir
        
        // copy to nuget directory with .symbols.nupkg extension
        let pkg = (!! (workingDir @@ "*.nupkg")) |> Seq.head

        let destFile = pkg |> filename |> changeExt ".symbols.nupkg" 
        let dest = nugetDir @@ destFile
        
        CopyFile dest pkg

    CleanDir workingDir
    DeleteDir workingDir
)

//--------------------------------------------------------------------------------
//  Target dependencies
//--------------------------------------------------------------------------------

Target "All" DoNothing

// build dependencies
"Clean" ==> "AssemblyInfo" ==> "Build" ==> "CopyOutput" ==> "BuildRelease"

// nuget dependencies
"CleanNuget" ==> "BuildRelease" ==> "Nuget"


"BuildRelease" ==> "All"
"Nuget" ==> "All"

RunTargetOrDefault "NuGet"