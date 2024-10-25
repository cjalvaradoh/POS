using System.ComponentModel.DataAnnotations.Schema;

namespace UspgPOS.Models
{
    public class Productos
    {
        [Column("id")]
        public long? Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("marca_id")]
        public long MarcaId { get; set; }

        [ForeignKey("MarcaId")]
        public Marca? Marca { get; set; } // Relacion muchos a uno con Marca

        [Column("clasificacion_id")]
        public long ClasificacionId { get; set; }

        [ForeignKey("ClasificacionId")]
        public Clasificacion? Clasificacion { get; set; } // Relacion muchos a uno con Clasificación

        [Column("precio")]
        public decimal Precio { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }
    }
}
