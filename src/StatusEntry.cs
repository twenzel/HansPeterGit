namespace HansPeterGit;

public class StatusEntry
{
    public FileStatus IndexStatus { get; set; }
    public FileStatus WorkDirStatus { get; set; }
    public string FilePath { get; init; } = string.Empty;
}
