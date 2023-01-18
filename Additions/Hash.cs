using System.Security.Cryptography;
using System.Text;

namespace Alan.Helpers
{
    /// <summary>
    /// помощник по работе с хешами
    /// </summary>
    public static class Hash
    {
        /// <summary>
        /// получить hash из строки
        /// </summary>
        /// <param name="value">строка</param>
        /// <returns></returns>
        public static string GetHash(string value)
        {
            // создание байтового массива
            byte[] tmpSource = Encoding.ASCII.GetBytes(value);

            // вычисление хеш
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);

            return ByteArrayToString(hash);
        }

        private static string ByteArrayToString(byte[] arrInput)
        {
            int i;
            StringBuilder sOutput = new(arrInput.Length);
            for (i = 0; i < arrInput.Length - 1; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
    }
}