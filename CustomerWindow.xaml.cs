using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;              //Пространство имён для работы с кэш таблицами, строками, столбцами и данными
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;

namespace DataSet_WPF_DB_App
{
    /// <summary>
    /// Логика взаимодействия для CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        MonitoringPC monitoringPC = new MonitoringPC();

        ArrayList values = new ArrayList();

        string qrCustomerInfo = "select [ID_Contract], [Contract_Number], [Urgent], [Term], [Date_Create_Term], [Security_Firm_ID], [Name_Security_Firm], [ID_Object_Protection],[Full_Name_Object_Protection], [Сustomer_ID] from [dbo].[Contract] inner join[dbo].[Security_Firm] on[ID_Security_Firm]=[Security_Firm_ID] inner join[dbo].[Customer] on [ID_Сustomer] = [Сustomer_ID] left join [dbo].[Object_Protection] on [Contract_ID] = [ID_Contract] where [ID_Сustomer] =",
            qrContract = "select [ID_Contract], [Contract_Number], [Urgent], [Term], [Date_Create_Term], [ID_Security_Firm], [Security_Firm_ID], [Name_Security_Firm], [ID_Сustomer], [Сustomer_ID], [Name_Сustomer] from [dbo].[Contract] inner join [dbo].[Security_Firm] on [Security_Firm_ID] = [ID_Security_Firm] inner join [dbo].[Customer] on [Сustomer_ID] = [ID_Сustomer]",
            qrObjProtection = "select [ID_Object_Protection], [Name_ Organization_Type] +' '+[Full_Name_Object_Protection],[Name_ Organization_Type]+' '+[Short_Name_Object_Protection], [Legal_Address], [Physical_Address], [Organization_Type_ID], [Area_Territory], [Number_Of_Posts], [Contract_ID], [Type_ID], [Contract_Number], [Name_Type] from [dbo].[Object_Protection] inner join [dbo].[Organization_Type] on[ID_Organization_Type] = [Organization_Type_ID] inner join[dbo].[Contract] on [ID_Contract] = [Contract_ID] inner join [dbo].[Type] on[ID_Type] = [Type_ID] where [Сustomer_ID] = ";

        /// <summary>
        /// Основной метод
        /// </summary>
        public CustomerWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Заполение значениями из БД ComboBox cbSecurityFirm
        /// </summary>
        private void cbSecurityFirm_Fill()
        {
            DataSetClass dataSetClass = new DataSetClass();
            dataSetClass.DataSetFill("select [ID_Security_Firm], [Name_Security_Firm] from [dbo].[Security_Firm]", "Security_Firm", DataSetClass.Function.select, null);

            cbSecurityFirm.ItemsSource = DataSetClass.dataSet.Tables["Security_Firm"].DefaultView;
            cbSecurityFirm.SelectedValuePath = DataSetClass.dataSet.Tables["Security_Firm"].Columns[0].ColumnName;
            cbSecurityFirm.DisplayMemberPath = DataSetClass.dataSet.Tables["Security_Firm"].Columns[1].ColumnName;
        }

        private void lblObjProtection_Fill()
        {
            DataSetClass dataSetClass = new DataSetClass();
            dataSetClass.DataSetFill(String.Format(qrCustomerInfo + "'{0}'", App.ID), "Contract", DataSetClass.Function.select, null);

            lbObjProtection.ItemsSource = DataSetClass.dataSet.Tables["Contract"].DefaultView;
            lbObjProtection.SelectedValuePath = DataSetClass.dataSet.Tables["Contract"].Columns[7].ColumnName;
            lbObjProtection.DisplayMemberPath = DataSetClass.dataSet.Tables["Contract"].Columns[8].ColumnName;
        }

