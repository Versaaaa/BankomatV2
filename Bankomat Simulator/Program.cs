using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace BankomatSimulator
{
    class Program
    {
        static SortedList<int, Banca> banche;

        /// <summary>
        /// Funzione di inizializzazione del Bankomat Simulator.
        /// </summary>
        private static void Inizializza()
        {
            using(var connection = new BankomatEntities())
            {
                banche = new SortedList<int, Banca>();
                foreach (var banca in connection.Banche)
                {
                    var x = new Banca();
                    var y = new SortedList<int, Banca.Funzionalita>();
                    var z = new List<Utente>();

                    int pointer = 0;
                    foreach (var funzione in connection.Banche_Funzionalita.Where(l => l.IdBanca == banca.Id))
                    {
                        pointer++;
                        y.Add(pointer, (Banca.Funzionalita)funzione.IdFunzionalita);
                    }
                    foreach (var utente in connection.Utenti.Where(i => i.IdBanca == banca.Id))
                    {
                        z.Add(new Utente((int)utente.Id) { contoCorrente = new ContoCorrente(connection.ContiCorrente.Where(k=> k.IdUtente == utente.Id).First().Id)});
                    }

                    x.Nome = banca.Nome;
                    x.ElencoFunzionalita = y;
                    x.Utenti = z;
                    
                    banche.Add((int)banca.Id, x);
                }
            }
        }
        static void Main(string[] args)
        {
            Inizializza();
            InterfacciaUtente interfacciaUtente = new InterfacciaUtente(banche);
            interfacciaUtente.Esegui();
        }
    }
}
