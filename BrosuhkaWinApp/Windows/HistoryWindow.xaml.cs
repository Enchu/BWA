using BrosuhkaWinApp.Entities;
using BrosuhkaWinApp.Utils;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BrosuhkaWinApp.Windows
{
    public partial class HistoryWindow : Window
    {
        /// <summary>
        /// Product for binding.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Constructor of <see cref="HistoryWindow"/>.
        /// </summary>
        /// <param name="product"></param>
        public HistoryWindow(Product product)
        {
            InitializeComponent();
            DataContext = this;

            Product = product;

            ProductsCombo.ItemsSource = DbUtils.Db.Product.ToList();
            ProductsCombo.SelectedItem = Product;
        }

        /// <summary>
        /// Updates product sell information.
        /// </summary>
        public void Update()
        {
            var data = DbUtils.Db.ProductSale
                .Where(x => x.ProductID == Product.ID)
                .OrderByDescending(x => x.SaleDate)
                .ToList();

            NoResult.Visibility = data.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;

            HistoryGrid.ItemsSource = data;
        }

        /// <summary>
        /// Actions for selecting new product.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductsComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product = (Product)ProductsCombo.SelectedItem;
            Update();
        }

        /// <summary>
        /// Actions for exit.
        /// </summary>
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            if (Msg.ShowQuestion("Вы действительно хотите вернуться назад?"))
            {
                Close();
            }
        }
    }
}
