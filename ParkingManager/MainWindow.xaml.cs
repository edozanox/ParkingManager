using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

//BUG NOTI DA SISTEMARE ENTRO IL 06/05/2020
//1. Non funziona il ricalcolo stato parcheggio in fase di uscita del veicolo (il semaformo non cambia stato).

namespace ParkingManager
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Parcheggio park = new Parcheggio();
        public enum TipoVeicolo { AUTOVEICOLO, MOTOCICLO };
        public int MAX_POSTI_MOTO;
        public int MAX_POSTI_AUTO;
        public int posti_auto_occ = 0, posti_moto_occ = 0;        
        public double prezzo_moto_ora, prezzo_auto_ora, totIncasso, costo;
        public bool IsOK = false;

        public MainWindow()
        {
            InitializeComponent();
            lblData.Content = DateTime.Today.Date.ToShortDateString();
            VisualizzaTempiMediSosta();
                     
            dgVeicoliInSosta.ItemsSource = park.VeicoliInSosta;


            foreach (var tipo in Enum.GetValues(typeof(TipoVeicolo)))
            {
                cbTipoMezzo.Items.Add(tipo);
            }
        }

        private void btnAccessoVeicolo_Click(object sender, RoutedEventArgs e)
        {
            IsOK = false;
            string txt_targa = tbTarga.Text;
            string txt_proprietario = tbNomeProp.Text;
            string txt_tipo = cbTipoMezzo.SelectedItem.ToString();

            if (cbTipoMezzo.SelectedItem.Equals(TipoVeicolo.AUTOVEICOLO))
            {
                if (posti_auto_occ < MAX_POSTI_AUTO)
                {
                    IsOK = true;
                    Veicolo new_auto = new Automobile(txt_targa, txt_proprietario, txt_tipo);
                    park.MezzoInArrivo(new_auto);
                    posti_auto_occ++;
                    lblContatoreAuto.Content = posti_auto_occ;
                    CalcoloStatoParcheggioAuto();                    
                }
                else
                    MessageBox.Show("POSTI AUTOVEICOLI TERMINATI", "Posti non disponibili", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else
            {
                if (posti_moto_occ < MAX_POSTI_MOTO)
                {
                    IsOK = true;
                    Veicolo new_moto = new Motociclo(txt_targa, txt_proprietario, txt_tipo);
                    park.MezzoInArrivo(new_moto);
                    posti_moto_occ++;
                    lblContatoreMoto.Content = posti_moto_occ;
                    CalcoloStatoParcheggioMoto();                    
                }
                else
                    MessageBox.Show("POSTI MOTOCICLI TERMINATI", "Posti non disponibili", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (IsOK)
            {
                MessageBox.Show("VEICOLO CON TARGA " + txt_targa + "  INSERITO.", DateTime.Now + " - VEICOLO INSERITO");
                lblContatoreTot.Content = park.VeicoliInSosta.Count().ToString();
            }
           

            dgVeicoliInSosta.Items.Refresh(); //Aggiorno la vista data-grid veicoli in sosta
            SalvaDatiParcheggio(park.VeicoliInSosta);
            
            //Reset campi form
            tbTarga.Clear();
            tbNomeProp.Clear();
            cbTipoMezzo.SelectedIndex = -1;
        }

        public void btnUscitaVeicolo_Click(object sender, RoutedEventArgs e)
        {
            if (dgVeicoliInSosta.Items.Count > 0 )
            { 
            
                Sosta fine_sosta = (Sosta)dgVeicoliInSosta.SelectedItem;        //Selezione dell'elemento corrispondente alla riga del DataGrid                                                                            

                if (fine_sosta.TipoVeicolo == "MOTOCICLO")                
                    costo = park.MezzoInUscita(fine_sosta, prezzo_moto_ora); 
                else                
                    costo = park.MezzoInUscita(fine_sosta, prezzo_auto_ora);   

                grdManage.Visibility = Visibility.Hidden;
                grdInfoOut.Visibility = Visibility.Visible;
                
                //Compilazione campi grid INFO_OUT
                tbTargaOut.Text = fine_sosta.TargaVeicolo;                
                tbOraIn.Text = fine_sosta.DataOraAccesso.ToString();
                tbOraOut.Text = fine_sosta.DataOraUscita.ToString();
                tbImportoOut.Text = costo.ToString();

                totIncasso += costo;                                         // L'importo calcolato nel metodo MezzoInUscita viene aggiunto alla variabile contatore incasso...
                lblTotIncasso.Content = "€ " + totIncasso;                  //...e relativo aggiornamento lbl
               
                dgVeicoliInSosta.Items.Refresh();                           //Aggiorno vista DataGrid
                
                park.VeicoliInSosta.Remove(fine_sosta);                      //L'oggetto viene rimosso dalla lista VeicoliInSosta...
                park.ArchivioVeicoli.Add(fine_sosta);                       //...e aggiunto alla lista ArchivioVeicoli

                CalcoloStatoParcheggioAuto();
                CalcoloStatoParcheggioMoto();

                //sezione  AGGIORNAMENTO CONTATORI (richiamo ai relativi metodi)
                lblContatoreMoto.Content = park.CalcoloPostiMotoOccupati(park.VeicoliInSosta).ToString();
                lblContatoreAuto.Content = park.CalcoloPostiAutoOccupati(park.VeicoliInSosta).ToString();
                lblContatoreTot.Content = park.VeicoliInSosta.Count();                            //Aggiorno contatore tot                             
                VisualizzaTempiMediSosta();

                //sezione SALVATAGGI
                SalvaArchivioPark(park.ArchivioVeicoli); //Salvataggio archivio old su file .dat
                SalvaDatiParcheggio(park.VeicoliInSosta); //Salvataggio archivio now su file .dat  
            }
            else            
                MessageBox.Show("NESSUN VEICOLO IN SOSTA!", "Impossibile eseguire il comando", MessageBoxButton.OK, MessageBoxImage.Error);                    
                
        }    

        public void btnReset_Click(object sender, RoutedEventArgs e)
        {
            var selected_option = MessageBox.Show("Sei sicuro di voler eliminare tutti i dati sui veicoli attualmente inseriti? Il file ArchivioVeicoli rimane inalterato.", "RESET ", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (selected_option == MessageBoxResult.Yes)
            {
                dgVeicoliInSosta.ItemsSource = null;
                dgVeicoliInSosta.Items.Clear();               
                park.ArchivioVeicoli.Clear(); //Reset lista

                //reset lbl contatori
                lblContatoreAuto.Content = 0;
                lblContatoreMoto.Content = 0;
                lblContatoreTot.Content = 0;
                icoSemaforoVerdeMoto.Visibility = Visibility.Visible; //Accensione semaforo verde moto
                icoSemaforoGialloMoto.Visibility = Visibility.Hidden;
                icoSemaforoRossoMoto.Visibility = Visibility.Hidden;
                icoSemaforoVerdeMoto.Visibility = Visibility.Visible; //Accensione semaforo verde auto
                icoSemaforoGialloMoto.Visibility = Visibility.Hidden;
                icoSemaforoRossoMoto.Visibility = Visibility.Hidden;
                lblTempoMedioMoto.Content = "..";
                lblTempoMedioTot.Content = "..";
                lblTempoMedioAuto.Content = "..";
                lblTotIncasso.Content = "€ 0,00";
                MessageBox.Show("ELIMINAZIONE DATI COMPLETATA!", "Operazione critica effettuata", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show("OPERAZIONE ANNULLATA", "Annullamento...", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SalvaDatiParcheggio(List<Sosta> saved_parks)
        {
            IFormatter fr = new BinaryFormatter();
            Stream file_park = new FileStream("VeicoliInSosta.dat", FileMode.Create);
            fr.Serialize(file_park, saved_parks);
            file_park.Close();
        }
        
        private void SalvaArchivioPark(List<Sosta> saved_old_parks)
        {
            IFormatter fr = new BinaryFormatter();
            Stream file_old_parks = new FileStream("ArchivioVeicoli.dat", FileMode.Create);
            fr.Serialize(file_old_parks, saved_old_parks);
            file_old_parks.Close();
        }
        


        private void CaricaDatiParcheggio(List<Sosta> saved_parks)
        {
            if (File.Exists("VeicoliInSosta.dat") == true)
            {
                IFormatter f = new BinaryFormatter();
                Stream file_park = new FileStream("VeicoliInSosta.dat", FileMode.Open);
                List<Sosta> lst_park = (List<Sosta>)f.Deserialize(file_park);
                saved_parks.AddRange(lst_park);
                file_park.Close();
                MessageBox.Show("FILE VEICOLI CARICATO", "Inizializzazione terminata", MessageBoxButton.OK, MessageBoxImage.None);
            }
            else
            {
                MessageBox.Show("NESSUN FILE DI CARICAMENTO VEICOLI TROVATO", "Init Warning - No data", MessageBoxButton.OK);
            }
        }

        private void btnConfExit_Click(object sender, RoutedEventArgs e)
        {
            grdInfoOut.Visibility = Visibility.Hidden;
            grdManage.Visibility = Visibility.Visible;
        }

        private void dgVeicoliInSosta_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string headername = e.Column.Header.ToString();
            if (headername == "DataOraAccesso")
            {
                e.Cancel = true;
            }
            if (headername == "IdPosto")
            {
                e.Cancel = true;
            }
            if (headername == "DataOraUscita")
            {
                e.Cancel = true;
            }
        }
    

        private void CaricaArchivioPark(List<Sosta> saved_parks)
        {
            if (File.Exists("ArchivioVeicoli.dat") == true)
            {
                IFormatter f = new BinaryFormatter();
                Stream file_old_park = new FileStream("ArchivioVeicoli.dat", FileMode.Open);
                List<Sosta> lst_old_park = (List<Sosta>)f.Deserialize(file_old_park);
                saved_parks.AddRange(lst_old_park);
                file_old_park.Close();
                //MessageBox.Show("FILE ARCHVIO VEICOLI CARICATO", "ParkManager", MessageBoxButton.OK, MessageBoxImage.None);
            }
            else            
                MessageBox.Show("NESSUN ARHIVIO VEICOLI TROVATO. IMPOSSIBILE CALCOLARE TEMPI DI SOSTA.", "Not Found", MessageBoxButton.OK, MessageBoxImage.Error);            
        }

        private void btnConfInitInfo_Click(object sender, RoutedEventArgs e)
        {
            CaricaDatiParcheggio(park.VeicoliInSosta);
            CaricaArchivioPark(park.ArchivioVeicoli);
            prezzo_auto_ora = double.Parse(tbInitPrezzoAuto.Text);
            prezzo_moto_ora = double.Parse(tbInitPrezzoMoto.Text);
            MAX_POSTI_AUTO = int.Parse(tbInitPostiAuto.Text);
            MAX_POSTI_MOTO = int.Parse(tbInitPostiMoto.Text);            
            //CODICE SPOSTATO DA RIGA 44 
            posti_auto_occ = park.CalcoloPostiAutoOccupati(park.VeicoliInSosta);
            posti_moto_occ = park.CalcoloPostiMotoOccupati(park.VeicoliInSosta);
            CalcoloStatoParcheggioAuto();
            CalcoloStatoParcheggioMoto();
            lblContatoreMoto.Content = posti_moto_occ.ToString();
            lblContatoreAuto.Content = posti_auto_occ.ToString();
            lblContatoreTot.Content = park.VeicoliInSosta.Count().ToString();
            //FINE CODICE SPOSTATO
            grdInit.Visibility = Visibility.Hidden;
            grdManage.Visibility = Visibility.Visible;
        }

        private void CalcoloStatoParcheggioMoto()
        {
            double percStatoParkMoto = 0;            
            percStatoParkMoto = ((double)posti_moto_occ / (double)MAX_POSTI_MOTO) * 100;
            percStatoParkMoto = Math.Round(percStatoParkMoto, 1);
                     
            if(percStatoParkMoto >= 0 && percStatoParkMoto <= 49.9)
            {
                icoSemaforoVerdeMoto.Visibility = Visibility.Visible; //Accensione semaforo verde
                icoSemaforoGialloMoto.Visibility = Visibility.Hidden;
                icoSemaforoRossoMoto.Visibility = Visibility.Hidden;
            }
            else if (percStatoParkMoto >= 50 && percStatoParkMoto<= 99.9)
            {
                icoSemaforoVerdeMoto.Visibility = Visibility.Hidden;
                icoSemaforoGialloMoto.Visibility = Visibility.Visible; //Accensione semaforo arancione
                icoSemaforoRossoMoto.Visibility = Visibility.Hidden;
            }
            else if (percStatoParkMoto > 99.9)
            {
                icoSemaforoVerdeMoto.Visibility = Visibility.Hidden;
                icoSemaforoGialloMoto.Visibility = Visibility.Hidden; 
                icoSemaforoRossoMoto.Visibility = Visibility.Visible; //Accensione semaforo rosso
            }
        }

        private void CalcoloStatoParcheggioAuto()
        {
            double percStatoParkAuto = 0;
            percStatoParkAuto = ((double)posti_auto_occ / (double)MAX_POSTI_AUTO) * 100;
            percStatoParkAuto = Math.Round(percStatoParkAuto, 1);

            if (percStatoParkAuto >= 0 && percStatoParkAuto <= 49.9)
            {
                icoSemaforoVerdeAuto.Visibility = Visibility.Visible; //Accensione semaforo verde
                icoSemaforoGialloAuto.Visibility = Visibility.Hidden;
                icoSemaforoRossoAuto.Visibility = Visibility.Hidden;
            }
            else if (percStatoParkAuto >= 50 && percStatoParkAuto <= 99.9)
            {
                icoSemaforoVerdeAuto.Visibility = Visibility.Hidden;
                icoSemaforoGialloAuto.Visibility = Visibility.Visible; //Accensione semaforo arancione
                icoSemaforoRossoAuto.Visibility = Visibility.Hidden;
            }
            else if (percStatoParkAuto > 99.9)
            {
                icoSemaforoVerdeAuto.Visibility = Visibility.Hidden;
                icoSemaforoGialloAuto.Visibility = Visibility.Hidden;
                icoSemaforoRossoAuto.Visibility = Visibility.Visible; //Accensione semaforo rosso
            }
        }

        public void VisualizzaTempiMediSosta()
        {
            lblTempoMedioTot.Content = park.CalcoloTempoMedioSosta(park.ArchivioVeicoli);   //Ricalcolo tempo medio di sosta 
            lblTempoMedioAuto.Content = park.CalcoloTempoMedioSostaAuto(park.ArchivioVeicoli); //Ricalcolo tempo medio di sosta 
            lblTempoMedioMoto.Content = park.CalcoloTempoMedioSostaMoto(park.ArchivioVeicoli); //Ricalcolo tempo medio di sosta 
        }

    }

}

