namespace EcoEarn.Indexer.Plugin;

public class ContractInfoOptions
{
    public List<ContractInfo> ContractInfos { get; set; }
}

public class ContractInfo
{
    public string ChainId { get; set; }
    public string EcoEarnPointsContractAddress { get; set; }
    public string EcoEarnTokenContractAddress { get; set; }
    public string EcoEarnRewardsContractAddress { get; set; }
    public string AElfTokenContractAddress { get; set; }
}