# Parking Manager
Progetto di verifica del corso Sviluppo OOP in C# (FITSTIC) - prof. Matteo Venturi

<b>CLASSI OOP</b>
- <i>Veicolo.cs</i>: classe base;
- <i> Automobile.cs </i>: classe derivata;
- <i> Motociclo.cs </i>: classe derivata;
- <i> Sosta.cs </i>: classe che definisce la stutta informativa della sosta (TipoVeicolo, Targa, OraIn, OraOut, IdPosto) 
- <i> Parcheggio.cs </i>: metodi per registrazione ingresso/uscita veicolo dal park e aggiornamento informazioni sullo stato del park;

<i>MainWindows.cs</i>: code-behind con gestori eventi e calcolo stato generale parcheggio (semaforo) 


Finestra MainWindow.xaml
- Form inserimento veicolo (targa, nome proprietario, tipo veicolo)
- DataGrid lista veicoli in sosta 
- Contatori posti occupati per tipo veicolo
- Contatore tempo medio sosta per tipo veicolo (calcolo effettuato anche con dati presenti in ArchivioVeicoli
- Contatore totale incasso
- Semafori stato parcheggio (<i>da correggere</i>)