        /// <summary>
        /// Заполнение таблицы "Информация о договоре"
        /// </summary>
        private void ContractInfoFill()
        {
            lblObjProtection_Fill();
            DataSetClass dataSetClass = new DataSetClass();
            dataSetClass.DataSetFill(String.Format(qrCustomerInfo + "'{0}'", App.ID), "Contract", DataSetClass.Function.select, null);

            dgContractInfo.ItemsSource = DataSetClass.dataSet.Tables["Contract"].DefaultView;
            
            cbSecurityFirm_Fill();
            dgContractInfo.SelectedIndex = App.IndexDGContractInfo;
        }

        /// <summary>
        /// Заполнение таблицы "Охраняемый объект"
        /// </summary>
        private void ObjectProtection_Fill()
        {
            DataSetClass dataSetClass = new DataSetClass();
            dataSetClass.DataSetFill(string.Format(qrObjProtection + "'{0}'", App.ID), "Object_Protection", DataSetClass.Function.select, null);

            dgObjectProtection.ItemsSource = DataSetClass.dataSet.Tables["Object_Protection"].DefaultView;

            dgObjectProtection.Columns[0].Visibility = Visibility.Hidden;
            dgObjectProtection.Columns[5].Visibility = Visibility.Hidden;
            dgObjectProtection.Columns[8].Visibility = Visibility.Hidden;
            dgObjectProtection.Columns[9].Visibility = Visibility.Hidden;

            dgObjectProtection.Columns[1].Header = "Полное название объекта";
            dgObjectProtection.Columns[2].Header = "Сокращенное название объекта";
            dgObjectProtection.Columns[3].Header = "Юр. адрес объекта";
            dgObjectProtection.Columns[4].Header = "Физический адрес объекта";
            dgObjectProtection.Columns[6].Header = "Площадь объекта";
            dgObjectProtection.Columns[7].Header = "Количество постов";
            dgObjectProtection.Columns[10].Header = "Номер договора";
            dgObjectProtection.Columns[11].Header = "Вид объекта";

            cbTypeOrg_Fill();
            cbTypeObject_Fill();
            btCreateEntryObj.IsEnabled = true;
            dgObjectProtection.SelectedIndex = App.IndexDGObj;
        }

        private void cbTypeOrg_Fill()
        {
            DataSetClass dataSetClass = new DataSetClass();
            dataSetClass.DataSetFill("select [ID_Organization_Type], [Name_ Organization_Type] from [dbo].[Organization_Type]", "Organization_Type", DataSetClass.Function.select, null);

            cbTypeOrg.ItemsSource = DataSetClass.dataSet.Tables["Organization_Type"].DefaultView;
            cbTypeOrg.SelectedValuePath = DataSetClass.dataSet.Tables["Organization_Type"].Columns[0].ColumnName;
            cbTypeOrg.DisplayMemberPath = DataSetClass.dataSet.Tables["Organization_Type"].Columns[1].ColumnName;
        }

