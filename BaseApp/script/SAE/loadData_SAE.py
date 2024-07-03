import pandas as pd
import numpy as np
import pyodbc
import os

#-------------- CONFIGURE -------------------#
csv_directory = r'C:\pat_mic\Buscador\BaseApp\script\SAE'
driver  = '{SQL Server}'
server  = 'PAT-PC'
db      = 'SAE'
tcon    = 'yes'
uname   = 'userSAE'
pword   = 'passSAE'

def read_csv_load_db(file_name):
    file_path = os.path.join(csv_directory, file_name)
    try:
        print(f"\n🧬 {file_name}")
        df = pd.read_csv(file_path, sep=';', encoding='latin1')
        df = df.replace({np.nan: None, '': None})

        conn = pyodbc.connect(driver=driver, server=server, database=db, 
                              trusted_connection=tcon, user=uname, password=pword)
        cursor = conn.cursor()

        table_name = file_name.split(".")[0]
        columns = df.columns

        # Crear tabla SQL
        sql_create_table = f'CREATE TABLE dbo.{table_name} (\n'
        for column in columns:
            dtype = df[column].dtype
            if 'int' in str(dtype):
                sql_create_table += f'\t{column} INT NULL,\n'
            elif 'float' in str(dtype):
                sql_create_table += f'\t{column} FLOAT NULL,\n'
            elif 'datetime' in str(dtype):
                sql_create_table += f'\t{column} DATETIME NULL,\n'
            else:
                sql_create_table += f'\t{column} NVARCHAR(MAX) NULL,\n'
        sql_create_table = sql_create_table.rstrip(',\n') + ' )'

        try:
            # Eliminar tabla si existe
            sql_drop_table = f"DROP TABLE IF EXISTS dbo.{table_name}"
            print(f"👌 {sql_drop_table}")
            cursor.execute(sql_drop_table)

            # Crear la tabla
            print(f"👌 {sql_create_table}")
            cursor.execute(sql_create_table)

            print(f"⌛ ...")    # Insertando ...
            for row in df.itertuples(index=False):
                insert_query = f'INSERT INTO dbo.{file_name.split(".")[0]} ({", ".join(columns)}) VALUES ({", ".join(["?"] * len(columns))})'
                try:
                    cursor.execute(insert_query, *row)
                except pyodbc.Error as e:
                    print(f"ERROR: inserting row {row} into {file_name.split('.')[0]}: {e}")

            # Crear vista
            sql_create_view = f'CREATE OR ALTER VIEW dbo.vs{table_name} AS SELECT * FROM dbo.{table_name}'
            print(f"👌 {sql_create_view}")
            cursor.execute(sql_create_view)

            conn.commit()
            print(f"👌 INSERT INTO {table_name} y vista creada")

        except pyodbc.Error as e:
            print(f"Error al crear la tabla o la vista {table_name}: {e}")

        finally:
            conn.close()

    except FileNotFoundError:
        print(f"💀 No existe: {file_name}")
    except pd.errors.EmptyDataError:
        print(f"💀 Está vacío: {file_name}")
    except pd.errors.ParserError:
        print(f"💀 Error al analizar: {file_name}")

# Inicio
for file in os.listdir(csv_directory):
    if file.endswith('.csv'):
        read_csv_load_db(file)
