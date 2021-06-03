using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class Crypto
{

    public static string SHA1(string str)
    {
        SHA256 sh = SHA256.Create();
        sh.ComputeHash(Encoding.ASCII.GetBytes(str));
        byte[] re = sh.Hash;
        StringBuilder sb = new StringBuilder();
        foreach (byte b in re)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }

}

