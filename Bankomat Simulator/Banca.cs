using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BankomatSimulator.ContoCorrente;

namespace BankomatSimulator
{
 
    class Banca
    {
        public enum Funzionalita
        {
            Uscita = 0,
            ReportSaldo = 1,
            Versamento = 2,
            Prelievo = 3,
            RegistroOperazioni = 4,
        }


        public enum EsitoLogin
        {
            AccessoConsentito,
            UtentePasswordErrati,
            PasswordErrata,
            AccountBloccato
        }


        private string _nome;
        private List<Utente> _utenti;
        private SortedList<int,Funzionalita> _funzionalita;


        private Utente _utenteCorrente;

        public string Nome { get => _nome; set => _nome = value; }

        public List<Utente> Utenti { get => _utenti; set => _utenti = value; }
        public Utente UtenteCorrente { get => _utenteCorrente; set => _utenteCorrente = value; }
        internal SortedList<int, Funzionalita> ElencoFunzionalita { get => _funzionalita; set => _funzionalita = value; }

        /// <summary>
        /// Verifica che sia presente un utente con NomeUtente uguale a quello indicato
        /// e, nel caso, verifica la password.
        /// </summary>
        /// <param name="credenziali">Dati inseriti dall'utente </param>
        /// <returns></returns>
        public EsitoLogin Login(string username, string password, out Utente utente)
        {
            Utente utenteDaValidare = null;
            //ricerco utente sul 
            utente = null;

            try
            {
                utenteDaValidare = Utenti.Where(x => x.NomeUtente == username).First();
            }
            catch (InvalidOperationException)
            {
                return EsitoLogin.UtentePasswordErrati;
            } 
            
            if (password != utenteDaValidare.Password)
            {
                utente = utenteDaValidare;
                utenteDaValidare.TentativiDiAccessoErrati++;
                if (utenteDaValidare.Bloccato)
                {
                    return EsitoLogin.AccountBloccato;
                }
                return EsitoLogin.PasswordErrata;
            }
            else
            {
                utente = utenteDaValidare;
                if (utenteDaValidare.Bloccato)
                {
                    return EsitoLogin.AccountBloccato;
                }
                utenteDaValidare.TentativiDiAccessoErrati = 0;
                return EsitoLogin.AccessoConsentito;   
            }
        }



    }
}
