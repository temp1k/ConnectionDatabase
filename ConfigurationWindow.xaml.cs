using System;
using System.Windows;
using Microsoft.Win32;          //Пространсво имён для работы с ресурсами ОС, в частности с реестром ОС
using System.Data;              //Пространство имён для работы с кэш таблицами, строками, столбцами и данными
using System.Data.Sql;          //Пространство имён для работы настройками сервера MS SQL
using System.Data.SqlClient;    //Пространство имён для работы с технологией доступа к данным ADO.Net и её классами

namespace DataSet_WPF_DB_App
{
    /// <summary>
    /// Логика взаимодействия для ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public ConfigurationWindow()
        {
            InitializeComponent();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Метод сохранения значения о стоках подключения в реестр ОС
        /// </summary>
        /// <param name="ds">Формальный параметр ввода названия сервера</param>
        /// <param name="ic">Формальный параметр ввода названия базы данных</param>
        private void regSet(string ds, string ic)
        {
            //Объявление экземпляра калсса с доступом к корневой папке реестра CurrentUser
            RegistryKey registry = Registry.CurrentUser;
            //Создание или открытие в папке CurrentUser папки с названием DBSetAPPConfigPSC
            RegistryKey key = registry.CreateSubKey("DBSetAPPConfigPSC");
            try
            {
                //Запись названия сервера в переменную в папке в реестре
                key.SetValue("DS", ds);
                //Запись названия базы данных в переменную в папке в реестре
                key.SetValue("IC", ic);
                key.SetValue("RED", "White");
                key.SetValue("GREEN", "White");
                key.SetValue("BLUE", "White");
            }
            catch (Exception ex)
            {
                //Вывод сообщения об исключительной ситуации
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //Запись в строкову статическую переменную значения из реестра ОС, о названии имени сервера 
                DataSetClass.DS = key.GetValue("DS").ToString();
                //Запись в строкову статическую переменную значения из реестра ОС, о названии имени базы данных
                DataSetClass.IC = key.GetValue("IC").ToString();
            }
        }

        /// <summary>
        /// Метод обработки события после прогрузки всех компонентов на форме
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Создание экземпляра класса для получения информации о доступных в локальной сети SQL серверах
            SqlDataSourceEnumerator sourceEnumerator = SqlDataSourceEnumerator.Instance;
            //Объявление экземпляра класса для дальнейшей выгрузки полученных серверов
            DataTable resTable = new DataTable();
            //Заполнение результирующей кеш таблицы, табличным представлением коллекции доступных серверов
            resTable = sourceEnumerator.GetDataSources();
            //Организация цикла для обращения к колекции строк кеш таблицы с помощью представления строк
            foreach (DataRow dataRow in resTable.Rows)
            {
                //Добавление в выпадающий список названия ПК и название SQL сервера
                cbServerList.Items.Add(string.Format("{0}\\{1}", dataRow[0], dataRow[1]));
            }
            //Выпадающий список с серверами доступен
            cbServerList.IsEnabled = true;
            //Кнопка получения списка баз данных доступна
            btGetDataBase.IsEnabled = true;
        }

        private void btGetDataBase_Click(object sender, RoutedEventArgs e)
        {
            //Объявление экземпляра класса, для подключения к серверу и базе данных по умолчанию master
            SqlConnection connection = new SqlConnection(string.Format("Data Source = {0}; Initial Catalog = master; Integrated Security = True;", cbServerList.Text));
            //Объявление экземпляра класса, с запросом на выборку данных о названиях доступных баз данных
            SqlCommand command = new SqlCommand("select name from sys.databases", connection);
            //Объявление экземпляра класса для дальнейшей выгрузки полученных баз данных
            DataTable resTable = new DataTable();
            try
            {
                //Открытие рабочего подключения к источнику данных
                connection.Open();
                //Загрузка в кеш таблицу данных о названии баз данных
                resTable.Load(command.ExecuteReader());
                //Организация цикла для обращения к колекции строк кеш таблицы с помощью представления строк
                foreach (DataRow dataRow in resTable.Rows)
                {
                    //Добавление в выпадающий список названия баз данных
                    cbDataBaseList.Items.Add(dataRow[0]);
                }
                //Выпадающий списолк баз данных доступен
                cbDataBaseList.IsEnabled = true;
                //Кнопка подключения доступна
                btConnect.IsEnabled = true;
            }
            catch (Exception ex)
            {
                //Вывод сообщения об исключительной ситуации
                MessageBox.Show(ex.Message);
            }
            finally
            {
                //Закрытие подключения к источнику данных
                connection.Close();
            }
        }

        private void btConnect_Click(object sender, RoutedEventArgs e)
        {
            //Вызов методла пол сохранению данных в реестре ОС с переджачей из выпадающих списков названий серверов и баз данных
            regSet(cbServerList.Text, cbDataBaseList.Text);
            //Вызов метода закрытие окна
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Объявление экземпляра класса для работы с запросами к конкретной БД
            DataSetClass dataSetClass = new DataSetClass();
            //Организация переключателя в результате проверки подключения к БД
            switch (dataSetClass.connection_Checking())
            {
                //Если подключение всё ещё закрыто
                case false:
                    //Запросить пользователя о продолжении настройки строки подключения
                    switch (MessageBox.Show("Подключение не было настроено! Завершить работу приложения?", "Настройка подключения", MessageBoxButton.YesNo, MessageBoxImage.Warning))
                    {
                        //Согласие на отмену настройки
                        case MessageBoxResult.Yes:
                            //Не останавливать закрытие формы
                            e.Cancel = false;
                            //Завершить работу всего программного продукта 
                            App.Current.Shutdown();
                            break;
                        case MessageBoxResult.No:
                            //Остановить закрытие формы
                            e.Cancel = true;
                            break;
                    }
                    break;
            }
        }
    }
}
