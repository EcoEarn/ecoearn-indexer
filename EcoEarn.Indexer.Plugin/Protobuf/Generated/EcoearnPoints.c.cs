// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: ecoearn_points.proto
// </auto-generated>
// Original file comments:
// the version of the language, use proto3 for contracts
#pragma warning disable 0414, 1591
#region Designer generated code

using System.Collections.Generic;
using aelf = global::AElf.CSharp.Core;

namespace EcoEarn.Contracts.Points {

  #region Events
  public partial class ConfigSet : aelf::IEvent<ConfigSet>
  {
    public global::System.Collections.Generic.IEnumerable<ConfigSet> GetIndexed()
    {
      return new List<ConfigSet>
      {
      };
    }

    public ConfigSet GetNonIndexed()
    {
      return new ConfigSet
      {
        Config = Config,
      };
    }
  }

  public partial class AdminSet : aelf::IEvent<AdminSet>
  {
    public global::System.Collections.Generic.IEnumerable<AdminSet> GetIndexed()
    {
      return new List<AdminSet>
      {
      };
    }

    public AdminSet GetNonIndexed()
    {
      return new AdminSet
      {
        Admin = Admin,
      };
    }
  }

  public partial class Registered : aelf::IEvent<Registered>
  {
    public global::System.Collections.Generic.IEnumerable<Registered> GetIndexed()
    {
      return new List<Registered>
      {
      };
    }

    public Registered GetNonIndexed()
    {
      return new Registered
      {
        DappId = DappId,
        Admin = Admin,
      };
    }
  }

  public partial class PointsPoolCreated : aelf::IEvent<PointsPoolCreated>
  {
    public global::System.Collections.Generic.IEnumerable<PointsPoolCreated> GetIndexed()
    {
      return new List<PointsPoolCreated>
      {
      };
    }

    public PointsPoolCreated GetNonIndexed()
    {
      return new PointsPoolCreated
      {
        DappId = DappId,
        Config = Config,
        PoolId = PoolId,
        PointsName = PointsName,
        Amount = Amount,
        PoolAddress = PoolAddress,
      };
    }
  }

  public partial class SnapshotUpdated : aelf::IEvent<SnapshotUpdated>
  {
    public global::System.Collections.Generic.IEnumerable<SnapshotUpdated> GetIndexed()
    {
      return new List<SnapshotUpdated>
      {
      };
    }

    public SnapshotUpdated GetNonIndexed()
    {
      return new SnapshotUpdated
      {
        PoolId = PoolId,
        MerkleTreeRoot = MerkleTreeRoot,
        UpdateBlockNumber = UpdateBlockNumber,
      };
    }
  }

  public partial class Claimed : aelf::IEvent<Claimed>
  {
    public global::System.Collections.Generic.IEnumerable<Claimed> GetIndexed()
    {
      return new List<Claimed>
      {
      };
    }

    public Claimed GetNonIndexed()
    {
      return new Claimed
      {
        ClaimInfo = ClaimInfo,
      };
    }
  }

  public partial class Withdrawn : aelf::IEvent<Withdrawn>
  {
    public global::System.Collections.Generic.IEnumerable<Withdrawn> GetIndexed()
    {
      return new List<Withdrawn>
      {
      };
    }

    public Withdrawn GetNonIndexed()
    {
      return new Withdrawn
      {
        ClaimInfos = ClaimInfos,
      };
    }
  }

  public partial class EarlyStaked : aelf::IEvent<EarlyStaked>
  {
    public global::System.Collections.Generic.IEnumerable<EarlyStaked> GetIndexed()
    {
      return new List<EarlyStaked>
      {
      };
    }

    public EarlyStaked GetNonIndexed()
    {
      return new EarlyStaked
      {
        PoolId = PoolId,
        Amount = Amount,
        Period = Period,
        ClaimInfos = ClaimInfos,
      };
    }
  }

  public partial class TokenRecovered : aelf::IEvent<TokenRecovered>
  {
    public global::System.Collections.Generic.IEnumerable<TokenRecovered> GetIndexed()
    {
      return new List<TokenRecovered>
      {
      };
    }

