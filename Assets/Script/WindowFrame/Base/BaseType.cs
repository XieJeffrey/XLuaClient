
using System.Collections.Generic;
/// <summary>
/// 窗口类型
/// </summary>


public enum WindowType
{
    LoginWindow = 1,
    FloatTipWindow = 2,  
    ConfirmWindow = 3,
    GameWindow = 4,
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
    #region LoginWindow & CreateRole
    OnLoginSuccess = 1,  //登录t7成功      

    OnRandomName = 3,
    OnCreatedRole = 4,
	#endregion
	
	#region MainWindow 10 ~ 20
	OnRefreshStories = 11,
    OnResultEat = 12,
    OnUserNameChanged = 13,
    #endregion

    #region TitleWindow 20-30
    OnUserDataChanged =21,
    #endregion

    #region SelectStoryWindow 30~40
    OnEnterBattle = 31,
    #endregion

    #region BattleWindow 41 ~ 50
    OnGameOver = 41,
    OnCollectedGold = 42,
    OnShrink=43,
    #endregion

    #region ActivityWindow 61~70
    OnDrawRespone=61,
    OnGainStrength = 62,
    #endregion

    #region SelectRoleWindow 71 ~ 80
    OnSelectedRole = 71,
    OnBoughtRole = 72,
    #endregion

    #region SkillWindow 81~90
    OnSelectSkill = 81,
    OnUpgradeSkill = 82,
    OnBoughtSkill = 83,
    OnUseSkill = 84,
    #endregion

    #region Common 很多地方会用到 1000+
    OnPlayShowOutEnd = 1001,
    OnSuccessBuyGood = 1002,
    OnSuccessSwapGood = 1003,
    OnBossStateChange = 1004,
    #endregion
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
