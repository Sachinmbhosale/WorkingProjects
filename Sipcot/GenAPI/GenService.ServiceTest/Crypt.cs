using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;


//
// A Class to encrypt/decrypt strings using the DES algorythm.
// Note: I don't like "Base 64" string format, so I often use hex strings
// instead
//
public class Crypt
{
    bool USE_BASE64 = true;  //Throws error if you set false. Decryption will not be supported by c# for Hex

	// The password is made up of a pair of arrays, each 8 bytes long
	private byte[] TheKey = {
		0x1f,
		0x27,
		0xb3,
		0x24,
		0x50,
		0x6,
		0x7a,
		0x88
	};
	private byte[] Vector = {
		0xf1,
		0x5e,
		0x33,
		0x30,
		0x2f,
		0x9a,
		0x99,
		0x81

	};
	//
	// A simple DES string Encrytion routine
	//
	public string Encrypt(string message)
	{
		DESCryptoServiceProvider des = new DESCryptoServiceProvider();
		MemoryStream ms = new MemoryStream();
		byte[] in_buf;
		byte[] out_buf;

		// a quick sanity check
		if (message == "") {
			return "";
		}

		// put the cleartext into the input buffer
		in_buf = Encoding.ASCII.GetBytes(message);

		try {
			// create an DES Encryptor output stream
			CryptoStream crStream = new CryptoStream(ms, des.CreateEncryptor(TheKey, Vector), CryptoStreamMode.Write);

			// push the cleartext into the "translator"
			crStream.Write(in_buf, 0, in_buf.Length);
			crStream.FlushFinalBlock();

			// read the ciphertext out of the translator
			out_buf = ms.ToArray();

			ms.Close();
			crStream.Close();
		} catch (System.Security.Cryptography.CryptographicException) {
			// if encryption fails, just silently return an empty string
			return "";
		}

		if(USE_BASE64 )
		return Convert.ToBase64String(out_buf);
		else
		return BitConverter.ToString(out_buf).Replace("-", "");
	}

	//
	// A simple DES decryption routine
	//
	public string Decrypt(string message)
	{
		DESCryptoServiceProvider des = new DESCryptoServiceProvider();
		MemoryStream ms = new MemoryStream();
		byte[] in_buf;
		byte[] out_buf;

		// a quick sanity check
		if (message == "") {
			return "";
		}

        //Line added by Yogeesha
        in_buf = Convert.FromBase64String(message);

		try {
			// put the ciphertext into the input buffer
			if (USE_BASE64 )
			in_buf = Convert.FromBase64String(message);
			else
			// There's no easy way to convert a hex byte string back into
			// a byte array, so we have parse each byte
			 // ERROR: Not supported in C#: ReDimStatement

			for (int i = 0; i <= in_buf.Length - 1; i++) {
				in_buf[i] = byte.Parse(message.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
			}
		} catch (System.FormatException) {
			// if the string isn't in the correct format, then just silently fail
			return "";
		}

		try {
			// Create an DES Decryptor stream
			CryptoStream crStream = new CryptoStream(ms, des.CreateDecryptor(TheKey, Vector), CryptoStreamMode.Write);

			// push the ciphertext into the "translator" 
			crStream.Write(in_buf, 0, in_buf.Length);
			crStream.FlushFinalBlock();

			// read the cleartext out of the translator
			out_buf = ms.ToArray();

			ms.Close();
			crStream.Close();
		} catch (System.Security.Cryptography.CryptographicException) {
			// if decryption fails, just silently return an empty string
			return "";
		}

		return Encoding.ASCII.GetString(out_buf);
	}
}