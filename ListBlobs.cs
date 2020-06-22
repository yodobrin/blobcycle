/*
Sample Code is provided for the purpose of illustration only and is not intended to be used in a production environment.
THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, 
INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
We grant You a nonexclusive, royalty-free right to use and modify the Sample Code and to reproduce and distribute the object code form of the Sample Code, provided that. 
You agree: 
	(i) to not use Our name, logo, or trademarks to market Your software product in which the Sample Code is embedded;
    (ii) to include a valid copyright notice on Your software product in which the Sample Code is embedded; and
	(iii) to indemnify, hold harmless, and defend Us and Our suppliers from and against any claims or lawsuits, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code
**/

// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;

namespace blobcycle
{
    public static class ListBlobs
    {
        [FunctionName("ListBlobs")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string connectionString = Environment.GetEnvironmentVariable("SA_CS");
            string containerName = Environment.GetEnvironmentVariable("SA_CONTAINER");

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);    

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            string path = getPath(name,log);
            log.LogInformation($"calling to extract path {path}");
            var dic = new Dictionary <string,string>();
            DateTime now = new DateTime();
            dic.Add("lastmodifydate",now.ToString());

            await foreach (BlobHierarchyItem blobItem in containerClient.GetBlobsByHierarchyAsync(Azure.Storage.Blobs.Models.BlobTraits.None,Azure.Storage.Blobs.Models.BlobStates.None,path))
            {
                log.LogInformation($"inner loop of blobs: {blobItem.Blob.Name}\n {blobItem.Blob.Properties.LastModified}");
                BlobClient bclient = containerClient.GetBlobClient(blobItem.Blob.Name);
                bclient.SetMetadata(dic);
            }

            return new OkObjectResult("yipikaye");
        }
        private static string getPath(string blobUrl, ILogger log)
        {
            int loc = blobUrl.LastIndexOf("/");
            string path = blobUrl.Substring(0,loc);
            log.LogInformation($"got: {blobUrl} \nindex:{loc}\n new path:{path}");
            
            return path;
        } 
    }
}
