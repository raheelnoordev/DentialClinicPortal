using DentialClinic.Api.Model;
using DentialClinic.Server.Data;
using DentialClinic.Server.Interfaces;
using DentialClinic.Server.Models;
using DentialClinic.Server.Models.Billing;
using DentialClinic.Server.Models.Visit;
using Microsoft.EntityFrameworkCore;

namespace DentialClinic.Server.Services
{
    public class BillingService : IBillingService
    {
        private readonly ApplicationDbContext _context;
        public BillingService(ApplicationDbContext context) => _context = context;

        private async Task<InvoiceDto> MapToDtoAsync(Invoice inv)
        {
            var payments = await _context.InvoicePayments
                .Include(p => p.ReceivedByUser)
                .Where(p => p.InvoiceId == inv.InvoiceId)
                .Select(p => new InvoicePaymentDto
                {
                    PaymentId = p.PaymentId,
                    InvoiceId = p.InvoiceId,
                    PaymentDate = p.PaymentDate,
                    Amount = p.Amount,
                    Method = p.Method,
                    ReferenceNo = p.ReferenceNo,
                    Notes = p.Notes,
                    ReceivedByUserId = p.ReceivedByUserId,
                    ReceivedByUserName = p.ReceivedByUser != null ? p.ReceivedByUser.FullName : null
                })
                .ToListAsync();

            return new InvoiceDto
            {
                InvoiceId = inv.InvoiceId,
                BranchId = inv.BranchId,
                BranchName = inv.Branch != null ? inv.Branch.Name : string.Empty,
                VisitId = inv.VisitId,
                //PatientName = inv.Visit != null && inv.Visit.Patient != null ? inv.Visit.Patient.FullName : string.Empty,
                InvoiceDate = inv.InvoiceDate,
                GrossAmount = inv.GrossAmount,
                DiscountAmount = inv.DiscountAmount,
                NetAmount = inv.NetAmount,
                PaidAmount = inv.PaidAmount,
                OutstandingAmount = inv.NetAmount - inv.PaidAmount,
                Status = inv.Status,
                CreatedByUserId = inv.CreatedByUserId,
                CreatedByUserName = inv.CreatedByUser != null ? inv.CreatedByUser.FullName : null,
                Payments = payments
            };
        }

