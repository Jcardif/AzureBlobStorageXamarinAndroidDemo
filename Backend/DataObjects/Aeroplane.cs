using System;
using Microsoft.Azure.Mobile.Server;

namespace Backend.DataObjects
{
    public class Aeroplane : EntityData
    {

        public string Name { get; set; }
        public Uri Uri { get; set; }
        public string Description { get; set; }

    }
}