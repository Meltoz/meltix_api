namespace Domain.Entities
{
    public class Tag : BaseEntity, IEquatable<Tag>, IComparable<Tag>
    {
        public string Value { get; private set; } = string.Empty;

        public ICollection<Video> Videos { get; set; } = new List<Video>();

        public Tag() { }

        public Tag(string value)
        { 
            if(IsValidTag(value))
                Value = value.ToLower(); 
        }

        public void ChangeValue(string value)
        {
            if(IsValidTag(value))
                Value = value.ToLower();
        }
        
        public int CompareTo(Tag? other)
        {
            if(other is null) return 1;
            return string.Compare(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        }
        public override int GetHashCode() => Value.GetHashCode();        

        public override bool Equals(object? obj) => obj is Tag tag && Equals(tag);

        public bool Equals(Tag? other) => other is not null && Value == other.Value;

        private static bool IsValidTag(string value)
        {
            if (string.IsNullOrEmpty(value.Trim()))
                throw new ArgumentException($"{value} is null or empty");

            return true;
        }
    }
}
