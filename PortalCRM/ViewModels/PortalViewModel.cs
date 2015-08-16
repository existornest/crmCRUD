using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortalCRM.ViewModels
{
    public class PortalViewModel
    {
        public Guid ID { get; set; }
        public string Nazwa { get; set; }
        public string Autor { get; set; }
        public string Ranking { get; set; }
        public string Wydawca { get; set; }
    }
}