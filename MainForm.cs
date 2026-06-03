using BorisTeka.Database;
using BorisTeka.Models;
using BorisTeka.Services;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BorisTeka
{
    public partial class MainForm : Form
    {
        private readonly LibraryService _libraryService = new LibraryService();

        private AppUser _currentUser;
        private ComboBox _borrowSearchComboBox;
        private ComboBox _deleteUserComboBox;

        private Panel _mainPanel;

        private TextBox _loginTextBox;
        private TextBox _passwordTextBox;

        private DataGridView _booksGrid;
        private DataGridView _borrowedGrid;

        private TextBox _titleTextBox;
        private TextBox _authorTextBox;
        private NumericUpDown _countNumeric;

        private ComboBox _usersComboBox;
        private DateTimePicker _returnDatePicker;
        private TextBox _newUserNameTextBox;
        private TextBox _newUserLoginTextBox;
        private TextBox _newUserPasswordTextBox;

        public MainForm()
        {
            InitializeComponent();

            DatabaseHelper.InitializeDatabase();

            ConfigureWindow();
            ShowLoginScreen();
        }

        private void BorrowedGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridView grid = sender as DataGridView;

            if (grid == null)
            {
                return;
            }

            BorrowedBook borrowedBook = grid.Rows[e.RowIndex].DataBoundItem as BorrowedBook;

            if (borrowedBook == null)
            {
                return;
            }

            if (borrowedBook.IsOverdue)
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230);
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(140, 0, 0);
            }
            else
            {
                grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
            }
        }

        private void ConfigureWindow()
        {
            Text = "BorisTeka";
            ClientSize = new Size(1100, 820);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1100, 820);
            BackColor = Color.FromArgb(245, 246, 250);
        }

        private void ClearWindow()
        {
            AcceptButton = null;

            Controls.Clear();

            _mainPanel = new Panel();
            _mainPanel.Dock = DockStyle.Fill;
            _mainPanel.AutoScroll = true;
            _mainPanel.BackColor = Color.FromArgb(245, 246, 250);

            Controls.Add(_mainPanel);
        }

        private Label CreateLabel(string text, int x, int y, int width = 200, int height = 25, int fontSize = 10)
        {
            Label label = new Label();
            label.Text = text;
            label.Left = x;
            label.Top = y;
            label.Width = width;
            label.Height = height;
            label.Font = new Font("Segoe UI", fontSize, FontStyle.Regular);
            label.ForeColor = Color.FromArgb(40, 40, 40);

            return label;
        }

        private Button CreateButton(string text, int x, int y, int width = 160, int height = 40)
        {
            Button button = new Button();
            button.Text = text;
            button.Left = x;
            button.Top = y;
            button.Width = width;
            button.Height = height;
            button.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            button.BackColor = Color.FromArgb(70, 130, 180);
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;

            return button;
        }

        private TextBox CreateTextBox(int x, int y, int width = 220)
        {
            TextBox textBox = new TextBox();
            textBox.Left = x;
            textBox.Top = y;
            textBox.Width = width;
            textBox.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            return textBox;
        }

        private DataGridView CreateGrid(int x, int y, int width, int height)
        {
            DataGridView grid = new DataGridView();
            grid.Left = x;
            grid.Top = y;
            grid.Width = width;
            grid.Height = height;

            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AutoGenerateColumns = false;

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.ScrollBars = ScrollBars.Both;

            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.RowHeadersVisible = false;

            grid.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            grid.ColumnHeadersHeight = 35;
            grid.RowTemplate.Height = 30;

            return grid;
        }

        private void ShowLoginScreen()
        {
            ConfigureWindow();
            ClearWindow();

            Label titleLabel = CreateLabel("BorisTeka", 0, 60, 1100, 50, 24);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(30, 80, 130);
            _mainPanel.Controls.Add(titleLabel);

            Label subtitleLabel = CreateLabel("Вход в библиотечную систему", 0, 115, 1100, 30, 12);
            subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            _mainPanel.Controls.Add(subtitleLabel);

            Panel loginPanel = new Panel();
            loginPanel.Width = 380;
            loginPanel.Height = 260;
            loginPanel.Left = (ClientSize.Width - loginPanel.Width) / 2;
            loginPanel.Top = 180;
            loginPanel.BackColor = Color.White;
            loginPanel.BorderStyle = BorderStyle.FixedSingle;
            loginPanel.Anchor = AnchorStyles.Top;
            _mainPanel.Controls.Add(loginPanel);

            loginPanel.Controls.Add(CreateLabel("Логин:", 40, 35, 300));
            _loginTextBox = CreateTextBox(40, 65, 290);
            loginPanel.Controls.Add(_loginTextBox);

            loginPanel.Controls.Add(CreateLabel("Пароль:", 40, 105, 300));
            _passwordTextBox = CreateTextBox(40, 135, 290);
            _passwordTextBox.PasswordChar = '*';
            loginPanel.Controls.Add(_passwordTextBox);

            Button loginButton = CreateButton("Войти", 40, 185, 290, 40);
            loginButton.Click += LoginButton_Click;
            loginPanel.Controls.Add(loginButton);

            AcceptButton = loginButton;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string login = _loginTextBox.Text.Trim();
            string password = _passwordTextBox.Text.Trim();

            if (login.Length == 0 || password.Length == 0)
            {
                MessageBox.Show("Введите логин и пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AppUser user = _libraryService.Login(login, password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _currentUser = user;

            if (_currentUser.Role == "Librarian")
            {
                ShowLibrarianScreen();
            }
            else
            {
                ShowUserScreen();
            }
        }

        private void ShowLibrarianScreen()
        {
            ConfigureWindow();
            ClearWindow();

            Label titleLabel = CreateLabel("Панель библиотекаря", 20, 15, 400, 40, 18);
            titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            _mainPanel.Controls.Add(titleLabel);

            Label userLabel = CreateLabel("Вы вошли как: " + _currentUser.FullName, 20, 55, 500, 25, 10);
            _mainPanel.Controls.Add(userLabel);

            Button logoutButton = CreateButton("Выйти", 900, 25, 140, 35);
            logoutButton.BackColor = Color.FromArgb(180, 70, 70);
            logoutButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            logoutButton.Click += LogoutButton_Click;
            _mainPanel.Controls.Add(logoutButton);

            CreateBookEditorBlock();
            CreateLibrarianBooksGrid();
            CreateBorrowBlock();
            CreateUserBlock();
            CreateAllBorrowedGrid();

            RefreshLibrarianData();
        }

        private void CreateUserBlock()
        {
            GroupBox groupBox = new GroupBox();
            groupBox.Text = "Пользователи";
            groupBox.Left = 20;
            groupBox.Top = 560;
            groupBox.Width = 320;
            groupBox.Height = 260;
            groupBox.Font = new Font("Segoe UI", 10);

            _mainPanel.Controls.Add(groupBox);

            groupBox.Controls.Add(CreateLabel("ФИО:", 15, 25, 280));
            _newUserNameTextBox = CreateTextBox(15, 50, 280);
            groupBox.Controls.Add(_newUserNameTextBox);

            groupBox.Controls.Add(CreateLabel("Логин:", 15, 80, 280));
            _newUserLoginTextBox = CreateTextBox(15, 105, 280);
            groupBox.Controls.Add(_newUserLoginTextBox);

            groupBox.Controls.Add(CreateLabel("Пароль:", 15, 135, 280));
            _newUserPasswordTextBox = CreateTextBox(15, 160, 180);
            groupBox.Controls.Add(_newUserPasswordTextBox);

            Button createButton = CreateButton("Создать", 205, 157, 90, 35);
            createButton.Click += CreateUserButton_Click;
            groupBox.Controls.Add(createButton);

            groupBox.Controls.Add(CreateLabel("Удалить пользователя:", 15, 195, 280));

            _deleteUserComboBox = new ComboBox();
            _deleteUserComboBox.Left = 15;
            _deleteUserComboBox.Top = 220;
            _deleteUserComboBox.Width = 180;
            _deleteUserComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _deleteUserComboBox.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            groupBox.Controls.Add(_deleteUserComboBox);

            Button deleteButton = CreateButton("Удалить", 205, 217, 90, 35);
            deleteButton.BackColor = Color.FromArgb(180, 70, 70);
            deleteButton.Click += DeleteUserButton_Click;
            groupBox.Controls.Add(deleteButton);
        }

        private void DeleteUserButton_Click(object sender, EventArgs e)
        {
            AppUser selectedUser = _deleteUserComboBox.SelectedItem as AppUser;

            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Удалить пользователя и все его записи о выданных книгах?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _libraryService.DeleteUser(selectedUser.Id);

                RefreshLibrarianData();

                MessageBox.Show(
                    "Пользователь удалён.",
                    "Готово",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CreateUserButton_Click(object sender, EventArgs e)
        {
            string fullName = _newUserNameTextBox.Text.Trim();
            string login = _newUserLoginTextBox.Text.Trim();
            string password = _newUserPasswordTextBox.Text.Trim();

            if (fullName.Length == 0 ||
                login.Length == 0 ||
                password.Length == 0)
            {
                MessageBox.Show(
                    "Заполните все поля.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                _libraryService.AddUser(
                    fullName,
                    login,
                    password);

                _newUserNameTextBox.Clear();
                _newUserLoginTextBox.Clear();
                _newUserPasswordTextBox.Clear();

                RefreshLibrarianData();

                MessageBox.Show(
                    "Пользователь создан.",
                    "Готово",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CreateBookEditorBlock()
        {
            GroupBox groupBox = new GroupBox();
            groupBox.Text = "Управление книгами";
            groupBox.Left = 20;
            groupBox.Top = 95;
            groupBox.Width = 320;
            groupBox.Height = 250;
            groupBox.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            _mainPanel.Controls.Add(groupBox);

            groupBox.Controls.Add(CreateLabel("Название:", 15, 30, 280));
            _titleTextBox = CreateTextBox(15, 60, 280);
            groupBox.Controls.Add(_titleTextBox);

            groupBox.Controls.Add(CreateLabel("Автор:", 15, 95, 280));
            _authorTextBox = CreateTextBox(15, 125, 280);
            groupBox.Controls.Add(_authorTextBox);

            groupBox.Controls.Add(CreateLabel("Количество в наличии:", 15, 160, 280));
            _countNumeric = new NumericUpDown();
            _countNumeric.Left = 15;
            _countNumeric.Top = 190;
            _countNumeric.Width = 120;
            _countNumeric.Minimum = 0;
            _countNumeric.Maximum = 10000;
            _countNumeric.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            groupBox.Controls.Add(_countNumeric);

            Button addButton = CreateButton("Добавить", 150, 185, 145, 35);
            addButton.Click += AddBookButton_Click;
            groupBox.Controls.Add(addButton);

            Button updateButton = CreateButton("Изменить", 15, 225, 135, 35);
            updateButton.Top = 210;
            updateButton.Visible = false;
        }

        private void CreateLibrarianBooksGrid()
        {
            Label booksLabel = CreateLabel("Список книг", 370, 95, 300, 25, 12);
            booksLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            _mainPanel.Controls.Add(booksLabel);

            _booksGrid = CreateGrid(370, 125, 690, 260);
            _booksGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "ID",
                DataPropertyName = "Id",
                FillWeight = 45,
                MinimumWidth = 45
            });

            _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Название",
                DataPropertyName = "Title",
                FillWeight = 260,
                MinimumWidth = 180
            });

            _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Автор",
                DataPropertyName = "Author",
                FillWeight = 210,
                MinimumWidth = 150
            });

            _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Дата добавления",
                DataPropertyName = "DateAdded",
                FillWeight = 130,
                MinimumWidth = 120,
                DefaultCellStyle = { Format = "dd.MM.yyyy" }
            });

            _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "В наличии",
                DataPropertyName = "AvailableCount",
                FillWeight = 90,
                MinimumWidth = 85
            });

            _booksGrid.SelectionChanged += BooksGrid_SelectionChanged;

            _mainPanel.Controls.Add(_booksGrid);

            Button updateBookButton = CreateButton("Сохранить изменения", 370, 395, 190, 35);
            updateBookButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            updateBookButton.Click += UpdateBookButton_Click;
            _mainPanel.Controls.Add(updateBookButton);

            Button deleteBookButton = CreateButton("Удалить книгу", 580, 395, 160, 35);
            deleteBookButton.BackColor = Color.FromArgb(180, 70, 70);
            deleteBookButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            deleteBookButton.Click += DeleteBookButton_Click;
            _mainPanel.Controls.Add(deleteBookButton);

            Button clearFieldsButton = CreateButton("Очистить поля", 760, 395, 160, 35);
            clearFieldsButton.BackColor = Color.FromArgb(100, 100, 100);
            clearFieldsButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            clearFieldsButton.Click += ClearBookFieldsButton_Click;
            _mainPanel.Controls.Add(clearFieldsButton);
        }

        private void CreateBorrowBlock()
        {
            GroupBox groupBox = new GroupBox();
            groupBox.Text = "Выдача книги пользователю";
            groupBox.Left = 20;
            groupBox.Top = 360;
            groupBox.Width = 320;
            groupBox.Height = 190;
            groupBox.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            _mainPanel.Controls.Add(groupBox);

            groupBox.Controls.Add(CreateLabel("Пользователь:", 15, 30, 280));

            _usersComboBox = new ComboBox();
            _usersComboBox.Left = 15;
            _usersComboBox.Top = 60;
            _usersComboBox.Width = 280;
            _usersComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _usersComboBox.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            groupBox.Controls.Add(_usersComboBox);

            groupBox.Controls.Add(CreateLabel("Дата возврата:", 15, 95, 280));

            _returnDatePicker = new DateTimePicker();
            _returnDatePicker.Left = 15;
            _returnDatePicker.Top = 125;
            _returnDatePicker.Width = 280;
            _returnDatePicker.Format = DateTimePickerFormat.Short;
            _returnDatePicker.Value = DateTime.Now.AddDays(14);
            _returnDatePicker.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            groupBox.Controls.Add(_returnDatePicker);

            Button borrowButton = CreateButton("Выдать выбранную книгу", 15, 155, 280, 35);
            borrowButton.Top = 150;
            borrowButton.Click += BorrowBookButton_Click;
            groupBox.Controls.Add(borrowButton);
        }

        private void CreateAllBorrowedGrid()
        {
            Label borrowedLabel = CreateLabel("Выданные книги", 370, 445, 150, 25, 12);
            borrowedLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            _mainPanel.Controls.Add(borrowedLabel);

            _borrowSearchComboBox = new ComboBox();
            _borrowSearchComboBox.Left = 520;
            _borrowSearchComboBox.Top = 442;
            _borrowSearchComboBox.Width = 200;
            _borrowSearchComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            _borrowSearchComboBox.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            _mainPanel.Controls.Add(_borrowSearchComboBox);

            Button searchButton = CreateButton("Найти", 735, 438, 90, 35);
            searchButton.Click += SearchBorrowedByUserButton_Click;
            _mainPanel.Controls.Add(searchButton);

            Button showAllButton = CreateButton("Показать все", 835, 438, 130, 35);
            showAllButton.BackColor = Color.FromArgb(100, 100, 100);
            showAllButton.Click += ShowAllBorrowedButton_Click;
            _mainPanel.Controls.Add(showAllButton);

            Button exportButton = CreateButton("PDF", 980, 438, 80, 35);
            exportButton.BackColor = Color.FromArgb(80, 140, 90);
            exportButton.Click += ExportBorrowedPdfButton_Click;
            _mainPanel.Controls.Add(exportButton);

            _borrowedGrid = CreateGrid(370, 485, 690, 180);
            _borrowedGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            ConfigureBorrowedGridColumns(_borrowedGrid, true);

            _mainPanel.Controls.Add(_borrowedGrid);

            Button returnButton = CreateButton("Принять возврат", 370, 675, 170, 35);
            returnButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            returnButton.Click += ReturnBookButton_Click;
            _mainPanel.Controls.Add(returnButton);
        }

        private void SearchBorrowedByUserButton_Click(object sender, EventArgs e)
        {
            AppUser selectedUser = _borrowSearchComboBox.SelectedItem as AppUser;

            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _borrowedGrid.DataSource = null;
            _borrowedGrid.DataSource = _libraryService.GetBorrowedBooksForUser(selectedUser.Id);
        }



        private void ShowAllBorrowedButton_Click(object sender, EventArgs e)
        {
            _borrowedGrid.DataSource = null;
            _borrowedGrid.DataSource = _libraryService.GetAllBorrowedBooks();
        }

        private void ExportBorrowedPdfButton_Click(object sender, EventArgs e)
        {
            if (_borrowedGrid == null || _borrowedGrid.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Нет данных для экспорта.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            ExportGridToPdf(_borrowedGrid, "Выданные книги");
        }

        private void ExportGridToPdf(DataGridView grid, string title)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PDF файл (*.pdf)|*.pdf";
                saveFileDialog.FileName = title + ".pdf";

                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                PdfDocument document = new PdfDocument();
                document.Info.Title = title;

                PdfPage page = document.AddPage();
                page.Orientation = PdfSharp.PageOrientation.Landscape;

                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont titleFont = new XFont("Arial", 16, XFontStyleEx.Bold);
                XFont headerFont = new XFont("Arial", 9, XFontStyleEx.Bold);
                XFont cellFont = new XFont("Arial", 9, XFontStyleEx.Regular);

                double margin = 35;
                double y = margin;
                double rowHeight = 30;

                gfx.DrawString(
                    title,
                    titleFont,
                    XBrushes.Black,
                    new XRect(margin, y, page.Width.Point - margin * 2, 30),
                    XStringFormats.TopLeft);

                y += 40;

                List<DataGridViewColumn> columns = grid.Columns
                    .Cast<DataGridViewColumn>()
                    .Where(column => column.Visible)
                    .ToList();

                int totalGridWidth = columns.Sum(column => column.Width);

                double[] columnWidths = columns
                    .Select(column => (page.Width.Point - margin * 2) * column.Width / totalGridWidth)
                    .ToArray();

                DrawPdfHeader(gfx, columns, columnWidths, margin, ref y, rowHeight, headerFont);

                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow)
                    {
                        continue;
                    }

                    if (y + rowHeight > page.Height.Point - margin)
                    {
                        page = document.AddPage();
                        page.Orientation = PdfSharp.PageOrientation.Landscape;

                        gfx = XGraphics.FromPdfPage(page);

                        y = margin;

                        gfx.DrawString(
                            title,
                            titleFont,
                            XBrushes.Black,
                            new XRect(margin, y, page.Width.Point - margin * 2, 30),
                            XStringFormats.TopLeft);

                        y += 40;

                        DrawPdfHeader(gfx, columns, columnWidths, margin, ref y, rowHeight, headerFont);
                    }

                    double x = margin;

                    for (int i = 0; i < columns.Count; i++)
                    {
                        DataGridViewColumn column = columns[i];

                        string text = "";

                        if (row.Cells[column.Index].FormattedValue != null)
                        {
                            text = row.Cells[column.Index].FormattedValue.ToString();
                        }

                        gfx.DrawRectangle(XPens.LightGray, x, y, columnWidths[i], rowHeight);

                        XTextFormatter formatter = new XTextFormatter(gfx);
                        formatter.DrawString(
                            text,
                            cellFont,
                            XBrushes.Black,
                            new XRect(x + 4, y + 5, columnWidths[i] - 8, rowHeight - 5));

                        x += columnWidths[i];
                    }

                    y += rowHeight;
                }

                document.Save(saveFileDialog.FileName);
                document.Close();

                MessageBox.Show(
                    "PDF сохранён.",
                    "Готово",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Process.Start(new ProcessStartInfo
                {
                    FileName = saveFileDialog.FileName,
                    UseShellExecute = true
                });
            }
        }

        private void DrawPdfHeader(
            XGraphics gfx,
            List<DataGridViewColumn> columns,
            double[] columnWidths,
            double margin,
            ref double y,
            double rowHeight,
            XFont headerFont)
        {
            double x = margin;

            for (int i = 0; i < columns.Count; i++)
            {
                gfx.DrawRectangle(XBrushes.LightGray, x, y, columnWidths[i], rowHeight);
                gfx.DrawRectangle(XPens.Gray, x, y, columnWidths[i], rowHeight);

                XTextFormatter formatter = new XTextFormatter(gfx);
                formatter.DrawString(
                    columns[i].HeaderText,
                    headerFont,
                    XBrushes.Black,
                    new XRect(x + 4, y + 5, columnWidths[i] - 8, rowHeight - 5));

                x += columnWidths[i];
            }

            y += rowHeight;
        }

        private void ShowUserScreen()
        {
            ConfigureWindow();
            ClearWindow();

            Label titleLabel = CreateLabel("Панель пользователя", 20, 15, 400, 40, 18);
            titleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            _mainPanel.Controls.Add(titleLabel);

            Label userLabel = CreateLabel("Вы вошли как: " + _currentUser.FullName, 20, 55, 500, 25, 10);
            _mainPanel.Controls.Add(userLabel);

            Button logoutButton = CreateButton("Выйти", 900, 25, 140, 35);
            logoutButton.BackColor = Color.FromArgb(180, 70, 70);
            logoutButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            logoutButton.Click += LogoutButton_Click;
            _mainPanel.Controls.Add(logoutButton);

            Label booksLabel = CreateLabel("Книги в наличии", 20, 100, 300, 25, 12);
            booksLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            _mainPanel.Controls.Add(booksLabel);

            _booksGrid = CreateGrid(20, 130, 1040, 240);
            _booksGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Название",
                DataPropertyName = "Title",
                FillWeight = 300,
                MinimumWidth = 200
            });

            _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Автор",
                DataPropertyName = "Author",
                FillWeight = 250,
                MinimumWidth = 160
            });

            _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Дата добавления",
                DataPropertyName = "DateAdded",
                FillWeight = 150,
                MinimumWidth = 130,
                DefaultCellStyle = { Format = "dd.MM.yyyy" }
            });

            _booksGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "В наличии",
                DataPropertyName = "AvailableCount",
                FillWeight = 100,
                MinimumWidth = 90
            });
            _mainPanel.Controls.Add(_booksGrid);

            Label borrowedLabel = CreateLabel("Мои взятые книги и даты возврата", 20, 400, 400, 25, 12);
            borrowedLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            _mainPanel.Controls.Add(borrowedLabel);

            _borrowedGrid = CreateGrid(20, 430, 1040, 180);
            _borrowedGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            ConfigureBorrowedGridColumns(_borrowedGrid, false);

            _mainPanel.Controls.Add(_borrowedGrid);

            RefreshUserData();
        }

        private void ConfigureBorrowedGridColumns(DataGridView grid, bool showUserName)
        {
            if (showUserName)
            {
                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Пользователь",
                    DataPropertyName = "UserFullName",
                    FillWeight = 170,
                    MinimumWidth = 140
                });
            }

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Книга",
                DataPropertyName = "BookTitle",
                FillWeight = 220,
                MinimumWidth = 160
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Автор",
                DataPropertyName = "Author",
                FillWeight = 170,
                MinimumWidth = 140
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Дата выдачи",
                DataPropertyName = "BorrowDate",
                FillWeight = 120,
                MinimumWidth = 110,
                DefaultCellStyle = { Format = "dd.MM.yyyy" }
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Вернуть до",
                DataPropertyName = "ReturnDate",
                FillWeight = 120,
                MinimumWidth = 110,
                DefaultCellStyle = { Format = "dd.MM.yyyy" }
            });

            grid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Статус",
                DataPropertyName = "Status",
                FillWeight = 130,
                MinimumWidth = 120
            });

            grid.CellFormatting += BorrowedGrid_CellFormatting;
        }

        private void RefreshLibrarianData()
        {
            _booksGrid.DataSource = null;
            _booksGrid.DataSource = _libraryService.GetBooks();

            _borrowedGrid.DataSource = null;
            _borrowedGrid.DataSource = _libraryService.GetAllBorrowedBooks();

            List<AppUser> users = _libraryService.GetUsersByRole("User");

            _usersComboBox.DataSource = null;
            _usersComboBox.DataSource = users;
            _usersComboBox.DisplayMember = "FullName";
            _usersComboBox.ValueMember = "Id";

            if (_borrowSearchComboBox != null)
            {
                _borrowSearchComboBox.DataSource = null;
                _borrowSearchComboBox.DataSource = new List<AppUser>(users);
                _borrowSearchComboBox.DisplayMember = "FullName";
                _borrowSearchComboBox.ValueMember = "Id";
            }

            if (_deleteUserComboBox != null)
            {
                _deleteUserComboBox.DataSource = null;
                _deleteUserComboBox.DataSource = new List<AppUser>(users);
                _deleteUserComboBox.DisplayMember = "FullName";
                _deleteUserComboBox.ValueMember = "Id";
            }
        }

        private void RefreshUserData()
        {
            _booksGrid.DataSource = null;
            _booksGrid.DataSource = _libraryService.GetAvailableBooks();

            _borrowedGrid.DataSource = null;
            _borrowedGrid.DataSource = _libraryService.GetBorrowedBooksForUser(_currentUser.Id);
        }

        private Book GetSelectedBook()
        {
            if (_booksGrid == null || _booksGrid.CurrentRow == null)
            {
                return null;
            }

            return _booksGrid.CurrentRow.DataBoundItem as Book;
        }

        private BorrowedBook GetSelectedBorrowedBook()
        {
            if (_borrowedGrid == null || _borrowedGrid.CurrentRow == null)
            {
                return null;
            }

            return _borrowedGrid.CurrentRow.DataBoundItem as BorrowedBook;
        }

        private void BooksGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (_currentUser == null || _currentUser.Role != "Librarian")
            {
                return;
            }

            Book selectedBook = GetSelectedBook();

            if (selectedBook == null)
            {
                return;
            }

            _titleTextBox.Text = selectedBook.Title;
            _authorTextBox.Text = selectedBook.Author;
            _countNumeric.Value = selectedBook.AvailableCount;
        }

        private void AddBookButton_Click(object sender, EventArgs e)
        {
            string title = _titleTextBox.Text.Trim();
            string author = _authorTextBox.Text.Trim();
            int count = (int)_countNumeric.Value;

            if (title.Length == 0 || author.Length == 0)
            {
                MessageBox.Show("Введите название и автора книги.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _libraryService.AddBook(title, author, count);

            ClearBookFields();
            RefreshLibrarianData();

            MessageBox.Show("Книга добавлена.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateBookButton_Click(object sender, EventArgs e)
        {
            Book selectedBook = GetSelectedBook();

            if (selectedBook == null)
            {
                MessageBox.Show("Выберите книгу для изменения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string title = _titleTextBox.Text.Trim();
            string author = _authorTextBox.Text.Trim();
            int count = (int)_countNumeric.Value;

            if (title.Length == 0 || author.Length == 0)
            {
                MessageBox.Show("Введите название и автора книги.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _libraryService.UpdateBook(selectedBook.Id, title, author, count);

            RefreshLibrarianData();

            MessageBox.Show("Книга изменена.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DeleteBookButton_Click(object sender, EventArgs e)
        {
            Book selectedBook = GetSelectedBook();

            if (selectedBook == null)
            {
                MessageBox.Show("Выберите книгу для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Удалить выбранную книгу?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                _libraryService.DeleteBook(selectedBook.Id);

                ClearBookFields();
                RefreshLibrarianData();

                MessageBox.Show("Книга удалена.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show(
                    "Нельзя удалить книгу, если она сейчас выдана пользователю.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BorrowBookButton_Click(object sender, EventArgs e)
        {
            Book selectedBook = GetSelectedBook();

            if (selectedBook == null)
            {
                MessageBox.Show("Выберите книгу для выдачи.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AppUser selectedUser = _usersComboBox.SelectedItem as AppUser;

            if (selectedUser == null)
            {
                MessageBox.Show("Выберите пользователя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_returnDatePicker.Value.Date <= DateTime.Now.Date)
            {
                MessageBox.Show("Дата возврата должна быть позже сегодняшней даты.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _libraryService.BorrowBook(selectedUser.Id, selectedBook.Id, _returnDatePicker.Value.Date);

                RefreshLibrarianData();

                MessageBox.Show("Книга выдана пользователю.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReturnBookButton_Click(object sender, EventArgs e)
        {
            BorrowedBook selectedBorrowedBook = GetSelectedBorrowedBook();

            if (selectedBorrowedBook == null)
            {
                MessageBox.Show("Выберите запись о выдаче книги.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Принять возврат выбранной книги?",
                "Подтверждение",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            _libraryService.ReturnBook(selectedBorrowedBook.Id);

            RefreshLibrarianData();

            MessageBox.Show("Возврат книги оформлен.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ClearBookFieldsButton_Click(object sender, EventArgs e)
        {
            ClearBookFields();
        }

        private void ClearBookFields()
        {
            _titleTextBox.Text = "";
            _authorTextBox.Text = "";
            _countNumeric.Value = 0;

            if (_booksGrid != null)
            {
                _booksGrid.ClearSelection();
            }
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            _currentUser = null;
            ShowLoginScreen();
        }
    }
}