using DentialClinic.Api.Model;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Banking;

namespace DentialClinic.Server.Interfaces
{
    public interface IBankingService
    {
        Task<ServiceResponse<List<BankAccountDto>>> GetAllBankAccountsAsync();
        Task<ServiceResponse<BankAccountDto>> GetBankAccountByIdAsync(int id);
        Task<ServiceResponse<BankAccountDto>> CreateBankAccountAsync(CreateBankAccountRequest request);
        Task<ServiceResponse<BankAccountDto>> UpdateBankAccountAsync(int id, UpdateBankAccountRequest request);
        Task<ServiceResponse<bool>> DeleteBankAccountAsync(int id);
    }
}
