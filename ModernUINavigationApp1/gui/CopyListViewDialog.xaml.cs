using FirstFloor.ModernUI.Windows.Controls;
using MCP.db;
using MCP.gui.Pages;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace MCP.gui
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    /// 
    public partial class CopyListViewDialog : ModernWindow
    {
        private int dbCopyId;

        public CopyListViewDialog(int dbCopyId)
        {
            InitializeComponent();
            this.dbCopyId = dbCopyId;
        }

        public int getDBCopyId()
        {
            return dbCopyId;
        }
    }
}
