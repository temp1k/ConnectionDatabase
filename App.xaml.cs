using System.Windows;

namespace DataSet_WPF_DB_App
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Глобальная  статическая строковая переменная ID для хранения значения первичного ключа пользователя,
        ///  с целью проведения процедуры разграничения прав доступа.
        ///  Переменная Employee_Name для вывода ФИО сотрудника
        ///  Переменная Email для вывода Email сотрудника
        ///  Переменная User_Role для вывода должности сотрудника
        ///  Переменная Login для хранения значения логина сотрудника,
        ///  с целью сохранения значения в реестр.
        ///  Переменная Password для хранения значения пароля сотрудника,
        ///  с целью сохранения значения в реестр.
        /// </summary>
        public static string ID = "-1", User_Name = "null", Email = "null", User_Role = "null", Login="null", Password = "null";

        /// <summary>
        /// Переменные для сохранения позиционирования окон
        /// </summary>
        public static int IndexTCEmployee = -1, IndexTCContract = -1, IndexDGEmployee = -1, IndexDGFast = -1, IndexDGContract = -1, IndexDGCustomer = -1, IndexDGSecurityFirm = -1;

        public static bool Monitoring = false, MonitoringCustomer = false;

        public static int IndexDGContractInfo = -1, IndexDGObj = -1;

        /// <summary>
        /// Переменные для палитры цвета
        /// </summary>
        public static byte Red = 255, Green = 255, Blue = 255;

        /// <summary>
        /// Метод шифрования данного слова
        /// </summary>
        /// <param name="word">Строка для шифроввания</param>
        /// <param name="key">Ключ шифрвоания</param>
        /// <returns></returns>
        public static string Encryption(string word, int key)
        {
            char[] chars = new char[word.Length];
            for (int i = 0; i < word.Length; i++)
            {
                chars[i] = (char)(word[i] + key);
            }
            string newWord = "";
            foreach (char el in chars)
            {
                newWord += el;
            }

            return newWord;
        }

        /// <summary>
        /// Метод расшифровки данной строки
        /// </summary>
        /// <param name="word">Строка для расшифровки</param>
        /// <param name="key">Ключ расшифровки</param>
        /// <returns></returns>
        public static string Descryptioin(string word, int key)
        {
            char[] chars = new char[word.Length];
            for (int i = 0; i < word.Length; i++)
            {
                chars[i] = (char)(word[i] - key);
            }
            string newWord = "";
            foreach (char el in chars)
            {
                newWord += el;
            }

            return newWord;
        }
    }
}
