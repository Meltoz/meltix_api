using Domain.Enums;
using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public Pseudo Pseudo { get; private set; }

        public Password Password { get; private set; }

        public Role Role { get; private set; }

        public DateTime? LastLogin { get; private set; }

        public DateTime? LastChangePassword { get;private set; }

        public ICollection<TokenInfo> Tokens { get; private set; }

        public User()
        {

        }

        public User(string pseudo, string password, Role role)
        {
            Role = role;
            ChangePseudo(pseudo);
            ChangePassword(password);
        }

        public void ChangePseudo(string newPseudo)
        {
            Pseudo = Pseudo.Create(newPseudo);
        }

        public void ChangePassword(string newPassword)
        {
            Password = Password.Create(newPassword);
            LastChangePassword = DateTime.UtcNow;
        }

        public void ChangeRole(Role newRole)
        {
            Role = newRole;
        }

        public void Login()
        {
            LastLogin = DateTime.UtcNow;
        }
    }
}
