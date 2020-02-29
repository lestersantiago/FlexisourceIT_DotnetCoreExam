using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace opg_201910_interview.Core.Models
{
    public class ClientFileName
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameSuffix { get; set; }
        public decimal SortOrder { get; set; }
        public long ClientId { get; set; }
    }
}
