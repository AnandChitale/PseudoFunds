using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

// For more information on enabling Web API Test for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Barclays.GenAIHackathon.OpenAIWrapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseSchemaController : ControllerBase
    {
        private string _connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\DRandhavan\source\repos\PseudoFunds\TradeManager.mdf;Integrated Security=True";
        
        // GET api/<DatabaseSchemaController>/5
        [HttpGet("{tableName}")]
        public async Task<string> GetSchema(string tableName)
        {
            return await GenerateSystemPromptForTableAsync(tableName);
        }

        private async Task<string> GenerateSystemPromptForTableAsync(string baseTableName)
        {
            var tables = await GetRelatedTablesAsync(baseTableName);

            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine("You are an assistant that helps users query a database with a specific table and its related tables. " +
                "The Database is MS SQL Server" +
                                    "The result should only include query and no english words or characters." +
                                     "Here is the database structure and relationships:");

            foreach (var table in tables)
            {
                promptBuilder.AppendLine();
                promptBuilder.AppendLine($"- **{table.Name} Table**");
                promptBuilder.AppendLine($"  Purpose: Stores information related to {table.Name.ToLower()}.");
                promptBuilder.AppendLine("  Columns:");

                foreach (var column in table.Columns)
                {
                    promptBuilder.AppendLine($"    - {column.Name} ({column.Type}){(column.IsForeignKey ? $" - Foreign Key to {column.ForeignKeyTable}" : "")}");
                }
            }

            promptBuilder.AppendLine();
            return promptBuilder.ToString();
        }

        private async Task<List<Table>> GetRelatedTablesAsync(string baseTableName)
        {
            var tables = new Dictionary<string, Table>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Query to retrieve base table and all linked tables via foreign keys
                var query = @"
                WITH TableRelations AS (
                    SELECT 
                        t.TABLE_NAME AS TableName,
                        c.COLUMN_NAME AS ColumnName,
                        c.DATA_TYPE AS DataType,
                        fk.ReferencedTableName AS ReferencedTable
                    FROM 
                        INFORMATION_SCHEMA.TABLES t
                    JOIN 
                        INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME
                    LEFT JOIN 
                        (SELECT 
                            fkTable.TABLE_NAME AS TableName,
                            fkColumn.COLUMN_NAME AS ColumnName,
                            pkTable.TABLE_NAME AS ReferencedTableName,
                            pkColumn.COLUMN_NAME AS ReferencedColumnName
                        FROM 
                            INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS rc
                        INNER JOIN 
                            INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS fkTable ON rc.CONSTRAINT_NAME = fkTable.CONSTRAINT_NAME
                        INNER JOIN 
                            INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS pkTable ON rc.UNIQUE_CONSTRAINT_NAME = pkTable.CONSTRAINT_NAME
                        INNER JOIN 
                            INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS fkColumn ON fkTable.CONSTRAINT_NAME = fkColumn.CONSTRAINT_NAME
                        INNER JOIN 
                            INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS pkColumn ON pkTable.CONSTRAINT_NAME = pkColumn.CONSTRAINT_NAME
                            AND fkColumn.ORDINAL_POSITION = pkColumn.ORDINAL_POSITION
                        WHERE 
                            fkTable.CONSTRAINT_TYPE = 'FOREIGN KEY') fk
                         ON c.TABLE_NAME = fk.TableName AND c.COLUMN_NAME = fk.ColumnName
                    WHERE 
                        t.TABLE_TYPE = 'BASE TABLE'
                        AND (t.TABLE_NAME = @BaseTableName OR fk.ReferencedTableName = @BaseTableName)
                )
                SELECT * FROM TableRelations
                ORDER BY TableName";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BaseTableName", baseTableName);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var tableName = reader.GetString(0);
                            var columnName = reader.GetString(1);
                            var dataType = reader.GetString(2);
                            var referencedTable = reader.IsDBNull(3) ? null : reader.GetString(3);

                            if (!tables.ContainsKey(tableName))
                            {
                                tables[tableName] = new Table { Name = tableName };
                            }

                            tables[tableName].Columns.Add(new Column
                            {
                                Name = columnName,
                                Type = dataType,
                                IsForeignKey = referencedTable != null,
                                ForeignKeyTable = referencedTable
                            });
                        }
                    }
                }
            }

            return new List<Table>(tables.Values);
        }

        // POST api/<DatabaseSchemaController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<DatabaseSchemaController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DatabaseSchemaController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
