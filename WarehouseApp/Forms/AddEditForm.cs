using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WarehouseApp.Data;

namespace WarehouseApp.Forms
{
    public partial class AddEditForm : Form
    {
        private string tableName;
        private int? editId;
        private bool isAdmin;
        private int currentUserId;
        private List<Control> inputControls = new List<Control>();

        public AddEditForm(string tableName, int? editId, bool isAdmin, int currentUserId)
        {
            InitializeComponent();
            this.tableName = tableName;
            this.editId = editId;
            this.isAdmin = isAdmin;
            this.currentUserId = currentUserId;
            this.Text = editId == null ? $"Добавление записи в {GetRussianName(tableName)}" : $"Редактирование записи в {GetRussianName(tableName)}";
            this.Size = new System.Drawing.Size(500, 600);
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
            // Получаем структуру таблицы
            string schemaQuery = @"
                SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
                FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = @tableName
                ORDER BY ORDINAL_POSITION";

            SqlParameter[] param = { new SqlParameter("@tableName", tableName) };
            DataTable schema = DatabaseHelper.ExecuteQuery(schemaQuery, param);

            int y = 60;
            int rowHeight = 40;

            foreach (DataRow row in schema.Rows)
            {
                string colName = row["COLUMN_NAME"].ToString();
                string dataType = row["DATA_TYPE"].ToString();
                string isNullable = row["IS_NULLABLE"].ToString();

                // Пропускаем автоинкрементные поля
                if (colName == "id") continue;

                // Автоматические даты не показываем при добавлении
                if ((colName == "Дата_последнего_обновления" || colName == "Дата_прихода" || colName == "Дата_ухода") && editId == null)
                    continue;

                // Панель для каждой строки
                Panel rowPanel = new Panel();
                rowPanel.Location = new System.Drawing.Point(10, y);
                rowPanel.Size = new System.Drawing.Size(460, 35);
                rowPanel.BorderStyle = BorderStyle.None;

                // Label
                Label lbl = new Label();
                lbl.Text = GetRussianColumnName(colName);
                lbl.Location = new System.Drawing.Point(0, 8);
                lbl.Size = new System.Drawing.Size(150, 20);
                lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9, System.Drawing.FontStyle.Bold);

                // Для поля "Кто_добавил" подставляем текущего пользователя
                if (colName == "Кто_добавил" && editId == null)
                {
                    TextBox txtReadOnly = new TextBox();
                    txtReadOnly.Text = currentUserId.ToString();
                    txtReadOnly.Location = new System.Drawing.Point(160, 5);
                    txtReadOnly.Size = new System.Drawing.Size(290, 23);
                    txtReadOnly.ReadOnly = true;
                    txtReadOnly.BackColor = System.Drawing.Color.LightGray;
                    txtReadOnly.Tag = new { colName, dataType, isNullable };
                    rowPanel.Controls.Add(lbl);
                    rowPanel.Controls.Add(txtReadOnly);
                    inputControls.Add(txtReadOnly);
                }
                // Для поля "ИНН" делаем выпадающий список
                else if (colName == "ИНН")
                {
                    ComboBox cb = new ComboBox();
                    cb.Location = new System.Drawing.Point(160, 5);
                    cb.Size = new System.Drawing.Size(290, 24);
                    cb.DropDownStyle = ComboBoxStyle.DropDownList;
                    cb.Tag = new { colName, dataType, isNullable };

                    DataTable innData = DatabaseHelper.ExecuteQuery("SELECT ИНН, Название_компании FROM ИНН_Справочник");
                    cb.DisplayMember = "Название_компании";
                    cb.ValueMember = "ИНН";
                    cb.DataSource = innData;

                    rowPanel.Controls.Add(lbl);
                    rowPanel.Controls.Add(cb);
                    inputControls.Add(cb);
                }
                else
                {
                    TextBox txt = new TextBox();
                    txt.Location = new System.Drawing.Point(160, 5);
                    txt.Size = new System.Drawing.Size(290, 23);
                    txt.Tag = new { colName, dataType, isNullable };
                    rowPanel.Controls.Add(lbl);
                    rowPanel.Controls.Add(txt);
                    inputControls.Add(txt);
                }

                panelFields.Controls.Add(rowPanel);
                y += rowHeight;

                // Увеличиваем высоту формы при необходимости
                if (y > this.Height - 100)
                {
                    this.Height = y + 120;
                }
            }

            // Если редактируем, загружаем данные
            if (editId.HasValue)
            {
                LoadDataForEdit();
            }

            panelFields.Height = y + 10;
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
        {"Причина", "Причина"},
        {"id_инвентаризации", "ID инвентаризации"},
        {"id", "ID"}
    };

            return dict.ContainsKey(columnName) ? dict[columnName] : columnName;
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

