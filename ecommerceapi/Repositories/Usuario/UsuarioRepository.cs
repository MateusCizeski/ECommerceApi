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

        public Usuario Get(int id)
        {
            return _dbConnection.Query<Usuario, Contato, Usuario>(
                "SELECT * FROM usuarios LEFT JOIN Contatos on Contatios.UsuarioId = Usuarios.Id WHERE Id = @Id",
                (usuario, contato) =>
                {
                    usuario.Contato = contato;
                    return usuario;
                },
                new { Id = id }).SingleOrDefault();
        }

        public void Insert(Usuario usuario)
        {
            _dbConnection.Open();
            var transaction = _dbConnection.BeginTransaction();

            try
            {
                string sql = "INSERT INTO Usuarios(Nome, Email, Sexo, RG, CPF, NomeMae, SituacaoCadastro, DataCadastro) VALUES (@Nome, @Email, @Sexo, @RG, @CPF, @NomeMae, @ituacaoCadastro, @DataCadastro); SELECT CAST(SCOPE_IDENTITY() AS INT)";
                usuario.Id = _dbConnection.Query<int>(sql, usuario, transaction).Single();

                if(usuario.Contato != null)
                {
                    usuario.Contato.CodigoUsuario = usuario.Id;
                    string sqlContato = "INSERT INTO Contatos(UsuarioId, Telefone, Celular) VALUES (@UsuarioId, @Telefone, @Celular); SELECT CAST(SCOPE_IDENTITY() AS INT)";
                    usuario.Contato.CodigoUsuario = _dbConnection.Query<int>(sqlContato, usuario.Contato, transaction).Single();
                }

                transaction.Commit();
            }
            catch(Exception)
            {
                try
                {
                    transaction.Rollback();
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            finally
            {
                _dbConnection.Close(); 
            }
        }

        public void Update(Usuario usuario) 
        {
            string sql = "UPDATE Usuarios SET Nome = @Nome, Email = @Email, Sexo = @Sexo, RG = @RG, CPF = @CPF, NomeMae = @NomeMae, SituacaoCadastro = @SituacaoCadastro, DataCadastro = @DataCadastro WHERE Id = @Id";
       
            _dbConnection.Execute(sql, usuario);
        }

        public void Delete(int id)
        {
            _dbConnection.Execute("DELETE FROM Usuarios WHERE Id = @Id", new { Id = id });
        }
    }
}
