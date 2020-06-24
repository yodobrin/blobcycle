# blobcycle

## Use Case
The use case covered in this repository, is as follows: files are orgenized in directories, the blob life cycle managment needs to address changes to files within the directory. However this feature is working at the blob level. Therfore files within the same directory, would not hold the same update stamp as the chaned, new file intreduced to the directory.
Storage does not contain real directories, rather these are files sharing similar prefix.

## What's included?
Function(s) code.

### ResetTimeStamp
The function is called upon created blob using an EventGrid subscription. The function parse the blob url, and perform list blobs sharing the same prefix, than after it iterate over these files and sets meta-data, this change is causing the last update stamp of the file to reflect new time.

### List Blobs
Similar functionality to the ResetTimeStamp, but instead of being triggered by the event grid, it is http triggered, it was used for testing, and it can be used to perform similar activity.

## What's not included?
(or things you will need to care for)
1. Clone this repo, and type `code .` within the created directory. (I use [VS Code](https://code.visualstudio.com/download) as my editor)
2. Create storage account & container. Grab the connection string and replace in `local.settings.json` the entries `SA_CS` and `SA_CONTAINER`.
3. Using the Azure extention for vs code, deploy the function to a new Function App. I recommend on using the advanced wizard.
4. Once the function is deployed, navigate to your newly created storage account, and associate a new event grid subscription with a your newly created function as the Endpoint.

## How to test?
Upload to your storage account files and folders. Note the timestamp of the files once uploaded. upload a new file to an existing folder and observ the time stamp is updated **only** to the files sharig the same folder. (its not a real folder just similar prefix)
Please see this [article](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-event-overview) on using event grid and blobs.
The main reasons, I am suggesting to use the Event Grid as the trigger to a function and not directly from blob, are:
- The trigger is immidiate
- event grid subscription, allows further features such as filtering, which can be leveraged
