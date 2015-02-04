//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.WindowsAzurePack.Samples.HelloWorld.ApiClient.DataContracts;
using System.Globalization;

namespace Microsoft.WindowsAzurePack.Samples.HelloWorld.Api.Controllers
{
    public class FileServersController : ApiController
    {        
        public static List<FileServer> fileServers;

        static FileServersController()
        {
            fileServers = new List<FileServer>();
            fileServers.Add(new FileServer { FileServerId = 1, FileServerName = "Server 2012", TotalSpace = 1000, DefaultSize = 200, FreeSpace = 1000 });
            fileServers.Add(new FileServer { FileServerId = 2, FileServerName = "Server 2012 R2", TotalSpace = 500, DefaultSize = 50, FreeSpace = 500 });
            fileServers.Add(new FileServer { FileServerId = 3, FileServerName = "Server 2010", TotalSpace = 800, DefaultSize = 100, FreeSpace = 800 });
        }

        [HttpGet]
        public List<FileServer> GetServerList()
        {
            return fileServers;
        }

        [HttpPut]
        public void UpdateServer(FileServer server)
        {
            if (server == null)
            {
                throw Utility.ThrowResponseException(this.Request, System.Net.HttpStatusCode.BadRequest, ErrorMessages.FileServerEmpty);
            }

            var fileServer = (from s in fileServers where s.FileServerId == server.FileServerId select s).FirstOrDefault();

            if (fileServer != null)
            {
                string message = string.Format(CultureInfo.CurrentCulture, ErrorMessages.FileServerNotFound, server.FileServerName);
                throw Utility.ThrowResponseException(this.Request, System.Net.HttpStatusCode.BadRequest, message);
            }
            else
            {
                fileServer.FileServerName = server.FileServerName;
                fileServer.TotalSpace = server.TotalSpace;
                fileServer.DefaultSize = server.DefaultSize;
                fileServer.FreeSpace = server.FreeSpace;
            }
        }

        [HttpPost]
        public void AddServer(FileServer server)
        {
            if (server == null)
            {
                throw Utility.ThrowResponseException(this.Request, System.Net.HttpStatusCode.BadRequest, ErrorMessages.FileServerEmpty);
            }

            var fileServer = (from s in fileServers where s.FileServerName == server.FileServerName select s).FirstOrDefault();

            if (fileServer != null)
            {
                string message = string.Format(CultureInfo.CurrentCulture, ErrorMessages.FileServerAlreadyExist, server.FileServerName);
                throw Utility.ThrowResponseException(this.Request, System.Net.HttpStatusCode.BadRequest, message);
            }           

            fileServers.Add(new FileServer
            {
                FileServerId = fileServers.Count,
                FileServerName = server.FileServerName,
                TotalSpace = server.TotalSpace,
                DefaultSize = server.DefaultSize,
                FreeSpace = server.FreeSpace
            });
        }
    }
}