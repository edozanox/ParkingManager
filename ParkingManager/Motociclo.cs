using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManager
{
    public class Motociclo : Veicolo
    {
        public Motociclo(string targa, string proprietario, string tipo_veic) : base (targa, proprietario, tipo_veic)
        {
            //if (!ControllaTarga(targa)) throw new Exception();
            Targa = targa;
            NomeProprietario = proprietario;
            TipoVeicolo = tipo_veic;
           // double prezzo_ora = 0.80;

        }
        //controlla se la targa del motociclo è corretta
        public static bool ControllaTarga(string targa)
        {
            return targa.Length == 7 && char.IsLetter(targa[0]) && char.IsLetter(targa[1]) && char.IsNumber(targa[2]) && char.IsNumber(targa[3]) && char.IsNumber(targa[4]) && char.IsNumber(targa[5]) && char.IsNumber(targa[6]);
        }
    }
}