    public TokenRecovered GetNonIndexed()
    {
      return new TokenRecovered
      {
        PoolId = PoolId,
        Token = Token,
        Amount = Amount,
        Account = Account,
      };
    }
  }

  public partial class PointsPoolEndTimeSet : aelf::IEvent<PointsPoolEndTimeSet>
  {
    public global::System.Collections.Generic.IEnumerable<PointsPoolEndTimeSet> GetIndexed()
    {
      return new List<PointsPoolEndTimeSet>
      {
      };
    }

    public PointsPoolEndTimeSet GetNonIndexed()
    {
      return new PointsPoolEndTimeSet
      {
        PoolId = PoolId,
        EndTime = EndTime,
        Amount = Amount,
      };
    }
  }

  public partial class PointsPoolRestarted : aelf::IEvent<PointsPoolRestarted>
  {
    public global::System.Collections.Generic.IEnumerable<PointsPoolRestarted> GetIndexed()
    {
      return new List<PointsPoolRestarted>
      {
      };
    }

    public PointsPoolRestarted GetNonIndexed()
    {
      return new PointsPoolRestarted
      {
        PoolId = PoolId,
        Config = Config,
        Amount = Amount,
      };
    }
  }

  public partial class PointsPoolUpdateAddressSet : aelf::IEvent<PointsPoolUpdateAddressSet>
  {
    public global::System.Collections.Generic.IEnumerable<PointsPoolUpdateAddressSet> GetIndexed()
    {
      return new List<PointsPoolUpdateAddressSet>
      {
      };
    }

    public PointsPoolUpdateAddressSet GetNonIndexed()
    {
      return new PointsPoolUpdateAddressSet
      {
        PoolId = PoolId,
        UpdateAddress = UpdateAddress,
      };
    }
  }

  public partial class PointsPoolRewardReleasePeriodSet : aelf::IEvent<PointsPoolRewardReleasePeriodSet>
  {
    public global::System.Collections.Generic.IEnumerable<PointsPoolRewardReleasePeriodSet> GetIndexed()
    {
      return new List<PointsPoolRewardReleasePeriodSet>
      {
      };
    }

    public PointsPoolRewardReleasePeriodSet GetNonIndexed()
    {
      return new PointsPoolRewardReleasePeriodSet
      {
        PoolId = PoolId,
        ReleasePeriod = ReleasePeriod,
      };
    }
  }

  public partial class PointsPoolRewardPerSecondSet : aelf::IEvent<PointsPoolRewardPerSecondSet>
  {
    public global::System.Collections.Generic.IEnumerable<PointsPoolRewardPerSecondSet> GetIndexed()
    {
      return new List<PointsPoolRewardPerSecondSet>
      {
      };
    }

    public PointsPoolRewardPerSecondSet GetNonIndexed()
    {
      return new PointsPoolRewardPerSecondSet
      {
        PoolId = PoolId,
        RewardPerSecond = RewardPerSecond,
      };
    }
  }

  public partial class DappAdminSet : aelf::IEvent<DappAdminSet>
  {
    public global::System.Collections.Generic.IEnumerable<DappAdminSet> GetIndexed()
    {
      return new List<DappAdminSet>
      {
      };
    }

    public DappAdminSet GetNonIndexed()
    {
      return new DappAdminSet
      {
        DappId = DappId,
        Admin = Admin,
      };
    }
  }

  #endregion
  public static partial class EcoEarnPointsContractContainer
  {
    static readonly string __ServiceName = "EcoEarnPointsContract";

