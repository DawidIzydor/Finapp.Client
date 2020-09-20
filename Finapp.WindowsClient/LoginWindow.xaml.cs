using System.Windows;
using Finapp.Connection;
using Finapp.Connection.Exception;

namespace Finapp.WindowsClient
{
    /// <summary>
    ///     Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = UserNameBox.Text;
            var password = PasswordBox.Password;

            try
            {
                await AuthenticationProvider.AuthenticateAsync(username, password);
            }
            catch (CannotLogInException ex)
            {
                MessageBox.Show($"Cannot login: {ex.Response.StatusCode} {ex.Response.ErrorMessage}",
                    "Problem logging in", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            catch (TokenNotReceivedException)
            {
                MessageBox.Show("Something went wrong: token was not saved", "Something went wrong",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            catch (AuthenticationException)
            {
                MessageBox.Show("Something went wrong.", "Something went wrong",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var financialStatusesWindow = new FinancialStatusesWindow();
            financialStatusesWindow.Show();

            Close();
        }
    }
}