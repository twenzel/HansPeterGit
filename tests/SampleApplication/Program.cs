using HansPeterGit;
using Microsoft.Extensions.Logging;

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Debug)
        .AddConsole();
});
ILogger logger = loggerFactory.CreateLogger<Program>();


var repository = GitRepository.Clone("https://github.com/twenzel/HansPeterGit", @"c:\temp\testgit", logger);

File.WriteAllText(Path.Combine(repository.WorkingDirectory, "test.txt"), "hello there");

repository.StageAll();

var status = repository.GetStatus();

if (status.IsDirty)
{
    var commit = repository.Commit("add test file");

    logger.LogInformation("Pushed new commit {commit} to repository", commit.Id);
}