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
            List<Usuario> usuarios = new List<Usuario>();

            string sql = "SELECT * FROM Usuarios LEFT JOIN Contatos ON Contatos.UsuarioId = Usuarios.Id LEFT JOIN EnderecosEntrega ON EnderecosEntrega.UsuarioId = Usuarios.Id";

            _dbConnection.Query<Usuario, Contato, EnderecoEntrega, Usuario>(sql,
                (usuario, contato, enderecoEntrega) =>
                {
                    if(usuarios.SingleOrDefault(a => a.Id == usuario.Id) == null)
                    {
                        usuarios.Add(usuario);
                    }else
                    {
                        usuario = usuarios.SingleOrDefault(a => a.Id == usuario.Id);
                    }

                    usuario.EnderecoEntregas.Add(enderecoEntrega);
                    return usuario;

                });
            return usuarios;
        }

        public Usuario Get(int id)
        {
            List<Usuario> usuarios = new List<Usuario>();

            string sql = "SELECT * FROM Usuarios LEFT JOIN Contatos ON Contatos.UsuarioId = Usuarios.Id LEFT JOIN EnderecosEntrega ON EnderecosEntrega.UsuarioId = Usuarios.Id WHERE Usuarios.Id = @Id";

            _dbConnection.Query<Usuario, Contato, EnderecoEntrega, Usuario>(sql,
                (usuario, contato, enderecoEntrega) =>
                {
                    if (usuarios.SingleOrDefault(a => a.Id == usuario.Id) == null)
                    {
                        usuarios.Add(usuario);
                    }
                    else
                    {
                        usuario = usuarios.SingleOrDefault(a => a.Id == usuario.Id);
                    }

                    usuario.EnderecoEntregas.Add(enderecoEntrega);
                    return usuario;

                }, new { Id = id });

            return usuarios.SingleOrDefault();
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

                if(usuario.EnderecoEntregas != null && usuario.EnderecoEntregas.Count > 0)
                {
                    foreach(var enderecoEntrega in usuario.EnderecoEntregas)
                    {
                        enderecoEntrega.CodigoUsuario = usuario.Id;
                        string sqlEndereco = "INSERT INTO EnderecosEntrega (UsuarioId, NomeEndereco, CEP, Estado, Cidade, Bairro, Endereco, Numero, Complemento) VALUES (@UsuarioId, @NomeEndereco, @CEP, @Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento); SELECT CAST(SCOPE_IDENTITY() AS INT)";
                        usuario.Id = _dbConnection.Query<int>(sqlEndereco, usuario.EnderecoEntregas, transaction).Single();

                    }
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
            _dbConnection.Open();
            var transaction = _dbConnection.BeginTransaction();

            try
            {
                string sql = "UPDATE Usuarios SET Nome = @Nome, Email = @Email, Sexo = @Sexo, RG = @RG, CPF = @CPF, NomeMae = @NomeMae, SituacaoCadastro = @SituacaoCadastro, DataCadastro = @DataCadastro WHERE Id = @Id";
                _dbConnection.Execute(sql, usuario);

                if(usuario.Contato != null)
                {
                    string sqlcontato = "UPDATE Contatos SET UsuarioId = @UsuarioId, Telefone = @Telefone, Celular = @Celular WHERE Id = @Id;";
                    _dbConnection.Execute(sqlcontato, usuario.Contato);
                }

                string sqlDeleteEnderecos = "DELETE FROM EnderecosEntrega WHERE UsuarioId = @Id";
                _dbConnection.Execute(sqlDeleteEnderecos, usuario, transaction);

                if (usuario.EnderecoEntregas != null && usuario.EnderecoEntregas.Count > 0)
                {
                    foreach (var enderecoEntrega in usuario.EnderecoEntregas)
                    {
                        enderecoEntrega.CodigoUsuario = usuario.Id;
                        string sqlEndereco = "INSERT INTO EnderecosEntrega (UsuarioId, NomeEndereco, CEP, Estado, Cidade, Bairro, Endereco, Numero, Complemento) VALUES (@UsuarioId, @NomeEndereco, @CEP, @Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento); SELECT CAST(SCOPE_IDENTITY() AS INT)";
                        usuario.Id = _dbConnection.Query<int>(sqlEndereco, usuario.EnderecoEntregas, transaction).Single();

                    }
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            finally
            {
                _dbConnection.Close();
            }
        }

        public void Delete(int id)
        {
            _dbConnection.Execute("DELETE FROM Usuarios WHERE Id = @Id", new { Id = id });
        }
    }
}
