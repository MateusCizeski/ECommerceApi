using System.ComponentModel.DataAnnotations;

namespace ecommerceapi.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public string RG { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string NomeMae { get; set; } = string.Empty;
        public string SituacaoCadastro { get; set; } = string.Empty;
        public DateTimeOffset DataCadastro { get; set; }

        public Contato Contato { get; set; } = new Contato();
        public List<EnderecoEntrega> EnderecoEntregas { get; set; }
        public List<Departamento> Departamentos { get; set; }
    }
}
