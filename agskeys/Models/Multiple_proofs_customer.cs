using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace agskeys.Models
{
    public class Multiple_proofs_customer
    {
        public IEnumerable<proof_table> proof_table { get; set; }
        public IEnumerable<loan_table> loan_table { get; set; }
    }
}