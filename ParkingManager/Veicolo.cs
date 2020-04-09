using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManager
{
    public abstract class  Veicolo
    {
        public string Targa { get; set; }
        public string NomeProprietario { get; set; }
        public string TipoVeicolo { get; set; }
            

        public Veicolo(string targa, string nome_prop, string tipo_veicolo)
        {
            targa = Targa;
            nome_prop = NomeProprietario;
            tipo_veicolo = TipoVeicolo;

        }

        /*
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Veicolo)) return false;
            return Targa == ((Veicolo)obj).Targa;
        }
        */

    }
}
