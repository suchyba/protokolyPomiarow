using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ProtokolyPomiarow.Windows
{
    /// <summary>
    /// Logika interakcji dla klasy PdfProgress.xaml
    /// </summary>
    public partial class PdfProgress : Window
    {
        public PdfProgress()
        {
            InitializeComponent();
        }
        public void SetProgress(int prog)
        {
            CreatingProgressBar.Value = prog;
        }
    }
}
