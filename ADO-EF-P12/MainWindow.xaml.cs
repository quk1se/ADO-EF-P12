using ADO_EF_P12.Data;
using ADO_EF_P12.Data.Entity;
using ADO_EF_P12.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ADO_EF_P12
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataContext dataContext;
        public ObservableCollection<Pair> Pairs { get; set; }
        public ObservableCollection<Data.Entity.Department> DepartmentsView { get; set; }
        private ICollectionView departmentsListView;
        public MainWindow()
        {
            InitializeComponent();
            dataContext = new();
            Pairs = new();
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DepartmentsCountLabel.Content = dataContext.Departments.Count().ToString();
            TopChiefCountLabel.Content =
                dataContext
                .Managers
                .Where(   // predicate - функція, що повертає bool
                    manager => manager.IdChief == null
                )  // ?? для кожного елементу здійснюється порівняння??
                   // Ні! з аналізу предикату будується SQL запит
                .Count()
                .ToString();

            dataContext.Departments.Load();
            DepartmentsView = dataContext.Departments.Local.ToObservableCollection();
            departmentsList.ItemsSource = DepartmentsView;
            // departmentsList.ItemsSource = dataContext.Departments.Local.ToObservableCollection();
            departmentsListView = 
                CollectionViewSource.GetDefaultView(DepartmentsView);
            departmentsListView.Filter = 
                item => (item as Data.Entity.Department)?.DeleteDt == null;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            /* Вивести Прізвище - І.П.(ініціали) співробітників бухгалтерії
             */
            var query = dataContext
                .Managers
                .Where(m => m.IdMainDep == Guid.Parse("131ef84b-f06e-494b-848f-bb4bc0604266"))
                .Select(                        // Select - правило перетворення
                    m =>                        // на "вході" елемент попередньої
                    new Pair                    // колекції (m - manager), а на
                    {                           // виході - результат лямбди
                        Key = m.Surname,        // 
                        Value = $"{m.Name[0]}. {m.Secname[0]}."
                    }
                );
            // query - "правило" побудови запиту. Сам запит ані надісланий, ані
            // одержано результати

            Pairs.Clear();
            foreach ( var pair in query )  // цикл-ітератор запускає виконання запиту
            {                              // саме з цього моменту іде звернення до БД
                Pairs.Add(pair);
            }
            /* Особливості:
             * - LINQ запит можна зберігти у змінній, сам запит є "правилом" і не
             *    ініціює звернення до БД
             * - LINQ-to-Entity вживає приєднаний режим, тобто кожен запит надсилається
             *    до БД, а не до "скачаної" колекції
             * - Виконання запиту здійснюється шляхами:
             *   = виклик агрегатора (.Count(), .Max(), тощо)
             *   = виклик явного перетворення (.ToList(), ToArray(), тощо)
             *   = запуск циклу з ітерування запиту
             *   
             * -- Фільтрування (.Where) краще здійснювати за індексованими полями,
             *      у т.ч. за первинним ключем (який автоматично індексується)
             * -- Інструкція .Select є перетворювачем, а не запуском запиту     
             */
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            /* Вивести Прізвище І.П. -- Назва робочого відділу
             * Обмежити вибірку першими 10 рядками та пропустити 
             * перших трьох
             */
            var query = dataContext           // Запит з поєднанням таблиць
                .Managers                     // Ліва таблиця
                .Join(                        // операція поєднання
                    dataContext.Departments,  //  права таблиця
                    m => m.IdMainDep,         //  outerKey - зовн. ключ з лівої таблиці
                    d => d.Id,                //  innerKey - внутр. ключ правої таблиці
                    (m, d) =>                 // selector - правило перетворення
                       new Pair               //  пари сутностей, для яких зареєстровано
                       {                      //  з'єднання (join)
                           Key = m.Surname,   // 
                           Value = d.Name     // 
                       }
                )
                .Skip(3)                      // пропустити 3 елементи
                .Take(10);                    // обмеження - 10 елементів з вибірки

            Pairs.Clear();
            foreach( var pair in query )
            {
                Pairs.Add(pair);
            }
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            /* Вивести відомості 
             * Прізвище І.П. співробітника -- Прізвище І.П. керівника
             * Впорядкувати за абеткою по співробітниках
             */
            var query = dataContext        // SELECT ... FROM
                .Managers                  // Managers as m
                .Join(                     // JOIN
                    dataContext.Managers,  // Managers as chief ON
                    m => m.IdChief,        // m.IdChief =
                    chief => chief.Id,     //             chief.Id
                    (m, chief) => new Pair
                    {
                        Key = m.Surname,
                        Value = chief.Surname
                    }
                )  // .OrderBy(pair => pair.Key)  // у цьому місці працює лише якщо
                   // у  pair.Key - посилання на поле entity
                   // наприклад m.Surname
                .ToList()  // запускає запит та перетворює результат на колекцію
                .OrderBy(pair => pair.Key);  // у цьому місці - інший LINQ, який діє на
                                             // колекцію, а не на запит SQL

            Pairs.Clear();
            foreach( var pair in query)
            {
                Pairs.Add(pair);
            }
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            /* Вивести дані Дата реєстрації -- Прізвище І.Б. співробітника
             * Впорядкувати по даті - останні зареєстровані ідуть першими
             * Обмежити вибірку 7ма рядками
             * (7 останніх зареєстрованих)
             */
            /* Д.З. Використовуючи LINQ-to-Entity та створену БД
             * Вивести дані  
             * Прізвище І.Б. співробітника  --  Назва відділу (за сумісництвом: SecDep)
             * Впорядкувати по назві відділу
             */
        }
        #region Генератор - збільшується при кожному зверненні
        private int _N;
        public int N { get => _N++; set => _N = value; }
        #endregion
        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            /* Вивести порядковий номер відділу -- назву відділу
             * Неправильне рішення
             */
            N = 1;
            var query = dataContext         // SELECT n++, d.Name FROM Departments
                .Departments                //   -- помилка, операції у запиті
                .OrderBy(d => d.Name)
                .Select(d => new Pair()     // SELECT N, d.Name .... =>
                {                           //  SELECT 1, d.Name ....
                    Key = (N).ToString(),   // до N є одноразове звертання при
                    Value = d.Name          // побудові SQL виразу
                });                         // НЕ багаторазове при перетвореннях

            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
            /* Як це працює
             * var query = ... задає "правило", з якого буде побудований SQL
             * Який SQL буде у підсумку?
             *  SELECT 1, d.Name FROM Departments d ORDER BY d.Name
             *  при формуванні запиту EF "побачить", що до нього входить змінна
             *  N, він її обрахує і додасть у SQL -- у запиті буде значення N (1)
             * Ітерування в циклі - по результатах запиту
             *  1 - Бухгалтерія
             *  1 - Відділ кадрів
             *  ...
             * Об'єкти  pair утворюються з цих результатів, як наслідок усі мають
             * "1" у ключі
             * 
             * N(1) --> SQL --> рез-ти з (1), а не з N
             */
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            N = 1;
            var query = dataContext
                .Departments
                .OrderBy(d => d.Name)
                .AsEnumerable()            // Перетворювач, далі LINQ-Enumerable, також можна .ToList()
                .Select(d => new Pair()
                {
                    Key = (N).ToString(),  // В режимі LINQ-Enumerable це дійсно
                    Value = d.Name         // повторюється і N збільшується
                });

            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
            /* Відмінності (від Button6)
             * Запит SQL формується інструкціями, що передують .AsEnumerable()
             * .Departments
             * .OrderBy(d => d.Name)   -> SELECT * FROM Departments d ORDER BY d.Name
             * Запит виконується із результатами 
             *  id - Бухгалтерія
             *  id - Відділ кадрів
             * Ітерування у циклі утворює об'єкти Pair у яких є звертання до N і з 
             * кожним звертанням N збільшується у гет-тері.
             *  (*) -> SQL -> Рез-ти з N
             */
        }

        /////////////////////////////////////////////////

        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            /* GroupJoin - аналог GROUP BY
             * Завдання: вивести дані
             * Назва відділу -- кількість співробітників
             */
            var query = dataContext           // Запит із групуванням (під одним
                .Departments                  //  id відділу декілька співробітників)
                .GroupJoin(                   // 
                    dataContext.Managers,     // 
                    d => d.Id,                // Ключ з "одним" значенням
                    m => m.IdMainDep,         // Ключ з "множинним" значенням
                    (dep, mans) => new Pair   // (группа) - (один відділ, коллекція співробітників)
                    {                         // 
                        Key = dep.Name,       // до першого параметра звертаємось як до об'єкта (одного)
                        Value = mans.Count()  // до другого - як до колекції (IEnumerable)
                                .ToString()   // 
                    }                         // 
                );                            // 
        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            /* Завдання: вивести дані
             * Прізвище І.Б. шефа -- кількість підлеглих
             * 1) всіх
             * 2) тільки тих, що мають підлеглих
             */

            // 1) GroupJoin ~ LeftJoin, залишає усіх, у т.ч. без підлеглих
            var query = dataContext.Managers  // as chef
                .GroupJoin(
                    dataContext.Managers, // as sub
                    chef => chef.Id,
                    sub => sub.IdChief, 
                    (chef, subs) => new Pair() 
                    {
                        Key = $"{chef.Surname} {chef.Name[0]}. {chef.Secname[0]}.", 
                        Value = subs.Count().ToString() 
                    }
                )
                .Where(p => Convert.ToInt32(p.Value) > 0);

            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        
        }

        private void Button10_Click(object sender, RoutedEventArgs e) // ДЗ========================ДЗ==================ДЗ=========Однофамильцы с нумерацией
        {
            /* Знайти однофамільців (згрупувати та подивитись де кількість 
             * більше ніж 1)
             */
            N = 1;  // генератор
           var query = dataContext
                .Managers
                .GroupBy(m => m.Surname)
                .AsEnumerable()
                .Where(m => m.Count() > 1)
                .Select(group => new Pair { Key = N.ToString(), Value = group.Key });

            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        }

        private void ButtonNav1_Click(object sender, RoutedEventArgs e)
        {
            // робота з навігаційними властивостями:
            // вивести: співробітник -- назва відділу
            var query = dataContext
                .Managers
                .Include(m => m.MainDep)  // включення навігаційної властивості 
                .Select(m => new Pair     // (в деяких системах не потрібна - вкл. автоматично)
                {
                    Key = m.Surname,
                    Value = m.MainDep.Name   // навігаційна властивість
                });

            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        }

        private void ButtonNav2_Click(object sender, RoutedEventArgs e)
        {
            // робота з навігаційними властивостями:
            // вивести: співробітник -- назва відділу (за сумісництвом)
            var query = dataContext
                .Managers
                .Include(m => m.SecDep)   // включення навігаційної властивості 
                .Select(m => new Pair     // (в деяких системах не потрібна - вкл. автоматично)
                {
                    Key = m.Surname,
                    Value = m.SecDep == null ? "--" : m.SecDep.Name
                });

            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(sender is ListViewItem item)
            {
                if(item.Content is Data.Entity.Department department)
                {
                    CrudDepartmentWindow dialog = new()
                    {
                        Department = department
                    };
                    if (dialog.ShowDialog() ?? false)  // Save or Delete pressed
                    {
                        if (dialog.Department != null)  // null - ознака видалення
                        {
                            if (dialog.IsDeleted)
                            {
                                // м'яке видалення
                                department.DeleteDt = DateTime.Now;
                                dataContext.SaveChanges();
                                departmentsListView.Refresh();
                            }
                            else
                            {
                                // шукаємо відповідний відділ у контексті
                                var dep = dataContext       // Find - метод, що шукає за Id 
                                    .Departments            // (тільки за Id). Швидкий,
                                    .Find(department.Id);   // рекомендований

                                if (dep != null) // якщо знайдено, вносимо зміни
                                {
                                    dep.Name = department.Name;
                                }
                                // внесення змін не відображається ані на БД, ані на 
                                // інших запитах. Для внесення змін необхідно викликати
                                // спеціальний метод.
                                dataContext.SaveChanges();  // зберігаємо зміни
                                departmentsListView.Refresh();
                            }
                        }
                        else  // dialog.Department == null  - видалення
                        {
                            dataContext.Departments.Remove(department);
                            dataContext.SaveChanges();
                        }
                    }
                }
            }
        }

        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            // Create - створення нового відділу
            // спочатку створюємо новий об'єкт (Entity)
            Data.Entity.Department newDepartment = new()
            {
                Id = Guid.NewGuid(),
                Name = null!
            };
            // заповнюємо його - викликаємо вікно-діалог
            CrudDepartmentWindow dialog = new()
            {
                Department = newDepartment
            };
            if (dialog.ShowDialog() ?? false)  // Save or Delete pressed
            {
                // Після заповнення - додаємо об'єкт до контексту даних
                dataContext.Departments.Add(newDepartment);
                // зберігаємо контекст
                dataContext.SaveChanges();
            }
        }

        private void ButtonNav3_Click(object sender, RoutedEventArgs e)
        {
            // Зворотні навігаційні властивості:
            // задача: вивести відділ - кількість співробітників
            var query = dataContext
                .Departments
                .Include(d => d.MainManagers)
                .Where(d => d.DeleteDt == null)
                .Select(d => new Pair
                {
                    Key = d.Name,
                    Value = d.MainManagers.Count().ToString()
                });

            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        }

        private void ButtonNav4_Click(object sender, RoutedEventArgs e)
        {
            var query = dataContext
                .Departments
                .Include(d => d.SecManagers)
                .Where(d => d.DeleteDt == null)
                .Select(d => new Pair
                {
                    Key = d.Name,
                    Value = d.SecManagers.Count().ToString()
                });

            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        }

        private void ButtonNav5_Click(object sender, RoutedEventArgs e)
        {

        }
        /* Д.З. Засобами LINQ на основі створеної БД реалізувати запити
* Назва відділу -- кількість сумісників (SecDep)
* Запит з однофамільцями переробити з нумерацією
*  1. Андріяш
*  2. Лешків
* Вивести трьох співробітників з найбільшою кількістю підлеглих 
*  к-сть підлеглих --- П.І.Б.
*/
    }
    public class Pair
    {
        public String Key { get; set; } = null!;
        public String? Value { get; set; }
    }
}
/* Д.З. По БД, створеної на зустрічі, вивести (у "моніторі БД") засобами LINQ
 * Кількість підлеглих (осіб, що мають керівника)
 * Кількість співробітників ІТ-відділу (як основних, так і сумісників)
 * Кількість співробітників, які працюють у двох відділах (основний та сумісний)
 */
