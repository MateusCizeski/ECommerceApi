using Dapper;
using ecommerceapi.Models;
using System.Data;
using System.Data.SqlClient;

namespace ecommerceapi.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private IDbConnection _dbConnection;
        public UsuarioRepository()
        {
            _dbConnection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=eCommerce;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");
        }

        public List<Usuario> Get()
        {
            return _dbConnection.Query<Usuario>("SELECT * FROM Usuarios").ToList();
        }
    }
}
