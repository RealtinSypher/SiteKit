namespace SiteKit;

public class Page(string content)
{
    private string _content = content;

    public Page Replace(string old, string @new)
    {
        _content = _content.Replace(old, @new);

        return this;
    }
}
