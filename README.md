# Password Manager

## First setup
1. ```dotnet user-secrets init -p WebApi/```
2. ```dotnet user-secrets set "ConnectionStrings:SQLite" "Data Source=data.db" -p WebApi/```
3. ```dotnet user-secrets set "TokenKey" "<strong password>" -p WebApi/```
4. ```dotnet user-secrets set "Pepper" "<strong password>" -p WebApi/```
