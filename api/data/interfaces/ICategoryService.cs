public interface ICategoryService
    {
        public Task<PagedList<CategoryDto>?> GetAllCategories(CategoryParams cp, CancellationToken cancellationToken = default);
        public Task<CategoryDto> GetSpecificCategory(int id, CancellationToken cancellationToken = default);
        public Task<PagedList<CategoryDto>?> GetAllowedCategories(CategoryParams cp, CancellationToken cancellationToken = default);
        public Task<string> GetCategoryDescription(int id, CancellationToken cancellationToken = default);
        public Task<string> UpdateCategory(Category up, CancellationToken cancellationToken = default);
        public Task DeleteCategory(int id);
        public Task<string> CreateCategory(Category up, CancellationToken cancellationToken = default);
        public Task<int> getTotalNumberOfCategoriesForUser(int userId,CancellationToken cancellationToken = default);
        public Task<CategoryDto[]> GetAllCategoriesForAdmin(CancellationToken cancellationToken = default);
        
}