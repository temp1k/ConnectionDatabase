using System;
using System.Windows;
using Microsoft.Win32;//Пространсво имён для работы с ресурсами ОС, в частности с реестром ОС
using System.Threading;

namespace DataSet_WPF_DB_App
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    { 
        public MainWindow()
        {
            InitializeComponent();
        }

        public void StartApp(object state)
        {
            
        }

        /// <summary>
        /// Метод по считыванию информации из реестра ОС о строке подключения к базе данных
        /// </summary>
        private bool regGet()
        {
            //Объявление экземпляра класса с доступом к корневой папке реестра CurrentUser
            RegistryKey registry = Registry.CurrentUser;
            //Создание или открытие в папке CurrentUser папки с названием DBSetAPPConfigPSC
            RegistryKey key = registry.CreateSubKey("DBSetAPPConfigPSC");
            try
            {
                //Запись в строкову статическую переменную значения из реестра ОС, о названии имени сервера 
                DataSetClass.DS = key.GetValue("DS").ToString();
                //Запись в строкову статическую переменную значения из реестра ОС, о названии имени базы данных
                DataSetClass.IC = key.GetValue("IC").ToString();
                return true;
            }
            catch (Exception ex)
            {
                //Вывод сообщения об исключительной ситуации
                MessageBox.Show(ex.Message);
                //Запись в строковую статическую переменную значения пустоты, в случае ошибки обращения к реестру ОС
                DataSetClass.DS = "Null";
                //Запись в строковую статическую переменную значения пустоты, в случае ошибки обращения к реестру ОС
                DataSetClass.IC = "Null";
                return false;
            }
        }

        /// <summary>
        /// Событие, которое срабатывает при загрузки окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AuthorizationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Вызов метода получения из реестра ОС информации о строке подключения присваивание переменной connect информации о подключении
            bool connect = regGet();
            //Объявление экземпляра класса для работы с SQL запросами
            DataSetClass dataSetClass = new DataSetClass();
            if (connect == true)
            {
                //Получение информации о подключении к серверу
                connect = dataSetClass.connection_Checking();
            }           
            //Организация переключателя для проверки правильного подключения к БД
            switch (connect)
            {
                //Если подключение открыто успешно
                case true:
                    if (DataSetClass.CheckLastUser() == true)
                    {
                        tbLogin.Text = App.Login;
                    }
                    //LoadEllipse.Visibility = Visibility.Hidden;
                    tbLogin.IsEnabled = true;
                    pbPassword.IsEnabled = true;
                    btEnter.IsEnabled = true;
                    btCancel.IsEnabled = true;
                    break;
                //Если подключение не открыто или с ошибками строка подключения
                case false:
                    //Объявление экземпляра класса окна конфигурирования строки подключения к источнику данных
                    ConfigurationWindow configurationWindow = new ConfigurationWindow();
                    //Вызов экземпляра класса окна в режиме диалогового окна 
                    configurationWindow.ShowDialog();
                    //Кнопка входа доступна
                    btEnter.IsEnabled = true;
                    //Поле ввода логина доступно
                    tbLogin.IsEnabled = true;
                    //Поле ввода пароля доступно
                    pbPassword.IsEnabled = true;
                    break;
            }
        }

        /// <summary>
        /// Событие выхода из приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Событие проверки входа, которое срабатывает при нажани на кнопку "Вход"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btEnter_Click(object sender, RoutedEventArgs e)
        {         
            DataSetClass dataSetClass = new DataSetClass();

            dataSetClass.DataSetFill(string.Format("select [ID_Сustomer], [Name_ Organization_Type] + ' ' + [Name_Сustomer], [Login_Customer], [Password_Customer], [Email_Customer] from [dbo].[Customer] inner join[dbo].[Organization_Type] on [ID_Organization_Type] = [Organization_Type_ID] where [Login_Customer] = '{0}' and [Password_Customer] = '{1}'", tbLogin.Text, pbPassword.Password),
                "Customer", DataSetClass.Function.select, null);

            if (DataSetClass.dataSet.Tables["Customer"].Rows.Count != 0)
            {
                App.Login = tbLogin.Text;
                App.Password = pbPassword.Password;
                Visibility = Visibility.Hidden;
                tbLogin.Text = String.Empty;
                pbPassword.Password = String.Empty;

                App.ID = DataSetClass.dataSet.Tables["Customer"].Rows[0][0].ToString();
                App.User_Name = DataSetClass.dataSet.Tables["Customer"].Rows[0][1].ToString();
                App.Email = DataSetClass.dataSet.Tables["Customer"].Rows[0][4].ToString();
                App.User_Role = "Заказчик";
                TwoFactorAutorization twoFactorAutorization = new TwoFactorAutorization();
                twoFactorAutorization.Show();
                //CustomerWindow customerWindow = new CustomerWindow();
                //customerWindow.Show();
            }
            else
            {
                dataSetClass.DataSetFill(string.Format("select [ID_Employee], [First_Name_Employee], [Login_Employee],[Password_Employee], [Email_Employee], [ID_Fast], [Name_Fast] from [dbo].[Combination] inner join[dbo].[Employee] on[Employee_ID] = [ID_Employee] inner join[dbo].[Fast] on[Fast_ID] = [ID_Fast] where [Login_Employee] = '{0}' and [Password_Employee] = '{1}'", tbLogin.Text, pbPassword.Password),
                    "Employee", DataSetClass.Function.select, null);

                if (DataSetClass.dataSet.Tables["Employee"].Rows.Count != 0)
                {
                    Visibility = Visibility.Hidden;
                    App.ID = DataSetClass.dataSet.Tables["Employee"].Rows[0][0].ToString();
                    App.User_Name = DataSetClass.dataSet.Tables["Employee"].Rows[0][1].ToString();
                    App.Email = DataSetClass.dataSet.Tables["Employee"].Rows[0][4].ToString();
                    App.User_Role = DataSetClass.dataSet.Tables["Employee"].Rows[0][6].ToString();

                    //TwoFactorAutorization twoFactorAutorization = new TwoFactorAutorization();
                    //twoFactorAutorization.Show();
                    switch (DataSetClass.dataSet.Tables["Employee"].Rows[0][6].ToString())
                    {
                        case "Администратор":
                            App.Login = tbLogin.Text;
                            App.Password = pbPassword.Password;
                            tbLogin.Text = String.Empty;
                            pbPassword.Password = String.Empty;
                            AdminWindow adminWindow = new AdminWindow();
                            adminWindow.Show();
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Такого логина или пароля не сущетсвует.", "Ошибка авторизации");
                }
            }         
        }
    }
}
