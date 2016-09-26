// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing.NUnit3

// Properties
let buildDir = "./bin/Debug/"
let objDir = "./obj/"
let testDir = buildDir

let nunitDLL =           "packages/NUnit/lib/net45/nunit.framework.dll"

// Targets
Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "Build" (fun _ ->
    !! "/**/*.fsproj"
    |> MSBuildRelease buildDir "Build"
    |> Log "AppBuild-Output: "
)

Target "Prepare-Test" (fun _ ->
    Copy testDir [nunitDLL] 
)

Target "Test" (fun _ ->
    !! (buildDir + "/UnionCaseLib.dll")
    |> NUnit3 (fun p -> NUnit3Defaults) 
)

// Dependencies
"Clean"
    ==> "Build"
    ==> "Prepare-Test"
    ==> "Test"

// start build
RunTargetOrDefault "Test"