#tool "dotnet:?package=GitVersion.Tool&version=6.1.0"
#tool "nuget:?package=dotnet-sonarscanner&version=9.0.2"
#tool "nuget:?package=NuGet.CommandLine&version=6.12.2"

#addin "nuget:?package=Cake.Sonar&version=1.1.33"

var target = Argument("target", "Default");
var nugetApiKey = Argument("nugetApiKey", EnvironmentVariable("nugetApiKey"));
var configuration = Argument("configuration", "Release");
var sonarLogin = Argument("sonarLogin", EnvironmentVariable("sonarLogin"));

//////////////////////////////////////////////////////////////////////
//    Build Variables
/////////////////////////////////////////////////////////////////////
var solutionFile = new FilePath("./HansPeterGit.sln");
var outputDirRoot = Directory("./buildArtifacts/").Path.MakeAbsolute(Context.Environment);
var outputDirPublished = outputDirRoot.Combine("Published");
var outputDirTemp = outputDirRoot.Combine("Temp");
var packageOutputDir = outputDirPublished.Combine("Package");
var outputDirTests = outputDirTemp.Combine("Tests/");

var codeCoverageResultFilePath = MakeAbsolute(outputDirTests).Combine("**/").CombineWithFilePath("coverage.opencover.xml");
var testResultsPath = MakeAbsolute(outputDirTests).CombineWithFilePath("*.trx");

var nugetPublishFeed = "https://api.nuget.org/v3/index.json";
var sonarProjectKey = "twenzel_HansPeterGit";
var sonarUrl = "https://sonarcloud.io";
var sonarOrganization = "twenzel";

var isLocalBuild = string.IsNullOrEmpty(EnvironmentVariable("GITHUB_REPOSITORY"));
var isPullRequest = !string.IsNullOrEmpty(EnvironmentVariable("GITHUB_HEAD_REF"));
var gitHubEvent = EnvironmentVariable("GITHUB_EVENT_NAME");
var isReleaseCreation = string.Equals(gitHubEvent, "release");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Setup(context =>
{
	Information($"Output directory: {outputDirRoot.FullPath}");
	Information($"Package output directory: {packageOutputDir.FullPath}");
	Information($"Local build: {isLocalBuild}");
	Information($"Is pull request: {isPullRequest}");	
	Information($"Is release creation: {isReleaseCreation}");
});

Task("Clean")
	.Description("Removes the output directory")
	.Does(() => {
	  
	    EnsureDirectoryDoesNotExist(outputDirRoot, new DeleteDirectorySettings {
			Recursive = true,
			Force = true
		});
		CreateDirectory(outputDirRoot);	
		CreateDirectory(outputDirPublished);	
});

GitVersion versionInfo = null;
Task("Version")
	.Description("Retrieves the current version from the git repository")
	.Does(() => {
		
		versionInfo = GitVersion(new GitVersionSettings {
			UpdateAssemblyInfo = false
		});
			
		Information("Major:\t\t\t\t\t" + versionInfo.Major);
		Information("Minor:\t\t\t\t\t" + versionInfo.Minor);
		Information("Patch:\t\t\t\t\t" + versionInfo.Patch);
		Information("MajorMinorPatch:\t\t\t" + versionInfo.MajorMinorPatch);
		Information("SemVer:\t\t\t\t\t" + versionInfo.SemVer);
		Information("LegacySemVer:\t\t\t\t" + versionInfo.LegacySemVer);
		Information("LegacySemVerPadded:\t\t\t" + versionInfo.LegacySemVerPadded);
		Information("AssemblySemVer:\t\t\t\t" + versionInfo.AssemblySemVer);
		Information("FullSemVer:\t\t\t\t" + versionInfo.FullSemVer);
		Information("InformationalVersion:\t\t\t" + versionInfo.InformationalVersion);
		Information("BranchName:\t\t\t\t" + versionInfo.BranchName);
		Information("Sha:\t\t\t\t\t" + versionInfo.Sha);
		Information("NuGetVersionV2:\t\t\t\t" + versionInfo.NuGetVersionV2);
		Information("NuGetVersion:\t\t\t\t" + versionInfo.NuGetVersion);
		Information("CommitsSinceVersionSource:\t\t" + versionInfo.CommitsSinceVersionSource);
		Information("CommitsSinceVersionSourcePadded:\t" + versionInfo.CommitsSinceVersionSourcePadded);
		Information("CommitDate:\t\t\t\t" + versionInfo.CommitDate);
  });	

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Version")
	.Description("build the library")
	.Does(() => {				

		var msBuildSettings = new DotNetMSBuildSettings()
		{
			Version =  versionInfo.AssemblySemVer,
			InformationalVersion = versionInfo.InformationalVersion,
			PackageVersion = versionInfo.SemVer
		}.WithProperty("PackageOutputPath", packageOutputDir.FullPath);	

		var settings = new DotNetBuildSettings {
			Configuration = configuration,			
			MSBuildSettings = msBuildSettings
		};	 		
	 
		DotNetBuild(solutionFile.FullPath, settings);
});

Task("Test")
	.IsDependentOn("Build")
	.Description("Executes the unit tests")
	.Does(() => {
   
	var settings = new DotNetTestSettings {
		Configuration = configuration,
		Loggers = new[]{"trx;"},
		ResultsDirectory = outputDirTests,
		Collectors = new[] {"XPlat Code Coverage"},	
		ArgumentCustomization = a => a.Append("-- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover"),
		NoBuild = true
	};		

	DotNetTest(solutionFile.FullPath, settings);	
 });

 Task("SonarBegin")
	.WithCriteria(!isLocalBuild)
	.Does(() => {
		SonarBegin(new SonarBeginSettings {
			Key = sonarProjectKey,
			Url = sonarUrl,
			Organization = sonarOrganization,
			Token = sonarLogin,
			UseCoreClr = true,
			VsTestReportsPath = testResultsPath.ToString(),
			OpenCoverReportsPath = codeCoverageResultFilePath.ToString()
		});
	});

Task("SonarEnd")
	.WithCriteria(!isLocalBuild)
	.Does(() => {
		SonarEnd(new SonarEndSettings {
			Token = sonarLogin
		});
	});

 Task("Publish")	
	.WithCriteria(isReleaseCreation)
	.IsDependentOn("Test")	
	.Description("Pushes the created NuGet packages to nuget.org")  
	.Does(() => {
	
		Information($"Upload packages from {packageOutputDir.FullPath}");

		// Get the paths to the packages ordered by the file names in order to get the nupkg first.
		var packages = GetFiles(packageOutputDir.CombineWithFilePath("*.*nupkg").ToString()).OrderBy(x => x.FullPath).ToArray();

		if (packages.Length == 0)
		{
			Error("No packages found to upload");
			return;
		}
		
		// Push the package.
		NuGetPush(packages, new NuGetPushSettings {
			Source = nugetPublishFeed,
			ApiKey = nugetApiKey,
			SkipDuplicate = true
		});	
	});

//////////////////////////////////////////////////////////////////////
// Common Targets
//////////////////////////////////////////////////////////////////////
Task("Default")
	.IsDependentOn("SonarBegin")
	.IsDependentOn("Test")	
	.IsDependentOn("SonarEnd")
	.IsDependentOn("Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
RunTarget(target);