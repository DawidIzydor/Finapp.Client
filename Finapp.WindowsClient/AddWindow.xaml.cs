using System;
using System.Windows;
using Finapp.Connection;
using Finapp.Dto;
using RestSharp;

namespace Finapp.WindowsClient
{
    /// <summary>
    ///     Logika interakcji dla klasy AddWindow.xaml
    /// </summary>
    public partial class AddWindow
    {
        private readonly Action _onClosingAction;

        public AddWindow(Action onClosingAction = null)
        {
            _onClosingAction = onClosingAction;
            InitializeComponent();
        }

        public DateTime Date { get; set; } = DateTime.Now;

        protected override void OnClosed(EventArgs e)
        {
            _onClosingAction?.Invoke();
            base.OnClosed(e);
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var date = Date;
            var source = SourceTextBox.Text;
            decimal amount;
            try
            {
                amount = decimal.Parse(AmountTextBox.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Wrong amount!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var insertDto = new FinancialStatusDto
            {
                Amount = amount,
                Date = date,
                Source = source
            };

            var restRequest = new RestRequest("api/finstatus") {Method = Method.POST};
            restRequest.AddJsonBody(insertDto);
            var response = await AuthenticationProvider.AuthClient.ExecuteAsync(restRequest);
            if (response.IsSuccessful == false)
            {
                MessageBox.Show($"Problem adding new status: {response.StatusCode} {response.ErrorMessage}");
                return;
            }

            Close();
        }
    }
}