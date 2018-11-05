using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Azure.Mobile.Server;

namespace Backend.DataObjects
{
    public class Aeroplane : EntityData
    {
        [Required, Column(TypeName = "VARCHAR")]
        public string Name { get; set; }
        [Required, Column(TypeName = "VARCHAR")]
        public string ImageUri { get; set; }
        [Required, Column(TypeName = "VARCHAR")]
        public string Description { get; set; }
    }
}