namespace Pardakht.PardakhtPay.Shared.Interfaces
{
    public interface IAesEncryptionService
    {
        byte[] Encrypt(string plainText);
        byte[] Encrypt(byte[] data);

        string Encrypt<T>(T item);

        string EncryptToBase64(string plainText);
        string EncryptToBase64(byte[] data);

        byte[] Decrypt(string encryptedText);
        byte[] Decrypt(byte[] data);
        string DecryptToString(string encryptedText);
        string DecryptToString(byte[] data);

        T Decrypt<T>(string text) where T : new();
    }
}
