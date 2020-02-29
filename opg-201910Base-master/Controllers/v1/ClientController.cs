using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using opg_201910_interview.Contracts.Responses;
using opg_201910_interview.Core;

namespace opg_201910_interview.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IClientRepository _clientRepository;
        private const string FILE_FORMAT = ".xml";

        public ClientController(IWebHostEnvironment webHostEnvironment, IClientRepository clientRepository)
        {
            _webHostEnvironment = webHostEnvironment;
            _clientRepository = clientRepository;
        }

        // GET: api/v1/ClientFile
        [HttpGet]
        public IActionResult Get()
        {
            //Get Clients.
            var clients = _clientRepository.GetClients();

            List<ClientResult> clientsResult = new List<ClientResult>();

            foreach (var client in clients)
            {
                var clientFileNames = client.FileNames.OrderBy(a => a.SortOrder).ToList();

                var rootPath = _webHostEnvironment.ContentRootPath;
                var fileDirectoryPath = client.FileDirectoryPath;
                var fullDirectoryPath = rootPath + "/" + fileDirectoryPath + "/";

                //Get Client Files.
                var files = Directory.GetFiles(fullDirectoryPath, "*" + FILE_FORMAT)
                    .Where(a => clientFileNames.Any(b => a.Contains(b.Name)))
                    .Select(a => a.Replace(fullDirectoryPath, string.Empty))
                    .ToList();

                //Process Client Files.
                List<ClientFileResult> clientFilesResult = new List<ClientFileResult>();

                foreach(var clientFileName in clientFileNames)
                {
                    var name = clientFileName.Name;

                    //Process by Name, SortOrder, Date.
                    var tmpClientFiles = (from a in files
                                          where a.Contains(name)
                                          let dateString = a.Replace((name + clientFileName.NameSuffix), string.Empty)
                                                        .Replace(FILE_FORMAT, string.Empty)
                                          let dateFromFileName = DateTime.ParseExact(dateString, client.FileNameDateFormat, null)
                                          orderby dateFromFileName ascending
                                          select new { 
                                              FileName = a,
                                              DateFromFileName = dateFromFileName
                                          }).ToList();

                    //Append to ClientFileResult.
                    for(int i = 0; i < tmpClientFiles.Count; i++)
                    {
                        var item = tmpClientFiles[i];

                        var newClientFileResult = new ClientFileResult
                        {
                            FileName = item.FileName,
                            DateFromFileName = item.DateFromFileName,
                            Name = name,
                            NameSortOrder = clientFileName.SortOrder,
                            ProcessedOn = DateTime.Now,
                            Id = clientFilesResult.Count + 1
                        };

                        clientFilesResult.Add(newClientFileResult);
                    }

                }

                //Append to ClientResult.
                var newClientResult = new ClientResult
                {
                    Id = client.Id,
                    ClientId = client.ClientId,
                    Name = client.Name,
                    SortOrder = client.SortOrder,
                    Files = clientFilesResult
                };

                clientsResult.Add(newClientResult);
            }

            //Serialize to JSON.
            var result = JsonConvert.SerializeObject(new
            {
                Clients = clientsResult
            },
                Formatting.None,
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    DateFormatString = "MM/dd/yyyy hh:mm:ss tt"
                });

            return Ok(result);
        }
    }
}
