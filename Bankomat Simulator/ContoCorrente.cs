using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BankomatSimulator
{
    public class ContoCorrente
    {
        private long _idContoCorrente;
        private DateTime _dataUltimoVersamento;
       
        public ContoCorrente(long idContoCorrente)
        {
            _idContoCorrente = idContoCorrente;
            DateTime date = DateTime.Now;
            _dataUltimoVersamento = date.AddDays(-1);
        }

        public struct DatiReport
        {
            public double saldo;
            public DateTime dataUltimoVersamento;
            public DateTime dataCorrente;
        }

        public double Saldo
        {
            get
            {
                using (var connection = new BankomatEntities())
                {
                    return connection.ContiCorrente.Find(_idContoCorrente).Saldo;
                }
            }
            set
            {
                using (var connection = new BankomatEntities())
                {
                    connection.ContiCorrente.Find(_idContoCorrente).Saldo = (int)value;
                    connection.SaveChanges();
                }
            }
        }

        public long IdContoCorrente { get => _idContoCorrente; set => _idContoCorrente = value; }

        /// <summary>
        /// Versa, nel conto corrente, la quantità indicata
        /// </summary>
        /// <param name="quantita"></param>
        /// <returns></returns>
        public void Versamento(double quantita)
        {
            Saldo += quantita;
            Saldo = Math.Round(Saldo, 4, MidpointRounding.AwayFromZero);
            _dataUltimoVersamento = DateTime.Now;


        }


        /// <summary>
        /// Sottrae dal saldo la quanità indicata.
        /// </summary>
        /// <returns></returns>
        public bool Prelievo(double quantita)
        {
            bool isOk = true;
            Saldo = Math.Round(Saldo, 4, MidpointRounding.AwayFromZero);
            if (Saldo >= quantita)
            {
                Saldo -= quantita;
                
            }
            else
                isOk = false;

            return isOk;
        }


        /// <summary>
        /// Popola una struttura <see cref="DatiReport"/> con le statistiche
        /// del conto corrente.
        /// </summary>
        /// <returns></returns>
        public DatiReport ReportSaldo()
        {
            DatiReport datiReport = new DatiReport();
            datiReport.saldo = Saldo;
            datiReport.dataUltimoVersamento = _dataUltimoVersamento;
            datiReport.dataCorrente = DateTime.Now;
            return datiReport;
        }

       
        
    }
}
