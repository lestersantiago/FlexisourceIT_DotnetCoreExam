using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace opg_201910_interview.Contracts.Responses
{
    public class ClientFileResult
    {
        public string FileName { get; set; }
        public string Name { get; set; }
        public decimal NameSortOrder { get; set; }
        public DateTime DateFromFileName { get; set; }
        public DateTime ProcessedOn { get; set; }
        public int Id { get; set; }
    }
}
