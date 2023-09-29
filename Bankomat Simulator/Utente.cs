using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankomatSimulator
{
    class Utente
    {

        private int _id;

        private int _tentativiDiAccessoErrati = 0;
        private const int _tentativiDiAccessoPermessi = 3;

        private ContoCorrente _contoCorrente;

        public string NomeUtente
        {
            get
            {
                using (var connection = new BankomatEntities())
                {
                    return connection.Utenti.Find(_id).NomeUtente;
                }
            }
            set
            {
                using (var connection = new BankomatEntities())
                {
                    connection.Utenti.Find(_id).NomeUtente = value;
                    connection.SaveChanges();
                }
            }
        }
        public string Password
        {
            get
            {
                using (var connection = new BankomatEntities())
                {
                    return connection.Utenti.Find(_id).Password;
                }
            }
            set
            {
                using (var connection = new BankomatEntities())
                {
                    connection.Utenti.Find(_id).Password = value;
                    connection.SaveChanges();
                }
            }
        }
        public bool Bloccato
        {
            get
            {
                using (var connection = new BankomatEntities())
                {
                    return connection.Utenti.Find(_id).Bloccato;
                }
            }
        }

        public ContoCorrente contoCorrente { get => _contoCorrente; set => _contoCorrente = value; }

        public Utente(int id)
        {
            _id = id;
        }

        public int TentativiDiAccessoResidui
        {
            get
            {
                return _tentativiDiAccessoPermessi - _tentativiDiAccessoErrati;
            }
        }
        public int TentativiDiAccessoErrati
        {
            get => _tentativiDiAccessoErrati;
            set
            {
                _tentativiDiAccessoErrati = value;
                if (_tentativiDiAccessoErrati >= _tentativiDiAccessoPermessi)
                {
                    using (var connection = new BankomatEntities())
                    {
                        connection.Utenti.Find(_id).Bloccato = true;
                        connection.SaveChanges();
                    }
                }
            }
        }      
    }
}
