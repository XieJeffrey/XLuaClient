using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;


    public class Global : Singleton<Global>
    {
        public void InitManager()
        {

        }

        public void StopAllCiroutines()
        {

        }

        public void PreLoad()
        {

        }

        public void LoadConfig()
        {

        }
        public void OnUpdate()
        {
        }

        #region  全局数据
        public int ClientType = 0;
        public string HallIPAddress = "";
        public int HallPort = 0;

        public class HallData
        {
            public string UserID = "";   //玩家账号
            public string Name = "";     //玩家姓名
            public string HeadIco = "";    //玩家头像
            public int Wealth = 0;       //礼券数
            public bool Novice = false;  //是否是新手玩家
        };
        public static HallData halldata = new HallData();

        public class Bt7Data
        {
            public bool Gameing = false;   //是否在游戏中

        }
        public static Bt7Data bt7data = new Bt7Data();
        #endregion

    }


