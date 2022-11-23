using System;
using System.Collections;
using System.Windows;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Win32;

namespace DataSet_WPF_DB_App
{
    class DataSetClass
    {
        public static string DS = "null", IC = "null";

        private SqlConnection connection = new SqlConnection(string.Format("Data Source = {0}; Initial Catalog = {1}; Integrated Security = true;", DS, IC));

        public static DataSet dataSet = new DataSet();

        private DataTable dtType = new DataTable("Type");

        private DataTable dtOrganization_Type = new DataTable("Organization_Type");

        private DataTable dtCustomer = new DataTable("Customer");

        private DataTable dtSecurity_Firm = new DataTable("Security_Firm");

        private DataTable dtContract = new DataTable("Contract");

        private DataTable dtObject_Protection = new DataTable("Object_Protection");

        private DataTable dtSecurity_Group = new DataTable("Security_Group");

        private DataTable dtFast = new DataTable("Fast");

        private DataTable dtEmployee = new DataTable("Employee");

        private DataTable dtCombination = new DataTable("Combination");

        private DataTable dtRepresentative = new DataTable("Representative");

        private DataTable dtBuilding = new DataTable("Building");

        private DataTable dtPost = new DataTable("Post");

        private DataTable dtDuty_Schedule = new DataTable("Duty_Schedule");

        /// <summary>
        /// Сохранение в реестр пароль и логин
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        public static void SetLastUser(string login, string password)
        {
            RegistryKey registry = Registry.CurrentUser;
            //Создание или открытие в папке CurrentUser папки с названием DBSetAPPConfigPSC
            RegistryKey key = registry.CreateSubKey("DBSetAPPConfigPSC");
            login = App.Encryption(login, 5);
            password = App.Encryption(password, 5);
            try
            {
                key.SetValue("LOGIN", login);
                key.SetValue("PASSWORD", password);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                App.Login = key.GetValue("LOGIN").ToString();
                App.Password = key.GetValue("PASSWORD").ToString();
            }
        }

