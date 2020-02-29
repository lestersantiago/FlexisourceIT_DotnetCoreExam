using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace opg_201910_interview.Core.Models
{
    public class Client
    {
        public long Id { get; set; }
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string FileDirectoryPath { get; set; }
        public string FileNameDateFormat { get; set; }
        public decimal SortOrder { get; set; }
        public IList<ClientFileName> FileNames { get; set; }
    }
}
