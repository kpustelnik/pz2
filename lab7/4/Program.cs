using System.Text;
using System.Security.Cryptography;

public class SymmetricCipher {
  public int iterationCount = 2000;
  public string password;
  byte[] salt = {0xf1, 0x0a, 0x5e, 0x19, 0x31, 0x51, 0x16, 0x11};
  byte[] initVector = {0x15, 0xfa, 0xef, 0xbb, 0x15, 0x19, 0x59, 0x15, 0xfe, 0xab, 0x18, 0x11, 0x00, 0xab, 0xaa, 0x19};
  public System.Security.Cryptography.HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA256;
  public SymmetricCipher(String password_) {
    password = password_;
  }
  public byte[]? Decrypt(byte[] sourceData) {
    Rfc2898DeriveBytes k1 = new Rfc2898DeriveBytes(password, salt, iterationCount, hashAlgorithm);

    Aes decAlg = Aes.Create();
    decAlg.Key = k1.GetBytes(16);
    decAlg.IV = initVector;
    k1.Reset();

    try {
      MemoryStream memStream = new MemoryStream();
      CryptoStream cryptStream = new CryptoStream(memStream, decAlg.CreateDecryptor(), CryptoStreamMode.Write);
      cryptStream.Write(sourceData, 0, sourceData.Length);
      cryptStream.FlushFinalBlock();
      cryptStream.Close();
      return memStream.ToArray();
    } catch {
      Console.WriteLine("Failed to decrypt.");
      return null;
    }
  }

  public byte[]? Encrypt(byte[] sourceData) {
    Rfc2898DeriveBytes k1 = new Rfc2898DeriveBytes(password, salt, iterationCount, hashAlgorithm);
    
    Aes encAlg = Aes.Create();
    encAlg.IV = initVector;
    encAlg.Key = k1.GetBytes(16);
    k1.Reset();
    
    try {
      MemoryStream memStream = new MemoryStream();
      CryptoStream cryptStream = new CryptoStream(memStream, encAlg.CreateEncryptor(), CryptoStreamMode.Write);
      cryptStream.Write(sourceData, 0, sourceData.Length);
      cryptStream.FlushFinalBlock();
      cryptStream.Close();
      return memStream.ToArray();
    } catch {
      Console.WriteLine("Failed to encrypt.");
      return null;
    }
  }

  public static byte[]? getFileBytes(string fileName) {
      if (!File.Exists(fileName)) {
          Console.WriteLine($"File {fileName} does not exist.");
          return null;
      }
      try {
          return File.ReadAllBytes(fileName);
      } catch {
          Console.WriteLine($"Failed to read the file {fileName}.");
          return null;
      }
  }
  public static bool setFileBytes(string fileName, byte[] content) {
    try {
        File.WriteAllBytes(fileName, content);
        return true;
    } catch {
        Console.WriteLine($"Failed to write the file {fileName} content.");
        return false;
    }
  }

  public static void Main(string[] Args) {
    if (Args.Length < 1) {
      Console.WriteLine("No command.");
      return;
    }
    int cmd = -1;
    if (!Int32.TryParse(Args[0], out cmd)) {
      Console.WriteLine("Failed to parse the command.");
      return;
    }
    if (Args.Length < 2) {
      Console.WriteLine("Specify the source file.");
      return;
    }
    if (Args.Length < 3) {
      Console.WriteLine("Specify the target file.");
      return;
    }
    if (Args.Length < 4) {
      Console.WriteLine("Please specify the shared password.");
      return;
    }

    SymmetricCipher handler = new SymmetricCipher(Args[3]);
    if (cmd == 0) {
      // Encrypt
      byte[]? sourceText = getFileBytes(Args[1]);
      if (sourceText == null) return;

      byte[]? encryptedText = handler.Encrypt(sourceText);
      if (encryptedText == null) return;
      if (!setFileBytes(Args[2], encryptedText)) return;
      Console.WriteLine($"Successfully encrypted the file {Args[1]} to {Args[2]} with shared password.");
      return;
    } else if (cmd == 1) {
      // Decrypt
      byte[]? sourceText = getFileBytes(Args[1]);
      if (sourceText == null) return;

      byte[]? decryptedText = handler.Decrypt(sourceText);
      if (decryptedText == null) return;
      if (!setFileBytes(Args[2], decryptedText)) return;

      Console.WriteLine($"Successfully decrypted the file {Args[1]} to {Args[2]} with shared password.");
      return;
    } else {
      Console.WriteLine("Invalid command.");
      return;
    }
  }
}