        /// <summary>
        /// Проверка парвильности Логина и Пароля последнего пользователя
        /// </summary>
        /// <returns></returns>
        public static bool CheckLastUser()
        {
            RegistryKey registry = Registry.CurrentUser;
            //Создание или открытие в папке CurrentUser папки с названием DBSetAPPConfigPSC
            RegistryKey key = registry.CreateSubKey("DBSetAPPConfigPSC");
            App.Login =App.Descryptioin(key.GetValue("LOGIN").ToString(), 5);
            App.Password =App.Descryptioin(key.GetValue("PASSWORD").ToString(), 5);
            if (App.Login != "null" && App.Password != "null") 
            {
                DataSetClass dataSetClass = new DataSetClass();

                dataSetClass.DataSetFill($"select [ID_Сustomer], [Name_Сustomer], [Email_Customer] from [dbo].[Customer] where [Login_Customer] = '{App.Login}'  and [Password_Customer] = '{App.Password}'", "Customer", Function.select, null);

                if (dataSet.Tables["Customer"].Rows.Count != 0)
                {
                    return true;
                }
                else
                {
                    dataSetClass.DataSetFill(string.Format("select [ID_Employee], [First_Name_Employee], [Login_Employee],[Password_Employee], [Email_Employee], [ID_Fast], [Name_Fast] from [dbo].[Combination] inner join[dbo].[Employee] on[Employee_ID] = [ID_Employee] inner join[dbo].[Fast] on[Fast_ID] = [ID_Fast] where [Login_Employee] = '{0}' and [Password_Employee] = '{1}'", App.Login, App.Password),
                    "Employee", DataSetClass.Function.select, null);
                    if (DataSetClass.dataSet.Tables["Employee"].Rows.Count != 0)
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// Сохранение позиционирования последнего Администратора
        /// </summary>
        /// <param name="Monitoring"></param>
        /// <param name="IndexTCEmployee"></param>
        /// <param name="IndexTCContract"></param>
        /// <param name="IndexDGEmployee"></param>
        /// <param name="IndexDGFast"></param>
        /// <param name="IndexDGContract"></param>
        /// <param name="IndexDGCustomer"></param>
        /// <param name="IndexDGSecurityFirm"></param>
        public static void SetWindowPositionAdmin(bool Monitoring, int IndexTCEmployee, int IndexTCContract, int IndexDGEmployee, int IndexDGFast, int IndexDGContract, int IndexDGCustomer, int IndexDGSecurityFirm)
        {
            RegistryKey registry = Registry.CurrentUser;
            //Создание или открытие в папке CurrentUser папки с названием DBSetAPPConfigPSC
            RegistryKey key = registry.CreateSubKey("DBSetAPPConfigPSC");
            try
            {
                key.SetValue("Monitoring", Monitoring.ToString());
                key.SetValue("IndexTCEmployee", IndexTCEmployee.ToString());
                key.SetValue("IndexTCContract", IndexTCContract.ToString());
                key.SetValue("IndexDGEmployee", IndexDGEmployee.ToString());
                key.SetValue("IndexDGFast", IndexDGFast.ToString());
                key.SetValue("IndexDGContract", IndexDGContract.ToString());
                key.SetValue("IndexDGCustomer", IndexDGCustomer.ToString());
                key.SetValue("IndexDGSecurityFirm", IndexDGSecurityFirm.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Получение ифнормации о позиционировании из реестра
        /// </summary>
        public static void GetWindowPositionAdmin()
        {
            RegistryKey registry = Registry.CurrentUser;
            //Создание или открытие в папке CurrentUser папки с названием DBSetAPPConfigPSC
            RegistryKey key = registry.CreateSubKey("DBSetAPPConfigPSC");
            App.Monitoring = Convert.ToBoolean(key.GetValue("Monitoring"));
            App.IndexTCEmployee = Convert.ToInt32(key.GetValue("IndexTCEmployee"));
            App.IndexTCContract = Convert.ToInt32(key.GetValue("IndexTCContract"));
            App.IndexDGEmployee = Convert.ToInt32(key.GetValue("IndexDGEmployee"));
            App.IndexDGFast = Convert.ToInt32(key.GetValue("IndexDGFast"));
            App.IndexDGContract = Convert.ToInt32(key.GetValue("IndexDGContract"));
            App.IndexDGCustomer = Convert.ToInt32(key.GetValue("IndexDGCustomer"));
            App.IndexDGSecurityFirm = Convert.ToInt32(key.GetValue("IndexDGSecurityFirm"));
        }

        public static void SetWindowPositionCutomer(bool MonitoringCustomer, int IndexDGContractInfo, int IndexDGObj)
        {
            RegistryKey registry = Registry.CurrentUser;
            //Создание или открытие в папке CurrentUser папки с названием DBSetAPPConfigPSC
            RegistryKey key = registry.CreateSubKey("DBSetAPPConfigPSC");
            try
            {
                key.SetValue("MonitoringCustomer", MonitoringCustomer.ToString());
                key.SetValue("IndexDGContractInfo", IndexDGContractInfo.ToString());
                key.SetValue("IndexDGObj", IndexDGObj.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void GetWindowPositionCustomer()
        {
            RegistryKey registry = Registry.CurrentUser;
            //Создание или открытие в папке CurrentUser папки с названием DBSetAPPConfigPSC
            RegistryKey key = registry.CreateSubKey("DBSetAPPConfigPSC");

            App.MonitoringCustomer = Convert.ToBoolean(key.GetValue("MonitoringCustomer"));
            App.IndexDGContractInfo = Convert.ToInt32(key.GetValue("IndexDGContractInfo"));
            App.IndexDGObj = Convert.ToInt32(key.GetValue("IndexDGObj"));
        }

        /// <summary>
        /// Сохранение в реестр информации о цвете фона текущего окна
        /// </summary>
        /// <param name="Red"></param>
        /// <param name="Green"></param>
        /// <param name="Blue"></param>
        public static void SetColorBrush(byte Red, byte Green, byte Blue)
        {
            RegistryKey registry = Registry.CurrentUser;
            //Создание или открытие в папке CurrentUser папки с названием DBSetAPPConfigPSC
            RegistryKey key = registry.CreateSubKey("DBSetAPPConfigPSC");
            key.SetValue("RED", Red.ToString());
            key.SetValue("GREEN", Green.ToString());
            key.SetValue("BLUE", Blue.ToString());
        }

        public static void GetColorBrush()
        {
            RegistryKey registry = Registry.CurrentUser;
            //Создание или открытие в папке CurrentUser папки с названием DBSetAPPConfigPSC
            RegistryKey key = registry.CreateSubKey("DBSetAPPConfigPSC");
            App.Red = Convert.ToByte(key.GetValue("RED").ToString());
            App.Green = Convert.ToByte(key.GetValue("GREEN").ToString());
            App.Blue = Convert.ToByte(key.GetValue("BLUE").ToString());
        }

        public enum Function { select, insert, update, delete};

        /// <summary>
        /// Проверка подключения к серверу
        /// </summary>
        /// <returns></returns>
        public bool connection_Checking()
        {
            try
            {
              
                connection.Open();
           
                dataSet.Tables.Add(dtType);

                dataSet.Tables.Add(dtOrganization_Type);

                dataSet.Tables.Add(dtCustomer);
    
                dataSet.Tables.Add(dtSecurity_Firm);

                dataSet.Tables.Add(dtContract);

                dataSet.Tables.Add(dtObject_Protection);

                dataSet.Tables.Add(dtSecurity_Group);
                dataSet.Tables.Add(dtFast);

                dataSet.Tables.Add(dtEmployee);

                dataSet.Tables.Add(dtCombination);

                dataSet.Tables.Add(dtRepresentative);

                dataSet.Tables.Add(dtBuilding);

                dataSet.Tables.Add(dtPost);

                dataSet.Tables.Add(dtDuty_Schedule);
                return true;
            }
            //Объявление экземпляра класса, исключительных ситуация связанных с обработкой SQL запросов и работой с базами данных  
            catch (SqlException ex)
            {
                //Вывод сообщения об ошибке в случае ошибки в строке подключения
                MessageBox.Show("Сервер не найден", "Ошибка подключения");
                //Возвращение методу значения лжи
                return false;
            }
            finally
            {
                //Закрытие подключения, в независимости от результата
                connection.Close();
            }
        }

        /// <summary>
        /// Метод работы с любым запросом DML SQL
        /// </summary>
        /// <param name="SQLQuery">Обязательный запрос на выборку данных</param>
        /// <param name="TableName">Обязательная результирующая таблица</param>
        /// <param name="function">Вид манипуляции select, insert, update, delete</param>
        /// <param name="valueList">Коллекция передаваемых значений, если select то передать null</param>
        public void DataSetFill(string SQLQuery, string TableName, Function function, ArrayList valueList)
        {
            //Создание экземпляра класса Адаптера - включает в себя свойства и методыв по выборке, добавлению, изменению и удалению данных, в конструкторе данный запрос помещается в свойство SelectCommand
            SqlDataAdapter adapter = new SqlDataAdapter(SQLQuery, connection);
            //Создание экземпляра класса кэш таблицы для выборки объектов из базы данных
            DataTable table = new DataTable();
            //Создание экзмепляра класса обработчика SQL команд, для выборки данных об объектах базы данных
            SqlCommand command = new SqlCommand("", connection);
            try
            {
                connection.Open();
                //Отчистка, в кэше данных, у указанной таблицы, столбцов, для избежания аккамулирования столбцов
                dataSet.Tables[TableName].Columns.Clear();
                //Отчистка, в кэше данных, у указанной таблицы, строк, для избежания аккамулирования строк
                dataSet.Tables[TableName].Rows.Clear();
                //Переключатель на выполнение одного из 4 действий
                switch (function)
                {
                    case Function.select:
                        //Заполнение, в кэше данных, указанной таблицы, запросом на выборку данных
                        adapter.Fill(dataSet.Tables[TableName]);
                        break;
                    case Function.insert:
                        //Формирование запроса на выборку объектов базы данных, а именно столбцов таблиц, с фильтрацией, где id таблицы равен введённому названию в метод и где поля не имеют свойство is_identity 1, то есть не являются PK
                        command.CommandText = string.Format("select name from sys.columns where object_id = (select object_id from sys.tables where name = '{0}') and is_identity <> 1", TableName);
                        //Заполнение кэш таблицы, реузльтатом выборки обектов из БД
                        table.Load(command.ExecuteReader());
                        //Формирование строки запроса на добавление данных в указанную таблицу
                        string insertquery = string.Format("insert into [dbo].[{0}] (", TableName);
                        //Организация цикла, для заполнения названия толбцов в соотвествии с запросом на выборку названий столбцов конкретной таблицы
                        for (int i = 0; i <= table.Rows.Count - 1; i++)
                        {
                            insertquery += string.Format(" [{0}]", table.Rows[i][0]);
                            //Проверка на то, является ли перечисленное поле не последнее в цикле, если да то ставим после названия столбца запятую 
                            if (i < table.Rows.Count - 1)
                                insertquery += ",";
                        }
                        //Дополнение строки запроса на выборку данных, командой values, которая раздеяет область описания столбцов и параметров
                        insertquery += ") values (";
                        //Организация цикла, для заполнения названия параметров к соотвествующим столбцам таблицы, куда будут добавлены данные
                        for (int i = 0; i <= table.Rows.Count - 1; i++)
                        {
                            //Дополнение запроса новыми параметрами
                            insertquery += string.Format(" @{0}", table.Rows[i][0]);
                            //Проверка на то, является ли перечисленный параметр не последнее в цикле, если да то ставим после названия параметра запятую 
                            if (i < table.Rows.Count - 1)
                                insertquery += ",";
                        }
                        //Дополнение запроса на добавление данных, закрывающей скобкой
                        insertquery += ")";
                        //Присвоение полученного запроса в свойство InsertCommand, через инициализацию нового обработчика SQL корманд
                        adapter.InsertCommand = new SqlCommand(insertquery);
                        //Инициализация свойству InsertCommand, свойству Connection экземпляра класса SQLConnection
                        adapter.InsertCommand.Connection = connection;
                        //Принудительная отчистка параметров у свойства InsertCommand, для избежания аккамулирования параметров
                        adapter.InsertCommand.Parameters.Clear();
                        //Организация цикла для присвоения полученного списка значений в параметры запроса на добавление данных
                        for (int i = 0; i <= table.Rows.Count - 1; i++)
                        {
                            //Добавление, в коллекцию свойства InsertCommand, значений в параметры по его названию
                            adapter.InsertCommand.Parameters.AddWithValue(string.Format("@{0}", table.Rows[i][0]), valueList[i]);
                        }
                        //Выполнение вложенного запроса на добавление данных
                        adapter.InsertCommand.ExecuteNonQuery();
                        //Перезапись кэш таблицы, с помощью запроса на выборку данных, для визуального обновления данных
                        adapter.Fill(dataSet.Tables[TableName]);
                        break;
                    case Function.update:
                        //Формирование запроса на выборку объектов базы данных, а именно столбцов таблиц, с фильтрацией, где id таблицы равен введённому названию в метод
                        command.CommandText = string.Format("select name from sys.columns where object_id = (select object_id from sys.tables where name = '{0}')", TableName);
                        //Заполнение кэш таблицы, реузльтатом выборки обектов из БД
                        table.Load(command.ExecuteReader());
                        //Формирование строки для изменения данных в указанной таблице базы данных
                        string updatequery = string.Format("update [dbo].[{0}] set", TableName);
                        //Организация цикла, для дополнения строки изменения базы данных, с учётом того, что цикл начинается не с 0-ой строки (PK), а с неключевых элементов данных
                        for (int i = 1; i <= table.Rows.Count - 1; i++)
                        {
                            //Дполнение запроса на изменение данных, строкой присвоения к полю таблицы, соответствующего параметра
                            updatequery += string.Format(" {0} = @{0}", table.Rows[i][0]);
                            //Проверка на то, является ли перечисленное поле не последнее в цикле, если да то ставим после названия поля запятую
                            if (i < table.Rows.Count - 1)
                                updatequery += ",";
                        }
                        //Дополнение запроса на изменение данных, условием и присвоением в поле первичного ключа соответствующего параметра
                        updatequery += string.Format(" where {0} = @{0}", table.Rows[0][0]);
                        //Присвоение полученного запроса в свойство UpdateCommand, через инициализацию нового обработчика SQL корманд
                        adapter.UpdateCommand = new SqlCommand(updatequery);
                        //Инициализация свойству UpdateCommand, свойству Connection экземпляра класса SQLConnection
                        adapter.UpdateCommand.Connection = connection;
                        //Принудительная отчистка параметров у свойства UpdateCommand, для избежания аккамулирования параметров
                        adapter.UpdateCommand.Parameters.Clear();
                        //Организация цикла для присвоения полученного списка значений в параметры запроса на изменение данных
                        for (int i = 0; i <= table.Rows.Count - 1; i++)
                        {
                            //Добавление, в коллекцию свойства UpdateCommand, значений в параметры по его названию
                            adapter.UpdateCommand.Parameters.AddWithValue(string.Format("@{0}", table.Rows[i][0]), valueList[i]);
                        }
                        //Выполнение вложенного запроса на изменение данных
                        adapter.UpdateCommand.ExecuteNonQuery();
                        //Перезапись кэш таблицы, с помощью запроса на выборку данных, для визуального обновления данных
                        adapter.Fill(dataSet.Tables[TableName]);
                        break;
                    case Function.delete:
                        //Формирование запроса на выборку объектов базы данных, а именно столбцов таблиц, с фильтрацией, где id таблицы равен введённому названию в метод и где поля имеют свойство is_identity 1, то есть являются PK
                        command.CommandText = string.Format("select name from sys.columns where object_id = (select object_id from sys.tables where name = '{0}') and is_identity = 1", TableName);
                        //Заполнение кэш таблицы, реузльтатом выборки обектов из БД
                        table.Load(command.ExecuteReader());
                        //Формирование строки запроса на удаление данных из указанной таблицы базы данных
                        string deletequery = string.Format("delete from [dbo].[{0}] where [{1}] = @{1}", TableName, table.Rows[0][0]);
                        //Присвоение полученного запроса в свойство DeleteCommand, через инициализацию нового обработчика SQL корманд
                        adapter.DeleteCommand = new SqlCommand(deletequery);
                        //Инициализация свойству DeleteCommand, свойству Connection экземпляра класса SQLConnection
                        adapter.DeleteCommand.Connection = connection;
                        //Принудительная отчистка параметров у свойства DeleteCommand, для избежания аккамулирования параметров
                        adapter.DeleteCommand.Parameters.Clear();
                        //Добавление в коллекцию свойства DeleteCommand значения с названием параметра для дальнейшего удаления данных
                        adapter.DeleteCommand.Parameters.AddWithValue(string.Format("@{0}", table.Rows[0][0]), valueList[0]);
                        //Выполнение вложенного запроса на удаление данных
                        adapter.DeleteCommand.ExecuteNonQuery();
                        //Перезапись кэш таблицы, с помощью запроса на выборку данных, для визуального обновления данных
                        adapter.Fill(dataSet.Tables[TableName]);
                        break;
                }
            }
            catch (SqlException ex)
            {
                //Вывод сообщения об ошибке при работе с базой данных
                MessageBox.Show(ex.Message, "Продажа товара");
            }
            finally
            {
                //Закрытие подключения к базе данных
                connection.Close();
            }
        }

    }
}