    #region Marshallers
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.InitializeInput> __Marshaller_InitializeInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.InitializeInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::Google.Protobuf.WellKnownTypes.Empty> __Marshaller_google_protobuf_Empty = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Google.Protobuf.WellKnownTypes.Empty.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.Config> __Marshaller_Config = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.Config.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::AElf.Types.Address> __Marshaller_aelf_Address = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::AElf.Types.Address.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.RegisterInput> __Marshaller_RegisterInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.RegisterInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.SetDappAdminInput> __Marshaller_SetDappAdminInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.SetDappAdminInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::AElf.Types.Hash> __Marshaller_aelf_Hash = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::AElf.Types.Hash.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.DappInfo> __Marshaller_DappInfo = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.DappInfo.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.CreatePointsPoolInput> __Marshaller_CreatePointsPoolInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.CreatePointsPoolInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.SetPointsPoolEndTimeInput> __Marshaller_SetPointsPoolEndTimeInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.SetPointsPoolEndTimeInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.RestartPointsPoolInput> __Marshaller_RestartPointsPoolInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.RestartPointsPoolInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.SetPointsPoolUpdateAddressInput> __Marshaller_SetPointsPoolUpdateAddressInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.SetPointsPoolUpdateAddressInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.SetPointsPoolRewardReleasePeriodInput> __Marshaller_SetPointsPoolRewardReleasePeriodInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.SetPointsPoolRewardReleasePeriodInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.SetPointsPoolRewardPerSecondInput> __Marshaller_SetPointsPoolRewardPerSecondInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.SetPointsPoolRewardPerSecondInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.GetPoolInfoOutput> __Marshaller_GetPoolInfoOutput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.GetPoolInfoOutput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.UpdateSnapshotInput> __Marshaller_UpdateSnapshotInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.UpdateSnapshotInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.GetSnapshotInput> __Marshaller_GetSnapshotInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.GetSnapshotInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.Snapshot> __Marshaller_Snapshot = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.Snapshot.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.ClaimInput> __Marshaller_ClaimInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.ClaimInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.ClaimInfo> __Marshaller_ClaimInfo = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.ClaimInfo.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.WithdrawInput> __Marshaller_WithdrawInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.WithdrawInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.EarlyStakeInput> __Marshaller_EarlyStakeInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.EarlyStakeInput.Parser.ParseFrom);
    static readonly aelf::Marshaller<global::EcoEarn.Contracts.Points.RecoverTokenInput> __Marshaller_RecoverTokenInput = aelf::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::EcoEarn.Contracts.Points.RecoverTokenInput.Parser.ParseFrom);
    #endregion

    #region Methods
    static readonly aelf::Method<global::EcoEarn.Contracts.Points.InitializeInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_Initialize = new aelf::Method<global::EcoEarn.Contracts.Points.InitializeInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "Initialize",
        __Marshaller_InitializeInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::EcoEarn.Contracts.Points.Config, global::Google.Protobuf.WellKnownTypes.Empty> __Method_SetConfig = new aelf::Method<global::EcoEarn.Contracts.Points.Config, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "SetConfig",
        __Marshaller_Config,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::EcoEarn.Contracts.Points.Config> __Method_GetConfig = new aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::EcoEarn.Contracts.Points.Config>(
        aelf::MethodType.View,
        __ServiceName,
        "GetConfig",
        __Marshaller_google_protobuf_Empty,
        __Marshaller_Config);

    static readonly aelf::Method<global::AElf.Types.Address, global::Google.Protobuf.WellKnownTypes.Empty> __Method_SetAdmin = new aelf::Method<global::AElf.Types.Address, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "SetAdmin",
        __Marshaller_aelf_Address,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::AElf.Types.Address> __Method_GetAdmin = new aelf::Method<global::Google.Protobuf.WellKnownTypes.Empty, global::AElf.Types.Address>(
        aelf::MethodType.View,
        __ServiceName,
        "GetAdmin",
        __Marshaller_google_protobuf_Empty,
        __Marshaller_aelf_Address);

    static readonly aelf::Method<global::EcoEarn.Contracts.Points.RegisterInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_Register = new aelf::Method<global::EcoEarn.Contracts.Points.RegisterInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "Register",
        __Marshaller_RegisterInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::EcoEarn.Contracts.Points.SetDappAdminInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_SetDappAdmin = new aelf::Method<global::EcoEarn.Contracts.Points.SetDappAdminInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "SetDappAdmin",
        __Marshaller_SetDappAdminInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::AElf.Types.Hash, global::EcoEarn.Contracts.Points.DappInfo> __Method_GetDappInfo = new aelf::Method<global::AElf.Types.Hash, global::EcoEarn.Contracts.Points.DappInfo>(
        aelf::MethodType.View,
        __ServiceName,
        "GetDappInfo",
        __Marshaller_aelf_Hash,
        __Marshaller_DappInfo);

    static readonly aelf::Method<global::EcoEarn.Contracts.Points.CreatePointsPoolInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_CreatePointsPool = new aelf::Method<global::EcoEarn.Contracts.Points.CreatePointsPoolInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "CreatePointsPool",
        __Marshaller_CreatePointsPoolInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::EcoEarn.Contracts.Points.SetPointsPoolEndTimeInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_SetPointsPoolEndTime = new aelf::Method<global::EcoEarn.Contracts.Points.SetPointsPoolEndTimeInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "SetPointsPoolEndTime",
        __Marshaller_SetPointsPoolEndTimeInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::EcoEarn.Contracts.Points.RestartPointsPoolInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_RestartPointsPool = new aelf::Method<global::EcoEarn.Contracts.Points.RestartPointsPoolInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "RestartPointsPool",
        __Marshaller_RestartPointsPoolInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::EcoEarn.Contracts.Points.SetPointsPoolUpdateAddressInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_SetPointsPoolUpdateAddress = new aelf::Method<global::EcoEarn.Contracts.Points.SetPointsPoolUpdateAddressInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "SetPointsPoolUpdateAddress",
        __Marshaller_SetPointsPoolUpdateAddressInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::EcoEarn.Contracts.Points.SetPointsPoolRewardReleasePeriodInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_SetPointsPoolRewardReleasePeriod = new aelf::Method<global::EcoEarn.Contracts.Points.SetPointsPoolRewardReleasePeriodInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "SetPointsPoolRewardReleasePeriod",
        __Marshaller_SetPointsPoolRewardReleasePeriodInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::EcoEarn.Contracts.Points.SetPointsPoolRewardPerSecondInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_SetPointsPoolRewardPerSecond = new aelf::Method<global::EcoEarn.Contracts.Points.SetPointsPoolRewardPerSecondInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "SetPointsPoolRewardPerSecond",
        __Marshaller_SetPointsPoolRewardPerSecondInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::AElf.Types.Hash, global::EcoEarn.Contracts.Points.GetPoolInfoOutput> __Method_GetPoolInfo = new aelf::Method<global::AElf.Types.Hash, global::EcoEarn.Contracts.Points.GetPoolInfoOutput>(
        aelf::MethodType.View,
        __ServiceName,
        "GetPoolInfo",
        __Marshaller_aelf_Hash,
        __Marshaller_GetPoolInfoOutput);

    static readonly aelf::Method<global::AElf.Types.Hash, global::AElf.Types.Address> __Method_GetPoolAddress = new aelf::Method<global::AElf.Types.Hash, global::AElf.Types.Address>(
        aelf::MethodType.View,
        __ServiceName,
        "GetPoolAddress",
        __Marshaller_aelf_Hash,
        __Marshaller_aelf_Address);

    static readonly aelf::Method<global::EcoEarn.Contracts.Points.UpdateSnapshotInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_UpdateSnapshot = new aelf::Method<global::EcoEarn.Contracts.Points.UpdateSnapshotInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "UpdateSnapshot",
        __Marshaller_UpdateSnapshotInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::EcoEarn.Contracts.Points.GetSnapshotInput, global::EcoEarn.Contracts.Points.Snapshot> __Method_GetSnapshot = new aelf::Method<global::EcoEarn.Contracts.Points.GetSnapshotInput, global::EcoEarn.Contracts.Points.Snapshot>(
        aelf::MethodType.View,
        __ServiceName,
        "GetSnapshot",
        __Marshaller_GetSnapshotInput,
        __Marshaller_Snapshot);

    static readonly aelf::Method<global::EcoEarn.Contracts.Points.ClaimInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_Claim = new aelf::Method<global::EcoEarn.Contracts.Points.ClaimInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "Claim",
        __Marshaller_ClaimInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::AElf.Types.Hash, global::EcoEarn.Contracts.Points.ClaimInfo> __Method_GetClaimInfo = new aelf::Method<global::AElf.Types.Hash, global::EcoEarn.Contracts.Points.ClaimInfo>(
        aelf::MethodType.View,
        __ServiceName,
        "GetClaimInfo",
        __Marshaller_aelf_Hash,
        __Marshaller_ClaimInfo);

    static readonly aelf::Method<global::EcoEarn.Contracts.Points.WithdrawInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_Withdraw = new aelf::Method<global::EcoEarn.Contracts.Points.WithdrawInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "Withdraw",
        __Marshaller_WithdrawInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::EcoEarn.Contracts.Points.EarlyStakeInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_EarlyStake = new aelf::Method<global::EcoEarn.Contracts.Points.EarlyStakeInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "EarlyStake",
        __Marshaller_EarlyStakeInput,
        __Marshaller_google_protobuf_Empty);

    static readonly aelf::Method<global::EcoEarn.Contracts.Points.RecoverTokenInput, global::Google.Protobuf.WellKnownTypes.Empty> __Method_RecoverToken = new aelf::Method<global::EcoEarn.Contracts.Points.RecoverTokenInput, global::Google.Protobuf.WellKnownTypes.Empty>(
        aelf::MethodType.Action,
        __ServiceName,
        "RecoverToken",
        __Marshaller_RecoverTokenInput,
        __Marshaller_google_protobuf_Empty);

    #endregion

    #region Descriptors
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::EcoEarn.Contracts.Points.EcoearnPointsReflection.Descriptor.Services[0]; }
    }

    public static global::System.Collections.Generic.IReadOnlyList<global::Google.Protobuf.Reflection.ServiceDescriptor> Descriptors
    {
      get
      {
        return new global::System.Collections.Generic.List<global::Google.Protobuf.Reflection.ServiceDescriptor>()
        {
          global::AElf.Standards.ACS12.Acs12Reflection.Descriptor.Services[0],
          global::EcoEarn.Contracts.Points.EcoearnPointsReflection.Descriptor.Services[0],
        };
      }
    }
    #endregion

    // /// <summary>Base class for the contract of EcoEarnPointsContract</summary>
    // public abstract partial class EcoEarnPointsContractBase : AElf.Sdk.CSharp.CSharpSmartContract<EcoEarn.Contracts.Points.EcoEarnPointsContractState>
    // {
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty Initialize(global::EcoEarn.Contracts.Points.InitializeInput input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty SetConfig(global::EcoEarn.Contracts.Points.Config input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::EcoEarn.Contracts.Points.Config GetConfig(global::Google.Protobuf.WellKnownTypes.Empty input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty SetAdmin(global::AElf.Types.Address input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::AElf.Types.Address GetAdmin(global::Google.Protobuf.WellKnownTypes.Empty input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty Register(global::EcoEarn.Contracts.Points.RegisterInput input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty SetDappAdmin(global::EcoEarn.Contracts.Points.SetDappAdminInput input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::EcoEarn.Contracts.Points.DappInfo GetDappInfo(global::AElf.Types.Hash input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty CreatePointsPool(global::EcoEarn.Contracts.Points.CreatePointsPoolInput input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty SetPointsPoolEndTime(global::EcoEarn.Contracts.Points.SetPointsPoolEndTimeInput input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty RestartPointsPool(global::EcoEarn.Contracts.Points.RestartPointsPoolInput input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty SetPointsPoolUpdateAddress(global::EcoEarn.Contracts.Points.SetPointsPoolUpdateAddressInput input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty SetPointsPoolRewardReleasePeriod(global::EcoEarn.Contracts.Points.SetPointsPoolRewardReleasePeriodInput input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty SetPointsPoolRewardPerSecond(global::EcoEarn.Contracts.Points.SetPointsPoolRewardPerSecondInput input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::EcoEarn.Contracts.Points.GetPoolInfoOutput GetPoolInfo(global::AElf.Types.Hash input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::AElf.Types.Address GetPoolAddress(global::AElf.Types.Hash input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty UpdateSnapshot(global::EcoEarn.Contracts.Points.UpdateSnapshotInput input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::EcoEarn.Contracts.Points.Snapshot GetSnapshot(global::EcoEarn.Contracts.Points.GetSnapshotInput input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty Claim(global::EcoEarn.Contracts.Points.ClaimInput input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::EcoEarn.Contracts.Points.ClaimInfo GetClaimInfo(global::AElf.Types.Hash input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty Withdraw(global::EcoEarn.Contracts.Points.WithdrawInput input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty EarlyStake(global::EcoEarn.Contracts.Points.EarlyStakeInput input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    //   public virtual global::Google.Protobuf.WellKnownTypes.Empty RecoverToken(global::EcoEarn.Contracts.Points.RecoverTokenInput input)
    //   {
    //     throw new global::System.NotImplementedException();
    //   }
    //
    // }
    //
    // public static aelf::ServerServiceDefinition BindService(EcoEarnPointsContractBase serviceImpl)
    // {
    //   return aelf::ServerServiceDefinition.CreateBuilder()
    //       .AddDescriptors(Descriptors)
    //       .AddMethod(__Method_Initialize, serviceImpl.Initialize)
    //       .AddMethod(__Method_SetConfig, serviceImpl.SetConfig)
    //       .AddMethod(__Method_GetConfig, serviceImpl.GetConfig)
    //       .AddMethod(__Method_SetAdmin, serviceImpl.SetAdmin)
    //       .AddMethod(__Method_GetAdmin, serviceImpl.GetAdmin)
    //       .AddMethod(__Method_Register, serviceImpl.Register)
    //       .AddMethod(__Method_SetDappAdmin, serviceImpl.SetDappAdmin)
    //       .AddMethod(__Method_GetDappInfo, serviceImpl.GetDappInfo)
    //       .AddMethod(__Method_CreatePointsPool, serviceImpl.CreatePointsPool)
    //       .AddMethod(__Method_SetPointsPoolEndTime, serviceImpl.SetPointsPoolEndTime)
    //       .AddMethod(__Method_RestartPointsPool, serviceImpl.RestartPointsPool)
    //       .AddMethod(__Method_SetPointsPoolUpdateAddress, serviceImpl.SetPointsPoolUpdateAddress)
    //       .AddMethod(__Method_SetPointsPoolRewardReleasePeriod, serviceImpl.SetPointsPoolRewardReleasePeriod)
    //       .AddMethod(__Method_SetPointsPoolRewardPerSecond, serviceImpl.SetPointsPoolRewardPerSecond)
    //       .AddMethod(__Method_GetPoolInfo, serviceImpl.GetPoolInfo)
    //       .AddMethod(__Method_GetPoolAddress, serviceImpl.GetPoolAddress)
    //       .AddMethod(__Method_UpdateSnapshot, serviceImpl.UpdateSnapshot)
    //       .AddMethod(__Method_GetSnapshot, serviceImpl.GetSnapshot)
    //       .AddMethod(__Method_Claim, serviceImpl.Claim)
    //       .AddMethod(__Method_GetClaimInfo, serviceImpl.GetClaimInfo)
    //       .AddMethod(__Method_Withdraw, serviceImpl.Withdraw)
    //       .AddMethod(__Method_EarlyStake, serviceImpl.EarlyStake)
    //       .AddMethod(__Method_RecoverToken, serviceImpl.RecoverToken).Build();
    // }

  }
}
#endregion

