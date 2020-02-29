using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using opg_201910_interview.Contracts.Requests;
using opg_201910_interview.Contracts.Responses;
using opg_201910_interview.Core;

namespace opg_201910_interview.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IClientRepository _clientRepository;
        private const string FILE_FORMAT = ".xml";
        private string _errorMessage = "Invalid inputs.";

        public ClientsController(IWebHostEnvironment webHostEnvironment, IClientRepository clientRepository)
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

                    //Filter by name.
                    var tmpClientFiles = (from a in files
                                          where a.Contains(name)
                                          let dateString = a.Replace((name + clientFileName.NameSuffix), string.Empty)
                                                            .Replace(FILE_FORMAT, string.Empty)
                                          select a).ToList();

                    List<dynamic> dynClientFiles = new List<dynamic>();
                    foreach (var item in tmpClientFiles)
                    {
                        string dateString = item.Replace((name + clientFileName.NameSuffix), string.Empty)
                                                    .Replace(FILE_FORMAT, string.Empty);
                        DateTime dateFromFileName;
                        DateTime.TryParseExact(dateString, client.FileNameDateFormat,
                                                System.Globalization.CultureInfo.InvariantCulture,
                                                System.Globalization.DateTimeStyles.None, out dateFromFileName);

                        //Filter by proper date format.
                        if(dateFromFileName != DateTime.MinValue)
                        {
                            dynClientFiles.Add(new
                            {
                                FileName = item,
                                DateFromFileName = dateFromFileName 
                            });
                        }
                    }

                    //Sort by date for the same names.
                    dynClientFiles = dynClientFiles.OrderBy(a => a.DateFromFileName).ToList();

                    //Append to ClientFileResult.
                    foreach(var item in dynClientFiles)
                    {
                        clientFilesResult.Add(new ClientFileResult
                        {
                            FileName = item.FileName,
                            DateFromFileName = item.DateFromFileName,
                            Name = name,
                            NameSortOrder = clientFileName.SortOrder,
                            ProcessedOn = DateTime.Now,
                            Id = clientFilesResult.Count + 1
                        });
                    }
                }

                //Append to ClientResult.
                clientsResult.Add(new ClientResult
                {
                    Id = client.Id,
                    ClientId = client.ClientId,
                    Name = client.Name,
                    SortOrder = client.SortOrder,
                    Files = clientFilesResult
                });
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
                    DateFormatString = "MM/dd/yyyy hh:mm:ss.ffffff tt"
                });

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ClientFileContentRequest request)
        {
            bool isValidInputs = true;

            if(request == null
                || request.ClientId < 1 
                || request.FileName.Trim().Length < 1)
            {
                isValidInputs = false;
            }

            if (isValidInputs == false)
            {
                return Ok(JsonConvert.SerializeObject(new
                {
                    ErrorMessage = _errorMessage
                }));
            }


            long clientId = request.ClientId;
            string fileName = request.FileName;

            //Get Clients.
            var clients = _clientRepository.GetClients();
            var client = clients.Where(a => a.Id == clientId).SingleOrDefault();

            string content = "";

            if(client != null)
            {
                var rootPath = _webHostEnvironment.ContentRootPath;
                var fileDirectoryPath = client.FileDirectoryPath;
                var fullDirectoryPath = rootPath + "/" + fileDirectoryPath + "/";

                content = System.IO.File.ReadAllText(fullDirectoryPath + fileName);
            }

            //Serialize to JSON.
            var result = JsonConvert.SerializeObject(new
            {
                FileContent = content
            },
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            return Ok(result);

        }
    }
}
