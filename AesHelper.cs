namespace Snippets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;
    using System.Threading.Tasks;
    using System.Security.Cryptography;
    using System.IO;

    public static class Extensions
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }

    public class AesHelper : IDisposable
    {
        private AesManaged aes;
        private ICryptoTransform encryptor;
        private ICryptoTransform decryptor;
        private Exception lastException;

        public Exception LastException
        {
            get
            {
                return lastException;
            }
        }

        public AesHelper(string pwd, string salt)
        {
            this.aes = new AesManaged();

            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);
            var rfc = new Rfc2898DeriveBytes(pwd, saltBytes, 10000);
            var rfcBytes = rfc.GetBytes(48);

            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.Key = rfcBytes.SubArray(0, 32);
            aes.IV = rfcBytes.SubArray(32, 16);
            aes.Padding = PaddingMode.PKCS7;
        }

        private ICryptoTransform GetEncryptor()
        {
            if (encryptor == null) encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            return encryptor;
        }

        private ICryptoTransform GetDecryptor()
        {
            if (decryptor == null) decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            return decryptor;
        }

        public byte[] Encrypt(byte[] data)
        {
            byte[] result = null;

            if (data == null) return null;
            if (data.Length == 0) return data;

            try
            {
                ICryptoTransform encryptor = GetEncryptor();

                // Create the streams used for encryption.
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();

                        result = ms.ToArray();
                    }
                }
            }

            catch (Exception ex)
            {
                lastException = ex;
                result = null;
            }

            return result;
        }

        public string Encrypt(string plainText)
        {
            string result;

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(plainText);

                data = Encrypt(data);
                result = Convert.ToBase64String(data);
            }
            catch (Exception ex)
            {
                lastException = ex;
                result = null;
            }

            return result;
        }

        public byte[] Decrypt(byte[] data)
        {
            byte[] result;

            try
            {
                ICryptoTransform decryptor = GetDecryptor();

                // Create the streams used for encryption.
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (BinaryReader br = new BinaryReader(cs))
                        {
                            result = br.ReadBytes(data.Length);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                lastException = ex;
                result = null;
            }

            return result;
        }

        public string Decrypt(string base64Text)
        {
            string result;

            try
            {
                byte[] data = Convert.FromBase64String(base64Text);

                data = Decrypt(data);
                result = Encoding.UTF8.GetString(data);
            }

            catch (Exception ex)
            {
                lastException = ex;
                result = null;
            }

            return result;
        }        

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                aes.Dispose();
                aes = null;
            }
        }
    }
}
