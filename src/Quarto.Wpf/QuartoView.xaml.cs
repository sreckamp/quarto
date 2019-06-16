using GameBase.WPF;
using Quarto.WPF.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Quarto.WPF
{
    /// <summary>
    /// Interaction logic for QuartoView.xaml
    /// </summary>
    public partial class QuartoView : Window
    {
        private readonly QuartoViewModel m_gameViewModel;
        private readonly BackgroundWorker m_worker = new BackgroundWorker();

        public QuartoView()
        {
            InitializeComponent();
            m_gameViewModel = new QuartoViewModel();
            DataContext = m_gameViewModel;
            m_worker.DoWork += m_worker_DoWork;
            m_worker.RunWorkerAsync();
        }

        private void m_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            m_gameViewModel.Run();
        }
    }
}
