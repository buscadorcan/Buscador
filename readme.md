#EVN:
1. SDK .NET : https://dotnet.microsoft.com/en-us/download
2. VSCode   : https:2//code.visualstudio.com/download
  - EXT:  
    - .NET Install Tool
    - Docker
    - Database Client JDBC
    - turder client /  Postman 
  - 
3. crear la base de datos
  - .devcontainer\database\query\canShemma.sql
  - .devcontainer\database\query\canOrgFullText.sql
4. Codificar WebApp
    - MODELO
      WebApp\Models

      - modelo de la apliacion CAN_DB
        - WebApp\Models\CANOrganizacion.cs  
        - WebApp\Models\CANOrganizacionPais.cs  
        - ...
      - modelos del CAN : Col, ecu, bov, peru
        - WebApp\Models\ColombiaCONACvwBusqueda.cs
        - WebApp\Models\EcuadorSAEvwBusqueda.cs    *
        - WebApp\Models\PeruINACALvwBusqueda.cs
        - WebApp\Models\BoliviaIBMETROvwBusqueda.cs

    - REPOSITORIO
    - WebApp\Repositories
      (persona)
      - repositories para los modelos del CAN_DB
      - repositories para los modelos de CAN : Col, ecu, bov, peru

    - CONTROLADORES
    - WebApp\Controllers
      (persona)
      - Controllers para los repositories del CAN_DB
      - Controllers para los repositories de CAN : Col, ecu, bov, peru


dotnet run

------------------------------------------------

buscador parte necesarias
- impacto del buscador
- detalle y analis de los reque
- procesmaiento de cada buscador con dire y reporte traccional, mapa tematico y requerimeinto del ONa 
- requerimeinto para la interoperabilidad
- 
Darwin Alexander: (Ecuador)
-  




## Agregar dependencias
url:

🦝 Buscador  ⚡
 ❰💀❱ cd DataAccess/
 dotnet add package Dapper
 dotnet add package System.Data.Sqlite.Core
 dotnet add package MySql.Data.EntityFrameworkCore
 dotnet add package Microsoft.EntityFrameworkCore.SqlServer

 Versiones instaladas:
 dotnet list package
 Top-level Package                                      Requested     Resolved
   > Dapper                                               2.1.28      2.1.28
   > Microsoft.EntityFrameworkCore.SqlServer              8.0.3       8.0.3
   > Microsoft.Extensions.Configuration.Abstractions      8.0.0       8.0.0
   > MySql.Data.EntityFrameworkCore                       8.0.22      8.0.22
   > System.Data.Sqlite.Core                              1.0.118     1.0.118

usar nuGet :
 dotnet add package Microsoft.Extensions.Confinguration.Abstractions

validamos las dependencias creadas
  <ItemGroup>
    <PackageReference Include="Dapper" Version="2.1.28" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.3" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Abstractions" Version="8.0.0" />
    <PackageReference Include="MySql.Data.EntityFrameworkCore" Version="8.0.22" />
    <PackageReference Include="System.Data.Sqlite.Core" Version="1.0.118" />
  </ItemGroup>

--------------------
dotnet new blazorwasm -n ClientApp
dotnet sln add ClientApp/ClientApp.csproj
Agregar en ClientApp.csproj = >
< UseRazorSourceGenerator>false</UseRazorSourceGenerator>
dotnet run

----------------------------
#git : igual las ramas
git rebase origin/pat_dev_rama


----------------------------
Prototipo en Figma
https://www.figma.com/proto/PiZQnYZZuHU4U49uI4zne0/Buscador-OEA-y-E%26E?node-id=4-505&source=email_invite&starting-point-node-id=4%3A505

----------------------------
Prototipo 2
https://www.figma.com/proto/PiZQnYZZuHU4U49uI4zne0/Buscador-OEA-y-E%26E?page-id=785%3A4982&node-id=849-1153&viewport=860%2C266%2C0.25&t=IZDdyMg7nVic8JxW-1&scaling=min-zoom&content-scaling=fixed
