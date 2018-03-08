using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfizer.PTEImporter.Core.Entities
{
    [Table("DEP_EpayRawDataLanding")]
    public class EpayRawDataLanding : Entity
    {
        [NotMapped]
        [Key]
        public override int Id { get => base.Id; set => base.Id = value; }
        [NotMapped]
        public string ReportId { get; set; }
        public string CompanyCode { get; set; }
        public string VendorInvoiceRef { get; set; }
        public string EpayApprovedDate { get; set; }
        public string CostCenter { get; set; }
        public string MaterialGroup { get; set; }
        public string MaterialGroupName { get; set; }
        public string MaterialCategory { get; set; }
        public string ItemStatus { get; set; }
        public string PONumber { get; set; }
        public string CreatedOn { get; set; }
        public string Amount { get; set; }
        public string Epay_Requester { get; set; }
        public string EpayNumber { get; set; }
        public string InvoiceSource { get; set; }
        public string Subject { get; set; }
        public string LineNumber { get; set; }


    }
}
