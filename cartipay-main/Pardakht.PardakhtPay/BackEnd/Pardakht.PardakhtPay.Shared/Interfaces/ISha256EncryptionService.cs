using System.Text;

namespace Pardakht.PardakhtPay.Shared.Interfaces
{
    /// <summary>
    /// Represents and interface to manage SHA256 encryption operations
    /// </summary>
    public interface ISha256EncryptionService
    {
        byte[] Encrypt(string input, Encoding encoding = null);

        byte[] Encrypt(byte[] input);

        string EncryptToString(string input, Encoding encoding = null);

        string EncryptToString(byte[] input);

        string EncryptToBase64(string input, Encoding encoding = null);

        string EncryptToBase64(byte[] input);
    }
}
