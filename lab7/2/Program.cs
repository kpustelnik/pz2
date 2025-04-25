using System.Security.Cryptography;
using System.Text;

class FileHash {
  public static Func<byte[], byte[]?>? getHashingFunc(string name) {
    if (name == "SHA256") {
      return (src) => {
        try {
          using SHA256 hash = SHA256.Create();
          return hash.ComputeHash(src);
        } catch {
          Console.WriteLine("Failed to calculate the SHA256 hash.");
          return null;
        }
      };
    } else if (name == "SHA512") {
      return (src) => {
        try {
          using SHA512 hash = SHA512.Create();
          return hash.ComputeHash(src);
        } catch {
          Console.WriteLine("Failed to calculate the SHA512 hash.");
          return null;
        }
      };
    } else if (name == "MD5") {
      return (src) => {
        try {
          using MD5 hash = MD5.Create();
          return hash.ComputeHash(src);
        } catch {
          Console.WriteLine("Failed to calculate the MD5 hash.");
          return null;
        }
      };
    }
    return null;
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

  public static void Main(String[] Args) {
    if (Args.Length < 1) {
      Console.WriteLine("No source file name specified.");
      return;
    }
    if (Args.Length < 2) {
      Console.WriteLine("No hash function specified.");
      return;
    }
    if (Args.Length < 3) {
      Console.WriteLine("No target file name specified.");
      return;
    }

    Func<byte[], byte[]?>? hashingFunc = getHashingFunc(Args[1]);
    if (hashingFunc == null) {
      Console.WriteLine("Hashing function not implemented.");
      return;
    }

    byte[]? srcFileBytes = getFileBytes(Args[0]);
    if (srcFileBytes == null) return;

    byte[]? hash = hashingFunc(srcFileBytes);
    if (hash == null) return;

    StringBuilder clrHash = new StringBuilder();
    foreach (byte b in hash) {
      clrHash.Append(b.ToString("x2"));
    }
    string clearHash = clrHash.ToString();

    Encoding enc = Encoding.UTF8;
    if (File.Exists(Args[2])) {
      byte[]? cHash = getFileBytes(Args[2]);
      if (cHash == null) return;

      // bool same = hash.SequenceEqual(cHash);
      string storedHash = enc.GetString(cHash);
      bool same = storedHash.Equals(clearHash);
      Console.WriteLine($"The hash is {(same ? "" : "NOT ")}correct.");
    } else {
      // setFileBytes(Args[2], hash);
      setFileBytes(Args[2], enc.GetBytes(clearHash));
      Console.WriteLine("Hash has been saved in the file.");
    }
  }
}