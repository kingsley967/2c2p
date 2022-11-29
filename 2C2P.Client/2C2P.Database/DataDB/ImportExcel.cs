using System;
using System.Collections.Generic;

namespace _2C2P.Database.DataDB
{
    public partial class ImportExcel
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string Format { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
