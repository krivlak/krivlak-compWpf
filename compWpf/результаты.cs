//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace compWpf
{
    using System;
    using System.Collections.Generic;
    
    public partial class результаты
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public результаты()
        {
            this.штрафы = new HashSet<штрафы>();
        }
    
        public System.Guid результат { get; set; }
        public int попытка { get; set; }
        public System.Guid экипаж { get; set; }
        public int время_мин { get; set; }
        public int время_сек { get; set; }
        public int секунд { get; set; }
        public int штраф { get; set; }
        public int итог { get; set; }
        public int порядок { get; set; }
        public string прим { get; set; }
    
        public virtual экипажи экипажи { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<штрафы> штрафы { get; set; }
    }
}
