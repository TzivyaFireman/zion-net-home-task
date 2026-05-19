# Improvements I made:
## Handling all channels in a notification
The code in the base did not handle sending to all channels in the notification, but only sending to the first channel in the array,
I updated a loop that goes through the entire array of channels and handles sending,
שליחת משוב
## send-bulk - Handling also the other statuses
In the base, sending is performed for notification in the pending status, I also added retry_pending,

handling the case of a notification that failed and was updated, when calling PUT on a notification,
if there was a change in the channel details, the notification status is updated to PENDING so that its sending will be handled the next time SEND-BULK is performed.

## Updating the correct status for each notification
Updating the notification status according to the status received from the provider in handling each channel.

## Improved code structure
instead of a separate CONST for each message status,
one CONST that includes all status types, then used in the various functions.


I used GPT to get an overview of the project, as well as for writing the code,  
I also used Copilot for code completion.
