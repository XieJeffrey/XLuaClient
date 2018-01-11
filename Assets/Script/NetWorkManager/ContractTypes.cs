using System;
namespace Protocal
{
    public class ContractTypes
    {
        #region 框架
        /// <summary>
        /// 心跳
        /// </summary>
        public const short Heartbeat = -1;

        /// <summary>
        /// 退出
        /// </summary>
        public const short LogOut = -3;

        /// <summary>
        /// 被踢出
        /// </summary>
        public const short KictedOut = -4;

        /// <summary>
        /// 紧急通知
        /// </summary>
        public const short UrgentNotify = -5;

        /// <summary>
        /// 请求密钥
        /// </summary>
        public const short ReqestKey = 2;

        /// <summary>
        /// 返回加密模块
        /// </summary>
        public const short ReturnSecurityModule = 3;

        /// <summary>
        /// 请求服务器信息
        /// </summary>
        public const short RequestServerInfo = 4;

        /// <summary>
        /// 发送服务器信息
        /// </summary>
        public const short SendServerInfo = 5;

        /// <summary>
        /// 用户登录/登出
        /// </summary>
        public const short UserInOrOut = 6;

        /// <summary>
        /// 退出服务器
        /// </summary>
        public const short ExitServer = 8;
        #endregion


        #region Login
        /// <summary>
        /// 登录
        /// </summary>
        public const short Login = 101;
        /// <summary>
        /// 返回登录
        /// </summary>
        public const short ResultLogin = 102;
        /// <summary>
        /// 注册用户
        /// </summary>
        public const short RegisterUser = 103;
        /// <summary>
        /// 返回注册用户状态
        /// </summary>
        public const short ResultRegisterUser = 104;
        /// <summary>
        /// 获取验证码
        /// </summary>
        public const short GetVerifyCode = 105;
        /// <summary>
        /// 验证码是否正确
        /// </summary>
        public const short IsVerifyCode = 107;
        /// <summary>
        /// 返回验证码结果 
        /// </summary>
        public const short ResultIsVerifyCode = 108;
        /// <summary>
        /// 生成用户名
        /// </summary>
        public const short CreateUserName = 109;
        /// <summary>
        /// 返回用户名
        /// </summary>
        public const short ResultCreateUserName = 110;
        /// <summary>
        /// 获取关卡信息
        /// </summary>
        public const short GetHurdleLevel = 111;
        /// <summary>
        /// 返回关卡信息
        /// </summary>
        public const short ResultGetHurdleLevel = 112;
        /// <summary>
        /// 抽奖
        /// </summary>
        public const short LuckDraw = 113;
        /// <summary>
        /// 抽奖结果
        /// </summary>
        public const short ResultLuckDraw = 114;
        /// <summary>
        /// 购买角色
        /// </summary>
        public const short BuyRole = 115;
        /// <summary>
        /// 返回购买角色结果
        /// </summary>
        public const short ResultBuyRole = 116;
        /// <summary>
        /// 技能升级
        /// </summary>
        public const short SkillUpgrade = 117;
        /// <summary>
        /// 返回技能升级
        /// </summary>
        public const short ResultSkillUpgrade = 118;
        /// <summary>
        /// 选择角色
        /// </summary>
        public const short SelectRole = 119;
        /// <summary>
        /// 返回选择角色结果
        /// </summary>
        public const short ResultSelectRole = 120;
        /// <summary>
        /// 选择技能
        /// </summary>
        public const short SelectSkill = 121;
        /// <summary>
        /// 返回选择技能结果
        /// </summary>
        public const short ResultSelectSkill = 122;
        /// <summary>
        /// 购买技能数
        /// </summary>
        public const short BuySkillCount = 123;
        /// <summary>
        /// 返回购买技能数结果
        /// </summary>
        public const short ResultBuySkillCount = 124;
        /// <summary>
        /// 使用技能
        /// </summary>
        public const short UseSkill = 125;
        /// <summary>
        /// 返回使用技能
        /// </summary>
        public const short ResultUseSkill = 126;
        /// <summary>
        /// 开始吃包子
        /// </summary>
        public const short StartEatBun = 127;
        /// <summary>
        /// 返回开始吃包子结果
        /// </summary>
        public const short ResultStartEat = 128;
        /// <summary>
        /// 获得吃包子
        /// </summary>
        public const short GetEatBun = 129;
        /// <summary>
        /// 返回吃包子结果
        /// </summary>
        public const short ResultGetEatBun = 130;
        /// <summary>
        /// 修改用户名称
        /// </summary>
        public const short UpdateName = 131;
        /// <summary>
        /// 返回修改用户名称
        /// </summary>
        public const short ResultUpdateName = 132;

        /// <summary>
        /// 购买商品
        /// </summary>
       // public const short BuyGoods = 133;
        /// <summary>
        /// 购买商品返回结果
        /// </summary>
       // public const short ResultBuyGoods = 134;
        /// <summary>
        /// 购买道具
        /// </summary>
        public const short BuyProp = 135;
        /// <summary>
        /// 返回购买道具
        /// </summary>
        public const short ResultBuyProp = 136;
        /// <summary>
        /// 互换商品
        /// </summary>
        public const short SwapGoods = 137;
        /// <summary>
        /// 返回互换商品
        /// </summary>
        public const short ResultSwapGoods = 138;

        /// <summary>
        /// 开始游戏
        /// </summary>
        public const short StartGame = 301;
        /// <summary>
        /// 返回开始游戏结果
        /// </summary>
        public const short ResultStartGame = 302;

        /// <summary>
        /// 游戏结束
        /// </summary>
        public const short GameOver = 303;
        /// <summary>
        /// 返回游戏结束
        /// </summary>
        public const short ResultGameOver = 304;

        /// <summary>
        /// 我的公会信息
        /// </summary>
        public const short MyGuildInfo = 501;
        /// <summary>
        /// 返回我的公会信息
        /// </summary>
        public const short ResultMyGuildInfo = 502;
        /// <summary>
        /// 签到
        /// </summary>
        public const short IsSign = 503;
        /// <summary>
        /// 返回是否签到
        /// </summary>
        public const short ResultIsSign = 504;
        /// <summary>
        /// 广播
        /// </summary>
        public const short BroadChangeActiveValue = 506;
        /// <summary>
        /// 修改公会名
        /// </summary>
        public const short UpdateGuildName = 507;
        /// <summary>
        /// 返回修改公会名
        /// </summary>
        public const short ResultUpdateGuildName = 508;
        /// <summary>
        /// 公会踢人
        /// </summary>
        public const short GuildKick = 509;
        /// <summary>
        /// 返回公会踢人
        /// </summary>
        public const short ResultGuildKick = 510;
        /// <summary>
        ///广播公会踢人
        /// </summary>
        public const short BroadGuildKick = 512;
        /// <summary>
        /// 转公会
        /// </summary>
        public const short TurnGuild = 513;
        /// <summary>
        /// 返回转公会
        /// </summary>
        public const short ResultTurnGuild = 514;
        /// <summary>
        /// 广播退出公会
        /// </summary>
        public const short BroadExiteGuild = 516;
        /// <summary>
        /// 广播加入公会
        /// </summary>
        public const short BroadJoinGuild = 518;
        /// <summary>
        /// 发送聊天
        /// </summary>
        public const short SendChar = 519;
        /// <summary>
        /// 广播聊天
        /// </summary>
        public const short BroadChar = 520;
        /// <summary>
        /// 上下线通知
        /// </summary>
        public const short GuildOnlineMember = 522;

        #endregion
    }
}
