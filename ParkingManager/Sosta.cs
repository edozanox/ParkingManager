using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManager
{
    [Serializable]
    class Sosta
    {
        public string TargaVeicolo { get; set; }
        public string TipoVeicolo { get; set; }
        public DateTime DataOraAccesso { get; set; } 
        public DateTime? DataOraUscita { get; set; }
        public int IdPosto { get; set; } 

        public Sosta (Veicolo v)
        {
            TargaVeicolo = v.Targa;
            TipoVeicolo = v.TipoVeicolo;
            DataOraAccesso = DateTime.Now;
            DataOraUscita = null;
            IdPosto++;
            
        }

    }
}
