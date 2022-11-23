using System;
using System.Windows;
using System.Net;
using System.Net.Mail;


namespace DataSet_WPF_DB_App
{
    /// <summary>
    /// Логика взаимодействия для TwoFactorAutorization.xaml
    /// </summary>
    public partial class TwoFactorAutorization : Window
    {
        Random rnd = new Random();
        int secret_code = 0;

        public TwoFactorAutorization()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Событие загрузки окна TwoFactorAutorization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            secret_code = rnd.Next(11111, 999999);

            lbInfo.Content += App.Email;           
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("vladislavgnusarov@gmail.com", "AutorizationCode@gmail.com");
            // кому отправляем
            MailAddress to = new MailAddress(App.Email);
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = "Код подтверждения:";
            // текст письма
            m.Body = "Код подтверждения: " + secret_code.ToString() + "";
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("vladislavgnusarov@gmail.com", "vladgva2003");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }

        /// <summary>
        /// Вход в личный кабинет
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            if (tbCode.Text == secret_code.ToString())
            {
                switch (App.User_Role)
                {
                    case "Администратор":
                        Close();
                        AdminWindow adminWindow = new AdminWindow();
                        adminWindow.Show();
                        break;
                    case "Заказчик":
                        CustomerWindow customerWindow = new CustomerWindow();
                        customerWindow.Show();
                        break;
                }
            }
            else
            {
                MessageBox.Show("Код неверн!", "Подтверждение аккаунта");
            }
        }

        /// <summary>
        /// Выход из окна TwoFactorAutorization
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Событие закрытия окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                //Если окно не активно
                if (!window.IsActive)
                    //Показать данное окно
                    window.Show();
            }
        }
    }
}
