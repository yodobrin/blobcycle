# blobcycle
The use case covered in this repository, is as follows: files are orgenized in directories, the blob life cycle managment needs to address changes to files within the directory. However this feature is working at the blob level. Therfore files within the same directory, would not hold the same update stamp as the chaned, new file intreduced to the directory.
Storage does not contain real directories, rather these are files sharing similar prefix.

## ResetTimeStamp
The function is called upon created blob using an EventGrid subscription. The function parse the blob url, and perform list blobs sharing the same prefix, than after it iterate over these files and sets meta-data, this change is causing the last update stamp of the file to reflect new time.

## List Blobs
Similar functionality to the ResetTimeStamp, but instead of being triggered by the event grid, it is http triggered, it was used for testing, and it can be used to perform similar activity.
