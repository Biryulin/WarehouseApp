using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WarehouseApp.Data;
//готовый диплом

namespace WarehouseApp.Forms
{
    public partial class AddEditForm : Form
    {
        private string tableName;
        private int? editId;
        private bool isAdmin;
        private int currentUserId;
        private List<Control> inputControls = new List<Control>();
        private ComboBox cmbProduct;
        private ComboBox cmbInnCombo;

        public AddEditForm(string tableName, int? editId, bool isAdmin, int currentUserId)
        {
            InitializeComponent();
            this.tableName = tableName;
            this.editId = editId;
            this.isAdmin = isAdmin;
            this.currentUserId = currentUserId;
            this.Text = editId == null ? $"Добавление записи в {GetRussianName(tableName)}" : $"Редактирование записи в {GetRussianName(tableName)}";
            this.Size = new System.Drawing.Size(580, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            LoadFields();
        }

        private string GetRussianName(string tableName)
        {
            switch (tableName)
            {
                case "Пользователи": return "Пользователи";
                case "ИНН_Справочник": return "ИНН Справочник";
                case "Инвентаризация": return "Инвентаризация";
                case "Приход_товара": return "Приход товара";
                case "Уход_товара": return "Уход товара";
                default: return tableName;
            }
        }

        private void LoadFields()
        {
            if (tableName == "Приход_товара" && !editId.HasValue)
            {
                LoadSpecialIncomingForm();
                return;
            }

            if (tableName == "ИНН_Справочник" && !editId.HasValue)
            {
                LoadSpecialInnForm();
                return;
            }

            if (tableName == "Уход_товара" && !editId.HasValue)
            {
                LoadSpecialDepartureForm();
                return;
            }

            LoadDefaultForm();
        }

        private void LoadDefaultForm()
        {
            string schemaQuery = @"
                SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = @tableName
                ORDER BY ORDINAL_POSITION";

            SqlParameter[] param = { new SqlParameter("@tableName", tableName) };
            DataTable schema = DatabaseHelper.ExecuteQuery(schemaQuery, param);

            int y = 60;
            int rowHeight = 40;

            inputControls.Clear();
            panelFields.Controls.Clear();

            foreach (DataRow row in schema.Rows)
            {
                string colName = row["COLUMN_NAME"].ToString();
                string dataType = row["DATA_TYPE"].ToString();
                string isNullable = row["IS_NULLABLE"].ToString();

                if (colName == "id") continue;
                if ((colName == "Дата_последнего_обновления" || colName == "Дата_прихода" || colName == "Дата_ухода") && editId == null)
                    continue;

                Panel rowPanel = new Panel();
                rowPanel.Location = new System.Drawing.Point(10, y);
                rowPanel.Size = new System.Drawing.Size(530, 35);
                rowPanel.BorderStyle = BorderStyle.None;

                Label lbl = new Label();
                lbl.Text = GetRussianColumnName(colName);
                lbl.Location = new System.Drawing.Point(0, 8);
                lbl.Size = new System.Drawing.Size(180, 20);
                lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9, System.Drawing.FontStyle.Bold);

                if (colName == "Кто_добавил" && editId == null)
                {
                    TextBox txtReadOnly = new TextBox();
                    txtReadOnly.Text = currentUserId.ToString();
                    txtReadOnly.Location = new System.Drawing.Point(190, 5);
                    txtReadOnly.Size = new System.Drawing.Size(310, 23);
                    txtReadOnly.ReadOnly = true;
                    txtReadOnly.BackColor = System.Drawing.Color.LightGray;
                    txtReadOnly.Tag = new { colName, dataType, isNullable };
                    rowPanel.Controls.Add(lbl);
                    rowPanel.Controls.Add(txtReadOnly);
                    inputControls.Add(txtReadOnly);
                }
                else
                {
                    TextBox txt = new TextBox();
                    txt.Location = new System.Drawing.Point(190, 5);
                    txt.Size = new System.Drawing.Size(310, 23);
                    txt.Tag = new { colName, dataType, isNullable };
                    rowPanel.Controls.Add(lbl);
                    rowPanel.Controls.Add(txt);
                    inputControls.Add(txt);
                }

                panelFields.Controls.Add(rowPanel);
                y += rowHeight;
            }

            if (editId.HasValue)
            {
                LoadDataForEdit();
            }

            panelFields.Height = y + 10;
        }

