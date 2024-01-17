namespace Luban.L10N;

public class TextKeyCollection
{
    private readonly HashSet<string> _keys = new();

    public void AddKey(string key)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            _keys.Add(key);
        }
    }

    public IEnumerable<string> Keys => _keys;
}
