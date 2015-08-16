using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalCRM.Models
{
    public class PortalModel
    {
        public int ID { get; set; }
        public string Nazwa { get; set; }
        public string Autor { get; set; }
        public string Ranking { get; set; }
        public string Wydawca { get; set; }
        public string WydawcaID { get; set; }
    }
}