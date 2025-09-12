namespace Application.Interfaces
{
    public interface ITagService
    {
        public Task<IEnumerable<string>> Search(int limit, string searchTerm);
    }
}
