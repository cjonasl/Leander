I describe how to deploy the application in Windows environment,
Visual studio and SQL-server only. It should be similar in other
environments.

Step 1:
Open Visual studio and create a new project and choose project type
"ASP.NET Web Application (.NET Framework). Give the project the name
"AddressBook". Press OK.

Step 2:
Choose an empty MVC project and press OK.

Step 3:
Right click on "Controllers"-folder, choose "Add - Existing Item..."
Browse to file "AddressBookController.cs" and add that file to the project.

Step 4:
Right click on "Models"-folder, choose "Add - Existing Item..."
Browse to folder with files "ChangePassword.cs", "PersonInfo.cs",
"PersonInfoDb.cs", "Security.cs" and "User.cs". Mark all files
(hold down keys ctrl and alt and click on the files) and add
them to the project.

Step 5:
Create folder "AddressBook" under Views-folder.

Step 6:
Right click on "AddressBook"-folder, choose "Add - Existing Item..."
Browse to folder with files "AddressBook.cshtml" and "Login.cshtml",
Mark the two files and add them to the project.

Step 7:
Create folder "Images" directly under root folder. Right click on 
"Images"-folder, choose "Add - Existing Item..."
Browse to file "AddressBook.png" and add that file to the project.

Step 8:
Open Web.config in root folder and add a connection string configuration
with name "dbAddressBook" to a database "AddressBook":
<connectionStrings>
  <add name="dbAddressBook" connectionString="Data Source=?????????;Initial Catalog=AddressBook;Integrated Security=True"/>
</connectionStrings>

Change ????????? to your SQL-server name

Step 9:
In Web.config in root folder add also an authentication-element:
<authentication mode="Forms">
  <forms loginUrl="AddressBook/LogIn"></forms>
</authentication>

Step 10:
Right click on project "AddressBook" and choose "Build".

Step 11:
Open Notepad as an administrator and then open file:
"C:\Windows\System32\drivers\etc\hosts"
Add the following line in the file: 127.0.0.1   www.addressbook.com
Save and close the hosts-file.

Step 12:
Open Internet Information Services Manager (IIS) as an administrator.
Right click on "Sites", choose "Add Website..."
In the dialog that appears enter "AddressBook" for "Site name:"
for "Physical path:" give the path to AddressBook.csproj that you
created in step 2 and for "Host name:" enter "www.addressbook.com".

Step 13:
Open Microsoft SQL Server Management Studio and create a new database
"AddressBook".

Step 14:
Execute the script "AddressBook.sql"

Step 15:
Open any browser and type: http://www.addressbook.com/AddressBook/LogIn