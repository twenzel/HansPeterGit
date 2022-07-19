namespace HansPeterGit;

public record Author(string Name, string Email)
{
    public override string ToString() => $"{Name} <{Email}>";
}
