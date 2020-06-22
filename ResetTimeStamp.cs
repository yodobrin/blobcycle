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
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace blobcycle
{
    public static class ResetTimeStamp
    {
        [FunctionName("ResetTimeStamp")]
        public static async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            string connectionString = Environment.GetEnvironmentVariable("SA_CS");
            string containerName = Environment.GetEnvironmentVariable("SA_CONTAINER");
            dynamic eventContent = JsonConvert.DeserializeObject(eventGridEvent.Data.ToString());
            string blobUrl = eventContent?.url;

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);    
            log.LogInformation("calling get blob async");
            
            string path = getPath(blobUrl,log);
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
