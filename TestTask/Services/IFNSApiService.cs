namespace TestTask.Services
{
    public interface IFNSApiService
    {
        Task<string> GetCompanyInfoAsync(string inn);
    }
}
