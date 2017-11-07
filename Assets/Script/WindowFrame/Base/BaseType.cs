
using System.Collections.Generic;
/// <summary>
/// 窗口类型
/// </summary>


public enum WindowType
{
    LoginWindow = 1,
}

public enum ManagerType
{
    LoginMgr = 1,
}

public enum ConfigType
{
    ConfigDataBase = 1,
    LanguageDataBase = 2,
    GameDataBase = 3,
}

/// <summary>
/// UI事件类型
/// </summary>
public enum UIEventType
{
    #region LoginWindow
    OnLoginSuccess = 1,  //登录t7成功      
    #endregion

    #region Common 很多地方会用到 1000+
    OnSynCountDown = 1001,
    OnOutTimeBuckleLife = 1002,
    OnVictoryOrDefeat = 1003,     //小游戏结束时发出来的
    OnGameResultBack = 1004,       //由服务器返回的消息发起

    OnHallException = 1005,
    OnHallDisconnect = 1006,
    OnHallNetNotConnect = 1007,
    OnBt7Excpetion = 1008,
    OnBt7Disconnect = 1009,
    OnBt7NetNotConnect = 1010,
    RefreshRedTip = 1011,
    OnRefreshPing = 1012,

    OnPkOfflineLoseLife = 1013,
    #endregion
}

public enum MgrEventType
{
    ConnectHallResult = 1,
}

public enum ResourceType
{
    Resource = 1,
    AssetBundle = 2,
}

public enum TipType
{
    NormalTip = 1,
    OfflineTip = 2,
    CodeTip = 3,
    NickNameTip = 4,
}

public enum ConfirmWindowType
{
    None = 0,
    OK = 1,
    OKAndCancle = 2,
}
