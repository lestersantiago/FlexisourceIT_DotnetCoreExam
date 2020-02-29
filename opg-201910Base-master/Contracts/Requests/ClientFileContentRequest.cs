using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace opg_201910_interview.Contracts.Requests
{
    public class ClientFileContentRequest
    {
        public long ClientId { get; set; }
        public string FileName { get; set; }
    }
}
