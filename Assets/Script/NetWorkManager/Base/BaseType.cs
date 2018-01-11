using System.Collections.Generic;

public enum ManagerType
{

}

//logic管理器之间的消息通知
public enum NetEventType
{
    Connected = 1,
    TimeOut = 2,

    #region Common
    OnSetUpdateWindowTipTxt = 1002,
    #endregion
}