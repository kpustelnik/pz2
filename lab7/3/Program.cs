using System.Security.Cryptography;
using System.IO;
using System.Text;

enum keyType {
    Both = 0,
    Private = 1,
    Public = 2
}
class SignatureHandler {
    public string publicKeyFile = "publicKey.crt";
    public string privateKeyFile = "privateKey.pem";
    public string hashType = "SHA512";
    private string? publicKey = null;
    private string? privateKey = null;

    public static byte[] byteToHex(byte[] src) {
      StringBuilder hexStrBuilder = new StringBuilder();
      foreach (byte b in src) hexStrBuilder.Append(b.ToString("x2"));
      return Encoding.UTF8.GetBytes(hexStrBuilder.ToString());
    }

    public byte[]? hash(byte[] src) {
      Func<byte[], byte[]?>? hashingFunc = getHashingFunc(hashType);
      if (hashingFunc == null) return null;
      return hashingFunc(src);
    }
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
    
    public bool LoadKeyFiles(keyType type = keyType.Both) {
        if (type == keyType.Both || type == keyType.Public) {
            if (!File.Exists(publicKeyFile)) {
                Console.WriteLine("Public key file does not exist.");
                return false;
            }
            try {
                publicKey = File.ReadAllText(publicKeyFile);
            } catch {
                Console.WriteLine("Failed to load the public key.");
                return false;
            }
        }
        if (type == keyType.Both || type == keyType.Private) {
            if (!File.Exists(privateKeyFile)) {
                Console.WriteLine("Private key file does not exist.");
                return false;
            }
            try {
                privateKey = File.ReadAllText(privateKeyFile);
            } catch {
                Console.WriteLine("Failed to load the private key.");
                return false;
            }
        }
        return true;
    }
    public bool CreateKeyFiles() {
        bool success = true;
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider()) {
            try {
                publicKey = rsa.ToXmlString(false);
            } catch {
                success = false;
                Console.WriteLine("Failed to create the public key");
            }
            try {
                privateKey = rsa.ToXmlString(true);
            } catch {
                success = false;
                Console.WriteLine("Failed to create the private key");
            }
        }
        try {
            File.WriteAllText(publicKeyFile, publicKey);
        } catch {
            Console.WriteLine("Failed to write the public key");
        }
        try {
            File.WriteAllText(privateKeyFile, privateKey);
        } catch {
            Console.WriteLine("Failed to write the private key");
        }
        return success;
    }

    public bool? Verify(byte[] hash, byte[] signature) {
        if (publicKey == null) {
            Console.WriteLine("No public key specified.");
            return null;
        }   
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider()) {
            try {
                rsa.FromXmlString(publicKey);
            } catch {
                Console.WriteLine("Failed to set the public key");
                return null;
            }
            try {
                return rsa.VerifyHash(hash, hashType, signature);
            } catch {
                Console.WriteLine("Failed to verify the content signature");
                return null;
            }
        }
    }

    public byte[]? Sign(byte[] hash) {
        if (privateKey == null) {
            Console.WriteLine("No private key specified.");
            return null;
        }
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider()) {
            try {   
                rsa.FromXmlString(privateKey);
            } catch {
                Console.WriteLine("Failed to set the private key");
                return null;
            }
            try {
                return rsa.SignHash(hash, hashType);
            } catch {
                Console.WriteLine("Failed to sign the content hash");
                return null;
            }
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

    static void Main(string[] args) {
      SignatureHandler handler = new SignatureHandler();
      if (args.Length == 0) {
        handler.CreateKeyFiles();
        return;
      }

      if (args.Length < 2) {
        Console.WriteLine("Specify the signature file.");
        return;
      }

      byte[]? fileContent = getFileBytes(args[0]);
      if (fileContent == null) return;

      byte[]? fileHash = handler.hash(fileContent);
      if (fileHash == null) return;

      // byte[]? hexFilehash = byteToHex(fileHash);

      if (File.Exists(args[1])) {
        // Verify the signature
        byte[]? signature = getFileBytes(args[1]);
        if (signature == null) return;
        
        if (!handler.LoadKeyFiles(keyType.Public)) {
            Console.WriteLine("Failed to load the required key files.");
            return;
        }

        bool? valid = handler.Verify(fileHash, signature);
        if (valid != null) {
          bool cValid = valid ?? false;
          Console.WriteLine($"The signature is {(cValid ? "" : "NOT ")}valid.");
        }
      } else {
        if (!handler.LoadKeyFiles(keyType.Private)) {
            Console.WriteLine("Failed to load the required key files.");
            return;
        }

        byte[]? signature = handler.Sign(fileHash);
        if (signature == null) return;
        if (!setFileBytes(args[1], signature)) return;

        Console.WriteLine("File content has been signed.");
      }
    }
}  