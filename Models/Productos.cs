using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UspgPOS.Models
{
    public class Productos
    {
        [Column("id")]
        public long? Id { get; set; }

        [Column("codigo")]
        [MaxLength(100)]
        public string? Codigo { get; set; }

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

        [Column("img_url")]
        public string? img_url { get; set; }

        [Column("thumbnail_url")]
        public string? thumbnail_url { get; set; }
    }
}
