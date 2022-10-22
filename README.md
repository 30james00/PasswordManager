# Password Manager

## First setup
1. ```dotnet user-secrets init -p WebApi/```
2. ```dotnet user-secrets set "ConnectionStrings:SQLite" "Data Source=data.db" -p API/```
3. ```dotnet user-secrets set "TokenKey" "<strong password>" -p API/```
4. ```dotnet user-secrets set "Pepper" "<strong password>" -p API/```
