using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Meltix.UnitTests.Domain
{
    public class UserTests
    {
        [Fact]
        public void User_ShouldCreate_WhenCorrectInfo()
        {
            // Arrange
            var pseudo = "Meltoz";
            var password = "P@ssw0rd";
            var role = Role.Administrator;

            // Act
            var user = new User(pseudo, password, role);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(role, user.Role);
            Assert.Equal(pseudo, user.Pseudo.Value);
        }

        [Fact]
        public void User_ShouldHashPassword_WhenPassword()
        {
            // Arrange
            var pseudo = "Meltoz";
            var password = "P@ssw0rd";
            var role = Role.Administrator;

            // Act
            var user = new User(pseudo, password, role);

            // Assert
            Assert.NotEqual(password, user.Password.Value);
            Assert.Equal(Password.FromPlainText(password).Value, user.Password.Value);
        }

        [Fact]
        public void User_ShouldThrow_WhenPasswordIsNotSecure()
        {
            // Arrange
            var pseudo = "Meltoz";
            var passord = "123";
            var role = Role.User;

            // Act
            var caughtException = Assert.Throws<ArgumentException>(() => new User(pseudo, passord, role));

            // Assert
            Assert.Equal("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters", caughtException.Message);
        }

        [Fact]
        public void User_ShouldThrow_WhenPseudoEmpty()
        {
            // Arrange
            var pseudo = string.Empty;
            var password = "P@ssw0rd";
            var role = Role.Administrator;

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new User(pseudo, password, role));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("Pseudo can't be empty", caughtException.Message);
        }

        [Fact]
        public void User_ShouldThrow_WhenPseudoWhiteSpace()
        {
            // Arrange
            var pseudo = "  ";
            var password = "P@ssw0rd";
            var role = Role.Administrator;

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new User(pseudo, password, role));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("Pseudo can't be empty", caughtException.Message);
        }

        [Fact]
        public void User_ShouldThrow_WhenPseudoNull()
        {
            // Arrange
            string pseudo = null;
            var password = "P@ssw0rd";
            var role = Role.Administrator;

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new User(pseudo, password, role));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("Pseudo can't be empty", caughtException.Message);
        }

        [Fact]
        public void User_ShouldThrow_WhenPasswordEmpty()
        {
            // Arrange
            var pseudo = "Meltoz";
            var password = string.Empty;
            var role = Role.Administrator;

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new User(pseudo, password, role));

            // Assert
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("Password can't be empty", caughtException.Message);
        }

        [Fact]
        public void ChangeRole_ShouldChange_WhenRoleCorrect()
        {
            // Arrange
            var pseudo = "Meltoz";
            var password = "P@ssw0rd";
            var role = Role.User;
            var newRole = Role.Administrator;
            var user = new User(pseudo, password, role);

            // Act
            user.ChangeRole(newRole);

            // Assert
            Assert.Equal(newRole, user.Role);
        }

        [Fact]
        public void ChangePassword_ShouldChange_WhenPasswordCorrect()
        {
            // Arrange
            var pseudo = "Meltoz";
            var password = "P@ssw0rd";
            var role = Role.User;
            var newPassword = "P@ssw0rd1";
            var user = new User(pseudo, password, role);

            // Act
            var before = DateTime.UtcNow;
            user.ChangePassword(newPassword);
            var after = DateTime.UtcNow;

            // Assert
            Assert.NotEqual(Password.FromPlainText(password), user.Password);
            Assert.NotEqual(newPassword, user.Password.Value);
            Assert.Equal(Password.FromPlainText(newPassword), user.Password);
            Assert.NotNull(user.LastChangePassword);
            Assert.InRange(user.LastChangePassword.Value, before, after);
        }

        [Fact]
        public void ChangePseudo_ShouldChange_WhenPseudoCorrect()
        {
            // Arrange
            var pseudo = "Meltoz";
            var password = "P@ssw0rd";
            var role = Role.User;
            var newPseudo = "Meltoz_";
            var user = new User(pseudo, password, role);

            // Act
            user.ChangePseudo(newPseudo);

            // Arrange
            Assert.NotEqual(pseudo, user.Pseudo.Value);
            Assert.Equal(newPseudo, user.Pseudo.Value);
        }

        [Fact]
        public void Login_ShouldChangeLastLogin_WhenLogin()
        {
            // Arrange
            var pseudo = "Meltoz";
            var password = "P@ssw0rd";
            var role = Role.User;
            var user = new User(pseudo, password, role);

            // Act
            var before = DateTime.UtcNow;
            user.Login();
            var after = DateTime.UtcNow;

            // Assert
            Assert.NotNull(user.LastLogin);
            Assert.InRange(user.LastLogin.Value, before, after);

        }
    }
}
