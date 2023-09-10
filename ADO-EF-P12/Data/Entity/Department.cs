using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_EF_P12.Data.Entity
{
    public class Department
    {
        public Guid   Id   { get; set; }
        public String Name { get; set; } = null!;

        // додано для м'якого видалення
        public DateTime? DeleteDt { get; set; }

        ///////// Inverse Navigation props /////////////////////
        
        // MainManagers - зворотна до Manager.MainDep властивість
        public IEnumerable<Manager> MainManagers { get; set; }
        public List<Manager> SecManagers { get; set; }

    }
}
/* Зміни у контексті, наприклад, додавання нового поля, 
 * має супроводжуватись
 * - створенням міграції
 * - застосуванням міграції
 */
/* Д.З. Реалізувати приховування видалених відділень у 
 * їх переліку на головному вікні.
 * Впровадити CRUD у підсумковий проєкт.
 */
