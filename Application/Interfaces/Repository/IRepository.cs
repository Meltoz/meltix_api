namespace Application.Interfaces.Repository
{
    public interface IRepository<T>
    {
        public Task<IEnumerable<T>> GetAllAsync();

        public Task<(IEnumerable<T> data, int totalCount)> GetPaginateAsync(int skip, int take);

        public Task<T?> GetByIdAsync(Guid id);

        public T? GetById(Guid id);

        public T? Insert(T entity);

        public Task<T?> InsertAsync(T entity);

        public Task InsertWithOutSave(T entity);

        public Task<T> UpdateAsync(T entityToUpdate);

        public Task UpdateWithOutSaveAsync(T entityToUpdate);

        public Task<T> UpsertAsync(T entity);

        public Task<T> UpsertWithOutSaveAsync(T entity);

        public void Delete(Guid id);

        public void Delete(T entity);

        public void Save();

        public Task SaveAsync();
    }
}
