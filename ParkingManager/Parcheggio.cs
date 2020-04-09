using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManager
{
    class Parcheggio
    {

        public List<Sosta> VeicoliInSosta = new List<Sosta>();
        public List<Sosta> ArchivioVeicoli = new List<Sosta>();
        public double prezzo_unitario_auto = 0.40;
        public double prezzo_unitario_moto = 0.10;
        
               
        public void MezzoInArrivo(Veicolo ve)
        {
            Sosta s = new Sosta(ve);
            VeicoliInSosta.Add(s);

        } 
        
        public double MezzoInUscita (Sosta s)
        {
            double prezzo = 0;            
            s.DataOraUscita = DateTime.Now;
            TimeSpan tm = s.DataOraUscita.Value - s.DataOraAccesso;
            double tempo_sosta = tm.TotalSeconds;
            if (s.TipoVeicolo == "AUTOVEICOLO")
            {
                prezzo = tempo_sosta * prezzo_unitario_auto;

            } else if (s.TipoVeicolo =="MOTOCICLO")
            {
                prezzo = tempo_sosta * prezzo_unitario_moto;
            }

            prezzo = Math.Round(prezzo, 2);
            return prezzo;

        }

        public double CalcoloTempoMedioSosta(List<Sosta> LstVeic)
        {
            TimeSpan TempoSosta = new TimeSpan();
            double TempoMedioSosta = 0, TimeSum = 0;

            int TotMoto = CalcoloPostiMotoOccupati(LstVeic);

            foreach (var vehic in LstVeic)
            {
                TempoSosta = vehic.DataOraUscita.Value.Subtract(vehic.DataOraAccesso); //equivale a [OraUscita - OraAccesso]
                TimeSum += TempoSosta.TotalMinutes;                 
            }

            TempoMedioSosta = (TimeSum / LstVeic.Count); //Calcolo media - (somma dei temi / totale veicoli)
            TempoMedioSosta = Math.Round(TempoMedioSosta, 0);

            return TempoMedioSosta;
            
            /*
            lblTempoMedioTot.Content = TempoMedioSosta;
            lblTempoMedioAuto.Content = MediaCar;
            lblTempoMedioMoto.Content = MediaMoto;
            */
        }

        public double CalcoloTempoMedioSostaAuto (List<Sosta> ListaVeicoli)
        {
            double MediaAuto = 0, TimeSum = 0;            
            TimeSpan TempoSostaAuto = new TimeSpan();
            int TotAuto = CalcoloPostiAutoOccupati(ListaVeicoli);

            foreach (var vehic in ListaVeicoli)
            {
                if (vehic.TipoVeicolo == "AUTOVEICOLO")
                {
                    TempoSostaAuto = vehic.DataOraUscita.Value.Subtract(vehic.DataOraAccesso); //equivale a OraUscita - OraAccesso
                    TimeSum += TempoSostaAuto.TotalMinutes; //somma delle ore e frazioni (valore var precedente) alla var double 
                }
            }

            MediaAuto = TimeSum / TotAuto;
            MediaAuto = Math.Round(MediaAuto, 0);

            return MediaAuto;

        }

        public double CalcoloTempoMedioSostaMoto(List<Sosta> ListaVeicoli)
        {
            double MediaMoto = 0, TimeSum = 0;
            TimeSpan TempoSostaMoto = new TimeSpan();
            int TotMoto = CalcoloPostiMotoOccupati(ListaVeicoli);

            foreach (var vehic in ListaVeicoli)
            {
                if (vehic.TipoVeicolo == "MOTOCICLO")
                {
                    TempoSostaMoto = vehic.DataOraUscita.Value.Subtract(vehic.DataOraAccesso); //equivale a OraUscita - OraAccesso
                    TimeSum += TempoSostaMoto.TotalMinutes; //somma delle ore e frazioni (valore var precedente) alla var double 
                }
            }

            MediaMoto = TimeSum / TotMoto;
            MediaMoto = Math.Round(MediaMoto, 0);

            return MediaMoto;

        }

        public int CalcoloPostiMotoOccupati(List<Sosta> VeicoliSosta)
        {
            int tot_moto = 0;

            foreach (var veic in VeicoliSosta)
            {
                if (veic.TipoVeicolo == "MOTOCICLO")
                    tot_moto++;
            }
            return tot_moto;
        }

        public int CalcoloPostiAutoOccupati(List<Sosta> VeicoliSosta)
        {
            int tot_auto = 0;

            foreach (var veic in VeicoliSosta)
            {
                if (veic.TipoVeicolo == "AUTOVEICOLO")
                    tot_auto++;
            }

            return tot_auto;
        }

    }
}
