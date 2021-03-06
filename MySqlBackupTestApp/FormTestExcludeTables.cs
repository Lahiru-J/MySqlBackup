﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MySqlBackupTestApp
{
    public partial class FormTestExcludeTables : Form
    {
        public FormTestExcludeTables()
        {
            InitializeComponent();
        }

        private void btGetTables_Click(object sender, EventArgs e)
        {
            using (var conn = new MySqlConnection(Program.ConnectionString))
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();

                    var dt = QueryExpress.GetTable(cmd, "show tables");
                    checkedListBox1.Items.Clear();

                    foreach (DataRow dr in dt.Rows)
                        checkedListBox1.Items.Add(dr[0] + "", false);
                }
            }

            lbTotal.Text = "Total Tables: " + checkedListBox1.Items.Count;
        }

        private void btAll_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, true);
        }

        private void btNone_Click(object sender, EventArgs e)
        {
            for (var i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, false);
        }

        private void btExport_Click(object sender, EventArgs e)
        {
            if (!Program.TargetDirectoryIsValid())
                return;

            if (checkedListBox1.Items.Count == 0)
            {
                MessageBox.Show("No tables are listed.");
                return;
            }
            try
            {
                var lst = new List<string>();

                foreach (var item in checkedListBox1.CheckedItems)
                    lst.Add(item.ToString());

                using (var conn = new MySqlConnection(Program.ConnectionString))
                {
                    using (var cmd = new MySqlCommand())
                    {
                        using (var mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            mb.ExportInfo.ExcludeTables = lst;
                            mb.ExportToFile(Program.TargetFile);
                        }
                    }
                }
                MessageBox.Show("Done.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}