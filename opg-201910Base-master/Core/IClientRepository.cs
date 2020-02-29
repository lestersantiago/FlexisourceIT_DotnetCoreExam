using opg_201910_interview.Core.Models;
using System.Collections.Generic;

namespace opg_201910_interview.Core
{
    public interface IClientRepository
    {
        IList<Client> GetClients();
    }
}