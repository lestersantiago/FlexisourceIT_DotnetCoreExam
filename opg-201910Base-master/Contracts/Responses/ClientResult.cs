using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace opg_201910_interview.Contracts.Responses
{
    public class ClientResult
    {
        public long Id { get; set; }
        public string ClientId { get; set; }
        public string Name { get; set; }
        public decimal SortOrder { get; set; }
        public IList<ClientFileResult> Files { get; set; }
    }
}
