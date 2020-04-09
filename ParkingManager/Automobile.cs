using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ParkingManager 
{
    class Automobile : Veicolo
    {
        public Automobile(string targa, string proprietario, string tipo_veic): base(targa, proprietario, tipo_veic)
        {
            //if (!ControllaTarga(targa)) throw new Exception();
            Targa = targa;
            NomeProprietario = proprietario;
            TipoVeicolo = tipo_veic;
            //double prezzo_ora = 2.00;
        }
        //controlla se la targa dell'automobile è corretta
        public static bool ControllaTarga(string targa)
        {
            return targa.Length == 7 && char.IsLetter(targa[0]) && char.IsLetter(targa[1]) && char.IsNumber(targa[2]) && char.IsNumber(targa[3]) && char.IsNumber(targa[4]) && char.IsLetter(targa[5]) && char.IsLetter(targa[6]);
        }
    }
}
