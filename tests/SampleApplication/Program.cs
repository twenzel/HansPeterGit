using HansPeterGit;
using Microsoft.Extensions.Logging;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Debug)
        .AddConsole();
});
ILogger logger = loggerFactory.CreateLogger<Program>();

var workingdir = @"c:\temp\testgit";

Helper.DeleteDirectory(workingdir);

var options = new GitOptions(workingdir, logger);

options.PathToGit = @"C:\Data\Git\ApiDocumentationPublisher\BuildArtifacts\App\git\cmd\git.exe";
//options.Authentication = new BasicAuthentication("pat", "aaa");

var repository = GitRepository.Clone("https://dev.chg-meridian.com/DefaultCollection/API%20Documentation/_git/api-documentation-internal", options);

File.WriteAllText(Path.Combine(repository.WorkingDirectory, "test.txt"), "hello there");

repository.StageAll();

var status = repository.GetStatus();

if (status.IsDirty)
{
    var commit = repository.Commit("add test file");

    logger.LogInformation("Pushed new commit {commit} to repository", commit.Id);
}

