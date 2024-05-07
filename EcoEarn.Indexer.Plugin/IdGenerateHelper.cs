namespace EcoEarn.Indexer.Plugin;

public class IdGenerateHelper
{
    public static string GetId(params object[] inputs)
    {
        return inputs.JoinAsString("-");
    }
}