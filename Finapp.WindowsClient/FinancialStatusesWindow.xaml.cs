using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Di.Tools.Creator;
using Finapp.Connection;
using Finapp.Dto;
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using RestSharp;

namespace Finapp.WindowsClient
{
    /// <summary>
    ///     Logika interakcji dla klasy FinancialStatusesWindow.xaml
    /// </summary>
    public partial class FinancialStatusesWindow
    {
        private readonly DependencyProperty _seriesDependencyProperty =
            DependencyProperty.Register(nameof(CartesianChart.Series), typeof(SeriesCollection), typeof(CartesianChart),
                new PropertyMetadata((SeriesCollection) null));

        public FinancialStatusesWindow()
        {
            InitializeComponent();
        }

        public List<FinancialStatusExpanded> Statuses { get; private set; }

        public SeriesCollection SeriesCollection { get; private set; }

        public bool ShowTotalAmount { get; set; } = true;
        public bool ShowIncrementalAmount { get; set; } = true;


        public override async void EndInit()
        {
            base.EndInit();

            await ReloadStatusesAsync();
        }

        private async Task ReloadStatusesAsync()
        {
            await GetNewStatusesAsync();

            RecreateSeriesCollection();

            StatusChart.Series = SeriesCollection;
            //var asd = StatusChart.GetBindingExpression(_seriesDependencyProperty);
            //asd?.UpdateTarget();
            UpdateTarget(StatusChart, _seriesDependencyProperty);
            UpdateTarget(FinancialStatusesDataGrid);
        }

        private async Task GetNewStatusesAsync()
        {
            var request = new RestRequest("api/finstatus");
            var response =
                await AuthenticationProvider.AuthClient.ExecuteAsync<IEnumerable<FinancialStatusDto>>(request);

            Statuses = GenerateStatuses(response.Data).ToList();
        }

        private IEnumerable<FinancialStatusExpanded> GenerateStatuses(
            IEnumerable<FinancialStatusDto> financianStatusesDto)
        {
            decimal lastAmount = default;
            foreach (var financialStatusDto in financianStatusesDto)
            {
                var child = financialStatusDto.CreateChild<FinancialStatusExpanded, FinancialStatusDto>();
                if (lastAmount != default)
                {
                    child.Increment = child.Amount - lastAmount;
                }

                lastAmount = child.Amount;

                yield return child;
            }
        }

        private void RecreateSeriesCollection()
        {
            var statuses = Statuses;
            if (statuses != null)
            {
                SeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Amount spend",
                        Values = new ChartValues<decimal>(statuses.Select(s => s.Amount).AsChartValues())
                    },
                    new LineSeries
                    {
                        Title = "Incremental",
                        Values = new ChartValues<decimal>(statuses.Select(s => s.Increment).AsChartValues())
                    }
                };
            }
            else
            {
                SeriesCollection = null;
            }
        }

        private static void UpdateTarget(FrameworkElement frameworkElement)
        {
            UpdateTarget(frameworkElement, ItemsControl.ItemsSourceProperty);
        }

        private static void UpdateTarget(FrameworkElement frameworkElement, DependencyProperty itemsSourceProperty)
        {
            frameworkElement.GetBindingExpression(itemsSourceProperty)?.UpdateTarget();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await ReloadStatusesAsync();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddWindow(async () => await ReloadStatusesAsync());
            addWindow.Show();
        }

        public class FinancialStatusExpanded : FinancialStatusDto
        {
            public decimal Increment { get; set; }
        }
    }
}