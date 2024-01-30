namespace Luban.Utils;

public static class CollectionExtensions
{
    public static void AddAll<K, V>(this Dictionary<K, V> resultDic, Dictionary<K, V> addDic, bool overWrite = true) where K : notnull
    {
        if (addDic == null)
        {
            return;
        }
        foreach (var e in addDic)
        {
            if (overWrite)
            {
                resultDic[e.Key] = e.Value;
            }
            else
            {
                if(resultDic.ContainsKey(e.Key)==false)
                    resultDic[e.Key] = e.Value;
            }
        }
    }
}
