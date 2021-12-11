using BrosuhkaWinApp.Entities;
using BrosuhkaWinApp.Utils;
using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace BrosuhkaWinApp.Windows
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>
		/// Raises property chagned event.
		/// </summary>
		/// <param name="property">Property name.</param>
		private void OnPropertyChanged(string property)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}

		/// <summary>
		/// Product to edit.
		/// </summary>
		public Product Product { get; set; }

		List<ProductPhoto> Photos { get; set; }

		/// <summary>
		/// Base constructor of <see cref="EditWindow"/>.
		/// </summary>
		public EditWindow(Product product)
		{
			InitializeComponent();
			DataContext = this;

			if (product.ID == 0)
				IdField.Visibility = Visibility.Collapsed;

			Product = product;

			ManEntry.ItemsSource = DbUtils.Db.Manufacturer.ToList();

			ProductsControl.ItemsSource = Product.Product1.ToList();
			var data = DbUtils.Db.Product.Where(x => x.ID != Product.ID).ToList();
			ProductsDataGrid.ItemsSource = data.Where(x => x.IsActive == true);

			LoadPhotos();
		}

		/// <summary>
		/// Validates form input.
		/// </summary>
		private bool Validate()
		{
			var sb = new StringBuilder();

			if (string.IsNullOrEmpty(TitleEntry.Text))
				sb.Append("\nНазвание продукта не введено!");
			if (string.IsNullOrEmpty(CostEntry.Text))
				sb.Append("\nНе указана стоимость продукта!");
			if (!double.TryParse(CostEntry.Text.Replace('.', ','), out double price))
				sb.Append("\nУказано не число!");
			if (price < 0)
				sb.Append("\nСтоимость не может быть отрицательной.");
			if (!(ManEntry.SelectedItem is Manufacturer))
				sb.Append("\nПроизводитель не выбран!");			
			if (Photos.Where(x => x.IsActive == true).Count() <= 0)
				sb.Append("\nТребуется хоть одна фотография!");

			if (sb.Length > 0)
			{
				Msg.ShowError($"Не удалось сохранить изменения: {sb}");
				return false;
			}
			return true;
		}
		/// <summary>
		/// Actions for saving changes.
		/// </summary>
		private void SaveBtnClick(object sender, RoutedEventArgs e)
		{
			if (!Validate())
				return;

			if (Product.ID == 0)
			{
				DbUtils.Db.Product.Add(Product);
			}

			if (DbUtils.SafeSave())
				Close();
		}

		/// <summary>
		/// Actions for closing window.
		/// </summary>
		protected override void OnClosing(CancelEventArgs e)
		{
			if(DbUtils.Db.ChangeTracker.HasChanges())
            {
				if (Msg.ShowQuestion("Вы действительно хотите вернуться назад? " +
				"Все несохраненные изменения будут отменены."))
				{
					DbUtils.RollBack();
				}
				else
			e.Cancel = true;
			}

			base.OnClosing(e);
		}

		/// <summary>
		/// Actions for pressing exit button.
		/// </summary>
		private void ExitBtnClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Actions for picking new photo.
		/// </summary>
		private void PhotoSelectClick(object sender, RoutedEventArgs e)
		{
			var fileDialog = new OpenFileDialog()
			{
				Filter = "Изображения |*.png;*.jpg"
			};

			if (fileDialog.ShowDialog() == false)
				return;

			if (new FileInfo(fileDialog.FileName).Length > 2000000)
			{
				Msg.ShowError("Размер фото должен быть меньше 2мб!");
				return;
			}

			var photo = new ProductPhoto()
			{
				Product = Product,
				IsActive = true
			};
			photo.Photo = File.ReadAllBytes(fileDialog.FileName);
			Product.ProductPhoto.Add(photo);

			LoadPhotos();

			OnPropertyChanged(nameof(Product));
		}

		private void LoadPhotos()
		{
			Photos = Product.ProductPhoto.Where(x => x.IsActive == true).ToList();
			PhotoControl.ItemsSource = Photos;
		}

		/// <summary>
		/// Attach product actions.
		/// </summary>
		private void AddProductsClick(object sender, RoutedEventArgs e)
		{
			var productsToAdd = new List<Product>();
			foreach (Product product in ProductsDataGrid.SelectedItems)
				productsToAdd.Add(product);

			var attachedProducts = Product.Product1.ToList();
			attachedProducts.AddRange(productsToAdd);

			attachedProducts = attachedProducts.Distinct().ToList();

			ProductsControl.ItemsSource = attachedProducts;
			Product.Product1 = attachedProducts;
		}

		private void RemoveBtnClick(object sender, RoutedEventArgs e)
		{
			var p = ((FrameworkElement)sender).DataContext as Product;

			Product.Product1.Remove(p);
			ProductsControl.ItemsSource = Product.Product1.ToList();
		}

		private void RemovePhotoBtnClick(object sender, RoutedEventArgs e)
		{
			var p = ((FrameworkElement)sender).DataContext as ProductPhoto;
			p.IsActive = false;
			PhotoControl.ItemsSource = Photos.Where(x => x.IsActive == true);
		}
	}
}
