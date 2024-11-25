using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class TransaksiModel : ModelBase
    {
        public int IdTransaksi { get; set; }
        public BukuModel BukuPenawar { get; set; }
        public BukuModel BukuPenerima { get; set; }
        public string Status { get; set; }
        public DateTime WaktuTransaksi { get; set; }
        public bool IsPembeliKonfirmasi { get; set; }
        public bool IsPenjualKonfirmasi { get; set; }
        public bool IsPembeliTerima { get; set; }
        public bool IsPenjualTerima { get; set; }
    }
}
