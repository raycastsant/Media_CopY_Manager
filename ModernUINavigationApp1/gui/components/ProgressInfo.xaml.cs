using System;
using System.Windows;
using System.Windows.Controls;

namespace MCP.gui.components
{
    /// <summary>
    /// Interaction logic for ProgressInfo.xaml
    /// </summary>
    public partial class ProgressInfo : UserControl
    {
        private int copyId;
        private ICopyController _controller;

        public ProgressInfo(ICopyController controller)
        {
            InitializeComponent();

            this._controller = controller;
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            int pos = System.Convert.ToInt32(this.Tag);
            if(pos >= 0)
            {
                _controller.Pause();
                BtnContinue.Visibility = Visibility.Visible;
                BtnPause.Visibility = Visibility.Hidden;
            }
        }

        private void BtnContinue_Click(object sender, RoutedEventArgs e)
        {
            int pos = System.Convert.ToInt32(this.Tag);
            if (pos >= 0)
            {
                _controller.Continue();
                BtnContinue.Visibility = Visibility.Hidden;
                BtnPause.Visibility = Visibility.Visible;
            }
        }

        private void BtnViewList_Click(object sender, RoutedEventArgs e)
        {
            new CopyListViewDialog(this.copyId).ShowDialog();
        }

        public void SetCompleted(int copyId)
        {
            this.copyId = copyId;
            BtnContinue.Visibility = Visibility.Hidden;
            BtnPause.Visibility = Visibility.Hidden;
            BtnViewList.Visibility = Visibility.Visible;
            _cCancelBtn.Width = new GridLength(25);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            CancelCopy();
            ((StackPanel)(this.Parent)).Children.Remove(this);
        }

        public void CancelCopy()
        {
            _controller.Quit();
        }
    }
}
