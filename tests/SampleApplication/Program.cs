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

//options.PathToGit = @"C:\Data\Git\ApiDocumentationPublisher\BuildArtifacts\Temp\App\git\cmd\git.exe";
//options.Authentication = new BasicAuthentication("pat", "aaa");

var repository = GitRepository.Clone("https://github.com/twenzel/CGM.git", options);
repository.Checkout("master");
//repository.Checkout("tags/v1.1.5");

repository.Pull();

File.WriteAllText(Path.Combine(repository.WorkingDirectory, "test.txt"), "hello there");

repository.StageAll();

var status = repository.GetStatus();

if (status.IsDirty)
{
    var commit = repository.Commit("add test file");

    logger.LogInformation("Pushed new commit {commit} to repository", commit.Id);
}

