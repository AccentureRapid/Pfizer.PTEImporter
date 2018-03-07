using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfizer.PTEImporter.Core.Entities
{
    public class EpayRawDataLanding : Entity
    {
        [NotMapped]
        public override int Id { get => base.Id; set => base.Id = value; }
        public string ReportId { get; set; }
    }
}
