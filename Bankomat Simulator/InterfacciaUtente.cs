
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankomatSimulator
{
   
    class InterfacciaUtente
    {
        enum Richiesta
        {
            SchermataDiBenvenuto,
            Login,
            MenuPrincipale,
            Versamento,
            ReportSaldo,
            Prelievo,
            Uscita
        };

        private SortedList<int, Banca> _banche;
        private Banca _bancaCorrente;

        public InterfacciaUtente(SortedList<int,Banca> banche)
        {
            _banche = banche;
        }

        /// <summary>
        /// Stampa l'intestazione del menu
        /// </summary>
        private void StampaIntestazione(string titoloMenu)
        {
            Console.Clear();
            Console.WriteLine("**************************************************");
            Console.WriteLine("*              Bankomat Simulator               *");
            Console.WriteLine("**************************************************");
            Console.WriteLine("".PadLeft((50 - titoloMenu.Length) / 2) 
                + titoloMenu);
            Console.WriteLine("--------------------------------------------------");
            return;
        }

        /// <summary>
        /// Gestisce la scelta dell'utente. Verfica che l'input sia una scelta
        /// compresa nell'intervallo <paramref name="min"/> - <paramref name="max"/>
        /// </summary>
        /// <returns>Il numero inserito dall'utente, -1 se la scelta non è consentita</returns>
        private int ScegliVoceMenu(int min, int max)
        {
            string rispostaUtente;

            
            Console.Write("Scelta: ");
            rispostaUtente = Console.ReadKey().KeyChar.ToString();
            if (!Int32.TryParse(rispostaUtente, out int scelta) || 
                !(min <= scelta && scelta <= max) )
            {
                scelta = -1;
                Console.WriteLine("");
                Console.WriteLine($"Scelta non consentita - {rispostaUtente}" );
                Console.Write("Premere un tasto per proseguire");
                Console.ReadKey();
            }           
            return scelta;
        }

        /// <summary>
        /// Mostra la maschera di selezione della banca.
        /// la scelta viene effettuata tra quelle presenti in <see cref="_banche"/>
        /// </summary>
        /// <returns>la scelta dell'utente - 0 per uscire</returns>
        private int SchermataDiBenvenuto()
        {
            int scelta = -1;
            while (scelta == -1)
            {
                StampaIntestazione("Selezione Banca");

                foreach (var banca in _banche)
                {
                    Console.WriteLine($"{banca.Key.ToString()} - {banca.Value.Nome}");
                }
                Console.WriteLine("0 - Uscita");

                scelta = ScegliVoceMenu(0, _banche.Count);
            }

            return scelta;
        
        }

        /// <summary>
        /// Mostra la maschera di Login. I dati inseriti vengono validati 
        /// rispetto a quelli presenti in <see cref="_banche"/> 
        /// </summary>
        /// <returns>
        /// <see langword="true"/> se l'utente è stato riconosciuto,
        /// <see langword="false"/> altrimenti
        /// </returns>
        private bool Login()
        {
            bool autenticato = false;
            
            StampaIntestazione($"Login - {_bancaCorrente.Nome}");

            Console.Write("Nome utente: ");
            var username = Console.ReadLine();
            Console.Write("Password: ");
            var password = Console.ReadLine();

            Banca.EsitoLogin esitoLogin =
                _bancaCorrente.Login(username, password, out Utente utente);

            switch (esitoLogin)
            {
                case Banca.EsitoLogin.PasswordErrata:
                    Console.WriteLine($"Password errata - " +
                        $"{utente.TentativiDiAccessoResidui} " +
                        @"tentativ{0} residu{0}", utente.TentativiDiAccessoResidui == 1 ? "o" : "i" );
                    Console.Write("Premere un tasto per proseguire");
                    Console.ReadKey();
                    break;
                case Banca.EsitoLogin.UtentePasswordErrati:
                    Console.WriteLine("Utente o password errati");
                    Console.Write("Premere un tasto per proseguire");
                    Console.ReadKey();
                    break;
                case Banca.EsitoLogin.AccountBloccato:
                    Console.WriteLine("*** Account utente bloccato ***");
                    Console.Write("Premere un tasto per proseguire");
                    Console.ReadKey();
                    break;
                case Banca.EsitoLogin.AccessoConsentito:
                    _bancaCorrente.UtenteCorrente = utente;
                    autenticato = true;
                    break;
            }
            
            return autenticato;
        }

        /// <summary>
        /// Mostra il menu principale. Viene proposta la scelte tra le funzionalita
        /// <see cref="Banca.ElencoFunzionalita"/> disponibili per
        /// l'utente selezionato
        /// </summary>
        /// <returns></returns>
        private Banca.Funzionalita MenuPrincipale()
        {
            int scelta = -1;
            
            while (scelta == -1)
            {
                StampaIntestazione($"Menu principale - {_bancaCorrente.Nome}");

                foreach (var funzionalita in _bancaCorrente.ElencoFunzionalita)
                {
                    Console.WriteLine($"{funzionalita.Key.ToString()} - {funzionalita.Value.ToString()}");
                }
                Console.WriteLine("0 - Uscita");

                scelta = ScegliVoceMenu(0, _bancaCorrente.ElencoFunzionalita.Count);
            }

            return scelta == 0 ? 
                Banca.Funzionalita.Uscita :
                _bancaCorrente.ElencoFunzionalita[scelta];
        }

      /// <summary>
      /// Permette all'utente di effettuare un versamento nel conto corrente.
      /// </summary>
      /// <returns></returns>
        private bool Versamento()
        {
            string risposta;
            ContoCorrente contoCorrente = _bancaCorrente.UtenteCorrente.contoCorrente;

            StampaIntestazione($"Versamento - {_bancaCorrente.Nome}");
            Console.Write("Inserisci l'importo del versamento: ");
            
            risposta = Console.ReadLine();
            if (!double.TryParse(risposta, out double importoVersamento)) { 
                Console.WriteLine("Operazione annullata - Inserire un numero");
                Console.Write("Premere un tasto per proseguire");
                Console.ReadKey();
                return false;
            }

            contoCorrente.Versamento(importoVersamento);

             Console.WriteLine($"Operazione completata - Conteggio saldo: " +
                    $"{contoCorrente.Saldo} ");

            Console.Write("Premere un tasto per proseguire");
            Console.ReadKey();
            return true;
        }

        /// <summary>
        /// Mostra all'utente il report del conto corrente
        /// </summary>
        /// <returns></returns>
        private void ReportSaldo()
        {

            ContoCorrente contoCorrente = _bancaCorrente.UtenteCorrente.contoCorrente;
            StampaIntestazione($"Report saldo - {_bancaCorrente.UtenteCorrente.NomeUtente}");

            ContoCorrente.DatiReport report = contoCorrente.ReportSaldo();
            Console.WriteLine($"Saldo conto: {report.saldo}");
            Console.WriteLine($"Data ultimo versamento: {report.dataUltimoVersamento}");
            Console.WriteLine($"Data / ora attuale: {report.dataCorrente}");
          

            Console.Write("Premere un tasto per proseguire");
            Console.ReadKey();

            return;
        }

        /// <summary>
        /// Mostra il menu di prelievo. Viene richiesto all'utente l'importo
        /// da prelevare dal conto corrente
        /// </summary>
        /// <returns></returns>
        private void Prelievo()
        {
            string risposta;
            ContoCorrente contoCorrente = _bancaCorrente.UtenteCorrente.contoCorrente;

            StampaIntestazione($"Prelievo - {_bancaCorrente.Nome}");
            Console.Write("Inserisci l'importo da prelevare: ");

            risposta = Console.ReadLine();
            if (!Double.TryParse(risposta, out double importoDaPrelevare))
            {
                Console.WriteLine("Operazione annullata - Inserire un numero");
                Console.Write("Premere un tasto per proseguire");
                Console.ReadKey();
                return;
            }

            if(contoCorrente.Prelievo(importoDaPrelevare) == false)
            {
                Console.WriteLine($"Impossibile procedere al prelievo: saldo non sufficiente!");
            }
            else
            {
                Console.WriteLine($"Operazione completata - Importo prelevato: " +
                  $"{importoDaPrelevare} ");
            }
       

            Console.Write("Premere un tasto per proseguire");
            Console.ReadKey();
            return;
        }

        ///<summary>
        ///Entry point della classe Interfaccia.
        ///Gestisce la navigazione tra i menu e l'interazione con l'utente
        ///</summary>
        public void Esegui()
        {
            int rispostaUtente = 0;
            Richiesta richiesta = Richiesta.SchermataDiBenvenuto;

            while (richiesta != Richiesta.Uscita)
            {
                switch (richiesta)
                {
                    case Richiesta.SchermataDiBenvenuto:
                        rispostaUtente = SchermataDiBenvenuto();
                        
                        if (rispostaUtente == 0)
                            richiesta = Richiesta.Uscita;
                        else
                        {
                            
                            _bancaCorrente = _banche[rispostaUtente];
                            richiesta = Richiesta.Login;
                        }
                        break;
                    case Richiesta.Login:
                        if (Login())
                            richiesta = Richiesta.MenuPrincipale;
                        else
                            richiesta = Richiesta.SchermataDiBenvenuto;
                        break;
                    case Richiesta.MenuPrincipale:
                        switch ( MenuPrincipale() )
                        {
                            case Banca.Funzionalita.Uscita:
                                richiesta = Richiesta.SchermataDiBenvenuto;
                                break;
                            case Banca.Funzionalita.Versamento:
                                richiesta = Richiesta.Versamento;
                                break;
                            case Banca.Funzionalita.ReportSaldo:
                                richiesta = Richiesta.ReportSaldo;
                                break;
                            case Banca.Funzionalita.Prelievo:
                                richiesta = Richiesta.Prelievo;
                                break;
                        }
                        break;
                    case Richiesta.Versamento:
                        bool esito = Versamento();
                        if (esito && _bancaCorrente.ElencoFunzionalita
                            .ContainsValue(Banca.Funzionalita.ReportSaldo))
                            richiesta = Richiesta.ReportSaldo;
                        else
                            richiesta = Richiesta.MenuPrincipale;
                        break;
                    case Richiesta.ReportSaldo:
                        ReportSaldo();
                        richiesta = Richiesta.MenuPrincipale;
                        break;
                    case Richiesta.Prelievo:
                        Prelievo();
                        richiesta = Richiesta.MenuPrincipale;
                        break;                
                    default:
                        break;
                }
            }
        }
    }
}
