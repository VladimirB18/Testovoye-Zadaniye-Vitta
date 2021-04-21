namespace Testovoe_Zadaniye
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Payments
    {
        public long Id { get; set; }

        public long IdOfOrder { get; set; }

        public long? IdOfMoneyIncome { get; set; }

        public double Payment { get; set; }

        public virtual MoneyIncome MoneyIncome { get; set; }

        public virtual Orders Orders { get; set; }
    }
}
