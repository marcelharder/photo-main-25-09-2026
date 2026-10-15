public interface IHolidayService
    {
        public Task<HolidayPageDto?> GetAllHolidays(HolidaysParams cp, CancellationToken cancellationToken = default);
        public Task<HolidayDto> GetSpecificHoliday(int id, CancellationToken cancellationToken = default);
        public Task<PagedList<HolidayDto>?> GetAllowedHolidays(HolidaysParams cp, CancellationToken cancellationToken = default);
        public Task<string> GetHolidayDescription(int id, CancellationToken cancellationToken = default);
        public Task<string> UpdateHoliday(Holiday up, CancellationToken cancellationToken = default);
        public Task<string> DeleteHoliday(int id, CancellationToken cancellationToken = default);
        public Task<string> CreateHoliday(Holiday up, CancellationToken cancellationToken = default);
        public Task<int> getTotalNumberOfHolidaysForUser(int userId,CancellationToken cancellationToken = default);
        public Task<HolidayDto[]> GetAllHolidaysForAdmin(CancellationToken cancellationToken = default);

        public Task<FileStreamResult> GetImage(int id, CancellationToken cancellationToken = default);
        
}