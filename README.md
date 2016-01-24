# Offliner

## How to setup a development environment

### Step 1 - Install ASP.NET 5

To install ASP.NET 5 on your machine, please follow instruction depending on your OS.
* [Install ASP.NET 5 on Windows](https://docs.asp.net/en/latest/getting-started/installing-on-windows.html)
* [Install ASP.NET 5 on Linux](https://docs.asp.net/en/latest/getting-started/installing-on-linux.html)
* [Install ASP.NET 5 on OSX](https://docs.asp.net/en/latest/getting-started/installing-on-mac.html)

### Step 2 - Restore packages (if any)  

Go to project root directory and execute via command prompt
```
dnu restore
```

### Step 3 - Run console app  

To run a console app, use following commands:
```
cd offliner.consolehost
dnx run
```
