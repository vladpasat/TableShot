using TableShot.Models;
using TableShot.ViewModels;

namespace TableShot
{

    [QueryProperty(nameof(TableId), "tableId")]
    public partial class TableViewPage : ContentPage
    {
        readonly TableItemsViewModel _vm = App.TableVm;
        string _tableId;
        List<string> _columnNames;
        int _additionalCols;
        List<TableRow> _rows;

        public string TableId
        {
            get => _tableId;
            set => _tableId = Uri.UnescapeDataString(value);
        }

        public TableViewPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (string.IsNullOrEmpty(_tableId))
                return;

            // 1) Load schema + data
            var def = await _vm.GetTableDefinitionAsync(_tableId);
            _additionalCols = def.AdditionalColumns;
            _columnNames = def.ColumnNames;

            _rows = await _vm.GetRowsForTableAsync(_tableId);

            // 2) Render header + rows
            RenderHeader();
            for (int i = 0; i < _rows.Count; i++)
                RenderRow(i, _rows[i]);
        }

        void RenderHeader()
        {
            TableGrid.Children.Clear();
            TableGrid.RowDefinitions.Clear();
            TableGrid.ColumnDefinitions.Clear();

            // single header row
            TableGrid.RowDefinitions.Add(new RowDefinition());

            // Columns: Id | Name | C1…CN | Final Score
            TableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            TableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            foreach (var _ in _columnNames)
                TableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            TableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Add header labels
            TableGrid.Add(new Label { Text = "Id", FontAttributes = FontAttributes.Bold }, 0, 0);
            TableGrid.Add(new Label { Text = "Name", FontAttributes = FontAttributes.Bold }, 1, 0);
            for (int c = 0; c < _columnNames.Count; c++)
            {
                TableGrid.Add(
                    new Label { Text = _columnNames[c], FontAttributes = FontAttributes.Bold },
                    2 + c, 0);
            }
            TableGrid.Add(
                new Label { Text = "Final Score", FontAttributes = FontAttributes.Bold },
                2 + _columnNames.Count, 0);
        }

        void RenderRow(int index, TableRow row)
        {
            int gridRow = index + 1;
            TableGrid.RowDefinitions.Add(new RowDefinition());

            // Id
            TableGrid.Add(new Label { Text = row.RowId }, 0, gridRow);

            // Name
            TableGrid.Add(new Label { Text = row.Name }, 1, gridRow);

            // Dynamic columns
            for (int c = 0; c < _columnNames.Count; c++)
            {
                var key = _columnNames[c];
                var value = row.Values?.GetValueOrDefault(key) ?? 0d;
                TableGrid.Add(new Label { Text = value.ToString() }, 2 + c, gridRow);
            }

            // Final Score = sum of all dynamic values
            double final = row.Values?.Values.Sum() ?? 0d;
            TableGrid.Add(
                new Label { Text = final.ToString() },
                2 + _columnNames.Count,
                gridRow);
        }
    }
}
