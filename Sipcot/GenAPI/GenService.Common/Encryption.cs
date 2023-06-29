using System;
using System.Security.Cryptography;
using System.Text;

namespace GenService.Common
{
    public class Cryptography
    {
        public static string EncryptData(string StringToEncrypt)
        {
            string strEncodingKey = "";
            RSACryptoServiceProvider RSACrypto;
            byte[] bHash, bEncryptedData;
            StringBuilder sbEncryptedData = new StringBuilder();
            strEncodingKey = @"<RSAKeyValue><Modulus>kgjZ3OWr1f7QlVbb+jHeB7uyXLa/N2PQVfc37T/8XscCyDH+JsMvbXaM5n2p7c0pi6b+VY+u9/HDJQEZGQXolhJ2zm9rCdJL6nzAVBtcBVfkurKNUhpp8+ENNlzVETaC18PqSztcrjBR00juAswHmfoszCFoeg+DkVSi5btV9hJMvgA7k3LPyaKYJMgrfAbAU0Uh6ruFMuWBhLJLKIzOnAzRsK+LB0EFvnLit4Nv/I7GIv4tBdq3Ujhb2Lb8Yh7p9t4mKJKe8QkK9mkfrFBaksjvOFzZ27nuOiWzg8+99IvLwtt8ApKtp+zxk/b5hpMKCZICsdJnLSCEruuO4DRoaw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
            sbEncryptedData = new StringBuilder();
            RSACrypto = new RSACryptoServiceProvider();
            bHash = System.Text.Encoding.Unicode.GetBytes(StringToEncrypt.ToCharArray(), 0, StringToEncrypt.ToCharArray().Length);
            RSACrypto.FromXmlString(strEncodingKey);
            bEncryptedData = RSACrypto.Encrypt(bHash, false);

            foreach (byte b in bEncryptedData)
                sbEncryptedData.Append(String.Format("{0:X2}", b));
            return sbEncryptedData.ToString();
        }

        public static string DecryptData(string StringToDecrypt)
        {
            byte[] bEncryptedData, bDecryptedData;
            string strHex, strDecodingKey = "", strDecryptedString = "";
            int intCounter, intPos = 0;
            RSACryptoServiceProvider RSACrypto;

            bEncryptedData = new byte[StringToDecrypt.Length / 2];

            for (intCounter = 0; intCounter < bEncryptedData.Length; intCounter++)
            {
                strHex = new String(new char[] { StringToDecrypt[intPos], StringToDecrypt[intPos + 1] });
                bEncryptedData[intCounter] = byte.Parse(strHex, System.Globalization.NumberStyles.HexNumber);
                intPos += 2;
            }

            RSACrypto = new RSACryptoServiceProvider();

            strDecodingKey = @"<RSAKeyValue><Modulus>kgjZ3OWr1f7QlVbb+jHeB7uyXLa/N2PQVfc37T/8XscCyDH+JsMvbXaM5n2p7c0pi6b+VY+u9/HDJQEZGQXolhJ2zm9rCdJL6nzAVBtcBVfkurKNUhpp8+ENNlzVETaC18PqSztcrjBR00juAswHmfoszCFoeg+DkVSi5btV9hJMvgA7k3LPyaKYJMgrfAbAU0Uh6ruFMuWBhLJLKIzOnAzRsK+LB0EFvnLit4Nv/I7GIv4tBdq3Ujhb2Lb8Yh7p9t4mKJKe8QkK9mkfrFBaksjvOFzZ27nuOiWzg8+99IvLwtt8ApKtp+zxk/b5hpMKCZICsdJnLSCEruuO4DRoaw==</Modulus><Exponent>AQAB</Exponent><P>w5O+ATGw5zqP6YRWuWybuXIxQ0hvXY4WGRxE451edYMqQ7roIs5Pqmrvtaa/lVMAFuvbjBRt8CC5oLBbx4pqwQ/4Ski7mDMCNATueNAumq/59DJLV/D5lkuUzSF+h2+dK8qOGWzH07UUbHkv9ZlPW76pxvgES6OZx+4eNFPwrFE=</P><Q>vybF3/ldYXYVwbd9Scthsh1aJdpHZyqsa0LGWvtM9cL7ZK7JGYoJ7iWeeLgw/ThWLxBHXd4sRzKZZm6dE9DZeJUh4wp/2Y3bCL2x2JoIgSznKV5UncbCRtXzR1rqfY5CsrydJreuz46fpjxJubEx3D3ZgQGYnBpYvmhOewzn5fs=</Q><DP>r2vyTiHi+dQGRz8jhpfLKdAqLZ5n/XM3kPhRNhPuKNsoaq3YD3gb7tCSB830I5zaBLUzLHcakPrZZS8qc1VNIbQQUZjhYsfF3yDZQVYBp0/Wk9kUyWFkjRFn+4Jielp7kE7TnCx9JABUvGMKyHDlxHXE1KmbOLkac0C6qNbtlbE=</DP><DQ>sDfNWXJonM2gtwoyLVKaiPo4PgchpkEX3HYdqIhdZX9QBHyBldLE3s+9bSrYtsg144NNV4LXLPe/pUe59SenJFvPdqAaRvRYhZFjH/y4dGVx4Zg9x4oRVf4tHY35+K+qW144PhY9yMiB811G1jI9df1qw1w2VUqQn1BHcXbvXfs=</DQ><InverseQ>vCyZn/yXxUrfuZZZegCoVGles8zNE5kRiW4A5SY3JoiHnOt23YiJqapZxint/cv305qLTU1G/H6g0a5hDvrblTLvocuCj1+KnqlIcJasY0P6XC5hcBAmc00ETBgkiqu9PGE6HJpFgbh1KK3TpIkDMPiCUecyYabr3YGZajQt9Y0=</InverseQ><D>M2AJxTzHhzFuEBvOp+aDRhUyWouwGbxzvsqKUl0AXBeHUwbDcr+YH9plF3F+JrrWstq8/zzdQT08efg47CS3/pPgWB+6eGoTaxsYTn6RkQ+q2EOYlBnWzIWQMF/YVYXn4iB6fJ0VrfIx1zMBCNrekb0BpY7bQpXSo34zEL8nLroZV8CXzcIQBSp17j53K/hcab8c0sFZNZ65kFWEhdm1KLN6Qg+VftLMBik/iSmh3gMSkpjwsAUxAmuLswzAcbjd7uYS1Hsm2puOn4k1hut6tGQpzekWGdN4EEDjgacRyzYQ777AURNsVHXtcjiBtUBPtWhJKCUp/+MeZ+O6nQOiAQ==</D></RSAKeyValue>";

            RSACrypto.FromXmlString(strDecodingKey);
            bDecryptedData = RSACrypto.Decrypt(bEncryptedData, false);
            strDecryptedString = System.Text.Encoding.Unicode.GetString(bDecryptedData, 0, bDecryptedData.Length);

            return strDecryptedString;
        }
    }

    public class Encryption : Cryptography
    { }
}