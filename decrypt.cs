using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        string encryptedConn = "qahF5fTqsAZzoCwWAcwdIASYZahIypR5m1NgVGbmu2SM8HRp71WZfD+FYcFiw9i1qVk/r17S8CJ+PBwwaUbz4Z7FOdLG50Nd9NO6yI+KLDWgQHXIUFiOsxlfa86hWTliox3Ham7M72fZNCqW7wZoMVnSXCl2YJHdvkR9Rd873wiqxWFY+t4Fy658TWh040uqAqZ654EMFJCMwzh5cgJ/WQ==";
        
        string aesKey = "1234567890123456";
        string aesIv = "1234567890123456";
        byte[] key = Encoding.UTF8.GetBytes(aesKey);
        byte[] iv = Encoding.UTF8.GetBytes(aesIv);

        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var decryptor = aes.CreateDecryptor();
        var cipherBytes = Convert.FromBase64String(encryptedConn);
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        
        Console.WriteLine(Encoding.UTF8.GetString(plainBytes));
    }
}