                    if (control is TextBox txt && data.Rows[0][colName] != DBNull.Value)
                    {
                        txt.Text = data.Rows[0][colName].ToString();
                    }
                    else if (control is ComboBox cb && cb.Name.Contains("ИНН") && data.Rows[0][colName] != DBNull.Value)
                    {
                        cb.SelectedValue = data.Rows[0][colName].ToString();
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // =====================================================
                // СПЕЦИАЛЬНАЯ ЛОГИКА ДЛЯ ТАБЛИЦЫ "Приход_товара"
                // =====================================================
                if (tableName == "Приход_товара" && !editId.HasValue) // Только при добавлении
                {
                    // Получаем значения из формы
                    string productName = "";
                    int quantity = 0;
                    string cell = "";
                    string inn = "";

                    foreach (Control control in inputControls)
                    {
                        var tag = (dynamic)control.Tag;
                        string colName = tag.colName;

                        if (control is TextBox txt)
                        {
                            if (colName == "Наименование_товара") productName = txt.Text;
                            if (colName == "Количество") int.TryParse(txt.Text, out quantity);
                            if (colName == "Ячейка") cell = txt.Text;
                        }
                        else if (control is ComboBox cb && colName == "ИНН")
                        {
                            inn = cb.SelectedValue?.ToString();
                        }
                    }

                    // Проверяем, есть ли такой товар в Инвентаризации
                    string checkQuery = "SELECT id, Количество FROM Инвентаризация WHERE Наименование_товара = @name AND ИНН = @inn";
                    SqlParameter[] checkParams = {
                new SqlParameter("@name", productName),
                new SqlParameter("@inn", inn)
            };

                    DataTable existingItem = DatabaseHelper.ExecuteQuery(checkQuery, checkParams);

                    if (existingItem.Rows.Count > 0)
                    {
                        // Товар уже есть - обновляем количество
                        int inventoryId = Convert.ToInt32(existingItem.Rows[0]["id"]);
                        int currentQty = Convert.ToInt32(existingItem.Rows[0]["Количество"]);
                        int newQty = currentQty + quantity;

                        string updateQuery = "UPDATE Инвентаризация SET Количество = @newQty, Ячейка = @cell, Дата_последнего_обновления = GETDATE() WHERE id = @id";
                        SqlParameter[] updateParams = {
                    new SqlParameter("@newQty", newQty),
                    new SqlParameter("@cell", cell),
                    new SqlParameter("@id", inventoryId)
                };
                        DatabaseHelper.ExecuteNonQuery(updateQuery, updateParams);
                    }
                    else
                    {
                        // Новый товар - добавляем в Инвентаризацию
                        string insertInventoryQuery = @"
                    INSERT INTO Инвентаризация (Наименование_товара, Количество, Ячейка, ИНН, Дата_последнего_обновления, Кто_добавил, Дата_ухода)
                    VALUES (@name, @qty, @cell, @inn, GETDATE(), @userId, NULL)";

                        SqlParameter[] inventoryParams = {
                    new SqlParameter("@name", productName),
                    new SqlParameter("@qty", quantity),
                    new SqlParameter("@cell", cell),
                    new SqlParameter("@inn", inn),
                    new SqlParameter("@userId", currentUserId)
                };
                        DatabaseHelper.ExecuteNonQuery(insertInventoryQuery, inventoryParams);
                    }
                }

                // =====================================================
                // СПЕЦИАЛЬНАЯ ЛОГИКА ДЛЯ ТАБЛИЦЫ "Уход_товара"
                // =====================================================
                if (tableName == "Уход_товара" && !editId.HasValue)
                {
                    string productName = "";
                    int outgoingQty = 0;
                    string inn = "";

                    foreach (Control control in inputControls)
                    {
                        var tag = (dynamic)control.Tag;
                        string colName = tag.colName;

                        if (control is TextBox txt)
                        {
                            if (colName == "Наименование_товара") productName = txt.Text;
                            if (colName == "Количество_ухода") int.TryParse(txt.Text, out outgoingQty);
                        }
                        else if (control is ComboBox cb && colName == "ИНН")
                        {
                            inn = cb.SelectedValue?.ToString();
                        }
                    }

                    // Получаем текущее количество на складе
                    string checkQuery = "SELECT id, Количество FROM Инвентаризация WHERE Наименование_товара = @name AND ИНН = @inn";
                    SqlParameter[] checkParams = {
                new SqlParameter("@name", productName),
                new SqlParameter("@inn", inn)
            };

                    DataTable existingItem = DatabaseHelper.ExecuteQuery(checkQuery, checkParams);

                    if (existingItem.Rows.Count > 0)
                    {
                        int inventoryId = Convert.ToInt32(existingItem.Rows[0]["id"]);
                        int currentQty = Convert.ToInt32(existingItem.Rows[0]["Количество"]);
                        int newQty = currentQty - outgoingQty;

                        if (newQty < 0)
                        {
                            MessageBox.Show("Ошибка: недостаточно товара на складе!", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        string updateQuery = @"UPDATE Инвентаризация 
                                       SET Количество = @newQty, 
                                           Дата_последнего_обновления = GETDATE(),
                                           Дата_ухода = CASE WHEN @newQty = 0 THEN GETDATE() ELSE NULL END
                                       WHERE id = @id";
                        SqlParameter[] updateParams = {
                    new SqlParameter("@newQty", newQty),
                    new SqlParameter("@id", inventoryId)
                };
                        DatabaseHelper.ExecuteNonQuery(updateQuery, updateParams);
                    }
                    else
                    {
                        MessageBox.Show("Товар с таким ИНН не найден на складе!", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // =====================================================
                // ОБЫЧНОЕ СОХРАНЕНИЕ (ДЛЯ ВСЕХ ТАБЛИЦ)
                // =====================================================
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

                MessageBox.Show("Запись успешно сохранена!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void InitializeComponent()
        {
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panelFields = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelFields
            // 
            this.panelFields.AutoScroll = true;
            this.panelFields.Location = new System.Drawing.Point(12, 50);
            this.panelFields.Name = "panelFields";
            this.panelFields.Size = new System.Drawing.Size(480, 450);
            this.panelFields.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.LightGreen;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.Location = new System.Drawing.Point(12, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 35);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "💾 Сохранить";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.LightGray;
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnCancel.Location = new System.Drawing.Point(138, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 35);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "❌ Отмена";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            // 
            // AddEditForm
            // 
            this.ClientSize = new System.Drawing.Size(504, 511);
            this.Controls.Add(this.panelFields);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Name = "AddEditForm";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panelFields;
    }
}
