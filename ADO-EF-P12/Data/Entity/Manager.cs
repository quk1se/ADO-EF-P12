using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_EF_P12.Data.Entity
{
    public class Manager
    {
        public Guid      Id          { get; set; }
        public String    Surname     { get; set; } = null!;
        public String    Name        { get; set; } = null!;
        public String    Secname     { get; set; } = null!;
        public String    Login       { get; set; } = null!;
        public String    PassSalt    { get; set; } = null!;   // за rfc2898
        public String    PassDk      { get; set; } = null!;   // за rfc2898
        public Guid      IdMainDep   { get; set; }  // відділ у якому працює
        public Guid?     IdSecDep    { get; set; }  // відділ за сумісництвом
        public Guid?     IdChief     { get; set; }  // керівник
        public DateTime  CreateDt    { get; set; }
        public DateTime? DeleteDt    { get; set; }
        public String    Email       { get; set; } = null!;
        public String?   Avatar      { get; set; }  // URL аватарки


        ////////////// Navigation props /////////////////////
        public Department MainDep { get; set; }  // навігаційна властивість
        public Department? SecDep { get; set; }  // опціональна навігаційна властивість
        public Manager Chief { get; set; }
        public IEnumerable<Manager> Subordinates { get; set; }
    }
}
/* У власних проєктах додати до сутностей навігаційні властивості
 * Налаштувати їх відповідним чином
 * Реалізувати зв'язки даних через ці елементи
 */
