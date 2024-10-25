using System.ComponentModel.DataAnnotations.Schema;

namespace UspgPOS.Models
{
    [Table("detalles_venta")]
    public class DetalleVenta
    {
        [Column("id")]
        public long? Id { get; set; }

        [Column("venta_id")]
        public long VentaId { get; set; }

        [ForeignKey("VentaId")]
        public Venta? Venta { get; set; } // Relacion muchos a uno con Venta

        [Column("producto_id")]
        public long ProductoId { get; set; }

        [ForeignKey("ProductoId")]
        public Productos? Producto { get; set; } // Relacion muchos a uno con Producto

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("precio_unitario")]
        public decimal PrecioUnitario { get; set; }
    }
}
