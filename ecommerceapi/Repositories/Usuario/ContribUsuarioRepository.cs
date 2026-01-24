using System.Data;
using ecommerceapi.Models;
using System.Data.SqlClient;
using Dapper.Contrib.Extensions;

namespace ecommerceapi.Repositories
{
    public class ContribUsuarioRepository : IUsuarioRepository
    {
        private IDbConnection _dbConnection;
        public ContribUsuarioRepository()
        {
            _dbConnection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=eCommerce;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");
        }

        public void Delete(int id)
        {
            _dbConnection.Delete(Get(id));
        }

        public List<Usuario> Get()
        {
          return _dbConnection.GetAll<Usuario>().ToList();
        }

        public Usuario Get(int id)
        {
            return _dbConnection.Get<Usuario>(id);
        }

        public void Insert(Usuario usuario)
        {
            _dbConnection.Insert(usuario);
        }

        public void Update(Usuario usuario)
        {
            _dbConnection.Update(usuario);
        }
    }
}
