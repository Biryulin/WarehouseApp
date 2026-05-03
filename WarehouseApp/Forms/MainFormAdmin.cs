using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WarehouseApp.Forms
{
    public partial class MainFormAdmin : Form
    {
        private int userId;
        private string userName;

        public MainFormAdmin(int userId, string userName)
        {
            InitializeComponent();
            this.userId = userId;
            this.userName = userName;
            lblWelcome.Text = $"Добро пожаловать, {userName} (Администратор)";
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            TableForm form = new TableForm("Пользователи", userId, true);
            form.ShowDialog();
        }

        private void btnInn_Click(object sender, EventArgs e)
        {
            TableForm form = new TableForm("ИНН_Справочник", userId, true);
            form.ShowDialog();
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            TableForm form = new TableForm("Инвентаризация", userId, true);
            form.ShowDialog();
        }

        private void btnStockIncoming_Click(object sender, EventArgs e)
        {
            TableForm form = new TableForm("Приход_товара", userId, true);
            form.ShowDialog();
        }

        private void btnStockOutgoing_Click(object sender, EventArgs e)
        {
            TableForm form = new TableForm("Уход_товара", userId, true);
            form.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginForm login = new LoginForm();
            login.Show();
            this.Close();
        }

        private void InitializeComponent()
        {
            this.btnUsers = new System.Windows.Forms.Button();
            this.btnInn = new System.Windows.Forms.Button();
            this.btnInventory = new System.Windows.Forms.Button();
            this.btnStockIncoming = new System.Windows.Forms.Button();
            this.btnStockOutgoing = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnUsers
            // 
            this.btnUsers.Location = new System.Drawing.Point(50, 80);
            this.btnUsers.Name = "btnUsers";
            this.btnUsers.Size = new System.Drawing.Size(200, 40);
            this.btnUsers.TabIndex = 0;
            this.btnUsers.Text = "Пользователи";
            this.btnUsers.UseVisualStyleBackColor = true;
            this.btnUsers.Click += new System.EventHandler(this.btnUsers_Click);
            // 
            // btnInn
            // 
            this.btnInn.Location = new System.Drawing.Point(50, 130);
            this.btnInn.Name = "btnInn";
            this.btnInn.Size = new System.Drawing.Size(200, 40);
            this.btnInn.TabIndex = 1;
            this.btnInn.Text = "ИНН Справочник";
            this.btnInn.UseVisualStyleBackColor = true;
            this.btnInn.Click += new System.EventHandler(this.btnInn_Click);
            // 
            // btnInventory
            // 
            this.btnInventory.Location = new System.Drawing.Point(50, 180);
            this.btnInventory.Name = "btnInventory";
            this.btnInventory.Size = new System.Drawing.Size(200, 40);
            this.btnInventory.TabIndex = 2;
            this.btnInventory.Text = "Инвентаризация";
            this.btnInventory.UseVisualStyleBackColor = true;
            this.btnInventory.Click += new System.EventHandler(this.btnInventory_Click);
            // 
            // btnStockIncoming
            // 
            this.btnStockIncoming.Location = new System.Drawing.Point(300, 80);
            this.btnStockIncoming.Name = "btnStockIncoming";
            this.btnStockIncoming.Size = new System.Drawing.Size(200, 40);
            this.btnStockIncoming.TabIndex = 3;
            this.btnStockIncoming.Text = "Приход товара";
            this.btnStockIncoming.UseVisualStyleBackColor = true;
            this.btnStockIncoming.Click += new System.EventHandler(this.btnStockIncoming_Click);
            // 
            // btnStockOutgoing
            // 
            this.btnStockOutgoing.Location = new System.Drawing.Point(300, 130);
            this.btnStockOutgoing.Name = "btnStockOutgoing";
            this.btnStockOutgoing.Size = new System.Drawing.Size(200, 40);
            this.btnStockOutgoing.TabIndex = 4;
            this.btnStockOutgoing.Text = "Уход товара";
            this.btnStockOutgoing.UseVisualStyleBackColor = true;
            this.btnStockOutgoing.Click += new System.EventHandler(this.btnStockOutgoing_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Location = new System.Drawing.Point(550, 20);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(100, 30);
            this.btnLogout.TabIndex = 5;
            this.btnLogout.Text = "Выйти";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Location = new System.Drawing.Point(50, 30);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(100, 17);
            this.lblWelcome.TabIndex = 6;
            this.lblWelcome.Text = "Добро пожаловать";
            // 
            // MainFormAdmin
            // 
            this.ClientSize = new System.Drawing.Size(680, 270);
            this.Controls.Add(this.lblWelcome);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnStockOutgoing);
            this.Controls.Add(this.btnStockIncoming);
            this.Controls.Add(this.btnInventory);
            this.Controls.Add(this.btnInn);
            this.Controls.Add(this.btnUsers);
            this.Name = "MainFormAdmin";
            this.Text = "Панель администратора";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button btnUsers;
        private System.Windows.Forms.Button btnInn;
        private System.Windows.Forms.Button btnInventory;
        private System.Windows.Forms.Button btnStockIncoming;
        private System.Windows.Forms.Button btnStockOutgoing;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label lblWelcome;
    }
}
