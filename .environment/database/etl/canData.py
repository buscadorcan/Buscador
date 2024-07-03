# pip install faker
from faker import Faker
import pyodbc

fake = Faker('es_ES')
# conn = pyodbc.connect('DRIVER={SQL Server};SERVER=PAT-PC;DATABASE=CAN_DB;UID=sa;PWD=pat_mic_PASS;TrustServerCertificate=True')
conn = pyodbc.connect('DRIVER={ODBC Driver 18 for SQL Server};SERVER=localhost,1434;DATABASE=CAN;UID=SA;PWD=pat_mic_DBKEY;SSLVerifyServer=0')

cursor = conn.cursor()
for i in range(1):
    IdOrganizacionPais = 8
    IdAcreditacion = 25
    IdActividad = 25
    IdCiudad = 25
    CodigoAcreditacion = 'KEY_CER_ANTISOB'
    RazonSocial = str(fake.name())
    AreaAcreditacion = str(fake.postcode())
    Actividad = str(fake.name())
    Ciudad = str(fake.city())
    Estado = 'ACREDITADO'
    FechaCrea = str(fake.date_of_birth())
    FechaModifica = str(fake.date_of_birth())
    IdUserCrea = 1
    IdUserModifica = 1

    cursor.execute("""INSERT INTO Organizacion (
        IdOrganizacionPais,
        IdAcreditacion,
        IdActividad,
        IdCiudad,
        CodigoAcreditacion,
        RazonSocial,
        AreaAcreditacion,
        Actividad,
        Ciudad,
        Estado,
        FechaCrea,
        FechaModifica,
        IdUserCrea,
        IdUserModifica
    )
    VALUES (
        ?,
        ?,
        ?,
        ?,
        ?,
        ?,
        ?,
        ?,
        ?,
        ?,
        ?,
        ?,
        ?,
        ?
    )""",
        (IdOrganizacionPais,
            IdAcreditacion,
            IdActividad,
            IdCiudad,
            CodigoAcreditacion,
            RazonSocial,
            AreaAcreditacion,
            Actividad,
            Ciudad,
            Estado,
            FechaCrea,
            FechaModifica,
            IdUserCrea,
            IdUserModifica))

    cursor.commit()

    # print(Nombre, Direccion, Celular, Fecha)
    # cursor.execute("INSERT INTO Persona (Nombre, Direccion, Celular, Fecha) VALUES (?, ?, ?, ?)",
    #                 (Nombre, Direccion, Celular, Fecha))
    # cursor.commit()

    # cursor.execute("SELECT @@IDENTITY AS NewID;")
    # new_id = cursor.fetchone()[0]
    # organizacionFullText = Nombre +" "+ Direccion +" "+ Celular +" "+ Fecha +" "+ street_address +" "+ Localidad +" "+ CodigoPostal + " "+ Email
    
    # print(new_id, organizacionFullText)
# Commit the changes
conn.commit()
cursor.close()
conn.close()