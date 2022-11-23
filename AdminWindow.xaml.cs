using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Data;              //Пространство имён для работы с кэш таблицами, строками, столбцами и данными
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace DataSet_WPF_DB_App
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        MonitoringPC monitoringPC = new MonitoringPC();

        ArrayList values = new ArrayList();

        OpenFileDialog openDialog = new OpenFileDialog();

        string File_Path = string.Empty;

        string qrEmployee = "select [ID_Employee],[First_Name_Employee],[Second_Name_Employee],[Middle_Name_Employee],[Login_Employee], [Password_Employee], [Email_Employee], [Security_Group_ID], [ID_Security_Group], [Name_ Security_Group], [ID_Fast], [Name_Fast] from [dbo].[Employee] inner join [dbo].[Security_Group] on [Security_Group_ID] = [ID_Security_Group] left join [dbo].[Combination] on [Employee_ID] = [ID_Employee] left join [dbo].[Fast] on [ID_Fast] = [Fast_ID] where[ID_Employee] <> ",
            qrCombination = "select [ID_Combination], [First_Name_Employee], [Login_Employee], [Fast_ID], [Employee_ID], [Name_Fast] from [dbo].[Combination] inner join [dbo].[Employee] on [Employee_ID] = [ID_Employee]inner join[dbo].[Fast] on [Fast_ID] = [ID_Fast] where [Employee_ID] <> ",
            qrContract = "select [ID_Contract], [Contract_Number], [Urgent], [Term], [Date_Create_Term], [ID_Security_Firm], [Security_Firm_ID], [Name_Security_Firm], [ID_Сustomer], [Сustomer_ID], [Name_Сustomer] from [dbo].[Contract] inner join [dbo].[Security_Firm] on [Security_Firm_ID] = [ID_Security_Firm] inner join [dbo].[Customer] on [Сustomer_ID] = [ID_Сustomer]",
            qrSecurityFirm = "select [ID_Security_Firm], [Name_Security_Firm] from  [dbo].[Security_Firm]", 
            qrCustomer = "select [ID_Сustomer], [Name_ Organization_Type],[Name_Сustomer], [Login_Customer], [Password_Customer], [Email_Customer], [Organization_Type_ID] from [dbo].[Customer] inner join[dbo].[Organization_Type] on[ID_Organization_Type] = [Organization_Type_ID]";
            
        /// <summary>
        /// Основной медот
        /// </summary>
        public AdminWindow()
        {
            InitializeComponent();
            //Настройка фильтра для диалогового окна
            openDialog.Filter = "Microsoft Excel|*.xlsx|Все файлы|*.*";
            //Обработка события по нажатию пользователем  кнопки OK в диалоговом окне, при выборе файла
            openDialog.FileOk += OpenDilaog_FileOk;
        }

        /// <summary>
        /// Метод взаимодействия с файловой системой
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenDilaog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            File_Path = openDialog.FileName;
            ExcelImport();
        }

        /// <summary>
        /// Метод экспорта данных из Excel
        /// </summary>
        private void ExcelImport()
        {
            //Объявление нетипизированной коллекции для данных таблицы "Тип продукта"
            ArrayList EmployeeList = new ArrayList();
            //Объявление нетипизированной коллекции для данных таблицы "Продукты"
            ArrayList SecurityGroupList = new ArrayList();
            //Объявление целочисленных переменных для значений новых первичных ключей в таблицах "Тип продукта" и "Продукт", соответственно
            int new_SG_ind = 0, employee_id = 0;
            //Инициализация класса для работы с запросами с базой данных
            DataSetClass dataSet = new DataSetClass();
            //Инициализация процесса EXCEL.EXCE
            Excel.Application application = new Excel.Application();
            try
            {
                //Открытие файла Excel в процессе EXCEL.EXE
                Excel.Workbook workbook = application.Workbooks.Open(File_Path);
                //Создание рабочей страницы и создание активной страницы с индексом 1
                Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Worksheets.get_Item(1);
                //Объявление целочисленной переменной и запись в неё количества заполненых строк
                int lastRow = worksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing).Row;
                //Огранизация цикла обращения к строкам таблицы Excel до последней строки (индексация строк начинается с 1)
                for (int row = 2; row <= lastRow; row++)
                {
                    //Вызов метода определения количества строк (функция count(*)) в таблице "Тип продукта", с условием что название типа должно получать
                    //значения из 5 столбца Excel "Название типа", если указанный тип есть count вернёт 1, в противном случе 0
                    dataSet.DataSetFill(string.Format("select count(*) from [dbo].[Security_Group] where [Name_ Security_Group] = '{0}'", Convert.ToString((worksheet.Cells[row, 7] as Excel.Range).Value)),
                        "Security_Group", DataSetClass.Function.select, null);
                    switch (DataSetClass.dataSet.Tables["Security_Group"].Rows[0][0].ToString())
                    {
                        case "0":
                            MessageBox.Show($"Указанной группы {Convert.ToString((worksheet.Cells[row, 7] as Excel.Range).Value)} не существует", "Ошибка!");
                            break;
                        case "1":
                            dataSet.DataSetFill(string.Format("select [ID_Security_Group] from [dbo].[Security_Group] where [Name_ Security_Group] = '{0}'", Convert.ToString((worksheet.Cells[row, 7] as Excel.Range).Value)),
                        "Security_Group", DataSetClass.Function.select, null);
                            break;
                    }
                    new_SG_ind = (int)DataSetClass.dataSet.Tables["Security_Group"].Rows[0][0];

                    for (int col = 1; col < 8; col++)
                    {
                        dataSet.DataSetFill(string.Format("select count(*) from [dbo].[Employee] where [Login_Employee] = '{0}'", Convert.ToString((worksheet.Cells[row, 4] as Excel.Range).Value)),
                            "Employee", DataSetClass.Function.select, null);
                        EmployeeList.Clear();
                        switch (DataSetClass.dataSet.Tables["Employee"].Rows[0][0].ToString())
                        {
                            case "0":
                                EmployeeList.Add(Convert.ToString((worksheet.Cells[row, 1] as Excel.Range).Value));
                                EmployeeList.Add(Convert.ToString((worksheet.Cells[row, 2] as Excel.Range).Value));
                                EmployeeList.Add(Convert.ToString((worksheet.Cells[row, 3] as Excel.Range).Value));
                                EmployeeList.Add(Convert.ToString((worksheet.Cells[row, 4] as Excel.Range).Value));
                                EmployeeList.Add(Convert.ToString((worksheet.Cells[row, 5] as Excel.Range).Value));
                                EmployeeList.Add(Convert.ToString((worksheet.Cells[row, 6] as Excel.Range).Value));
                                EmployeeList.Add(new_SG_ind);

                                dataSet.DataSetFill(string.Format(qrEmployee + "{0}", App.ID), "Employee", DataSetClass.Function.insert, EmployeeList);
                                break;
                            case "1":
                                dataSet.DataSetFill(string.Format("select [ID_Employee] from [dbo].[Employee] where [Login_Employee] = '{0}'", Convert.ToString((worksheet.Cells[row, 4] as Excel.Range).Value)),
                                    "Employee", DataSetClass.Function.select, null);
                                employee_id = (int)DataSetClass.dataSet.Tables["Employee"].Rows[0][0];
                                EmployeeList.Add(employee_id);
                                EmployeeList.Add(Convert.ToString((worksheet.Cells[row, 1] as Excel.Range).Value));
                                EmployeeList.Add(Convert.ToString((worksheet.Cells[row, 2] as Excel.Range).Value));
                                EmployeeList.Add(Convert.ToString((worksheet.Cells[row, 3] as Excel.Range).Value));
                                EmployeeList.Add(Convert.ToString((worksheet.Cells[row, 4] as Excel.Range).Value));
                                EmployeeList.Add(Convert.ToString((worksheet.Cells[row, 5] as Excel.Range).Value));
                                EmployeeList.Add(Convert.ToString((worksheet.Cells[row, 6] as Excel.Range).Value));
                                EmployeeList.Add(new_SG_ind);
                                dataSet.DataSetFill(string.Format(qrEmployee + "{0}", App.ID), "Employee", DataSetClass.Function.update, EmployeeList);
                                break;
                        }
                    }
                }
                workbook.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                application.Quit();
            }
        }

        /// <summary>
        /// Событие закрытия окна AdminWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            DataSetClass.SetLastUser(App.Login, App.Password);
            DataSetClass.SetColorBrush(App.Red, App.Green, App.Blue);
            DataSetClass.SetWindowPositionAdmin(monitoringPC.IsVisible, tbcEmployee.SelectedIndex, tbcContract.SelectedIndex, dgEmployee.SelectedIndex,
                dgFast.SelectedIndex, dgContract.SelectedIndex, dgCustomer.SelectedIndex, dgSecurityFirm.SelectedIndex);
            monitoringPC.Close();


            foreach (Window window in Application.Current.Windows)
            {
                //Если окно не активно
                if (!window.IsActive)
                    //Показать данное окно
                    window.Show();
            }
        }

        /// <summary>
        /// Заполнение таблицы Сотрудника
        /// </summary>
        private void employeeFill()
        {
            //Создание экземпляра класса работы с базой данных
            DataSetClass dataSetClass = new DataSetClass();

            dataSetClass.DataSetFill(string.Format(qrEmployee + "{0}", App.ID), "Employee", DataSetClass.Function.select, null);

            dgEmployee.ItemsSource = DataSetClass.dataSet.Tables["Employee"].DefaultView;

            dgEmployee.Columns[0].Visibility = Visibility.Hidden;
            dgEmployee.Columns[7].Visibility = Visibility.Hidden;
            dgEmployee.Columns[8].Visibility = Visibility.Hidden;
            dgEmployee.Columns[10].Visibility = Visibility.Hidden;

            dgEmployee.Columns[1].Header = "Фамилия";
            dgEmployee.Columns[2].Header = "Имя";
            dgEmployee.Columns[3].Header = "Отчетсво";
            dgEmployee.Columns[4].Header = "Логин";
            dgEmployee.Columns[5].Header = "Пароль";
            dgEmployee.Columns[6].Header = "Эл.Почта";
            dgEmployee.Columns[9].Header = "Название Охранной группы";
            dgEmployee.Columns[11].Header = "Должность";

            btInsert_Entry.IsEnabled = true;

            cbSecurityGroup_Fill();
            cbFast_Fill();
        }

        /// <summary>
        /// Заполенение значениями из БД ComboBox cbSecurityGroup
        /// </summary>
        private void cbSecurityGroup_Fill()
        {
            DataSetClass dataSetClass = new DataSetClass();

            dataSetClass.DataSetFill("select [ID_Security_Group], [Name_ Security_Group] from [dbo].[Security_Group]", "Security_Group", DataSetClass.Function.select, null);

            cbSecurityGroup.ItemsSource = DataSetClass.dataSet.Tables["Security_Group"].DefaultView;
            //Присвоение в свойство "Путь к выбранному значению (PK)", название столбца первичного ключа, кэш таблицы "Охранная группа"
            cbSecurityGroup.SelectedValuePath = DataSetClass.dataSet.Tables["Security_Group"].Columns[0].ColumnName;
            //Присвоение в свойство "Путь к визуальному члену", название столбца "Название типа продукта", кэш таблицы "Охранная группа"
            cbSecurityGroup.DisplayMemberPath = DataSetClass.dataSet.Tables["Security_Group"].Columns[1].ColumnName;
        }

        /// <summary>
        /// Заполенение значениями из БД ComboBox cbFast
        /// </summary>
        private void cbFast_Fill()
        {
            DataSetClass dataSetClass = new DataSetClass();

            dataSetClass.DataSetFill("select * from [dbo].[Fast]", "Fast", DataSetClass.Function.select, null);

            cbFastEmployee.ItemsSource = DataSetClass.dataSet.Tables["Fast"].DefaultView;
            //Присвоение в свойство "Путь к выбранному значению (PK)", название столбца первичного ключа, кэш таблицы "Охранная группа"
            cbFastEmployee.SelectedValuePath = DataSetClass.dataSet.Tables["Fast"].Columns[0].ColumnName;
            //Присвоение в свойство "Путь к визуальному члену", название столбца "Название типа продукта", кэш таблицы "Охранная группа"
            cbFastEmployee.DisplayMemberPath = DataSetClass.dataSet.Tables["Fast"].Columns[1].ColumnName;
        }

        /// <summary>
        /// Заполенение таблицы "Должности"
        /// </summary>
        public void FastFill()
        {
            DataSetClass dataSetClass = new DataSetClass();

            dataSetClass.DataSetFill(string.Format(qrCombination+"'{0}'", App.ID), "Combination", DataSetClass.Function.select, null);

            dgFast.ItemsSource = DataSetClass.dataSet.Tables["Combination"].DefaultView;
            dgFast.Columns[0].Visibility = Visibility.Hidden;
            dgFast.Columns[3].Visibility = Visibility.Hidden;
            dgFast.Columns[4].Visibility = Visibility.Hidden;
            dgFast.Columns[1].Header = "Фамилия сотрудника";
            dgFast.Columns[2].Header = "Логин сотрудника";
            dgFast.Columns[5].Header = "Должность";
        }

        /// <summary>
        /// Заполенение таблицы "Охранные организации"
        /// </summary>
        public void SecurityFirmFill()
        {
            DataSetClass dataSetClass = new DataSetClass();

            dataSetClass.DataSetFill(qrSecurityFirm, "Security_Firm", DataSetClass.Function.select, null);

            dgSecurityFirm.ItemsSource = DataSetClass.dataSet.Tables["Security_Firm"].DefaultView;

            dgSecurityFirm.Columns[0].Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Заполнение таблицы "Договоры"
        /// </summary>
        public void ContractlFill()
        {
            DataSetClass dataSetClass = new DataSetClass();

            dataSetClass.DataSetFill(qrContract, "Contract", DataSetClass.Function.select, null);

            dgContract.ItemsSource = DataSetClass.dataSet.Tables["Contract"].DefaultView;
            dgContract.Columns[0].Visibility = Visibility.Hidden;
            dgContract.Columns[5].Visibility = Visibility.Hidden;
            dgContract.Columns[6].Visibility = Visibility.Hidden;
            dgContract.Columns[8].Visibility = Visibility.Hidden;
            dgContract.Columns[9].Visibility = Visibility.Hidden;

            dgContract.Columns[1].Header = "Номер контракта";
            dgContract.Columns[2].Header = "Срочный";
            dgContract.Columns[3].Header = "Срок";
            dgContract.Columns[4].Header = "Дата заключения";
            dgContract.Columns[7].Header = "Название ЧОП";
            dgContract.Columns[10].Header = "Название заказчика";

            btInsert_Entry_Contract.IsEnabled = true;
        }

        /// <summary>
        /// Заполенение значениями из БД ComboBox cbSecurityFirm
        /// </summary>
        private void cbSecurityFirm_Fill()
        {
            DataSetClass dataSetClass = new DataSetClass();

            dataSetClass.DataSetFill("select [ID_Security_Firm], [Name_Security_Firm] from [dbo].[Security_Firm]", "Security_Firm", DataSetClass.Function.select, null);

            cbSecurityFirmName.ItemsSource = DataSetClass.dataSet.Tables["Security_Firm"].DefaultView;
            cbSecurityFirmName.SelectedValuePath = DataSetClass.dataSet.Tables["Security_Firm"].Columns[0].ColumnName;
            cbSecurityFirmName.DisplayMemberPath = DataSetClass.dataSet.Tables["Security_Firm"].Columns[1].ColumnName;
        }

        /// <summary>
        /// Заполенение значениями из БД ComboBox cbCustomer
        /// </summary>
        private void cbCustomerFill()
        {
            DataSetClass dataSetClass = new DataSetClass();

            dataSetClass.DataSetFill("select [ID_Сustomer], [Name_Сustomer] from [dbo].[Customer]", "Customer", DataSetClass.Function.select, null);

            cbCustomerName.ItemsSource = DataSetClass.dataSet.Tables["Customer"].DefaultView;
            cbCustomerName.SelectedValuePath = DataSetClass.dataSet.Tables["Customer"].Columns[0].ColumnName;
            cbCustomerName.DisplayMemberPath = DataSetClass.dataSet.Tables["Customer"].Columns[1].ColumnName;
        }

        /// <summary>
        /// Событие загрузки окна AdminWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataSetClass.GetWindowPositionAdmin();
            DataSetClass.GetColorBrush();
            UpdateBackgroundColor(App.Red, App.Green, App.Blue);
            Red.Value = App.Red;
            Green.Value = App.Green;
            Blue.Value = App.Blue;
            employeeFill();

            ContractlFill();

            cbSecurityFirm_Fill();
            cbCustomerFill();
            cbUrgent.Items.Add("Да");
            cbUrgent.Items.Add("Нет");
            dgEmployee.SelectedIndex = App.IndexDGEmployee;
            dgContract.SelectedIndex = App.IndexDGContract;
            tbcContract.SelectedIndex = App.IndexTCContract;
            tbcEmployee.SelectedIndex = App.IndexTCEmployee;
            if (App.Monitoring == true) monitoringPC.Show();
        }

        /// <summary>
        /// Обновление таблицы "Охранные организации"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btRefreshSF_Click(object sender, RoutedEventArgs e)
        {
            SecurityFirmFill();
            if (btInsertEntrySF.IsEnabled == false) btInsertEntrySF.IsEnabled = true;
            dgSecurityFirm.SelectedIndex = App.IndexDGSecurityFirm;
        }

        /// <summary>
        /// Событие выбора ячейки в таблице "Сотрудник"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (btUpdate.IsEnabled == false) btUpdate.IsEnabled = true;

            if (btDelete.IsEnabled == false) btDelete.IsEnabled = true;
            if (btInsert_Employee.IsEnabled == true)
            {
                btInsert_Employee.IsEnabled = false;
                btInsert_Entry.IsEnabled = true;
            }

            if (dgEmployee.SelectedItem != null) {
                if ((dgEmployee.Items.Count != 0) && (dgEmployee.SelectedItems[0] != null))
                {
                    DataRowView selectRow = (DataRowView)dgEmployee.SelectedItems[0];
                    tbFirstName.Text = selectRow[1].ToString();
                    tbSecondName.Text = selectRow[2].ToString();
                    tbMiddleName.Text = selectRow[3].ToString();
                    tbLogin.Text = selectRow[4].ToString();
                    pbPassword.Password = selectRow[5].ToString();
                    tbEmail.Text = selectRow[6].ToString();

                    cbSecurityGroup.SelectedValue = selectRow[7].ToString();
                    if (selectRow[10].ToString() != string.Empty) cbFastEmployee.SelectedValue = selectRow[10].ToString();
                    else cbFastEmployee.SelectedValue = null;
                }
            }
        }

        /// <summary>
        /// Событие выбора ячейки в таблице "Заказчик"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (btUpdateCustomer.IsEnabled == false) btUpdateCustomer.IsEnabled = true;
            if (btInsertCustomer.IsEnabled == true) { 
                btInsertCustomer.IsEnabled = false;
                btInsertEntryCustomer.IsEnabled = true;
            }
            if (btDeleteCustomer.IsEnabled == false) btDeleteCustomer.IsEnabled = true;

            if (dgCustomer.SelectedItem != null)
            {
                if((dgCustomer.Items.Count != 0) && (dgCustomer.SelectedItems[0] != null))
                {
                    DataRowView selectRow = (DataRowView)dgCustomer.SelectedItems[0];
                    cbTypeCustomer.SelectedValue = selectRow[6].ToString();
                    tbNameCustomer.Text = selectRow[2].ToString();
                    tbLoginCustomer.Text = selectRow[3].ToString();
                    pbPasswordCustomer.Password = selectRow[4].ToString();
                    tbEmailCustomer.Text = selectRow[5].ToString();
                }
            }
        }

        /// <summary>
        /// Заполнение таблицы "Заказчик"
        /// </summary>
        private void CustomerFill()
        {
            DataSetClass dataSetClass = new DataSetClass();

            dataSetClass.DataSetFill(qrCustomer, "Customer", DataSetClass.Function.select, null);

            dgCustomer.ItemsSource = DataSetClass.dataSet.Tables["Customer"].DefaultView;

            dataSetClass.DataSetFill("select [ID_Organization_Type], [Name_ Organization_Type] from [dbo].[Organization_Type]", "Organization_Type", DataSetClass.Function.select, null);
            cbTypeCustomer.ItemsSource = DataSetClass.dataSet.Tables["Organization_Type"].DefaultView;
            cbTypeCustomer.SelectedValuePath = DataSetClass.dataSet.Tables["Organization_Type"].Columns[0].ColumnName;
            cbTypeCustomer.DisplayMemberPath = DataSetClass.dataSet.Tables["Organization_Type"].Columns[1].ColumnName;
        }

        /// <summary>
        /// Обновление таблицы "Заказчики"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btRefreshCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerFill();
            if (btInsertEntryCustomer.IsEnabled == false) btInsertEntryCustomer.IsEnabled = true;
            dgCustomer.SelectedIndex = App.IndexDGContract;
        }

        /// <summary>
        /// Событие обновление значений БД таблицы Employee
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployee.SelectedItem != null)
            {
                //Создание экземпляра класса, кэш представление строки данных, с присвоением, с явным преобразованием, выбранной строки таблицы
                DataRowView rowView = (DataRowView)dgEmployee.SelectedItems[0];
                //Принудительная отчистка коллекции не типизированного списка вводимых параметров, для избежания аккамулирования значений
                values.Clear();
                if (rowView[0] != null)
                {
                    //Добавление в не типизированную коллекцию, значения кэш строки из первого столбца, значение первичного ключа таблицы "Сотрудники"
                    values.Add(rowView[0]);
                    object PK = rowView[0];
                    if (!string.IsNullOrEmpty(tbFirstName.Text))
                    {
                        values.Add(tbFirstName.Text);
                        if (!string.IsNullOrEmpty(tbSecondName.Text))
                        {
                            values.Add(tbSecondName.Text);
                            values.Add(tbMiddleName.Text);
                            if ((tbLogin.Text != null) && (tbLogin.Text.Length >= 8))
                            {
                                values.Add(tbLogin.Text);

                                if (!string.IsNullOrEmpty(pbPassword.Password) && (pbPassword.Password.Length >= 8))
                                {
                                    if (pbPassword.Password == pbPasswordConf.Password)
                                    {
                                        values.Add(pbPassword.Password);
                                        if (!string.IsNullOrEmpty(tbEmail.Text))
                                        {
                                            values.Add(tbEmail.Text);
                                            if (cbSecurityGroup.SelectedValue != null)
                                            {
                                                if (cbFastEmployee.SelectedValue != null)
                                                {
                                                    values.Add(cbSecurityGroup.SelectedValue);
                                                    //Создание экземпляра класса работы с базой данных
                                                    DataSetClass dataSetClass = new DataSetClass();
                                                    //Вызов метода работы с запросами базы данных, с передачей аргументов: строковой переменной с запросом на выборку данных из таблицы "Сотрудники",
                                                    // название кэш таблицы, иструкции к алгоритму формирования запроса на изменение данных, не типизированный список с входными данными в запрос
                                                    dataSetClass.DataSetFill(string.Format(qrEmployee + "{0}", App.ID), "Employee", DataSetClass.Function.update, values);

                                                    //Отчистка полей ввода
                                                    tbFirstName.Text = string.Empty;
                                                    tbSecondName.Text = string.Empty;
                                                    tbMiddleName.Text = string.Empty;
                                                    tbLogin.Text = string.Empty;
                                                    pbPassword.Password = string.Empty;
                                                    pbPasswordConf.Password = string.Empty;
                                                    tbEmail.Text = string.Empty;
                                                    cbSecurityGroup.SelectedValue = null;
                                                    cbFastEmployee.SelectedValue = null;
                                                    employeeFill();
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Выберите должность", "Изменение сотрудника");
                                                    cbFastEmployee.Focus();
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("Выберите Охранную группу.", "Изменение сотрудника");
                                                cbSecurityGroup.Focus();
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Поле \"Эл.Почта\" не может быть пустым.", "Изменение сотрудника");
                                            tbEmail.Focus();
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Пароли должны совпадать", "Изменение сотрудника");
                                        pbPasswordConf.Focus();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Некорректно введен пароль.", "Изменение сотрудника");
                                    pbPassword.Focus();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Некорректно введен логин", "Изменение сотрудника");
                                tbLogin.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Поле \"Имя\" не может быть пустым", "Изменение сотрудника");
                            tbSecondName.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Поле \"Фамилия\" не может быть пустым", "Изменение сотрудника");
                        tbFirstName.Focus();
                    }

                }
                else
                {
                    MessageBox.Show("Выберите сотрудника, котого хотите изменить.", "Изменение сотрудника");
                    dgEmployee.Focus();
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника, котого хотите изменить.", "Изменение сотрудника");
                dgEmployee.Focus();
            }

        }

        /// <summary>
        /// добавление записи для добавление нового значения в таблицу Employee
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btInsert_Entry_Click(object sender, RoutedEventArgs e)
        {
            tbFirstName.Text = string.Empty;
            tbSecondName.Text = string.Empty;
            tbMiddleName.Text = string.Empty;
            tbLogin.Text = string.Empty;
            pbPassword.Password = string.Empty;
            pbPasswordConf.Password = string.Empty;
            tbEmail.Text = string.Empty;
            cbSecurityGroup.SelectedValue = null;
            cbFastEmployee.SelectedValue = null;
            btInsert_Employee.IsEnabled = true;
            btUpdate.IsEnabled = false;
            btDelete.IsEnabled = false;
            sender.GetType().GetProperty("IsEnabled").SetValue(sender, false);
        }

        /// <summary>
        /// Добавление нового значения в БД в таблицу Employee
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btInsert_Employee_Click(object sender, RoutedEventArgs e)
        {
            //Принудительная отчистка коллекции не типизированного списка вводимых параметров, для избежания аккамулирования значений
            values.Clear();
            if (!string.IsNullOrEmpty(tbFirstName.Text))
            {
                values.Add(tbFirstName.Text);
                if (!string.IsNullOrEmpty(tbSecondName.Text))
                {
                    values.Add(tbSecondName.Text);
                    values.Add(tbMiddleName.Text);
                    if ((tbLogin.Text != null) && (tbLogin.Text.Length >= 8))
                    {
                        values.Add(tbLogin.Text);

                        if (!string.IsNullOrEmpty(pbPassword.Password) && (pbPassword.Password.Length >= 8))
                        {
                            if (pbPassword.Password == pbPasswordConf.Password)
                            {
                                values.Add(pbPassword.Password);
                                if (!string.IsNullOrEmpty(tbEmail.Text))
                                {
                                    values.Add(tbEmail.Text);
                                    if (cbSecurityGroup.SelectedValue != null)
                                    {
                                        values.Add(cbSecurityGroup.SelectedValue);
                                        //Создание экземпляра класса работы с базой данных
                                        DataSetClass dataSetClass = new DataSetClass();
                                        //Вызов метода работы с запросами базы данных, с передачей аргументов: строковой переменной с запросом на выборку данных из таблицы "Сотрудники",
                                        // название кэш таблицы, иструкции к алгоритму формирования запроса на изменение данных, не типизированный список с входными данными в запрос
                                        dataSetClass.DataSetFill(string.Format(qrEmployee + "{0}", App.ID), "Employee", DataSetClass.Function.insert, values);

                                        //Отчистка полей ввода
                                        tbFirstName.Text = string.Empty;
                                        tbSecondName.Text = string.Empty;
                                        tbMiddleName.Text = string.Empty;
                                        tbLogin.Text = string.Empty;
                                        pbPassword.Password = string.Empty;
                                        pbPasswordConf.Password = string.Empty;
                                        tbEmail.Text = string.Empty;
                                        cbSecurityGroup.SelectedValue = null;
                                        cbFastEmployee.SelectedValue = null;

                                        employeeFill();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Выберите Охранную группу.", "Создание сотрудника");
                                        cbSecurityGroup.Focus();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Поле \"Эл.Почта\" не может быть пустым.", "Создание сотрудника");
                                    tbEmail.Focus();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Пароли должны совпадать", "Создание сотрудника");
                                pbPasswordConf.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Некорректно введен пароль.", "Создание сотрудника");
                            pbPassword.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Некорректно введен логин", "Создание сотрудника");
                        tbLogin.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Поле \"Имя\" не может быть пустым", "Создание сотрудника");
                    tbSecondName.Focus();
                }
            }
            else
            {
                MessageBox.Show("Поле \"Фамилия\" не может быть пустым", "Создание сотрудника");
                tbFirstName.Focus();
            }
            btInsert_Entry.IsEnabled = true;
            btDelete.IsEnabled = true;
            btUpdate.IsEnabled = true;
            sender.GetType().GetProperty("IsEnabled").SetValue(sender, false);
        }       

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployee.SelectedItem != null)
            {
                //Создание экземпляра класса, кэш представление строки данных, с присвоением, с явным преобразованием, выбранной строки таблицы
                DataRowView rowView = (DataRowView)dgEmployee.SelectedItems[0];
                //Принудительная отчистка коллекции не типизированного списка вводимых параметров, для избежания аккамулирования значений
                values.Clear();
                if (rowView[0] != null)
                {
                    switch (MessageBox.Show("Удалить выбранную запись?", "Удаление сотрудника", MessageBoxButton.YesNo, MessageBoxImage.Warning))
                    {
                        //Реакция программы после нажатия кнопки Да
                        case MessageBoxResult.Yes:
                            values.Add(rowView[0]);
                            //Создание экземпляра класса работы с базой данных
                            DataSetClass dataSetClass = new DataSetClass();
                            //Вызов метода работы с запросами базы данных, с передачей аргументов: строковой переменной с запросом на выборку данных из таблицы "Сотрудники",
                            //название кэш таблицы, иструкции к алгоритму формирования запроса на изменение данных, не типизированный список с входными данными в запрос
                            dataSetClass.DataSetFill(string.Format(qrEmployee + "{0}", App.ID), "Employee", DataSetClass.Function.delete, values);

                            //Отчистка полей ввода
                            tbFirstName.Text = string.Empty;
                            tbSecondName.Text = string.Empty;
                            tbMiddleName.Text = string.Empty;
                            tbLogin.Text = string.Empty;
                            pbPassword.Password = string.Empty;
                            pbPasswordConf.Password = string.Empty;
                            tbEmail.Text = string.Empty;
                            cbSecurityGroup.SelectedValue = null;
                            cbFastEmployee.SelectedValue = null;
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Выберите сотрудника, которого хотите удалить.");
                    dgEmployee.Focus();
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника, которого хотите удалить.");
                dgEmployee.Focus();
            }
        }

        /// <summary>
        /// Событие выбора ячейки в таблице "Договор"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgContract_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (btUpdateContract.IsEnabled == false) btUpdateContract.IsEnabled = true;

            if (btDeleteContract.IsEnabled == false) btDeleteContract.IsEnabled = true;
            if (btInsert_Contract.IsEnabled == true)
            {
                btInsert_Contract.IsEnabled = false;
                btInsert_Entry_Contract.IsEnabled = true;
            }

            if (dgContract.SelectedItem != null)
            {
                if ((dgContract.Items.Count != 0) && (dgContract.SelectedItems[0] != null))
                {
                    DataRowView selectRow = (DataRowView)dgContract.SelectedItems[0];
                    tbContractNumber.Text = selectRow[1].ToString();
                    cbUrgent.SelectedValue = selectRow[2].ToString();
                    tbTerm.Text = selectRow[3].ToString();
                    dpDateCreate.SelectedDate = Convert.ToDateTime(selectRow[4].ToString());
                    cbSecurityFirmName.SelectedValue = selectRow[5].ToString();
                    cbCustomerName.SelectedValue = selectRow[8].ToString();
                }
            }
        }

        /// <summary>
        /// Удаление записи в БД из таблицы "Договор"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDelete_Contract_Click(object sender, RoutedEventArgs e)
        {
            if (dgContract.SelectedItem != null)
            {
                //Создание экземпляра класса, кэш представление строки данных, с присвоением, с явным преобразованием, выбранной строки таблицы
                DataRowView rowView = (DataRowView)dgContract.SelectedItems[0];
                //Принудительная отчистка коллекции не типизированного списка вводимых параметров, для избежания аккамулирования значений
                values.Clear();
                if (rowView[0] != null)
                {
                    switch (MessageBox.Show("Удалить выбранный договор?", "Удаление договора", MessageBoxButton.YesNo, MessageBoxImage.Warning))
                    {
                        //Реакция программы после нажатия кнопки Да
                        case MessageBoxResult.Yes:
                            values.Add(rowView[0]);
                            //Создание экземпляра класса работы с базой данных
                            DataSetClass dataSetClass = new DataSetClass();
                            //Вызов метода работы с запросами базы данных, с передачей аргументов: строковой переменной с запросом на выборку данных из таблицы "Сотрудники",
                            //название кэш таблицы, иструкции к алгоритму формирования запроса на изменение данных, не типизированный список с входными данными в запрос
                            dataSetClass.DataSetFill(qrContract, "Contract", DataSetClass.Function.delete, values);

                            //Отчистка полей ввода
                            tbContractNumber.Text = string.Empty;
                            cbUrgent.SelectedValue = null;
                            tbTerm.Text = string.Empty;
                            dpDateCreate.SelectedDate = null;
                            cbSecurityFirmName.SelectedValue = null;
                            cbCustomerName.SelectedValue = null;
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Выберите договор, который хотите разорвать");
                    dgContract.Focus();
                }
            }
            else
            {
                MessageBox.Show("Выберите договор, который хотите разорвать");
                dgContract.Focus();
            }
        }


        private void btInsert_Entry_Contract_Click(object sender, RoutedEventArgs e)
        {
            dgContract.SelectedItem = null;
            tbContractNumber.Text = string.Empty;
            cbUrgent.SelectedValue = null;
            tbTerm.Text = string.Empty;
            dpDateCreate.SelectedDate = null;
            cbSecurityFirmName.SelectedValue = null;
            cbCustomerName.SelectedItem = null;
            btInsert_Contract.IsEnabled = true;
            sender.GetType().GetProperty("IsEnabled").SetValue(sender, false);
        }

        /// <summary>
        /// Добавление нового значние в БД в таблицу "Договор"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btInsert_Contract_Click(object sender, RoutedEventArgs e)
        {
            //Принудительная отчистка коллекции не типизированного списка вводимых параметров, для избежания аккамулирования значений
            values.Clear();
            if (!string.IsNullOrEmpty(tbContractNumber.Text))
            {
                values.Add(tbContractNumber.Text);
                if (cbUrgent.SelectedValue != null)
                {
                    values.Add(cbUrgent.SelectedValue);
                    if (!string.IsNullOrEmpty(tbTerm.Text))
                    {
                        values.Add(tbTerm.Text);
                        if ((dpDateCreate.SelectedDate != null) && (dpDateCreate.SelectedDate <= DateTime.Today))
                        {
                            values.Add(dpDateCreate.SelectedDate);

                            if (cbSecurityFirmName.SelectedValue != null)
                            {
                                values.Add(cbSecurityFirmName.SelectedValue);
                                if (cbCustomerName.SelectedValue != null)
                                {
                                    values.Add(cbCustomerName.SelectedValue);
                                    //Создание экземпляра класса работы с базой данных
                                    DataSetClass dataSetClass = new DataSetClass();
                                    //Вызов метода работы с запросами базы данных, с передачей аргументов: строковой переменной с запросом на выборку данных из таблицы "Сотрудники",
                                    // название кэш таблицы, иструкции к алгоритму формирования запроса на изменение данных, не типизированный список с входными данными в запрос
                                    dataSetClass.DataSetFill(qrContract, "Contract", DataSetClass.Function.insert, values);

                                    //Отчистка полей ввода
                                    tbContractNumber.Text = string.Empty;
                                    cbUrgent.SelectedValue = null;
                                    tbTerm.Text = string.Empty;
                                    dpDateCreate.SelectedDate = null;
                                    cbSecurityFirmName.SelectedValue = null;
                                    cbCustomerName.SelectedValue = null;
                                    btInsert_Entry_Contract.IsEnabled = true;
                                    sender.GetType().GetProperty("IsEnabled").SetValue(sender, false);
                                }
                                else
                                {
                                    MessageBox.Show("Поле \"Название заказчика\" не может быть пустым.");
                                    cbCustomerName.Focus();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Поле \"Название ЧОП\" не может быть пустым.");
                                cbSecurityFirmName.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Поле \"Дата создания\" не может быть пустым. Так же дата создания не может быть больше сегоднешней даты");
                            dpDateCreate.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Поле \"Срок действия\" не может быть пустым");
                    }
                }
                else
                {
                    MessageBox.Show("Поле \"Срочный\" не может быть пустым");
                    cbUrgent.Focus();
                }
            }
            else
            {
                MessageBox.Show("Поле \"Номер договора\" не может быть пустым");
                tbContractNumber.Focus();
            }
        }

        /// <summary>
        /// Обновление таблицы "Должности"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btRefreshFast_Click(object sender, RoutedEventArgs e)
        {
            DataSetClass dataSetClass = new DataSetClass();

            dataSetClass.DataSetFill("select * from [dbo].[Fast]", "Fast", DataSetClass.Function.select, null);
            cbFasts.ItemsSource = DataSetClass.dataSet.Tables["Fast"].DefaultView;
            cbFasts.SelectedValuePath = DataSetClass.dataSet.Tables["Fast"].Columns[0].ColumnName;
            cbFasts.DisplayMemberPath = DataSetClass.dataSet.Tables["Fast"].Columns[1].ColumnName;

            dataSetClass.DataSetFill(string.Format(qrEmployee + "'{0}'", App.ID), "Employee", DataSetClass.Function.select, null);
            cbLogin.ItemsSource = DataSetClass.dataSet.Tables["Employee"].DefaultView;
            cbLogin.SelectedValuePath = DataSetClass.dataSet.Tables["Employee"].Columns[0].ColumnName;
            cbLogin.DisplayMemberPath = DataSetClass.dataSet.Tables["Employee"].Columns[4].ColumnName;

            FastFill();
            employeeFill();
            btInsertEntryFast.IsEnabled = true;
            dgFast.SelectedIndex = App.IndexDGFast;
        }

        private void btInsertEntryFast_Click(object sender, RoutedEventArgs e)
        {
            cbLogin.SelectedValue = null;
            cbFasts.SelectedValue = null;
            cbFasts.IsEnabled = true;
            btInsertNewFast.IsEnabled = true;
            sender.GetType().GetProperty("IsEnabled").SetValue(sender, false);
        }

        private void btInsertNewFast_Click(object sender, RoutedEventArgs e)
        {
            values.Clear();
            if (cbFasts.SelectedValue != null)
            {
                values.Add(cbFasts.SelectedValue);
                if (cbLogin.SelectedValue != null)
                {
                    values.Add(cbLogin.SelectedValue);
                    //Создание экземпляра класса работы с базой данных
                    DataSetClass dataSetClass = new DataSetClass();
                    //Вызов метода работы с запросами базы данных, с передачей аргументов: строковой переменной с запросом на выборку данных из таблицы "Сотрудники",
                    // название кэш таблицы, иструкции к алгоритму формирования запроса на изменение данных, не типизированный список с входными данными в запрос
                    dataSetClass.DataSetFill(string.Format(qrCombination + "'{0}'", App.ID), "Combination", DataSetClass.Function.insert, values);
                    cbLogin.SelectedValue = null;
                    cbFasts.SelectedValue = null;
                    employeeFill();
                }
                else
                {
                    MessageBox.Show("Поле \"Логин сотрудника\" не может быть пустым.", "Добавление новой должности.");
                }
            }
            else
            {
                MessageBox.Show("Поле \"Должность\" не может быть пустым.", "Добавление новой должности.");
                cbFasts.Focus();
            }
            btInsertEntryFast.IsEnabled = true;
            sender.GetType().GetProperty("IsEnabled").SetValue(sender, false);
        }

        /// <summary>
        /// Удаление выбранной записи в БД из таблицы Combination
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDeleteCombination_Click(object sender, RoutedEventArgs e)
        {
            if(dgFast.SelectedItem != null)
            {
                //Создание экземпляра класса, кэш представление строки данных, с присвоением, с явным преобразованием, выбранной строки таблицы
                DataRowView rowView = (DataRowView)dgFast.SelectedItems[0];
                //Принудительная отчистка коллекции не типизированного списка вводимых параметров, для избежания аккамулирования значений
                values.Clear();
                if (rowView[0] != null)
                {
                    switch (MessageBox.Show(string.Format("Уволить сотрудника {0} с должности {1}?", rowView[1].ToString(), rowView[5].ToString()), "Увольнение", MessageBoxButton.YesNo, MessageBoxImage.Warning))
                    {
                        //Реакция программы после нажатия кнопки Да
                        case MessageBoxResult.Yes:
                            values.Add(rowView[0]);
                            //Создание экземпляра класса работы с базой данных
                            DataSetClass dataSetClass = new DataSetClass();
                            //Вызов метода работы с запросами базы данных, с передачей аргументов: строковой переменной с запросом на выборку данных из таблицы "Сотрудники",
                            //название кэш таблицы, иструкции к алгоритму формирования запроса на изменение данных, не типизированный список с входными данными в запрос
                            dataSetClass.DataSetFill(string.Format(qrCombination + "'{0}'", App.ID), "Combination", DataSetClass.Function.delete, values);

                            //Отчистка полей ввода
                            cbLogin.SelectedValue = null;
                            cbFasts.SelectedValue = null;
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Выберите сотрудника, которого хотите уволить.", "Увольнение");
                    dgFast.Focus();
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника, которого хотите уволить.", "Увольнение");
                dgFast.Focus();
            }
        }

        /// <summary>
        /// Событие нажатие на кнопку "Импорт"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Import_Click(object sender, RoutedEventArgs e)
        {
            //Вызов диалогового окна
            openDialog.ShowDialog();
        }

        private void btUpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomer.SelectedItem != null)
            {
                //Создание экземпляра класса, кэш представление строки данных, с присвоением, с явным преобразованием, выбранной строки таблицы
                DataRowView rowView = (DataRowView)dgCustomer.SelectedItems[0];
                values.Clear();
                //Принудительная отчистка коллекции не типизированного списка вводимых параметров, для избежания аккамулирования значений
                values.Add(rowView[0]);
                if (!string.IsNullOrEmpty(tbNameCustomer.Text))
                {
                    values.Add(tbNameCustomer.Text);
                    if ((!string.IsNullOrEmpty(tbLoginCustomer.Text)) && (tbLoginCustomer.Text.Length >= 8))
                    {
                        values.Add(tbLoginCustomer.Text);
                        if ((!string.IsNullOrEmpty(pbPasswordCustomer.Password)) && (pbPasswordCustomer.Password.Length >= 8))
                        {
                            if(pbPasswordCustomer.Password == pbPasswordCustomerConfig.Password)
                            {
                                values.Add(pbPasswordCustomer.Password);
                                if (!string.IsNullOrEmpty(tbEmailCustomer.Text))
                                {
                                    values.Add(tbEmailCustomer.Text);
                                    if (cbTypeCustomer.SelectedValue != null)
                                    {
                                        values.Add(cbTypeCustomer.SelectedValue);
                                        DataSetClass dataSetClass = new DataSetClass();
                                        dataSetClass.DataSetFill(qrCustomer, "Customer", DataSetClass.Function.update, values);
                                        ContractlFill();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Укажите тип заказчика.", "Изменение заказчика");
                                        cbTypeCustomer.Focus();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Поле \"Эл.Почта\" не может быть пустым.", "Изменение заказчика");
                                    tbEmailCustomer.Focus();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Пароли должны совпадать.", "Изменение заказчика");
                                pbPasswordCustomerConfig.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Неверный формат пароля. Пароль должен быть больше 8 символов.", "Изменение заказчика");
                            pbPasswordCustomer.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверный формат логина. Логин должен быть больше 8 символов.", "Изменение заказчика");
                        tbLoginCustomer.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Поле \"Название заказчика\" не может быть пустым.", "Изменение заказчика");
                    tbNameCustomer.Focus();
                }
            }
        }

        private void btInsertEntryCustomer_Click(object sender, RoutedEventArgs e)
        {
            cbTypeCustomer.SelectedValue = null;
            tbNameCustomer.Text = string.Empty;
            tbLoginCustomer.Text = string.Empty;
            pbPasswordCustomer.Password = string.Empty;
            pbPasswordCustomerConfig.Password = string.Empty;
            tbEmailCustomer.Text = string.Empty;

            btInsertCustomer.IsEnabled = true;
            sender.GetType().GetProperty("IsEnabled").SetValue(sender, false);
        }

        /// <summary>
        /// Добавление новой записи в БД в таблицу Customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btInsertCustomer_Click(object sender, RoutedEventArgs e)
        {
            values.Clear();
            if (!string.IsNullOrEmpty(tbNameCustomer.Text))
            {
                values.Add(tbNameCustomer.Text);
                if ((!string.IsNullOrEmpty(tbLoginCustomer.Text)) && (tbLoginCustomer.Text.Length >= 8))
                {
                    values.Add(tbLoginCustomer.Text);
                    if ((!string.IsNullOrEmpty(pbPasswordCustomer.Password)) && (pbPasswordCustomer.Password.Length >= 8))
                    {
                        if (pbPasswordCustomer.Password == pbPasswordCustomerConfig.Password)
                        {
                            values.Add(pbPasswordCustomer.Password);
                            if (!string.IsNullOrEmpty(tbEmailCustomer.Text))
                            {
                                values.Add(tbEmailCustomer.Text);
                                if (cbTypeCustomer.SelectedValue != null)
                                {
                                    values.Add(cbTypeCustomer.SelectedValue);
                                    DataSetClass dataSetClass = new DataSetClass();
                                    dataSetClass.DataSetFill(qrCustomer, "Customer", DataSetClass.Function.insert, values);

                                    cbTypeCustomer.SelectedValue = null;
                                    tbNameCustomer.Text = string.Empty;
                                    tbLoginCustomer.Text = string.Empty;
                                    pbPasswordCustomer.Password = string.Empty;
                                    pbPasswordCustomerConfig.Password = string.Empty;
                                    tbEmailCustomer.Text = string.Empty;

                                    ContractlFill();

                                    btInsertEntryCustomer.IsEnabled = true;
                                    sender.GetType().GetProperty("IsEnabled").SetValue(sender, false);
                                }
                                else
                                {
                                    MessageBox.Show("Укажите тип заказчика", "Изменение заказчика");
                                    cbTypeCustomer.Focus();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Поле \"Эл.почта\" не может быть пустым.", "Изменение заказчика");
                                tbEmailCustomer.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Пароли должны совпадать.", "Изменение заказчика");
                            pbPasswordCustomerConfig.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неверный формат пароля. Пароль должен быть больше 8 символов.", "Изменение заказчика");
                        pbPasswordCustomer.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Неверный формат логина. Логин должен быть больше 8 символов.", "Изменение заказчика");
                    tbLoginCustomer.Focus();
                }
            }
            else
            {
                MessageBox.Show("Поле \"Название заказчика\" не может быть пустым.", "Изменение заказчика");
                tbNameCustomer.Focus();
            }
        }

        /// <summary>
        /// Удаление выбранной записи в БД из таблицы Customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            //Создание экземпляра класса, кэш представление строки данных, с присвоением, с явным преобразованием, выбранной строки таблицы
            DataRowView rowView = (DataRowView)dgCustomer.SelectedItems[0];
            //Принудительная отчистка коллекции не типизированного списка вводимых параметров, для избежания аккамулирования значений
            values.Clear();
            if (rowView[0] != null)
            {
                switch (MessageBox.Show("Удалить выбранную запись?", "Удаление заказчика", MessageBoxButton.YesNo, MessageBoxImage.Warning))
                {
                    //Реакция программы после нажатия кнопки Да
                    case MessageBoxResult.Yes:
                        values.Add(rowView[0]);
                        //Создание экземпляра класса работы с базой данных
                        DataSetClass dataSetClass = new DataSetClass();
                        //Вызов метода работы с запросами базы данных, с передачей аргументов: строковой переменной с запросом на выборку данных из таблицы "Сотрудники",
                        //название кэш таблицы, иструкции к алгоритму формирования запроса на изменение данных, не типизированный список с входными данными в запрос
                        dataSetClass.DataSetFill(qrCustomer, "Customer", DataSetClass.Function.delete, values);

                        //Отчистка полей ввода
                        cbTypeCustomer.SelectedValue = null;
                        tbNameCustomer.Text = string.Empty;
                        tbLoginCustomer.Text = string.Empty;
                        pbPasswordCustomer.Password = string.Empty;
                        pbPasswordCustomerConfig.Password = string.Empty;
                        tbEmailCustomer.Text = string.Empty;
                        ContractlFill();
                        break;
                }
            }
            else
            {
                MessageBox.Show("Выберите заказчика, которого хотите удалить.");
                dgCustomer.Focus();
            }
        }

        private void dgSecurityFirm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (btUpdateSecurityFirm.IsEnabled == false) btUpdateSecurityFirm.IsEnabled = true;
            if (btInsertSF.IsEnabled == true)
            {
                btInsertSF.IsEnabled = false;
                btInsertEntrySF.IsEnabled = true;
            }
            if (btDeleteSF.IsEnabled == false) btDeleteSF.IsEnabled = true;

            if (dgSecurityFirm.SelectedItem != null)
            {
                if ((dgSecurityFirm.Items.Count != 0) && (dgSecurityFirm.SelectedItems[0] != null))
                {
                    DataRowView selectRow = (DataRowView)dgSecurityFirm.SelectedItems[0];
                    tbNameSF.Text = selectRow[1].ToString();
                }
            }
        }

        /// <summary>
        /// Изменение выбранной записи в БД в таблицe SecurityFirm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btUpdateSecurityFirm_Click(object sender, RoutedEventArgs e)
        {
            if (dgSecurityFirm.SelectedItem != null)
            {
                //Создание экземпляра класса, кэш представление строки данных, с присвоением, с явным преобразованием, выбранной строки таблицы
                DataRowView rowView = (DataRowView)dgSecurityFirm.SelectedItems[0];
                //Принудительная отчистка коллекции не типизированного списка вводимых параметров, для избежания аккамулирования значений
                values.Clear();
                if (rowView[0] != null)
                {
                    //Добавление в не типизированную коллекцию, значения кэш строки из первого столбца, значение первичного ключа таблицы "Сотрудники"
                    values.Add(rowView[0]);
                    if (!string.IsNullOrEmpty(tbNameSF.Text))
                    {
                        values.Add(tbNameSF.Text);
                        DataSetClass dataSetClass = new DataSetClass();
                        dataSetClass.DataSetFill(qrSecurityFirm, "Security_Firm", DataSetClass.Function.update, values);

                        tbNameSF.Text = string.Empty;
                        ContractlFill();
                    }
                    else
                    {
                        MessageBox.Show("Поле \"Название ЧОП\" должно быть заполнено.", "Изменение ЧОП");
                        tbNameSF.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Выберите фирму, которую хотите изменить", "Изменение ЧОП");
                    dgSecurityFirm.Focus();
                }
            }
        }

        private void InsertEnrtySF_Click(object sender, RoutedEventArgs e)
        {
            tbNameSF.Text = string.Empty;

            btInsertSF.IsEnabled = true;
            sender.GetType().GetProperty("IsEnabled").SetValue(sender, false);
        }

        /// <summary>
        /// Добавлине новой записи в БД в таблицу Security_Firm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btInsertSF_Click(object sender, RoutedEventArgs e)
        {
            values.Clear();
            if (!string.IsNullOrEmpty(tbNameSF.Text))
            {
                values.Add(tbNameSF.Text);

                DataSetClass dataSetClass = new DataSetClass();
                dataSetClass.DataSetFill(qrSecurityFirm, "Security_Firm", DataSetClass.Function.insert, values);
                tbNameSF.Text = string.Empty;
                btInsertEntrySF.IsEnabled = true;
                sender.GetType().GetProperty("IsEnabled").SetValue(sender, false);

                ContractlFill();
            }
            else
            {
                MessageBox.Show("Поле \"Название ЧОП\" должно быть заполнено.", "Изменение ЧОП");
                tbNameSF.Focus();
            }
        }

        /// <summary>
        /// Удаление записи в БД из таблицы Security_Firm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteSF_Click(object sender, RoutedEventArgs e)
        {
            if (dgSecurityFirm.SelectedItem != null)
            {
                //Создание экземпляра класса, кэш представление строки данных, с присвоением, с явным преобразованием, выбранной строки таблицы
                DataRowView rowView = (DataRowView)dgSecurityFirm.SelectedItems[0];
                //Принудительная отчистка коллекции не типизированного списка вводимых параметров, для избежания аккамулирования значений
                values.Clear();
                if (rowView[0] != null)
                {
                    switch (MessageBox.Show(string.Format("Удалить ЧОП {0}?", rowView[1].ToString()), "Удаление охранной фирмы", MessageBoxButton.YesNo, MessageBoxImage.Warning))
                    {
                        //Реакция программы после нажатия кнопки Да
                        case MessageBoxResult.Yes:
                            values.Add(rowView[0]);
                            //Создание экземпляра класса работы с базой данных
                            DataSetClass dataSetClass = new DataSetClass();
                            //Вызов метода работы с запросами базы данных, с передачей аргументов: строковой переменной с запросом на выборку данных из таблицы "Сотрудники",
                            //название кэш таблицы, иструкции к алгоритму формирования запроса на изменение данных, не типизированный список с входными данными в запрос
                            dataSetClass.DataSetFill(qrSecurityFirm, "Security_Firm", DataSetClass.Function.delete, values);
                            ContractlFill();

                            //Отчистка полей ввода
                            tbNameSF.Text = string.Empty;
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Выберите фирму, которую хотите удалить.", "Удаление охранной фирмы");
                    dgSecurityFirm.Focus();
                }
            }
            else
            {
                MessageBox.Show("Выберите фирму, которую хотите удалить.", "Удаление охранной фирмы");
                dgSecurityFirm.Focus();
            }
        }

        /// <summary>
        /// Обноваление глобальных переменных Red, Green, Blue и 
        /// вызов метода обновление цвета заднего фона окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton pressed = (RadioButton)sender;
            switch (pressed.Tag)
            {
                case "White":
                    App.Red = 255; App.Green = 255; App.Blue = 255;
                    break;
                case "Black":
                    App.Red = 27; App.Green = 24; App.Blue = 37;
                    break;
            }
            UpdateBackgroundColor(App.Red, App.Green, App.Blue);
        }

        /// <summary>
        /// Обновление цвета заднего фона окна
        /// </summary>
        /// <param name="Red"></param>
        /// <param name="Green"></param>
        /// <param name="Blue"></param>
        private void UpdateBackgroundColor(int Red, int Green, int Blue)
        {
            SolidColorBrush Background = new SolidColorBrush(Color.FromRgb(App.Red, App.Green, App.Blue));
            MainGrid.Background = Background;
            GridFast.Background = Background;
            GridEmployee.Background = Background;
            GridFast.Background = Background;
            GridContract.Background = Background;
            GridCustomer.Background = Background;
            GridSF.Background = Background;
        }

        private void UpdateBackground_Click(object sender, RoutedEventArgs e)
        {
            App.Red =Convert.ToByte(Red.Value);
            App.Green = Convert.ToByte(Green.Value);
            App.Blue = Convert.ToByte(Blue.Value);
            UpdateBackgroundColor(App.Red, App.Green, App.Blue);
        }

        /// <summary>
        /// Вывод окна о информации действия Администратора
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Данное приложение предназначего для администрирования системы взаимодействия Охранных фирм с заказчиками. Администратор может удалять, нанимать сотрудников, изменять их данные." +
                "Все взаимодействия с сотрудниками представлено в левой стороне окна приложения. Там представлены таблицы сотрудников и их должностей. Справа представлены компоненты для администратирования договоров между " +
                "заказчиком и Охранной организацией. Так же есть таблицы для редактирвоания заказчиков и охранных организаций");
        }

        /// <summary>
        /// Вывод окна о информации аппаратной части ПК
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btMonitoring_Click(object sender, RoutedEventArgs e)
        {
            monitoringPC.Show();
        }

        private void btUpdateContract_Click(object sender, RoutedEventArgs e)
        {
            //Создание экземпляра класса, кэш представление строки данных, с присвоением, с явным преобразованием, выбранной строки таблицы
            DataRowView rowView = (DataRowView)dgContract.SelectedItems[0];
            //Принудительная отчистка коллекции не типизированного списка вводимых параметров, для избежания аккамулирования значений
            values.Clear();
            if (rowView[0] != null)
            {

                //Добавление в не типизированную коллекцию, значения кэш строки из первого столбца, значение первичного ключа таблицы "Сотрудники"
                values.Add(rowView[0]);
                if (!string.IsNullOrEmpty(tbContractNumber.Text))
                {
                    values.Add(tbContractNumber.Text);
                    if (cbUrgent.SelectedValue != null)
                    {
                        values.Add(cbUrgent.SelectedValue);
                        if (!string.IsNullOrEmpty(tbTerm.Text))
                        {
                            values.Add(tbTerm.Text);
                            if ((dpDateCreate.SelectedDate != null) && (dpDateCreate.SelectedDate <= DateTime.Today))
                            {
                                values.Add(dpDateCreate.SelectedDate);

                                if (cbSecurityFirmName.SelectedValue != null)
                                {
                                    values.Add(cbSecurityFirmName.SelectedValue);
                                    if (cbCustomerName.SelectedValue != null)
                                    {
                                        values.Add(cbCustomerName.SelectedValue);
                                        //Создание экземпляра класса работы с базой данных
                                        DataSetClass dataSetClass = new DataSetClass();
                                        //Вызов метода работы с запросами базы данных, с передачей аргументов: строковой переменной с запросом на выборку данных из таблицы "Сотрудники",
                                        // название кэш таблицы, иструкции к алгоритму формирования запроса на изменение данных, не типизированный список с входными данными в запрос
                                        dataSetClass.DataSetFill(qrContract, "Contract", DataSetClass.Function.update, values);

                                        //Отчистка полей ввода
                                        tbContractNumber.Text = string.Empty;
                                        cbUrgent.SelectedValue = null;
                                        tbTerm.Text = string.Empty;
                                        dpDateCreate.SelectedDate = null;
                                        cbSecurityFirmName.SelectedValue = null;
                                        cbCustomerName.SelectedValue = null;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Поле \"Название заказчика\" не может быть пустым.");
                                        cbCustomerName.Focus();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Поле \"Название ЧОП\" не может быть пустым.");
                                    cbSecurityFirmName.Focus();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Поле \"Дата создания\" не может быть пустым. Так же дата создания не может быть больше сегоднешней даты");
                                dpDateCreate.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Поле \"Срок действия\" не может быть пустым");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Поле \"Срочный\" не может быть пустым");
                        cbUrgent.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Поле \"Номер договора\" не может быть пустым");
                    tbContractNumber.Focus();
                }

            }
            else
            {
                MessageBox.Show("Выберите контракт, который хотите изменить.");
                dgContract.Focus();
            }
        }

        private void btUpdateFast_Click(object sender, RoutedEventArgs e)
        {
            if (dgFast.SelectedItem != null)
            {
                //Создание экземпляра класса, кэш представление строки данных, с присвоением, с явным преобразованием, выбранной строки таблицы
                DataRowView rowView = (DataRowView)dgFast.SelectedItems[0];
                //Принудительная отчистка коллекции не типизированного списка вводимых параметров, для избежания аккамулирования значений
                values.Clear();
                if (rowView[0] != null)
                {
                    values.Add(rowView[0]);
                    if (cbFasts.SelectedValue != null)
                    {
                        values.Add(cbFasts.SelectedValue);
                        values.Add(rowView[3]);
                        DataSetClass dataSetClass = new DataSetClass();
                        //Вызов метода работы с запросами базы данных, с передачей аргументов: строковой переменной с запросом на выборку данных из таблицы "Сотрудники",
                        // название кэш таблицы, иструкции к алгоритму формирования запроса на изменение данных, не типизированный список с входными данными в запрос
                        dataSetClass.DataSetFill(string.Format(qrCombination + "'{0}'", App.ID), "Combination", DataSetClass.Function.update, values);

                        cbLogin.SelectedValue = null;
                        cbFasts.SelectedValue = null;
                        employeeFill();
                    }
                }
                else
                {
                    MessageBox.Show("Выберите сотрудника, которого хотите изменить.", "Изменение должности.");
                    dgFast.Focus();
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника, которого хотите изменить.", "Изменение должности.");
                dgFast.Focus();
            }
        }

        /// <summary>
        /// Событие выбора ячейки из таблицы Должность
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgFast_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbFasts.IsEnabled = true;
            if (dgFast.SelectedItem != null)
            {
                if ((dgFast.Items.Count != 0) && (dgFast.SelectedItems[0] != null))
                {
                    DataRowView selectRow = (DataRowView)dgFast.SelectedItems[0];
                    cbLogin.SelectedValue = selectRow[4].ToString();
                    cbFasts.SelectedValue = selectRow[3].ToString();
                }
            }
        }
    }
}
