using Microsoft.Extensions.Configuration;
using opg_201910_interview.Core;
using opg_201910_interview.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace opg_201910_interview.Persistence
{
    public class ClientRepository : IClientRepository
    {
        private readonly IConfiguration _iConfiguration;
        public ClientRepository(IConfiguration iConfiguration)
        {
            _iConfiguration = iConfiguration;
        }

        public IList<Client> GetClients()
        {
            var result = _iConfiguration.GetSection("ClientSettings").Get<List<Client>>();

            return result;
        }

    }
}
