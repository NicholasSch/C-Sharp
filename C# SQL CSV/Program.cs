using MySql.Data.MySqlClient;

class Program
{
    static void Main(string[] args)
    {
        string connectionString = "Server=localhost;Database=supermercado;User ID=user1;Password=Password;";

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Connection successful to Supermercado DB!\n");

                    // 1. Create Standalone Address & Client
                    Console.WriteLine("--- Inserting Initial Records ---");
                    InsertEndereco(connection, "Av. Central 123", "60000-000");
                    InsertCliente(connection, "John Doe", 1);

                    // 2. Read Records
                    Console.WriteLine("--- Reading Current Customers ---");
                    ReadClientes(connection);  

                    // 3. Update Record
                    Console.WriteLine("--- Updating Customer Name ---");
                    UpdateCliente(connection, 1, "John Doe Supermarket VIP");

                    // 4. Read After Update
                    Console.WriteLine("--- Reading Customers (Post-Update) ---");
                    ReadClientes(connection);

                    // 5. Query data using an SQL INNER JOIN
                    Console.WriteLine("--- Executing Customer Address Join Query ---");
                    ConsultaComJoin(connection);

                    // 6. Delete Record
                    Console.WriteLine("--- Deleting Testing Customer ---");
                    DeleteCliente(connection, 1);

                    // 7. Read After Delete
                    Console.WriteLine("--- Reading Customers (Post-Delete) ---");
                    ReadClientes(connection);

                    // 8. Controlled Transaction Insert
                    Console.WriteLine("--- Executing Transaction-Scoped Insertion ---");
                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            InsertClientewtransaction(connection, transaction, "Jane Smith", 2, 1);
                            transaction.Commit();
                            Console.WriteLine("Transaction committed successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Transaction failed, rolling back. Error: {ex.Message}");
                            transaction.Rollback();
                        }
                    }
                    
                    // 9. Bulk Imports from CSV Files
                    Console.WriteLine("--- Processing Bulk CSV Imports ---");
                    CadastrarEmMassaCsvEndereco(connection);
                    CadastrarEmMassaCsv(connection);

                    // 10. Export Data to a New CSV File
                    Console.WriteLine("--- Exporting Database State to CSV ---");
                    ExportarParaCsv(connection);
                }
                else
                {
                    Console.WriteLine("Connection failed!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical Application Error: {ex.Message}");
            }
        }
    }

    static void InsertCliente(MySqlConnection connection, string nome, int enderecoId, MySqlTransaction? transaction = null)
    {
        string query = "INSERT INTO Cliente (Nome, Endereco_ID) VALUES (@Nome, @Endereco_ID)";

        using (MySqlCommand cmd = new MySqlCommand(query, connection, transaction))
        {
            cmd.Parameters.AddWithValue("@Nome", nome);
            cmd.Parameters.AddWithValue("@Endereco_ID", enderecoId);

            int rowsAffected = cmd.ExecuteNonQuery();
            Console.WriteLine($"[Client Table] {rowsAffected} row(s) inserted.");
        }
    }

    static void InsertEndereco(MySqlConnection connection, string rua, string cep, MySqlTransaction? transaction = null)
    {
        string query = "INSERT INTO Endereco (Rua, CEP) VALUES (@Rua, @CEP)";

        using (MySqlCommand cmd = new MySqlCommand(query, connection, transaction))
        {
            cmd.Parameters.AddWithValue("@Rua", rua);
            cmd.Parameters.AddWithValue("@CEP", cep);

            int rowsAffected = cmd.ExecuteNonQuery();
            Console.WriteLine($"[Address Table] {rowsAffected} row(s) inserted.");
        }
    }

    static void ReadClientes(MySqlConnection connection)
    {
        string query = "SELECT * FROM Cliente";

        using (MySqlCommand cmd = new MySqlCommand(query, connection))
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                Console.WriteLine("Cliente_ID\tNome\tEndereco_ID");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Cliente_ID"]}\t{reader["Nome"]}\t{reader["Endereco_ID"]}");
                }
            }
        }
        Console.WriteLine();
    }

    static void UpdateCliente(MySqlConnection connection, int clienteId, string nome)
    {
        string query = "UPDATE Cliente SET Nome = @Nome WHERE Cliente_ID = @Cliente_ID";

        using (MySqlCommand cmd = new MySqlCommand(query, connection))
        {
            cmd.Parameters.AddWithValue("@Nome", nome);
            cmd.Parameters.AddWithValue("@Cliente_ID", clienteId);

            int rowsAffected = cmd.ExecuteNonQuery();
            Console.WriteLine($"{rowsAffected} row(s) updated.");
        }
    }

    static void DeleteCliente(MySqlConnection connection, int clienteId)
    {
        string query = "DELETE FROM Cliente WHERE Cliente_ID = @Cliente_ID";

        using (MySqlCommand cmd = new MySqlCommand(query, connection))
        {
            cmd.Parameters.AddWithValue("@Cliente_ID", clienteId);

            int rowsAffected = cmd.ExecuteNonQuery();
            Console.WriteLine($"{rowsAffected} row(s) deleted.");
        }
    }

    static void ConsultaComJoin(MySqlConnection connection)
    {
        string query = "SELECT Cliente.Cliente_ID, Cliente.Nome, Cliente.Endereco_ID, Endereco.CEP, Endereco.Rua " +
                       "FROM Cliente " +
                       "INNER JOIN Endereco ON Cliente.Endereco_ID = Endereco.Endereco_ID";

        using (MySqlCommand cmd = new MySqlCommand(query, connection))
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                Console.WriteLine("Cliente_ID\tNome\tEndereco_ID\tCEP\tRua");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Cliente_ID"]}\t{reader["Nome"]}\t{reader["Endereco_ID"]}\t{reader["CEP"]}\t{reader["Rua"]}");
                }
            }
        }
        Console.WriteLine();
    }

    static void InsertClientewtransaction(MySqlConnection connection, MySqlTransaction transaction, string nome, int clienteId, int enderecoId)
    {
        string query = "INSERT INTO Cliente (Cliente_ID, Nome, Endereco_ID) VALUES (@Cliente_ID, @Nome, @Endereco_ID)";
        using (MySqlCommand cmd = new MySqlCommand(query, connection, transaction))
        {
            cmd.Parameters.AddWithValue("@Cliente_ID", clienteId);
            cmd.Parameters.AddWithValue("@Nome", nome);
            cmd.Parameters.AddWithValue("@Endereco_ID", enderecoId);

            int rowsAffected = cmd.ExecuteNonQuery();
            Console.WriteLine($"[Transaction Block] {rowsAffected} row(s) inserted.");
        }
    }

    static void CadastrarEmMassaCsv(MySqlConnection connection)
    {
        string filename = "Supermercado_Clientes.csv";
        if (!File.Exists(filename))
        {
            Console.WriteLine($"Skipping Bulk Clients: '{filename}' file not found.");
            return;
        }

        using (MySqlTransaction transaction = connection.BeginTransaction())
        {
            try
            {
                List<string[]> csvData = LerDadosDoCsv(filename);

                foreach (string[] data in csvData)
                {
                    if (data.Length < 3) continue; 
                    string nome = data[1];
                    int enderecoId = int.Parse(data[2]);

                    InsertCliente(connection, nome, enderecoId, transaction);
                }

                transaction.Commit();
                Console.WriteLine("Bulk Client upload successfully committed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during bulk Client insertion: {ex.Message}");
                transaction.Rollback();
            }
        }
    }

    static void CadastrarEmMassaCsvEndereco(MySqlConnection connection)
    {
        string filename = "Supermercado_Enderecos.csv";
        if (!File.Exists(filename))
        {
            Console.WriteLine($"Skipping Bulk Addresses: '{filename}' file not found.");
            return;
        }

        using (MySqlTransaction transaction = connection.BeginTransaction())
        {
            try
            {
                List<string[]> csvData = LerDadosDoCsv(filename);

                foreach (string[] data in csvData)
                {
                    if (data.Length < 4) continue;
                    string rua = data[1] + " " + data[2];
                    string cep = data[3];

                    InsertEndereco(connection, rua, cep, transaction);
                }

                transaction.Commit();
                Console.WriteLine("Bulk Address upload successfully committed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during bulk Address insertion: {ex.Message}");
                transaction.Rollback();
            }
        }
    }

    static List<string[]> LerDadosDoCsv(string caminhoDoArquivo)
    {
        List<string[]> csvData = new List<string[]>();
        using (StreamReader reader = new StreamReader(caminhoDoArquivo))
        {
            while (!reader.EndOfStream)
            {
                string? linha = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(linha)) continue;
                string[] valores = linha.Split(',');
                csvData.Add(valores);
            }
        }
        return csvData;
    }

    static void ExportarParaCsv(MySqlConnection connection)
    {
        try
        {
            string query = "SELECT * FROM Cliente";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    using (StreamWriter writer = new StreamWriter("Clientes_Exportados.csv"))
                    {
                        writer.WriteLine("Cliente_ID,Nome,Endereco_ID");

                        while (reader.Read())
                        {
                            writer.WriteLine($"{reader["Cliente_ID"]},{reader["Nome"]},{reader["Endereco_ID"]}");
                        }
                    }
                }
            }
            Console.WriteLine("Export to 'Clientes_Exportados.csv' complete.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during CSV export: {ex.Message}");
        }
    }
}