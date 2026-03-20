using EyeGlasses_Store.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EyeGlasses_Store
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly OpticShopDbContext _db;
        public MainWindow()
        {
            InitializeComponent();

            _db = new OpticShopDbContext();

            try
            {
                int productCount = _db.Products.Count();
                MessageBox.Show($"Kết nối DB thành công. Có {productCount} sản phẩm.");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi kết nối DB: " + ex.Message);
            }
        }
    }
}