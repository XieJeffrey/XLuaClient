using UnityEngine;
using System.Collections;


namespace NetWorkManager
{
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

        public static byte[] Decrypt(short protocal, byte[] oldBytes,string key)
        {
            return null;
        }

        public static byte[] Encrypt(short protocal, byte[] oldBytes, string key)
        {           
            return null;
        }        
    }
}