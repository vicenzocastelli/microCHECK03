using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;
using web_restuarante.Entidades;

namespace web_restuarante.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly string? _connectionString;
        public ProdutoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private IDbConnection OpenConnection() 
        {
            IDbConnection dbConnection = new SqliteConnection(_connectionString);
            dbConnection.Open();
            return dbConnection;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            using IDbConnection dbConnection = OpenConnection();
            string sql = "SELECT id, nome, descricao, imagemUrl FROM Produto;";
            var result = await dbConnection.QueryAsync<Produto>(sql);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using IDbConnection dbConnection = OpenConnection();
            string sql = "SELECT id, nome, descricao, imagemUrl FROM Produto where id = @id;";
            var result = await dbConnection.QueryFirstOrDefaultAsync<Produto>(sql, new { id });

            dbConnection.Close();

            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Produto produto)
        {
            using IDbConnection dbConnection = OpenConnection();
            string sql = @"INSERT into Produto(nome, descricao, imagemUrl) 
                VALUES(@Nome, @Descricao, @ImagemUrl);";

            await dbConnection.ExecuteAsync(sql, produto);
            dbConnection.Close();
            return Ok();
        }

        [HttpPut]
        public IActionResult Put([FromBody] Produto produto)
        {

            using IDbConnection dbConnection = OpenConnection();

            // Atualiza o produto
            var query = @"UPDATE Produto SET 
                          Nome = @Nome,
                          Descricao = @Descricao,
                          ImagemUrl = @ImagemUrl
                          WHERE Id = @Id";

            dbConnection.Execute(query, produto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using IDbConnection dbConnection = OpenConnection();

            var produto = await dbConnection.QueryAsync<Produto>("delete from produto where id = @id;", new { id });
            return Ok();
        }
    }
}