        /// <summary>
        /// Заполение значениями из БД ComboBox cbTypeObject
        /// </summary>
        private void cbTypeObject_Fill()
        {
            DataSetClass dataSetClass = new DataSetClass();
            dataSetClass.DataSetFill("select [ID_Type], [Name_Type] from [dbo].[Type]", "Type", DataSetClass.Function.select, null);

            cbType.ItemsSource = DataSetClass.dataSet.Tables["Type"].DefaultView;
            cbType.SelectedValuePath = DataSetClass.dataSet.Tables["Type"].Columns[0].ColumnName;
            cbType.DisplayMemberPath = DataSetClass.dataSet.Tables["Type"].Columns[1].ColumnName;
        }
        /// <summary>
        /// Заполение значениями из БД ComboBox cbNumberContract
        /// </summary>
        private void cbNumberContract_Fill()
        {
            DataSetClass dataSetClass = new DataSetClass();
            dataSetClass.DataSetFill($"select [ID_Contract], [Contract_Number] from [dbo].[Contract] where [Сustomer_ID] = {App.ID}", "Contract", DataSetClass.Function.select, null);

            cbContractNumberObj.ItemsSource = DataSetClass.dataSet.Tables["Contract"].DefaultView;
            cbContractNumberObj.SelectedValuePath = DataSetClass.dataSet.Tables["Contract"].Columns[0].ColumnName;
            cbContractNumberObj.DisplayMemberPath = DataSetClass.dataSet.Tables["Contract"].Columns[1].ColumnName;
        }
        /// <summary>
        /// Событие загрузки окна CustomerWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            DataSetClass.GetWindowPositionCustomer();
            cbNumberContract_Fill();
            ContractInfoFill();
            ObjectProtection_Fill();
            cbUrgent.Items.Add("Да");
            cbUrgent.Items.Add("Нет");
            btCreateEntry.IsEnabled = true;
            btDelete.IsEnabled = true;
            if (App.MonitoringCustomer == true) monitoringPC.Show();
        }
        /// <summary>
        /// Событите выбора ячейки таблицы "Информация о договоре"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgContractInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (btCreateContract.IsEnabled == true)
            {
                btCreateEntry.IsEnabled = true;
                btCreateContract.IsEnabled = false;
            }

            if ( dgContractInfo.SelectedItem != null)
            {
                if ((dgContractInfo.Items.Count != 0) && (dgContractInfo.SelectedItems[0] != null))
                {
                    DataRowView selectRow = (DataRowView)dgContractInfo.SelectedItems[0];
                    tbContractNumber.Text = selectRow[1].ToString();
                    cbUrgent.SelectedValue = selectRow[2].ToString();
                    tbTerm.Text = selectRow[3].ToString();
                    dpDateCreate.SelectedDate = Convert.ToDateTime(selectRow[4].ToString());
                    cbSecurityFirm.SelectedValue = selectRow[5].ToString();

                    if (selectRow[7].ToString() != string.Empty) lbObjProtection.SelectedValue = selectRow[7].ToString();
                }
            }
        }
        /// <summary>
        /// Событие закрытия окна CustomerWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            DataSetClass.SetLastUser(App.Login, App.Password);
            DataSetClass.SetWindowPositionCutomer(monitoringPC.IsVisible, dgContractInfo.SelectedIndex, dgObjectProtection.SelectedIndex);
            monitoringPC.Close();
            
            foreach (Window window in Application.Current.Windows)
            {
                //Если окно не активно
                if (!window.IsActive)
                    //Показать данное окно
                    window.Show();
            }
        }

        private void btCreateEntry_Click(object sender, RoutedEventArgs e)
        {
            dgContractInfo.SelectedItem = false;
            tbContractNumber.Text = string.Empty;
            cbUrgent.SelectedItem = null;
            dpDateCreate.SelectedDate = null;
            cbSecurityFirm.SelectedItem = null;
            lbObjProtection.SelectedItem = null;
            tbTerm.Text = string.Empty;

            tbContractNumber.IsReadOnly = false;
            cbUrgent.IsReadOnly = false;
            dpDateCreate.IsEnabled = true;
            cbSecurityFirm.IsReadOnly = false;
            tbTerm.IsReadOnly = false;

            btCreateContract.IsEnabled = true;
            sender.GetType().GetProperty("IsEnabled").SetValue(sender, false);
        }
        /// <summary>
        /// Событие выбора ячейки в таблице "Охраняемые объект"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgObjectProtection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (btUpdateObj.IsEnabled == false) btUpdateObj.IsEnabled = true;
            if (btCreateObj.IsEnabled == true)
            {
                btCreateObj.IsEnabled = false;
                btCreateEntryObj.IsEnabled = true;
            }
            if (btDeleteObj.IsEnabled == false) btDeleteObj.IsEnabled = true;

            if (dgObjectProtection.SelectedItem != null)
            {
                if ((dgObjectProtection.Items.Count != 0) && (dgObjectProtection.SelectedItems[0] != null))
                {
                    DataRowView selectRow = (DataRowView)dgObjectProtection.SelectedItems[0];
                    tbFullName.Text = selectRow[1].ToString();
                    tbShortName.Text = selectRow[2].ToString();
                    cbTypeOrg.SelectedValue = selectRow[5].ToString();
                    tbLegAddress.Text = selectRow[3].ToString();
                    tbPhAddress.Text = selectRow[4].ToString();
                    tbArea.Text = selectRow[6].ToString();
                    tbNumPosts.Text = selectRow[7].ToString();
                    cbContractNumberObj.SelectedValue = selectRow[8].ToString();
                    cbType.SelectedValue = selectRow[9].ToString();
                }
            }
        }

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            //Создание экземпляра класса, кэш представление строки данных, с присвоением, с явным преобразованием, выбранной строки таблицы
            DataRowView rowView = (DataRowView)dgContractInfo.SelectedItems[0];
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
                        dataSetClass.DataSetFill(string.Format(qrCustomerInfo + "{0}", App.ID), "Contract", DataSetClass.Function.delete, values);
                        ObjectProtection_Fill();

                        //Отчистка полей ввода
                        tbContractNumber.Text = string.Empty;
                        cbUrgent.SelectedValue = null;
                        dpDateCreate.SelectedDate = null;
                        cbSecurityFirm.SelectedValue = null;
                        lbObjProtection.SelectedValue = null;
                        tbTerm.Text = string.Empty;
                        break;
                }
            }
            else
            {
                MessageBox.Show("Выберите договор, который хотите разорвать");
                dgContractInfo.Focus();
            }
        }

        private void btCreateContract_Click(object sender, RoutedEventArgs e)
        {
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
                        if (dpDateCreate.SelectedDate != null)
                        {
                            values.Add(dpDateCreate.SelectedDate);
                            if (cbSecurityFirm.SelectedValue != null)
                            {
                                values.Add(cbSecurityFirm.SelectedValue);
                                values.Add(App.ID);
                                DataSetClass dataSetClass = new DataSetClass();
                                dataSetClass.DataSetFill(qrContract, "Contract", DataSetClass.Function.insert, values);

                                //Отчистка полей ввода
                                tbContractNumber.Text = string.Empty;
                                cbUrgent.SelectedValue = null;
                                dpDateCreate.SelectedDate = null;
                                tbTerm.Text = string.Empty;
                                cbSecurityFirm.SelectedValue = null;
                                lbObjProtection.SelectedValue = null;

                                ContractInfoFill();
                            }
                            else
                            {
                                MessageBox.Show("Некорректно введен пароль.");
                                cbSecurityFirm.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Некорректно введен логин");
                            dpDateCreate.Focus();
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    MessageBox.Show("Поле \"Имя\" не может быть пустым");
                    cbUrgent.Focus();
                }
            }
            else
            {
                MessageBox.Show("Поле \"Фамилия\" не может быть пустым");
                tbContractNumber.Focus();
            }
            btCreateEntry.IsEnabled = true;
            sender.GetType().GetProperty("IsEnabled").SetValue(sender, false);
        }

        private void btMonitoringPC_Click(object sender, RoutedEventArgs e)
        {
            monitoringPC.Show();
        }

        private void btCreateEntryObj_Click(object sender, RoutedEventArgs e)
        {
            tbFullName.Text = string.Empty;
            tbShortName.Text = string.Empty;
            cbTypeOrg.SelectedValue = null;
            tbLegAddress.Text = string.Empty;
            tbPhAddress.Text = string.Empty;
            tbArea.Text = string.Empty;
            tbNumPosts.Text = string.Empty;
            cbContractNumberObj.SelectedValue = null;
            cbType.SelectedValue = null;

            btCreateObj.IsEnabled = true;
            sender.GetType().GetProperty("IsEnabled").SetValue(sender, false);
        }

        private void btCreateObj_Click(object sender, RoutedEventArgs e)
        {
            values.Clear();
            if (!string.IsNullOrEmpty(tbLegAddress.Text))
            {
                values.Add(tbLegAddress.Text);
                if (!string.IsNullOrEmpty(tbPhAddress.Text))
                {
                    values.Add(tbPhAddress.Text);
                    if (cbTypeOrg.SelectedValue != null)
                    {
                        values.Add(cbTypeOrg.SelectedValue);
                        if (!string.IsNullOrEmpty(tbFullName.Text))
                        {
                            values.Add(tbFullName.Text);
                            if (!string.IsNullOrEmpty(tbShortName.Text))
                            {
                                values.Add(tbShortName.Text);
                                if (!string.IsNullOrEmpty(tbArea.Text))
                                {
                                    values.Add(tbArea.Text);
                                    if (!string.IsNullOrEmpty(tbNumPosts.Text))
                                    {
                                        values.Add(tbNumPosts.Text);
                                        if (cbContractNumberObj.SelectedValue != null)
                                        {
                                            values.Add(cbContractNumberObj.SelectedValue);
                                            if (cbType.SelectedValue != null)
                                            {
                                                values.Add(cbType.SelectedValue);

                                                DataSetClass dataSetClass = new DataSetClass();
                                                dataSetClass.DataSetFill(string.Format(qrObjProtection + "'{0}'", App.ID), "Object_Protection", DataSetClass.Function.insert, values);
                                                ContractInfoFill();

                                                tbFullName.Text = string.Empty;
                                                tbShortName.Text = string.Empty;
                                                cbTypeOrg.SelectedValue = null;
                                                tbLegAddress.Text = string.Empty;
                                                tbPhAddress.Text = string.Empty;
                                                tbArea.Text = string.Empty;
                                                tbNumPosts.Text = string.Empty;
                                                cbContractNumberObj.SelectedValue = null;
                                                cbType.SelectedValue = null;

                                                btCreateEntryObj.IsEnabled = true;
                                                sender.GetType().GetProperty("IsEnabled").SetValue(sender, false);
                                            }
                                            else
                                            {
                                                MessageBox.Show("Поле \"Тип объекта\" должно быть заполнено.", "Создание объекта");
                                                cbType.Focus();
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Поле \"Номер договора\" должно быть заполнено.", "Создание объекта");
                                            cbContractNumberObj.Focus();
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Поле \"Количество постов\" должно быть заполнено.", "Создание объекта");
                                        tbNumPosts.Focus();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Поле \"Площадь\" должно быть заполнено.", "Создание объекта");
                                    tbArea.Focus();
                                }
                            }                          
                            else
                            {
                                MessageBox.Show("Поле \"Сокращенное название\" должно быть заполнено.", "Создание объекта");
                                tbShortName.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Поле \"Полное название объекта\" должно быть заполнено.", "Создание объекта");
                            tbFullName.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Поле \"Тип организации\" должно быть заполнено.", "Создание объекта");
                        cbTypeOrg.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Поле \"Физический адрес\" должно быть заполнено.", "Создание объекта");
                    tbPhAddress.Focus();
                }
            }
            else
            {
                MessageBox.Show("Поле \"Юридический адрес\" должно быть заполнено.", "Создание объекта");
                tbLegAddress.Focus();
            }

        }
        /// <summary>
        /// Изменение выбранной записи в БД в таблице "Охраняемый объект"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btUpdateObj_Click(object sender, RoutedEventArgs e)
        {
            if (dgObjectProtection.SelectedItem != null)
            {
                //Создание экземпляра класса, кэш представление строки данных, с присвоением, с явным преобразованием, выбранной строки таблицы
                DataRowView rowView = (DataRowView)dgObjectProtection.SelectedItems[0];
                //Принудительная отчистка коллекции не типизированного списка вводимых параметров, для избежания аккамулирования значений
                values.Clear();
                if (rowView[0] != null)
                {
                    //Добавление в не типизированную коллекцию, значения кэш строки из первого столбца, значение первичного ключа таблицы "Сотрудники"
                    values.Add(rowView[0]);
                    if (!string.IsNullOrEmpty(tbLegAddress.Text))
                    {
                        values.Add(tbLegAddress.Text);
                        if (!string.IsNullOrEmpty(tbPhAddress.Text))
                        {
                            values.Add(tbPhAddress.Text);
                            if (cbTypeOrg.SelectedValue != null)
                            {
                                values.Add(cbTypeOrg.SelectedValue);
                                if (!string.IsNullOrEmpty(tbFullName.Text))
                                {
                                    values.Add(tbFullName.Text);
                                    if (!string.IsNullOrEmpty(tbShortName.Text))
                                    {
                                        values.Add(tbShortName.Text);
                                        if (!string.IsNullOrEmpty(tbArea.Text))
                                        {
                                            values.Add(tbArea.Text);
                                            if (!string.IsNullOrEmpty(tbNumPosts.Text))
                                            {
                                                values.Add(tbNumPosts.Text);
                                                if (cbContractNumberObj.SelectedValue != null)
                                                {
                                                    values.Add(cbContractNumberObj.SelectedValue);
                                                    if (cbType.SelectedValue != null)
                                                    {
                                                        values.Add(cbType.SelectedValue);

                                                        DataSetClass dataSetClass = new DataSetClass();
                                                        dataSetClass.DataSetFill(string.Format(qrObjProtection + "'{0}'", App.ID), "Object_Protection", DataSetClass.Function.update, values);
                                                        ContractInfoFill();

                                                        tbFullName.Text = string.Empty;
                                                        tbShortName.Text = string.Empty;
                                                        cbTypeOrg.SelectedValue = null;
                                                        tbLegAddress.Text = string.Empty;
                                                        tbPhAddress.Text = string.Empty;
                                                        tbArea.Text = string.Empty;
                                                        tbNumPosts.Text = string.Empty;
                                                        cbContractNumberObj.SelectedValue = null;
                                                        cbType.SelectedValue = null;

                                                        btCreateEntryObj.IsEnabled = true;
                                                        sender.GetType().GetProperty("IsEnabled").SetValue(sender, false);
                                                    }
                                                    else
                                                    {
                                                        MessageBox.Show("Поле \"Тип объекта\" должно быть заполнено.", "Изменение объекта");
                                                        cbType.Focus();
                                                    }
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Поле \"Номер договора\" должно быть заполнено.", "Изменение объекта");
                                                    cbContractNumberObj.Focus();
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("Поле \"Количество постов\" должно быть заполнено.", "Изменение объекта");
                                                tbNumPosts.Focus();
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Поле \"Площадь\" должно быть заполнено.", "Изменение объекта");
                                            tbArea.Focus();
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Поле \"Сокращенное название\" должно быть заполнено.", "Изменение объекта");
                                        tbShortName.Focus();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Поле \"Полное название объекта\" должно быть заполнено.", "Изменение объекта");
                                    tbFullName.Focus();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Поле \"Тип организации\" должно быть заполнено.", "Изменение объекта");
                                cbTypeOrg.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Поле \"Физический адрес\" должно быть заполнено.", "Изменение объекта");
                            tbPhAddress.Focus();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Поле \"Юридический адрес\" должно быть заполнено.", "Изменение объекта");
                        tbLegAddress.Focus();
                    }
                }
            }
        }
        /// <summary>
        /// Удаление выбранной записи в БД из таблцы "Охраняемый объект"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDeleteObj_Click(object sender, RoutedEventArgs e)
        {
            if (dgObjectProtection.SelectedItem != null)
            {
                //Создание экземпляра класса, кэш представление строки данных, с присвоением, с явным преобразованием, выбранной строки таблицы
                DataRowView rowView = (DataRowView)dgObjectProtection.SelectedItems[0];
                //Принудительная отчистка коллекции не типизированного списка вводимых параметров, для избежания аккамулирования значений
                values.Clear();
                if (rowView[0] != null)
                {
                    switch (MessageBox.Show(string.Format("Удалить ЧОП {0}?", rowView[1].ToString()), "Удаление охранной фирмы", MessageBoxButton.YesNo, MessageBoxImage.Warning))
                    {
                        //Реакция программы после нажатия кнопки Да
                        case MessageBoxResult.Yes:
                            //Добавление в не типизированную коллекцию, значения кэш строки из первого столбца, значение первичного ключа таблицы "Сотрудники"
                            values.Add(rowView[0]);
                            //Создание экземпляра класса работы с базой данных
                            DataSetClass dataSetClass = new DataSetClass();
                            //Вызов метода работы с запросами базы данных, с передачей аргументов: строковой переменной с запросом на выборку данных из таблицы "Сотрудники",
                            //название кэш таблицы, иструкции к алгоритму формирования запроса на изменение данных, не типизированный список с входными данными в запрос
                            dataSetClass.DataSetFill($"{qrObjProtection} {App.ID}", "Object_Protection", DataSetClass.Function.delete, values);
                            ContractInfoFill();

                            //Отчистка полей ввода
                            tbFullName.Text = string.Empty;
                            tbShortName.Text = string.Empty;
                            cbTypeOrg.SelectedValue = null;
                            tbLegAddress.Text = string.Empty;
                            tbPhAddress.Text = string.Empty;
                            tbArea.Text = string.Empty;
                            tbNumPosts.Text = string.Empty;
                            cbContractNumberObj.SelectedValue = null;
                            cbType.SelectedValue = null;
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Нажатие на кнопку "Экспорт"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btExport_Click(object sender, RoutedEventArgs e)
        {
            ExportExcel();
        }

        /// <summary>
        /// Экспорт в таблицу Excel
        /// </summary>
        private void ExportExcel()
        {
            Excel.Application application = new Excel.Application();
            application.SheetsInNewWorkbook = 2;
            try
            {
                Excel.Workbook workbook = application.Workbooks.Add();
                Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Sheets[1];
                DataSetClass dataSet = new DataSetClass();
                dataSet.DataSetFill($"select [Name_Сustomer] from [dbo].[Customer] where [ID_Сustomer] = {App.ID}", "Customer", DataSetClass.Function.select, null);
                worksheet.Name = $"Договора заказчика {DataSetClass.dataSet.Tables["Customer"].Rows[0][0]}";
                worksheet.Cells[1, 1] = "Номер договара"; worksheet.Cells[1, 2] = "Срочный"; worksheet.Cells[1, 3] = "Срок действия"; worksheet.Cells[1, 4] = "Дата создания";
                worksheet.Cells[1, 5] = "Охранная фирма"; worksheet.Cells[1, 6] = "Заказчик";
                dataSet.DataSetFill($"select [Contract_Number], [Urgent], [Term], [Date_Create_Term], [Name_Security_Firm], [Name_Сustomer] from [dbo].[Contract] inner join [dbo].[Security_Firm] on[ID_Security_Firm] = [Security_Firm_ID] inner join[dbo].[Customer] on[ID_Сustomer] = [Сustomer_ID] where [Сustomer_ID] = '{App.ID}'",
                    "Contract", DataSetClass.Function.select, null);
                switch (DataSetClass.dataSet.Tables["Contract"].Rows.Count)
                {
                    case 0:
                        MessageBox.Show("У вас нет договоров", "Экспорт договоров.");
                        break;
                    default:
                        for (int i = 2; i < DataSetClass.dataSet.Tables["Contract"].Rows.Count + 2; i++)
                        {
                            worksheet.Cells[1][i] = DataSetClass.dataSet.Tables["Contract"].Rows[i - 2][0].ToString();
                            worksheet.Cells[2][i] = DataSetClass.dataSet.Tables["Contract"].Rows[i - 2][1].ToString();
                            worksheet.Cells[3][i] = DataSetClass.dataSet.Tables["Contract"].Rows[i - 2][2].ToString();
                            worksheet.Cells[4][i] = DataSetClass.dataSet.Tables["Contract"].Rows[i - 2][3].ToString();
                            worksheet.Cells[5][i] = DataSetClass.dataSet.Tables["Contract"].Rows[i - 2][4].ToString();
                            worksheet.Cells[6][i] = DataSetClass.dataSet.Tables["Contract"].Rows[i - 2][5].ToString();
                        }
                        Excel.Range He = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 6]];
                        He.Font.Bold = true;
                        He.Font.Name = "Times New Roman";
                        He.Font.Size = 14;

                        //application.Range[worksheet.Cells[1, 1], worksheet.Cells[DataSetClass.dataSet.Tables["Contract"].Rows.Count + 2, 6]].Copy();
                        break;
                }
                worksheet.Columns.AutoFit();

                Excel.Worksheet worksheet2 = (Excel.Worksheet)workbook.Sheets[2];
                worksheet2.Name = "Охраняемые объекты";
                worksheet2.Cells[1, 1] = "Полное название объекта"; worksheet2.Cells[1, 2] = "Сокращенное название"; worksheet2.Cells[1, 3] = "Юридический адрес";
                worksheet2.Cells[1, 4] = "Физический адрес"; worksheet2.Cells[1, 5] = "Площадь объекта"; worksheet2.Cells[1, 6] = "Количество постов";
                worksheet2.Cells[1, 7] = "Номер договора"; worksheet2.Cells[1, 8] = "Вид объекта";
                dataSet.DataSetFill(string.Format("select[Name_ Organization_Type] + ' ' +[Full_Name_Object_Protection],[Name_ Organization_Type] + ' ' +[Short_Name_Object_Protection], [Legal_Address], [Physical_Address], [Area_Territory], [Number_Of_Posts], [Contract_Number], [Name_Type] from[dbo].[Object_Protection] inner join[dbo].[Organization_Type] on[ID_Organization_Type] = [Organization_Type_ID] inner join[dbo].[Contract] on[ID_Contract] = [Contract_ID] inner join[dbo].[Type] on[ID_Type] = [Type_ID] where[Сustomer_ID] = "+ "'{0}'", App.ID), "Object_Protection", DataSetClass.Function.select, null);
                for (int i = 0; i < DataSetClass.dataSet.Tables["Object_Protection"].Rows.Count; i++)
                {
                    worksheet2.Cells[1][i + 2] = DataSetClass.dataSet.Tables["Object_Protection"].Rows[i][0].ToString();
                    worksheet2.Cells[2][i + 2] = DataSetClass.dataSet.Tables["Object_Protection"].Rows[i][1].ToString();
                    worksheet2.Cells[3][i + 2] = DataSetClass.dataSet.Tables["Object_Protection"].Rows[i][2].ToString();
                    worksheet2.Cells[4][i + 2] = DataSetClass.dataSet.Tables["Object_Protection"].Rows[i][3].ToString();
                    worksheet2.Cells[5][i + 2] = DataSetClass.dataSet.Tables["Object_Protection"].Rows[i][4].ToString();
                    worksheet2.Cells[6][i + 2] = DataSetClass.dataSet.Tables["Object_Protection"].Rows[i][5].ToString();
                    worksheet2.Cells[7][i + 2] = DataSetClass.dataSet.Tables["Object_Protection"].Rows[i][6].ToString();
                    worksheet2.Cells[8][i + 2] = DataSetClass.dataSet.Tables["Object_Protection"].Rows[i][7].ToString();
                }
                Excel.Range Style = worksheet2.Range[worksheet2.Cells[1, 1], worksheet2.Cells[1, 8]];
                Style.Font.Bold = true;
                Style.Font.Name = "Times New Roman";
                Style.Font.Size = 14;

                application.Range[worksheet2.Cells[1, 1], worksheet2.Cells[DataSetClass.dataSet.Tables["Object_Protection"].Rows.Count + 2, 8]].Copy();
                worksheet2.Columns.AutoFit();

                application.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка экспорта");
            }
            finally
            {
                ContractInfoFill();
            }
        }
    }
}
