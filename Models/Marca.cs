﻿using System.ComponentModel.DataAnnotations.Schema;

namespace UspgPOS.Models
{
    public class Marca
    {
        [Column("id")]
        public long Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("img_url")]
        public string? img_url { get; set; }

        [Column("thumbnail_url")]
        public string? thumbnail_url { get; set; }
    }
}
