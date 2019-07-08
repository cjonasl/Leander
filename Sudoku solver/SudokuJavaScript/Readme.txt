I describe how to deploy the application in Windows environment
only. It should be similar in other environments.

Step 1:
Create a folder "Sudoku" in any location, for example
directy on C-drive: C:\Sudoku

Step 2:
In the folder add the files "sudoku.html", "sudoku.js"
and "bootstrap.min.css".

Step 3:
Open Notepad as an administrator and then open file:
"C:\Windows\System32\drivers\etc\hosts"
Add the following line in the file: 127.0.0.1  www.sudoku.com
Save and close the hosts-file.

Step 4:
Open Internet Information Services Manager (IIS) as an administrator.
Right click on "Sites", choose "Add Website..."
In the dialog that appears enter "Sudoku" for "Site name:"
for "Physical path:" give the path to the folder you
created in step 1 and for "Host name:" enter "www.sudoku.com".

Step 5:
Open any browser and type: http://www.sudoku.com/sudoku.html