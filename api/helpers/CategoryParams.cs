namespace fotoservice.api.helpers;

public class CategoryParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;

        public int TotalPages { get; set; }
        private int pageSize = 9;
        public int PageSize
        {
            get { return pageSize;}
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value;}
        }
        public int Id { get; set; } 

        public int userId { get; set; }
        public required int Category { get; set; } 

        public List<int>? AllowedCategories { get; set; }
    }
