namespace EcoEarn.Indexer.Plugin;

public class ContractInfoOptions
{
    public List<ContractInfo> ContractInfos { get; set; }
}

public class ContractInfo
{
    public string ChainId { get; set; }
    public string EcoEarnContractAddress { get; set; }
    public string AElfTokenContractAddress { get; set; }
}