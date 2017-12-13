#I @"packages\fake\tools\"
#r "FakeLib.dll"

open System
open System.IO

open Fake
open Fake.FileUtils

cd __SOURCE_DIRECTORY__

//--------------------------------------------------------------------------------
// Information about the project for Nuget and Assembly info files
//--------------------------------------------------------------------------------

let sln = "AkkaMonitoring.sln"
let configuration = "Release"
let nugetProjects = [
  "Akka.Monitoring"
  "Akka.Monitoring.StatsD"
  "Akka.Monitoring.ApplicationInsights"
  "Akka.Monitoring.PerformanceCounters"
]
let assertExitCodeZero x = if x = 0 then () else failwithf "Command failed with exit code %i" x

// Read release notes and version
let release =
    File.ReadLines "RELEASE_NOTES.md"
    |> ReleaseNotesHelper.parseReleaseNotes

//--------------------------------------------------------------------------------
// Directories

let binDir = "bin"
let nugetDir = binDir @@ "nuget"
let nugetExe = FullName @".nuget\NuGet.exe"

//--------------------------------------------------------------------------------
// Clean build results

Target "Clean" (fun _ ->
    CleanDir binDir
)

Target "Restore" (fun _ ->
    let arguments = String.Format("restore {0}", sln)
    Shell.Exec("dotnet", arguments) |> assertExitCodeZero
)

//--------------------------------------------------------------------------------
// Build the solution

Target "Build" (fun _ ->
    let arguments = String.Format(@"msbuild {0} /p:Configuration={1} /p:Version={2} /p:GeneratePackages=True /p:ReleaseNotes=""{3}""", sln, configuration, release.NugetVersion, String.Join("\n", release.Notes))
    Shell.Exec("dotnet", arguments) |> assertExitCodeZero
)

//--------------------------------------------------------------------------------
// Copy the build output to bin directory
//--------------------------------------------------------------------------------

Target "CopyOutput" (fun _ ->    
    let copyOutput project =
        let src = "src" @@ project @@ @"bin\release\"
        let dst = binDir @@ project
        CopyDir dst src allFiles
    nugetProjects
    |> List.iter copyOutput
)

Target "BuildRelease" DoNothing

//--------------------------------------------------------------------------------
// Clean nuget directory

Target "CleanNuget" (fun _ ->
    CleanDir nugetDir
)

//--------------------------------------------------------------------------------
// Pack nuget for all projects
// Publish to nuget.org if nugetkey is specified

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
    publishNugetPackages()

Target "PublishNuget" <| fun _ -> 
    publishNugetPackages()
//--------------------------------------------------------------------------------
//  Target dependencies
//--------------------------------------------------------------------------------

Target "All" DoNothing

// build dependencies
"Clean" ==> "Restore" ==> "Build" ==> "CopyOutput" ==> "BuildRelease"

// nuget dependencies
"CleanNuget" ==> "BuildRelease" ==> "Nuget"


"BuildRelease" ==> "All"
"Nuget" ==> "All"

RunTargetOrDefault "BuildRelease"