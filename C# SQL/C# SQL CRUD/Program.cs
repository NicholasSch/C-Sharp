using System;
using MySql.Data.MySqlClient;

class Program
{
    static void Main(string[] args)
{
    string connectionString = "Server=localhost;Database=concessionaria;User ID=User1;Password=Password;";

    using (MySqlConnection connection = new MySqlConnection(connectionString))
    {
        try
        {
            connection.Open();

            if (connection.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine("Connection successful!\n");
                
                // Create
                Console.WriteLine("--- Inserting Cliente ---");
                InsertCliente(connection, "John Doe", 1, 1);

                // Read
                Console.WriteLine("--- Reading Clientes ---");
                ReadClientes(connection);

                // Update
                Console.WriteLine("--- Updating Cliente ---");
                UpdateCliente(connection, 1, "Updated Name");

                // Read after update
                Console.WriteLine("--- Reading Clientes ---");
                ReadClientes(connection);

                // Delete
                Console.WriteLine("--- Deleting Cliente ---");
                DeleteCliente(connection, 1);

                // Read after delete
                Console.WriteLine("--- Reading Clientes ---");
                ReadClientes(connection);
            }
            else
            {
                Console.WriteLine("Connection failed!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    } 
}
    static void InsertCliente(MySqlConnection connection, string nome, int clienteId, int enderecoId)
    {
        string query = "INSERT INTO Cliente (Nome, Cliente_ID, Endereco_ID) VALUES (@Nome, @Cliente_ID, @Endereco_ID)";

        using (MySqlCommand cmd = new MySqlCommand(query, connection))
        {
            cmd.Parameters.AddWithValue("@Nome", nome);
            cmd.Parameters.AddWithValue("@Cliente_ID", clienteId);
            cmd.Parameters.AddWithValue("@Endereco_ID", enderecoId);

            int rowsAffected = cmd.ExecuteNonQuery();

            Console.WriteLine($"{rowsAffected} row(s) inserted.");
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
}
