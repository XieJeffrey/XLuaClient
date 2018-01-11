using UnityEngine;
using System.Collections;
using JH.Game.Security.AES;
using JH.Game.Security.Interface;
using JH.Game.Security.RSA;
using JH.Game.Security.ThreeDes;

public enum ServiceType
{
    AES,
    RSA,
    DES,
}

public class SecurityManager
{

    static string aes = @"nihaoJH2013";
    static string rsa = @"<RSAKeyValue><Modulus>rkJ7cz6ZERne/0GIa0ehXjRrUvL2nkHDBn+mRpv5tzWbwvRXyGTxBYSMc6TKFI4Bt20Uc1dpO1t6DHPCuXOn+w==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

    public static byte[] Decrypt(short protocal, byte[] oldBytes, string key)
    {
        byte[] newBytes = null;
        // 解密
        if (protocal == Protocal.ContractTypes.ResultLogin)//AES
        {
            ICryptography item = GetService(ServiceType.AES, aes);
            if (item != null)
            {
                newBytes = item.Decrypt(oldBytes);
            }
            else
            {
                newBytes = oldBytes;
                Debug.LogError("Encryption failed! With no encryption to send!");
            }
        }
        else
        {
            if (key == null || key.Equals(""))
            {
                newBytes = oldBytes;
                Debug.LogError("key == null or key == ''");
            }
            else
            {
                ICryptography item = GetService(ServiceType.DES, key);
                if (item != null)
                {
                    newBytes = item.Decrypt(oldBytes);
                }
                else
                {
                    newBytes = oldBytes;
                    Debug.LogError("Encryption failed! With no encryption to send!");
                }
            }
        }
        return newBytes;
    }

    public static byte[] Encrypt(short protocal, byte[] oldBytes, string key)
    {
        byte[] newBytes = null;

        if (protocal == Protocal.ContractTypes.Login)//RSA
        {
            ICryptography item = GetService(ServiceType.RSA, rsa);
            if (item != null)
            {
                newBytes = item.Encrypt(oldBytes);
            }
            else
            {
                newBytes = oldBytes;
                Debug.LogError("Encryption failed! With no encryption to send!");
            }
        }
        else
        {
            if (key == null || key.Equals(""))
            {
                newBytes = oldBytes;
                Debug.LogError("AppConst.desKey == null or AppConst.desKey == ''");
            }
            else
            {
                ICryptography item = GetService(ServiceType.DES, key);
                if (item != null)
                {
                    newBytes = item.Encrypt(oldBytes);
                }
                else
                {
                    newBytes = oldBytes;
                    Debug.LogError("Encryption failed! With no encryption to send!");
                }
            }
        }
        return newBytes;
    }

    /// <summary>
    /// 加密模块的创建
    /// </summary>
    /// <param name="type">加密类型</param>
    /// <param name="key">加密密钥</param>
    private static ICryptography GetService(ServiceType type, string key)
    {
        ICryptography _cryptography = null;
        if (type.Equals(ServiceType.AES))
        {
            AESService aes = new AESService();
            aes.Key = key;
            _cryptography = aes;

        }
        else if (type.Equals(ServiceType.DES))
        {
            ThreeDesService des = new ThreeDesService();
            des.Key = key;
            _cryptography = des;
        }
        else if (type.Equals(ServiceType.RSA))
        {
            RSAService rsa = new RSAService();
            rsa.PublicKey = key;
            _cryptography = rsa;
        }
        else
        {
        }
        return _cryptography;
    }
}