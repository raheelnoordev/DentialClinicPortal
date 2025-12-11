using DentialClinic.Api.Model;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Billing;

namespace DentialClinic.Server.Interfaces
{
    public interface IBillingService
    {
        Task<ServiceResponse<List<InvoiceDto>>> GetAllInvoicesAsync();
        Task<ServiceResponse<InvoiceDto>> GetInvoiceByIdAsync(int id);
        Task<ServiceResponse<InvoiceDto>> CreateInvoiceFromVisitAsync(int visitId);
        Task<ServiceResponse<InvoiceDto>> UpdateInvoiceAsync(int id, InvoiceDto request);
        Task<ServiceResponse<bool>> DeleteInvoiceAsync(int id);
        Task<ServiceResponse<InvoicePaymentDto>> AddPaymentAsync(int invoiceId, InvoicePaymentDto payment);
        Task<ServiceResponse<List<InvoiceDto>>> GetInvoicesByBranchAsync(int branchId, DateTime? startDate, DateTime? endDate);
        Task<ServiceResponse<List<InvoiceDto>>> GetInvoicesByPatientAsync(int patientId);
        Task<ServiceResponse<decimal>> GetOutstandingBalanceAsync(int invoiceId);
    }
}

