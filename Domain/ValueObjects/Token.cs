namespace Domain.ValueObjects
{
    public class Token : ValueObject
    {

        public string Value { get; private set; }

        private Token()
        {

        }

        public Token(string token)
        {
            Value = token;
        }


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLowerInvariant();
        }

        public static implicit operator string(Token token) => token.Value;
    }
}
