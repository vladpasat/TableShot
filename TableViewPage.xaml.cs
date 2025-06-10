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
        View CreateCell(string text, bool isHeader = false)
        {
            return new Frame
            {
                Padding = new Thickness(6, 4),
                Margin = new Thickness(2),
                BorderColor = Colors.LightGray,
                BackgroundColor = isHeader ? Colors.LightGray : Colors.Transparent,
                Content = new Label
                {
                    Text = text,
                    FontAttributes = isHeader ? FontAttributes.Bold : FontAttributes.None,
                    LineBreakMode = LineBreakMode.TailTruncation,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                },
                HasShadow = false
            };
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
            TableGrid.Add(CreateCell("Id", isHeader: true), 0, 0);
            TableGrid.Add(CreateCell("Name", isHeader: true), 1, 0);
            for (int c = 0; c < _columnNames.Count; c++)
            {
                TableGrid.Add(CreateCell(_columnNames[c], isHeader: true), 2 + c, 0);
            }
            TableGrid.Add(CreateCell("Final Score", isHeader: true), 2 + _columnNames.Count, 0);
        }

        void RenderRow(int index, TableRow row)
        {
            int gridRow = index + 1;
            TableGrid.RowDefinitions.Add(new RowDefinition());

            TableGrid.Add(CreateCell(row.RowId), 0, gridRow);
            TableGrid.Add(CreateCell(row.Name), 1, gridRow);

            for (int c = 0; c < _columnNames.Count; c++)
            {
                var key = _columnNames[c];
                var value = row.Values?.GetValueOrDefault(key) ?? 0d;
                TableGrid.Add(CreateCell(value.ToString()), 2 + c, gridRow);
            }

            // Final Score = sum of all dynamic values
            double final = row.Values?.Values.Sum() ?? 0d;
            TableGrid.Add(CreateCell(final.ToString()), 2 + _columnNames.Count, gridRow);
        }
    }
}
