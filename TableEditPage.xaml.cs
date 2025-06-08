namespace TableShot;
using TableShot.Models;
using TableShot.ViewModels;
[QueryProperty(nameof(TableId), "tableId")]
public partial class TableEditPage : ContentPage
{
    readonly TableItemsViewModel _vm;
    string _tableId;
    List<string> _columnNames;
    int _additionalCols;
    List<TableRow> _rows;

    public string TableId
    {
        get => _tableId;
        set => _tableId = Uri.UnescapeDataString(value);
    }

    public TableEditPage()
    {
        InitializeComponent();
        _vm = new TableItemsViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (string.IsNullOrEmpty(_tableId))
            return;

        // load schema
        var def = await _vm.GetTableDefinitionAsync(_tableId);
        _additionalCols = def.AdditionalColumns;
        _columnNames = def.ColumnNames;

        // load existing rows
        _rows = await _vm.GetRowsForTableAsync(_tableId);

        // render header + existing rows
        RenderHeader();
        for (int i = 0; i < _rows.Count; i++)
            RenderRow(i, _rows[i]);

        // show AddRow button for admins
        AddRowButton.IsVisible = App.UserGroups?.Contains("admin") == true;
    }

    void RenderHeader()
    {
        TableGrid.Children.Clear();
        TableGrid.RowDefinitions.Clear();
        TableGrid.ColumnDefinitions.Clear();

        // single header row
        TableGrid.RowDefinitions.Add(new RowDefinition());

        // columns: Id | Name | C1…CN | Final Score
        TableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        TableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        foreach (var _ in _columnNames)
            TableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        TableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        // add header cells
        TableGrid.Add(new Label { Text = "Id", FontAttributes = FontAttributes.Bold }, 0, 0);
        TableGrid.Add(new Label { Text = "Name", FontAttributes = FontAttributes.Bold }, 1, 0);
        for (int c = 0; c < _columnNames.Count; c++)
            TableGrid.Add(new Label { Text = _columnNames[c], FontAttributes = FontAttributes.Bold }, 2 + c, 0);
        TableGrid.Add(new Label { Text = "Final Score", FontAttributes = FontAttributes.Bold }, 2 + _columnNames.Count, 0);
    }

    private int _nextRowIndex => _rows?.Count ?? 0;

    private async void OnAddRowClicked(object sender, EventArgs e)
    {
        // ask for the row’s Name
        var name = await DisplayPromptAsync(
            "New Row", "Enter Name:",
            accept: "OK", cancel: "Cancel",
            placeholder: "e.g. Alice");

        if (string.IsNullOrWhiteSpace(name))
            return;

        // prepare blank values
        var values = _columnNames.ToDictionary(c => c, _ => 0d);

        // create the row object
        var row = new TableRow
        {
            TableId = _tableId,
            RowId = $"R{_nextRowIndex}",
            Name = name,
            Values = values
        };

        // persist in DynamoDB
        await _vm.AddRowAsync(row);

        // add to local list and render
        _rows.Add(row);
        RenderRow(_rows.Count - 1, row);
    }

    void RenderRow(int index, TableRow row)
    {
        int gridRow = index + 1;
        TableGrid.RowDefinitions.Add(new RowDefinition());

        // Id label
        TableGrid.Add(new Label { Text = row.RowId }, 0, gridRow);

        // Name entry
        var nameEntry = new Entry { Text = row.Name };
        nameEntry.TextChanged += async (_, args) =>
        {
            row.Name = args.NewTextValue;
            await _vm.UpdateRowAsync(row);
        };
        TableGrid.Add(nameEntry, 1, gridRow);

        // dynamic column entries
        for (int c = 0; c < _columnNames.Count; c++)
        {
            string key = _columnNames[c];
            double initial = row.Values?.GetValueOrDefault(key) ?? 0d;

            var entry = new Entry
            {
                Keyboard = Keyboard.Numeric,
                Text = initial.ToString()
            };
            entry.TextChanged += async (_, args) =>
            {
                if (double.TryParse(args.NewTextValue, out var v))
                {
                    row.Values[key] = v;
                    await _vm.UpdateRowAsync(row);

                    // update Final Score cell
                    var fsLabel = TableGrid.Children
                        .OfType<Label>()
                        .First(l =>
                            Grid.GetRow(l) == gridRow &&
                            Grid.GetColumn(l) == 2 + _columnNames.Count);
                    fsLabel.Text = row.Values.Values.Sum().ToString();
                }
            };
            TableGrid.Add(entry, 2 + c, gridRow);
        }

        // Final Score label
        var sum = row.Values.Values.Sum();
        TableGrid.Add(new Label { Text = sum.ToString() }, 2 + _columnNames.Count, gridRow);
    }
}