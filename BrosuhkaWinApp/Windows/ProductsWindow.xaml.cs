using BrosuhkaWinApp.Entities;
using BrosuhkaWinApp.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BrosuhkaWinApp.Windows
{
    /// <summary>
    /// Interaction logic for ProductsWindow.xaml
    /// </summary>
    public partial class ProductsWindow : Window
    {
        public ObservableCollection<Product> Products { get; set; }

        /// <summary>
        /// <see cref="ProductsWindow"/> constructor.
        /// </summary>
        public ProductsWindow()
        {
            InitializeComponent();
            DataContext = this;

            Products = new ObservableCollection<Product>();

            var manufacturers = DbUtils.Db.Manufacturer.Select(x => x.Name).ToList();
            manufacturers.Insert(0, "Все элементы");
            ManufacturersBox.ItemsSource = manufacturers;
            ManufacturersBox.SelectedIndex = 0;

            PriceBox.ItemsSource = new List<string>()
            {
                "Нет сортировки",
                "По возрастанию",
                "По убыванию"
            };
            PriceBox.SelectedIndex = 0;

            Update();
        }

        /// <summary>
        /// Refreshes products view.
        /// </summary>
        public void Update()
        {
            var data = DbUtils.Db.Product.ToList();

            ItemsTotal.Text = data.Count().ToString();

            // Search
            if(!string.IsNullOrEmpty(SearchEntry.Text))
            {
                data = data.Where(x =>
                x.Title
                .ToLower()
                .Contains(SearchEntry.Text
                .ToLower())
                ||
                x.Description
                .ToLower()
                .Contains(SearchEntry.Text
                .ToLower())).ToList();
            }

            // Manufacturers filter
            if((string) ManufacturersBox.SelectedItem != "Все элементы")
            {
                data = data
                    .Where(x => x.Manufacturer.Name == (string)ManufacturersBox.SelectedItem).ToList();
            }

            // Price sorting
            if((string) PriceBox.SelectedItem != "Нет сортировки")
            {
                if((string) PriceBox.SelectedItem == "По возрастанию")
                {
                    data = data.OrderBy(x => x.Cost).ToList();
                }
                else
                {
                    data = data.OrderByDescending(x => x.Cost).ToList();
                }
            }

            ItemsShown.Text = data.Count().ToString();

            NoResult.Visibility = data.Count() == 0 ? Visibility.Visible : Visibility.Collapsed;

            Products.Clear();
            foreach (var product in data)
            {
                product.ProductPhoto1 = product.ProductPhoto.Where(x => x.IsActive == true).FirstOrDefault();
                Products.Add(product);
            }
        }

        /// <summary>
        /// Actions for exit.
        /// </summary>
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            if(Msg.ShowQuestion("Вы действительно хотите выйти?"))
            {
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Actions for searching product.
        /// </summary>
        private void SearchBoxTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Update();
        }

        /// <summary>
        /// Actions for changing filters.
        /// </summary>
        private void FilterBoxChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Update();
        }

        /// <summary>
        /// Actions for deleting product.
        /// </summary>
        private void ProductDeleteButton(object sender, RoutedEventArgs e)
        {
            if (Msg.ShowQuestion("Вы действительно хотите удалить продукт? " +
                "С ним так же будут удалены все приклепленные товары. Действие отменить невозможно."))
            {
                var product = ((FrameworkElement)sender).DataContext as Product;

                if(product.ProductSale.Count() > 0)
                {
                    Msg.ShowError("Невозможно удалить продукт с продажами!");
                    return;
                }

                product.Product1.ToList().ForEach(x => x.IsActive = false);
                product.IsActive = false;

                DbUtils.SafeSave();

                Update();
            }
        }

        /// <summary>
        /// Actions for editing product.
        /// </summary>
        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            var product = ((FrameworkElement)sender).DataContext as Product;

            new EditWindow(product).ShowDialog();

            Update();
        }

        /// <summary>
        /// Actions for adding product.
        /// </summary>
        private void AddProductClick(object sender, RoutedEventArgs e)
        {
            var product = new Product()
            {
                IsActive = true
            };

            new EditWindow(product).ShowDialog();

            Update();
        }

        /// <summary>
        /// Actions for viewing product sale history.
        /// </summary>
        private void ViewProductHistory(object sender, RoutedEventArgs e)
        {
            var product = ((FrameworkElement)sender).DataContext as Product;

            new HistoryWindow(product).ShowDialog();
        }

        /// <summary>
        /// Actions for calculating mouse position on tile.
        /// </summary>
        private void BorderMouseMove(object sender, MouseEventArgs e)
        {
            var product = ((FrameworkElement)sender).DataContext as Product;
            var productPhotos = product.ProductPhoto.Where(x => x.IsActive == true);

            // Force update binding
            var image = ((StackPanel)((Border)sender).Child).Children[0];
            ((Image)image).GetBindingExpression(Image.SourceProperty)
                  .UpdateTarget();

            // Get mouse position relative to tile
            var pos = Mouse.GetPosition((FrameworkElement) sender).X;

            // Get width of the tile
            var width = ((FrameworkElement)sender).ActualWidth;

            // Get number of photos attached to tile's product
            var photosCount = productPhotos.Count();

            // Calculate segment based on mouse position
            double currentWidth = 0;
            double segmentWidth = width / photosCount;
            int segment = 0;
            for (int i = 0; i < photosCount; i++)
            {
                currentWidth += segmentWidth;

                if (pos <= currentWidth)
                {
                    segment = i;
                    break;
                }
            }

            product.ProductPhoto1 = productPhotos
                .Single(x => productPhotos
                .ToList()
                .IndexOf(x) == segment);
        }
    }
}
