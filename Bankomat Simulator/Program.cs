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
        static SqlConnection sqlConnection;

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
                    
                    foreach (var funzione in connection.Banche_Funzionalita.Where(l => l.IdBanca == banca.Id))
                    {
                        y.Add((int)funzione.Id, (Banca.Funzionalita)funzione.IdFunzionalita);
                    }
                    
                    x.Nome = banca.Nome;
                    x.ElencoFunzionalita = y;
                    
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
