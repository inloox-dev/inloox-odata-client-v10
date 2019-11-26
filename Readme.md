## InLoox OData Api Example for Version 10

The example loads the first 10 documents from https://app.inlooxnow.com with custom field 'DocTest' set to true and updates the status field.

The following custom field is required to run:  
Name: DocTest  
Place: Document  
Type: Checkbox  


## Setup InLooxNowOdataClient
1. Enter your credentials [username/password](https://github.com/inloox-dev/inloox-api-client/blob/2ca69ca8110d32fbe05d9a1a15fc81613512331b/InLooxNowODataClient/Program.cs#L17).
Please note, this is an example and you shouldn't save your credentials in clear text. Better use environment variables / config files.
2. Update endpoint to InLoox [endPoint](https://github.com/inloox-dev/inloox-api-client/blob/2ca69ca8110d32fbe05d9a1a15fc81613512331b/InLooxNowODataClient/Program.cs#L14). For InLoox now use https://app.inlooxnow.de or https://app.inlooxnow.com
for InLoox Enterprise Version (Saas) use your iis url

![update config settings](./images/program_main.png "update config settings")

## Setup InLoox Account
First, create a custom field named DocTest under options->General->Custom Fields
https://app.inlooxnow.com/options/#/options/customfields
![add custom field](./images/add_custom_field.png "Add custom field")

Second, open up a project and go to the document section. Upload a new file or open an exisiting one.
The custom fields section of the file dialog shows the new field to check.
![activate DocTest checkbox](./images/activate_checkbox.png "activate DocTest checkbox")

Now, run the InLooxNowOdataClient example. After a few seconds the checked documents should show an updated status entry
![document new status](./images/document_new_status.png "document new status")