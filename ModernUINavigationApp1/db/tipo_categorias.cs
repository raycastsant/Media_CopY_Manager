//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MCP.db
{
    using System;
    using System.Collections.Generic;
    
    public partial class tipo_categorias
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tipo_categorias()
        {
            this.categorias = new HashSet<categoria>();
            this.coeficientes_pago = new HashSet<coeficientes_pago>();
        }
    
        public int id { get; set; }
        public string nombre { get; set; }
        public string codigo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<categoria> categorias { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<coeficientes_pago> coeficientes_pago { get; set; }

        public override string ToString()
        {
            return nombre;
        }
    }
}
