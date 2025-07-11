using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using MyPersonalDiary.Core.Models;

namespace MyPersonalDiary.Application.Services;

public class AesEncryptionService
{
    private readonly byte[] _key;
    
    public AesEncryptionService(IOptions<EncryptionSettings> options)
    {
        string key = GenerateKey();
        Console.WriteLine($"Generated key: {key}");
        EncryptionSettings encryptionSettings = options.Value;
        if (string.IsNullOrEmpty(encryptionSettings.Key))
        {
            throw new ArgumentException("AES encryption key must be provided");
        }
        
        _key = Convert.FromBase64String(encryptionSettings.Key);
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        using var aes = Aes.Create();
        aes.Key = _key;
        
        byte[] iv = aes.IV;
        
        using var encryptor = aes.CreateEncryptor(aes.Key, iv);
        using var ms = new MemoryStream();
        
        ms.Write(iv, 0, iv.Length);
        
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        byte[] buffer = Convert.FromBase64String(cipherText);
        
        using var aes = Aes.Create();
        aes.Key = _key;
        
        int ivSize = aes.BlockSize / 8;
        if (buffer.Length < ivSize)
            throw new ArgumentException("Invalid ciphertext");
        
        byte[] iv = new byte[ivSize];
        Array.Copy(buffer, 0, iv, 0, ivSize);
        
        using var ms = new MemoryStream();
        ms.Write(buffer, ivSize, buffer.Length - ivSize);
        ms.Position = 0;
        
        using var decryptor = aes.CreateDecryptor(aes.Key, iv);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cs);

        return reader.ReadToEnd();
    }

    public static string GenerateKey()
    {
        using var aes = Aes.Create();
        aes.GenerateKey();
        return Convert.ToBase64String(aes.Key);
    }
}