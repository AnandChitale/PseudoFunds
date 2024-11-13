using System.Data;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Barclays.GenAIHackathon.OpenAIWrapper.Controllers
{

    
    public class DatabaseReadController 
    {
        private string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\DRandhavan\source\repos\PseudoFunds\TradeManager.mdf;Integrated Security=True";

        // GET: api/<DatabaseReadController>
        
        public async Task<List<ExpandoObject>> Get(string query)
        {
            List<ExpandoObject> result = new List<ExpandoObject>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Example query to select all columns from a table
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var columnNames = new List<string>();

                        // Get column names
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columnNames.Add(reader.GetName(i));
                        }

                        // Read data and store it in a list of ExpandoObjects
                        while (await reader.ReadAsync())
                        {
                            var row = new ExpandoObject() as IDictionary<string, object>;
                            foreach (var column in columnNames)
                            {
                                row[column] = reader[column];
                            }
                            result.Add(row as ExpandoObject);
                        }
                    }
                }
            }

            return result;  // Return as JSON

        }

       
    }
}
