using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
    public class Pseudo : ValueObject
    {
        public string Value { get; private set; }

        private Pseudo() { }

        private Pseudo(string value)
        {
            Value = value;
        }

        public static Pseudo Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Pseudo can't be empty");

            if (value.Length < 3 || value.Length > 50)
                throw new ArgumentException("Must be between 3 and 50 character");

            if (!Regex.IsMatch(value, @"^[\d\w_-]+$"))
                throw new ArgumentException("Must contains letter and digit only");

            return new Pseudo(value);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLowerInvariant();
        }

        public override string ToString() => Value;

        public bool Contains (string substring)
        {
            return Value != null && Value.Contains(substring, StringComparison.OrdinalIgnoreCase);
        }

        public static implicit operator string(Pseudo pseudo) => pseudo.Value;
    }
}
