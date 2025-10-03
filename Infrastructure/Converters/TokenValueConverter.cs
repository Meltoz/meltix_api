using Application.Services;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace Infrastructure.Converters
{
    public class TokenValueConverter : ValueConverter<Token, string>
    {
        public TokenValueConverter(AesEncryptionService encryptionService, ConverterMappingHints? mappingHints) : base(
            token => encryptionService.Encrypt(token.Value),
            value => new Token(encryptionService.Decrypt(value)), 
            mappingHints)
        { }
    }
}
