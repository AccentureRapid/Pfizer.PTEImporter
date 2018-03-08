using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfizer.PTEImporter.Core.Entities
{
    [Table("Employee")]
    public class Employee : Entity
    {
        public string Code { get; set; }
        public string ChineseName { get; set; }
        public string EnglishName { get; set; }
        public int? SupervisorId { get; set; }
        public int? TempSupervisorId { get; set; }
        public DateTime? TempValidBy { get; set; }
        public string CostCenter { get; set; }
        public string LogonId { get; set; }
        public string Domain { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int? Title { get; set; }
        public string ProfessionalTitle { get; set; }
        public bool? IsGceUser { get; set; }
        public string Phone { get; set; }
        public string Contract_Employee { get; set; }
        public string PfizerLocationCode { get; set; }
        public string PfizerLocationDescription { get; set; }
        public string CityLocationCode { get; set; }
        public string CityLocationDescription { get; set; }
        public string WorkedInCityLocation { get; set; }
        public byte RecordStatus { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