        private void LoadSpecialIncomingForm()
        {
            panelFields.Controls.Clear();
            inputControls.Clear();
            int y = 60;
            int rowHeight = 50;

            Label lblProduct = new Label();
            lblProduct.Text = "Наименование товара:";
            lblProduct.Location = new System.Drawing.Point(10, y);
            lblProduct.Size = new System.Drawing.Size(180, 25);
            lblProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            panelFields.Controls.Add(lblProduct);

            TextBox txtProduct = new TextBox();
            txtProduct.Name = "txtProduct";
            txtProduct.Location = new System.Drawing.Point(200, y);
            txtProduct.Size = new System.Drawing.Size(300, 27);
            txtProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            panelFields.Controls.Add(txtProduct);
            inputControls.Add(txtProduct);
            y += rowHeight;

            Label lblCell = new Label();
            lblCell.Text = "Ячейка:";
            lblCell.Location = new System.Drawing.Point(10, y);
            lblCell.Size = new System.Drawing.Size(180, 25);
            lblCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            panelFields.Controls.Add(lblCell);

            TextBox txtCell = new TextBox();
            txtCell.Name = "txtCell";
            txtCell.Location = new System.Drawing.Point(200, y);
            txtCell.Size = new System.Drawing.Size(300, 27);
            txtCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            panelFields.Controls.Add(txtCell);
            inputControls.Add(txtCell);
            y += rowHeight;

            Label lblQuantity = new Label();
            lblQuantity.Text = "Количество:";
            lblQuantity.Location = new System.Drawing.Point(10, y);
            lblQuantity.Size = new System.Drawing.Size(180, 25);
            lblQuantity.Font = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            panelFields.Controls.Add(lblQuantity);

            TextBox txtQuantity = new TextBox();
            txtQuantity.Name = "txtQuantity";
            txtQuantity.Location = new System.Drawing.Point(200, y);
            txtQuantity.Size = new System.Drawing.Size(150, 27);
            txtQuantity.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            txtQuantity.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) && !char.IsControl(ev.KeyChar))
                    ev.Handled = true;
            };
            panelFields.Controls.Add(txtQuantity);
            inputControls.Add(txtQuantity);
            y += rowHeight;

            Label lblInn = new Label();
            lblInn.Text = "ИНН:";
            lblInn.Location = new System.Drawing.Point(10, y);
            lblInn.Size = new System.Drawing.Size(180, 25);
            lblInn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            panelFields.Controls.Add(lblInn);

            cmbInnCombo = new ComboBox();
            cmbInnCombo.Name = "cmbInnCombo";
            cmbInnCombo.Location = new System.Drawing.Point(200, y);
            cmbInnCombo.Size = new System.Drawing.Size(220, 28);
            cmbInnCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbInnCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            LoadInnCombo();
            panelFields.Controls.Add(cmbInnCombo);

            Button btnNewInn = new Button();
            btnNewInn.Text = "➕ Новый";
            btnNewInn.Location = new System.Drawing.Point(430, y);
            btnNewInn.Size = new System.Drawing.Size(80, 28);
            btnNewInn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9);
            btnNewInn.BackColor = System.Drawing.Color.LightBlue;
            btnNewInn.Click += (s, ev) => ShowNewCompanyDialog();
            panelFields.Controls.Add(btnNewInn);

            inputControls.Add(cmbInnCombo);
            y += rowHeight;

            panelFields.Height = y + 20;
        }

        private void LoadInnCombo()
        {
            try
            {
                DataTable innData = DatabaseHelper.ExecuteQuery("SELECT ИНН, Название_компании FROM ИНН_Справочник ORDER BY Название_компании");
                var items = new List<dynamic>();
                foreach (DataRow row in innData.Rows)
                {
                    items.Add(new
                    {
                        ИНН = row["ИНН"].ToString(),
                        DisplayText = $"{row["Название_компании"]} (ИНН: {row["ИНН"]})"
                    });
                }
                cmbInnCombo.DataSource = items;
                cmbInnCombo.DisplayMember = "DisplayText";
                cmbInnCombo.ValueMember = "ИНН";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки ИНН: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowNewCompanyDialog()
        {
            Form dialog = new Form();
            dialog.Text = "Добавление новой компании";
            dialog.Size = new System.Drawing.Size(400, 200);
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            dialog.MaximizeBox = false;
            dialog.MinimizeBox = false;

            Label lblNewInn = new Label();
            lblNewInn.Text = "ИНН (10 или 12 цифр):";
            lblNewInn.Location = new System.Drawing.Point(20, 20);
            lblNewInn.Size = new System.Drawing.Size(150, 25);
            dialog.Controls.Add(lblNewInn);

            TextBox txtNewInn = new TextBox();
            txtNewInn.Location = new System.Drawing.Point(180, 20);
            txtNewInn.Size = new System.Drawing.Size(180, 23);
            txtNewInn.MaxLength = 12;
            txtNewInn.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) && !char.IsControl(ev.KeyChar))
                    ev.Handled = true;
            };
            dialog.Controls.Add(txtNewInn);

            Label lblNewCompany = new Label();
            lblNewCompany.Text = "Название компании:";
            lblNewCompany.Location = new System.Drawing.Point(20, 55);
            lblNewCompany.Size = new System.Drawing.Size(150, 25);
            dialog.Controls.Add(lblNewCompany);

            TextBox txtNewCompany = new TextBox();
            txtNewCompany.Location = new System.Drawing.Point(180, 55);
            txtNewCompany.Size = new System.Drawing.Size(180, 23);
            dialog.Controls.Add(txtNewCompany);

            Button btnOk = new Button();
            btnOk.Text = "Добавить";
            btnOk.Location = new System.Drawing.Point(180, 95);
            btnOk.Size = new System.Drawing.Size(100, 30);
            btnOk.BackColor = System.Drawing.Color.LightGreen;
            btnOk.Click += (s, ev) =>
            {
                string inn = txtNewInn.Text.Trim();
                string companyName = txtNewCompany.Text.Trim();

                if (string.IsNullOrEmpty(inn) || string.IsNullOrEmpty(companyName))
                {
                    MessageBox.Show("Заполните оба поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (inn.Length != 10 && inn.Length != 12)
                {
                    MessageBox.Show("ИНН должен содержать 10 или 12 цифр!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string checkQuery = "SELECT COUNT(*) FROM ИНН_Справочник WHERE ИНН = @inn";
                SqlParameter[] checkParams = { new SqlParameter("@inn", inn) };
                int exists = Convert.ToInt32(DatabaseHelper.ExecuteQuery(checkQuery, checkParams).Rows[0][0]);

                if (exists > 0)
                {
                    MessageBox.Show($"Компания с ИНН {inn} уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string insertQuery = "INSERT INTO ИНН_Справочник (ИНН, Название_компании) VALUES (@inn, @name)";
                SqlParameter[] parameters = {
                    new SqlParameter("@inn", inn),
                    new SqlParameter("@name", companyName)
                };
                DatabaseHelper.ExecuteNonQuery(insertQuery, parameters);

                MessageBox.Show("Компания успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadInnCombo();

                foreach (var item in cmbInnCombo.Items)
                {
                    var dynamicItem = (dynamic)item;
                    if (dynamicItem.ИНН == inn)
                    {
                        cmbInnCombo.SelectedItem = item;
                        break;
                    }
                }

                dialog.Close();
            };
            dialog.Controls.Add(btnOk);

            Button btnCancel = new Button();
            btnCancel.Text = "Отмена";
            btnCancel.Location = new System.Drawing.Point(290, 95);
            btnCancel.Size = new System.Drawing.Size(80, 30);
            btnCancel.Click += (s, ev) => dialog.Close();
            dialog.Controls.Add(btnCancel);

            dialog.ShowDialog();
        }

        private void LoadSpecialInnForm()
        {
            panelFields.Controls.Clear();
            inputControls.Clear();
            int y = 60;
            int rowHeight = 50;

            Label lblInn = new Label();
            lblInn.Text = "ИНН (10 или 12 цифр):";
            lblInn.Location = new System.Drawing.Point(10, y);
            lblInn.Size = new System.Drawing.Size(180, 25);
            lblInn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            panelFields.Controls.Add(lblInn);

            TextBox txtInn = new TextBox();
            txtInn.Name = "txtInn";
            txtInn.Location = new System.Drawing.Point(200, y);
            txtInn.Size = new System.Drawing.Size(250, 27);
            txtInn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            txtInn.MaxLength = 12;
            txtInn.KeyPress += (s, ev) =>
            {
                if (!char.IsDigit(ev.KeyChar) && !char.IsControl(ev.KeyChar))
                    ev.Handled = true;
            };
            panelFields.Controls.Add(txtInn);
            inputControls.Add(txtInn);
            y += rowHeight;

            Label lblCompany = new Label();
            lblCompany.Text = "Название компании:";
            lblCompany.Location = new System.Drawing.Point(10, y);
            lblCompany.Size = new System.Drawing.Size(180, 25);
            lblCompany.Font = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            panelFields.Controls.Add(lblCompany);

            TextBox txtCompany = new TextBox();
            txtCompany.Name = "txtCompany";
            txtCompany.Location = new System.Drawing.Point(200, y);
            txtCompany.Size = new System.Drawing.Size(300, 27);
            txtCompany.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            panelFields.Controls.Add(txtCompany);
            inputControls.Add(txtCompany);
            y += rowHeight;

            panelFields.Height = y + 20;
        }

        private void LoadSpecialDepartureForm()
        {
            panelFields.Controls.Clear();
            inputControls.Clear();
            int y = 60;
            int rowHeight = 45;

            Label lblProduct = new Label();
            lblProduct.Text = "Выберите товар:";
            lblProduct.Location = new System.Drawing.Point(10, y);
            lblProduct.Size = new System.Drawing.Size(180, 25);
            lblProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            panelFields.Controls.Add(lblProduct);

            cmbProduct = new ComboBox();
            cmbProduct.Location = new System.Drawing.Point(200, y);
            cmbProduct.Size = new System.Drawing.Size(300, 28);
            cmbProduct.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            LoadAvailableProducts();
            panelFields.Controls.Add(cmbProduct);
            inputControls.Add(cmbProduct);
            y += rowHeight;

            Label lblStock = new Label();
            lblStock.Text = "Остаток на складе:";
            lblStock.Location = new System.Drawing.Point(10, y);
            lblStock.Size = new System.Drawing.Size(180, 25);
            lblStock.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            panelFields.Controls.Add(lblStock);

            Label lblStockValue = new Label();
            lblStockValue.Name = "lblStockValue";
            lblStockValue.Text = "0";
            lblStockValue.Location = new System.Drawing.Point(200, y);
            lblStockValue.Size = new System.Drawing.Size(100, 25);
            lblStockValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            lblStockValue.ForeColor = System.Drawing.Color.Blue;
            panelFields.Controls.Add(lblStockValue);
            y += rowHeight;

            Label lblQty = new Label();
            lblQty.Text = "Количество ухода:";
            lblQty.Location = new System.Drawing.Point(10, y);
            lblQty.Size = new System.Drawing.Size(180, 25);
            lblQty.Font = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            panelFields.Controls.Add(lblQty);

            TextBox txtQuantity = new TextBox();
            txtQuantity.Name = "txtQuantity";
            txtQuantity.Location = new System.Drawing.Point(200, y);
            txtQuantity.Size = new System.Drawing.Size(150, 27);
            txtQuantity.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            panelFields.Controls.Add(txtQuantity);
            inputControls.Add(txtQuantity);
            y += rowHeight;

            Label lblReason = new Label();
            lblReason.Text = "Причина:";
            lblReason.Location = new System.Drawing.Point(10, y);
            lblReason.Size = new System.Drawing.Size(180, 25);
            lblReason.Font = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            panelFields.Controls.Add(lblReason);

            TextBox txtReason = new TextBox();
            txtReason.Name = "txtReason";
            txtReason.Location = new System.Drawing.Point(200, y);
            txtReason.Size = new System.Drawing.Size(300, 27);
            txtReason.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            panelFields.Controls.Add(txtReason);
            inputControls.Add(txtReason);
            y += rowHeight;

            Label lblDate = new Label();
            lblDate.Text = "Дата ухода:";
            lblDate.Location = new System.Drawing.Point(10, y);
            lblDate.Size = new System.Drawing.Size(180, 25);
            lblDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
            panelFields.Controls.Add(lblDate);

            DateTimePicker dtpDate = new DateTimePicker();
            dtpDate.Name = "dtpDate";
            dtpDate.Location = new System.Drawing.Point(200, y);
            dtpDate.Size = new System.Drawing.Size(200, 27);
            dtpDate.Format = DateTimePickerFormat.Short;
            dtpDate.Value = DateTime.Now;
            dtpDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10);
            panelFields.Controls.Add(dtpDate);
            inputControls.Add(dtpDate);
            y += rowHeight;

            cmbProduct.SelectedIndexChanged += (s, ev) =>
            {
                if (cmbProduct.SelectedItem != null)
                {
                    var selected = (dynamic)cmbProduct.SelectedItem;
                    lblStockValue.Text = selected.Quantity.ToString();
                }
            };

            txtQuantity.TextChanged += (s, ev) =>
            {
                if (int.TryParse(txtQuantity.Text, out int qty))
                {
                    int stock = int.Parse(lblStockValue.Text);
                    if (qty > stock)
                    {
                        errorProvider.SetError(txtQuantity, $"Недостаточно товара! Доступно: {stock}");
                    }
                    else
                    {
                        errorProvider.SetError(txtQuantity, "");
                    }
                }
            };

            panelFields.Height = y + 20;
        }

        private void LoadAvailableProducts()
        {
            try
            {
                string query = @"
                    SELECT i.id, i.Наименование_товара, i.Количество, inn.Название_компании
                    FROM Инвентаризация i
                    INNER JOIN ИНН_Справочник inn ON i.ИНН = inn.ИНН
                    WHERE i.Количество > 0
                    ORDER BY i.Наименование_товара";

                DataTable products = DatabaseHelper.ExecuteQuery(query);

                var items = new List<dynamic>();
                foreach (DataRow row in products.Rows)
                {
                    items.Add(new
                    {
                        id = row["id"],
                        DisplayText = $"{row["Наименование_товара"]} ({row["Название_компании"]}) - Остаток: {row["Количество"]}",
                        Quantity = row["Количество"]
                    });
                }

                cmbProduct.DataSource = items;
                cmbProduct.DisplayMember = "DisplayText";
                cmbProduct.ValueMember = "id";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки товаров: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDataForEdit()
        {
            string query = $"SELECT * FROM {tableName} WHERE id = @id";
            SqlParameter[] param = { new SqlParameter("@id", editId.Value) };
            DataTable data = DatabaseHelper.ExecuteQuery(query, param);

            if (data.Rows.Count > 0)
            {
                foreach (Control control in inputControls)
                {
                    var tag = (dynamic)control.Tag;
                    string colName = tag.colName;

                    if (data.Rows[0][colName] == DBNull.Value) continue;

                    if (control is TextBox txt)
                    {
                        txt.Text = data.Rows[0][colName].ToString();
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (tableName == "Приход_товара" && !editId.HasValue)
                {
                    SaveIncomingRecord();
                    return;
                }

                if (tableName == "ИНН_Справочник" && !editId.HasValue)
                {
                    SaveInnRecord();
                    return;
                }

                if (tableName == "Уход_товара" && !editId.HasValue)
                {
                    SaveDepartureRecord();
                    return;
                }

                SaveDefaultRecord();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveDefaultRecord()
        {
            List<string> columns = new List<string>();
            List<string> values = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            foreach (Control control in inputControls)
            {
                var tag = (dynamic)control.Tag;
                string colName = tag.colName;
                string value = "";

                if (control is TextBox txt)
                {
                    value = txt.Text;
                }
                else if (control is ComboBox cb)
                {
                    value = cb.SelectedValue?.ToString();
                }

                if (string.IsNullOrEmpty(value) && tag.isNullable == "YES")
                    continue;

                columns.Add(colName);
                values.Add($"@{colName}");
                parameters.Add(new SqlParameter($"@{colName}", value ?? ""));
            }

            string query;
            if (editId.HasValue)
            {
                List<string> setClauses = new List<string>();
                for (int i = 0; i < columns.Count; i++)
                {
                    setClauses.Add($"{columns[i]} = {values[i]}");
                }
                query = $"UPDATE {tableName} SET {string.Join(", ", setClauses)} WHERE id = @id";
                parameters.Add(new SqlParameter("@id", editId.Value));
            }
            else
            {
                query = $"INSERT INTO {tableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", values)})";
            }

            DatabaseHelper.ExecuteNonQuery(query, parameters.ToArray());

            MessageBox.Show("Запись успешно сохранена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SaveIncomingRecord()
        {
            TextBox txtProduct = (TextBox)inputControls[0];
            TextBox txtCell = (TextBox)inputControls[1];
            TextBox txtQuantity = (TextBox)inputControls[2];
            ComboBox cbInn = (ComboBox)inputControls[3];

            string productName = txtProduct.Text.Trim();
            string cell = txtCell.Text.Trim();
            string quantityText = txtQuantity.Text.Trim();
            string inn = cbInn.SelectedValue?.ToString();

            if (string.IsNullOrEmpty(productName))
            {
                MessageBox.Show("Введите наименование товара!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(cell))
            {
                MessageBox.Show("Введите ячейку!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(quantityText, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(inn))
            {
                MessageBox.Show("Выберите ИНН!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string insertIncoming = @"
                INSERT INTO Приход_товара (Наименование_товара, Ячейка, Количество, ИНН, Дата_прихода)
                VALUES (@name, @cell, @qty, @inn, GETDATE())";

            SqlParameter[] incomingParams = {
                new SqlParameter("@name", productName),
                new SqlParameter("@cell", cell),
                new SqlParameter("@qty", quantity),
                new SqlParameter("@inn", inn)
            };
            DatabaseHelper.ExecuteNonQuery(insertIncoming, incomingParams);

            string checkInventory = "SELECT id, Количество FROM Инвентаризация WHERE Наименование_товара = @name AND ИНН = @inn";
            SqlParameter[] checkParams = {
                new SqlParameter("@name", productName),
                new SqlParameter("@inn", inn)
            };
            DataTable existingItem = DatabaseHelper.ExecuteQuery(checkInventory, checkParams);

            if (existingItem.Rows.Count > 0)
            {
                int newQty = Convert.ToInt32(existingItem.Rows[0]["Количество"]) + quantity;
                string updateQuery = "UPDATE Инвентаризация SET Количество = @qty, Ячейка = @cell, Дата_последнего_обновления = GETDATE() WHERE id = @id";
                SqlParameter[] updateParams = {
                    new SqlParameter("@qty", newQty),
                    new SqlParameter("@cell", cell),
                    new SqlParameter("@id", existingItem.Rows[0]["id"])
                };
                DatabaseHelper.ExecuteNonQuery(updateQuery, updateParams);
            }
            else
            {
                string insertInventory = @"
                    INSERT INTO Инвентаризация (Наименование_товара, Количество, Ячейка, ИНН, Дата_последнего_обновления, Кто_добавил)
                    VALUES (@name, @qty, @cell, @inn, GETDATE(), @userId)";
                SqlParameter[] insertParams = {
                    new SqlParameter("@name", productName),
                    new SqlParameter("@qty", quantity),
                    new SqlParameter("@cell", cell),
                    new SqlParameter("@inn", inn),
                    new SqlParameter("@userId", currentUserId)
                };
                DatabaseHelper.ExecuteNonQuery(insertInventory, insertParams);
            }

            MessageBox.Show("Товар успешно добавлен!\nИнвентаризация обновлена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SaveInnRecord()
        {
            TextBox txtInn = (TextBox)inputControls[0];
            TextBox txtCompany = (TextBox)inputControls[1];

            string inn = txtInn.Text.Trim();
            string companyName = txtCompany.Text.Trim();

            if (string.IsNullOrEmpty(inn))
            {
                MessageBox.Show("Введите ИНН!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(companyName))
            {
                MessageBox.Show("Введите название компании!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (inn.Length != 10 && inn.Length != 12)
            {
                MessageBox.Show("ИНН должен содержать 10 или 12 цифр!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string checkQuery = "SELECT COUNT(*) FROM ИНН_Справочник WHERE ИНН = @inn";
            SqlParameter[] checkParams = { new SqlParameter("@inn", inn) };
            int exists = Convert.ToInt32(DatabaseHelper.ExecuteQuery(checkQuery, checkParams).Rows[0][0]);

            if (exists > 0)
            {
                MessageBox.Show($"Компания с ИНН {inn} уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string insertQuery = "INSERT INTO ИНН_Справочник (ИНН, Название_компании) VALUES (@inn, @name)";
            SqlParameter[] parameters = {
                new SqlParameter("@inn", inn),
                new SqlParameter("@name", companyName)
            };

            DatabaseHelper.ExecuteNonQuery(insertQuery, parameters);

            MessageBox.Show("Компания успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SaveDepartureRecord()
        {
            if (cmbProduct.SelectedItem == null)
            {
                MessageBox.Show("Выберите товар!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedProduct = (dynamic)cmbProduct.SelectedItem;
            int inventoryId = selectedProduct.id;
            int currentStock = selectedProduct.Quantity;

            TextBox txtQuantity = (TextBox)inputControls.FirstOrDefault(c => c.Name == "txtQuantity");
            if (txtQuantity == null || !int.TryParse(txtQuantity.Text, out int quantityOut) || quantityOut <= 0)
            {
                MessageBox.Show("Введите корректное количество ухода!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (quantityOut > currentStock)
            {
                MessageBox.Show($"Недостаточно товара! Доступно: {currentStock}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TextBox txtReason = (TextBox)inputControls.FirstOrDefault(c => c.Name == "txtReason");
            string reason = txtReason?.Text ?? "";

            DateTimePicker dtpDate = (DateTimePicker)inputControls.FirstOrDefault(c => c.Name == "dtpDate");
            DateTime departureDate = dtpDate?.Value ?? DateTime.Now;

            string productInfoQuery = @"
                SELECT Наименование_товара, ИНН 
                FROM Инвентаризация 
                WHERE id = @id";
            SqlParameter[] infoParams = { new SqlParameter("@id", inventoryId) };
            DataTable productInfo = DatabaseHelper.ExecuteQuery(productInfoQuery, infoParams);

            string productName = productInfo.Rows[0]["Наименование_товара"].ToString();
            string inn = productInfo.Rows[0]["ИНН"].ToString();

            string insertOutgoing = @"
                INSERT INTO Уход_товара (Наименование_товара, Количество_на_складе_до_ухода, Количество_ухода, Причина, Дата_ухода, ИНН)
                VALUES (@name, @stockBefore, @qtyOut, @reason, @date, @inn)";

            SqlParameter[] outgoingParams = {
                new SqlParameter("@name", productName),
                new SqlParameter("@stockBefore", currentStock),
                new SqlParameter("@qtyOut", quantityOut),
                new SqlParameter("@reason", reason),
                new SqlParameter("@date", departureDate),
                new SqlParameter("@inn", inn)
            };
            DatabaseHelper.ExecuteNonQuery(insertOutgoing, outgoingParams);

            int newStock = currentStock - quantityOut;
            string updateInventory = @"
                UPDATE Инвентаризация 
                SET Количество = @newStock,
                    Дата_последнего_обновления = GETDATE(),
                    Дата_ухода = CASE WHEN @newStock = 0 THEN GETDATE() ELSE NULL END
                WHERE id = @id";

            SqlParameter[] updateParams = {
                new SqlParameter("@newStock", newStock),
                new SqlParameter("@id", inventoryId)
            };
            DatabaseHelper.ExecuteNonQuery(updateInventory, updateParams);

            MessageBox.Show($"Товар успешно списан!\nОстаток на складе: {newStock}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private string GetRussianColumnName(string columnName)
        {
            var dict = new Dictionary<string, string>
            {
                {"Логин", "Логин"},
                {"Пароль", "Пароль"},
                {"Роль", "Роль"},
                {"Название_компании", "Название компании"},
                {"Наименование_товара", "Наименование товара"},
                {"Количество", "Количество"},
                {"Ячейка", "Ячейка"},
                {"ИНН", "ИНН"},
                {"Кто_добавил", "Кто добавил"},
                {"Дата_последнего_обновления", "Дата последнего обновления"},
                {"Дата_прихода", "Дата прихода"},
                {"Дата_ухода", "Дата ухода"},
                {"Количество_ухода", "Количество ухода"},
                {"Количество_на_складе_до_ухода", "Количество на складе до ухода"},
                {"Причина", "Причина"}
            };
            return dict.ContainsKey(columnName) ? dict[columnName] : columnName;
        }

        private void InitializeComponent()
        {
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panelFields = new System.Windows.Forms.Panel();
            this.errorProvider = new System.Windows.Forms.ErrorProvider();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();

            this.panelFields.AutoScroll = true;
            this.panelFields.Location = new System.Drawing.Point(12, 50);
            this.panelFields.Name = "panelFields";
            this.panelFields.Size = new System.Drawing.Size(540, 500);
            this.panelFields.TabIndex = 2;

            this.btnSave.BackColor = System.Drawing.Color.LightGreen;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(12, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 35);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "💾 Сохранить";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnCancel.BackColor = System.Drawing.Color.LightGray;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnCancel.Location = new System.Drawing.Point(138, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 35);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "❌ Отмена";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += (s, ev) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.ClientSize = new System.Drawing.Size(564, 562);
            this.Controls.Add(this.panelFields);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Name = "AddEditForm";
            this.StartPosition = FormStartPosition.CenterScreen;

            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panelFields;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
        
