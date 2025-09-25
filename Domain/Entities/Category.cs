namespace Domain.Entities
{
    public class Category : BaseEntity, IEquatable<Category>
    {
        public string Name { get; private set; } = string.Empty;

        public ICollection<Video> Videos { get; set; } = new List<Video>();

        public Category(string categoryName) 
        {
            ChangeName(categoryName);
        }
        public Category()
        {

        }

        public void ChangeName(string name)
        {
            if (string.IsNullOrEmpty(name?.Trim()) || name.ToLower() == Name.ToLower())
            {
                throw new ArgumentException();
            }
            Name = name.ToLower();
        }

        public override bool Equals(object? obj) => obj is Category cat && Equals(cat);

        public override int GetHashCode() => Name.GetHashCode();

        public bool Equals(Category? other) => other is not null && string.Equals(Name, other.Name, StringComparison.CurrentCultureIgnoreCase);
    }
}
