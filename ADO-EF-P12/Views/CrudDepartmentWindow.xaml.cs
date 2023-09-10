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

namespace ADO_EF_P12.Views
{
    /// <summary>
    /// Interaction logic for CrudDepartmentWindow.xaml
    /// </summary>
    public partial class CrudDepartmentWindow : Window
    {
        public Data.Entity.Department? Department { get; set; }
        public bool IsDeleted { get; set; }

        public CrudDepartmentWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IdTextBox.Text = Department?.Id.ToString() ?? "";
            NameTextBox.Text = Department?.Name?.ToString() ?? "";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Фіксуємо зміни - переносимо введений текст до Department
            Department.Name = NameTextBox.Text;
            DialogResult = true;
        }

        private void SoftDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            IsDeleted = true;  // ознака м'якого видалення
            DialogResult = true;
        }

        private void HardDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Повне видалення - помітимо об'єкт як null
            Department = null;
            DialogResult = true;
        }
    }
}
