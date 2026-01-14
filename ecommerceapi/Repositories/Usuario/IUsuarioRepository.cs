using ecommerceapi.Models;

namespace ecommerceapi.Repositories
{
    public interface IUsuarioRepository
    {
        List<Usuario> Get();
        Usuario Get(int id);
        void Insert(Usuario usuario);
        void Update(Usuario usuario);
        void Delete(int id);
    }
}
