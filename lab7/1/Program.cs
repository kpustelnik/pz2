using System.Security.Cryptography;
using System.IO;
using System.Text;

enum keyType {
    Both = 0,
    Private = 1,
    Public = 2
}
class AssymetricCipher {
    public string publicKeyFile = "publicKey.crt";
    public string privateKeyFile = "privateKey.pem";
    private string? publicKey = null;
    private string? privateKey = null;
    
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

    public byte[]? Encrypt(byte[] src) {
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
                return rsa.Encrypt(src, false);
            } catch {
                Console.WriteLine("Failed to encrypt the source content");
                return null;
            }
        }
    }

    public byte[]? Decrypt(byte[] src) {
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
                return rsa.Decrypt(src, false);
            } catch {
                Console.WriteLine("Failed to decrypt the source content");
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
        if (args.Length < 1) {
            Console.WriteLine("No command.");
            return;
        }
        int cmd = -1;
        if (!Int32.TryParse(args[0], out cmd)) {
            Console.WriteLine("Failed to parse the command.");
            return;
        }
        AssymetricCipher handler = new AssymetricCipher();
        if (cmd == 0) {
            // Stworzenie instancji klasy implementującej algorytm RSA z losową
            // inicjacją klucza prywatnego i publicznego
            handler.CreateKeyFiles();
        } else if (cmd == 1) {
            // Szyfrowanie
            if (args.Length < 2) {
                Console.WriteLine("Source file was not specified.");
                return;
            }
            if (args.Length < 3) {
                Console.WriteLine("Target file was not specified.");
                return;
            }
            if (!handler.LoadKeyFiles(keyType.Public)) {
                Console.WriteLine("Failed to load the required key files.");
                return;
            }

            byte[]? sourceBytes = getFileBytes(args[1]);
            if (sourceBytes == null) return;

            byte[]? encryptedBytes = handler.Encrypt(sourceBytes);
            if (encryptedBytes == null) return;

            if (setFileBytes(args[2], encryptedBytes)) {
                Console.WriteLine($"Successfully encrypted the content of {args[1]} to {args[2]}");
            }
        } else if (cmd == 2) {
            // Odszyfrowywanie
            if (args.Length < 2) {
                Console.WriteLine("Source file was not specified.");
                return;
            }
            if (args.Length < 3) {
                Console.WriteLine("Target file was not specified.");
                return;
            }
            if (!handler.LoadKeyFiles(keyType.Private)) {
                Console.WriteLine("Failed to load the required key files.");
                return;
            }

            byte[]? sourceBytes = getFileBytes(args[1]);
            if (sourceBytes == null) return;

            byte[]? decryptedBytes = handler.Decrypt(sourceBytes);
            if (decryptedBytes == null) return;

            if (setFileBytes(args[2], decryptedBytes)) {
                Console.WriteLine($"Successfully decrypted the content of {args[1]} to {args[2]}");
            }
        } else {
            Console.WriteLine("Incorrect command.");
            return;
        }
    }
}  