        public async Task<ServiceResponse<List<InvoiceDto>>> GetAllInvoicesAsync()
        {
            var response = new ServiceResponse<List<InvoiceDto>>();
            try
            {
                var invoices = await _context.Invoices
                    .Include(i => i.Branch)
                    .Include(i => i.Visit).ThenInclude(v => v.PatientId)
                    .Include(i => i.CreatedByUser)
                    .OrderByDescending(i => i.InvoiceDate)
                    .ToListAsync();
                response.Result = new List<InvoiceDto>();
                foreach (var inv in invoices)
                {
                    response.Result.Add(await MapToDtoAsync(inv));
                }
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<InvoiceDto>> GetInvoiceByIdAsync(int id)
        {
            var response = new ServiceResponse<InvoiceDto>();
            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.Branch)
                    .Include(i => i.Visit).ThenInclude(v => v.PatientId)
                    .Include(i => i.CreatedByUser)
                    .FirstOrDefaultAsync(i => i.InvoiceId == id);
                if (invoice == null) { response.Success = false; response.Message = "Invoice not found"; return response; }
                response.Result = await MapToDtoAsync(invoice);
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<InvoiceDto>> CreateInvoiceFromVisitAsync(int visitId)
        {
            var response = new ServiceResponse<InvoiceDto>();
            try
            {
                var visit = await _context.Visits
                    .Include(v => v.VisitTreatments)
                    .ThenInclude(vt => vt.Treatment)
                    .FirstOrDefaultAsync(v => v.VisitId == visitId);
                if (visit == null) { response.Success = false; response.Message = "Visit not found"; return response; }

                var grossAmount = visit.VisitTreatments.Sum(vt => vt.UnitPrice * vt.Quantity);
                var discountAmount = visit.VisitTreatments.Sum(vt => vt.DiscountAmount);
                var netAmount = grossAmount - discountAmount;

                var invoice = new Invoice
                {
                    BranchId = visit.BranchId,
                    VisitId = visitId,
                    InvoiceDate = DateTime.Now,
                    GrossAmount = grossAmount,
                    DiscountAmount = discountAmount,
                    NetAmount = netAmount,
                    PaidAmount = 0,
                    Status = "UNPAID"
                };
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                var getResponse = await GetInvoiceByIdAsync(invoice.InvoiceId);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<InvoiceDto>> UpdateInvoiceAsync(int id, InvoiceDto request)
        {
            var response = new ServiceResponse<InvoiceDto>();
            try
            {
                var invoice = await _context.Invoices.FindAsync(id);
                if (invoice == null) { response.Success = false; response.Message = "Invoice not found"; return response; }
                invoice.GrossAmount = request.GrossAmount;
                invoice.DiscountAmount = request.DiscountAmount;
                invoice.NetAmount = request.NetAmount;
                await _context.SaveChangesAsync();
                var getResponse = await GetInvoiceByIdAsync(id);
                response.Result = getResponse.Result;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteInvoiceAsync(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var invoice = await _context.Invoices.FindAsync(id);
                if (invoice == null) { response.Success = false; response.Message = "Invoice not found"; return response; }
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
                response.Result = true;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<InvoicePaymentDto>> AddPaymentAsync(int invoiceId, InvoicePaymentDto payment)
        {
            var response = new ServiceResponse<InvoicePaymentDto>();
            try
            {
                var invoice = await _context.Invoices.FindAsync(invoiceId);
                if (invoice == null) { response.Success = false; response.Message = "Invoice not found"; return response; }

                var paymentEntity = new InvoicePayment
                {
                    InvoiceId = invoiceId,
                    BranchId = invoice.BranchId,
                    PaymentDate = payment.PaymentDate,
                    Amount = payment.Amount,
                    Method = payment.Method,
                    ReferenceNo = payment.ReferenceNo,
                    Notes = payment.Notes,
                    ReceivedByUserId = payment.ReceivedByUserId
                };
                _context.InvoicePayments.Add(paymentEntity);
                
                invoice.PaidAmount += payment.Amount;
                invoice.Status = invoice.PaidAmount >= invoice.NetAmount ? "PAID" : 
                                invoice.PaidAmount > 0 ? "PARTIAL" : "UNPAID";
                
                await _context.SaveChangesAsync();

                response.Result = new InvoicePaymentDto
                {
                    PaymentId = paymentEntity.PaymentId,
                    InvoiceId = paymentEntity.InvoiceId,
                    PaymentDate = paymentEntity.PaymentDate,
                    Amount = paymentEntity.Amount,
                    Method = paymentEntity.Method,
                    ReferenceNo = paymentEntity.ReferenceNo,
                    Notes = paymentEntity.Notes,
                    ReceivedByUserId = paymentEntity.ReceivedByUserId
                };
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<List<InvoiceDto>>> GetInvoicesByBranchAsync(int branchId, DateTime? startDate, DateTime? endDate)
        {
            var response = new ServiceResponse<List<InvoiceDto>>();
            try
            {
                var query = _context.Invoices
                    .Include(i => i.Branch)
                    .Include(i => i.Visit).ThenInclude(v => v.PatientId)
                    .Include(i => i.CreatedByUser)
                    .Where(i => i.BranchId == branchId);
                if (startDate.HasValue) query = query.Where(i => i.InvoiceDate >= startDate.Value);
                if (endDate.HasValue) query = query.Where(i => i.InvoiceDate <= endDate.Value);
                var invoices = await query.OrderByDescending(i => i.InvoiceDate).ToListAsync();
                response.Result = new List<InvoiceDto>();
                foreach (var inv in invoices)
                {
                    response.Result.Add(await MapToDtoAsync(inv));
                }
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<List<InvoiceDto>>> GetInvoicesByPatientAsync(int patientId)
        {
            var response = new ServiceResponse<List<InvoiceDto>>();
            try
            {
                var invoices = await _context.Invoices
                    .Include(i => i.Branch)
                    .Include(i => i.Visit).ThenInclude(v => v.PatientId)
                    .Include(i => i.CreatedByUser)
                    .Where(i => i.Visit.PatientId == patientId)
                    .OrderByDescending(i => i.InvoiceDate)
                    .ToListAsync();
                response.Result = new List<InvoiceDto>();
                foreach (var inv in invoices)
                {
                    response.Result.Add(await MapToDtoAsync(inv));
                }
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }

        public async Task<ServiceResponse<decimal>> GetOutstandingBalanceAsync(int invoiceId)
        {
            var response = new ServiceResponse<decimal>();
            try
            {
                var invoice = await _context.Invoices.FindAsync(invoiceId);
                if (invoice == null) { response.Success = false; response.Message = "Invoice not found"; return response; }
                response.Result = invoice.NetAmount - invoice.PaidAmount;
                response.Success = true;
            }
            catch (Exception ex) { response.Success = false; response.Message = ex.Message; }
            return response;
        }
    }
}

