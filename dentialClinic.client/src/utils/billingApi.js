import { apiRequest } from './api.js';

export const billingApi = {
  getAllInvoices: () => apiRequest('/api/Billing/GetAllInvoices'),
  getInvoiceById: (id) => apiRequest(`/api/Billing/GetInvoiceById/${id}`),
  createInvoiceFromVisit: (visitId) => apiRequest(`/api/Billing/CreateInvoiceFromVisit/${visitId}`, {
    method: 'POST',
  }),
  updateInvoice: (id, data) => apiRequest(`/api/Billing/UpdateInvoice/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
  deleteInvoice: (id) => apiRequest(`/api/Billing/DeleteInvoice/${id}`, {
    method: 'DELETE',
  }),
  addPayment: (invoiceId, payment) => apiRequest(`/api/Billing/AddPayment/${invoiceId}`, {
    method: 'POST',
    body: JSON.stringify(payment),
  }),
  getInvoicesByBranch: (branchId, startDate, endDate) => {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate.toISOString());
    if (endDate) params.append('endDate', endDate.toISOString());
    return apiRequest(`/api/Billing/GetInvoicesByBranch/${branchId}?${params.toString()}`);
  },
  getInvoicesByPatient: (patientId) => apiRequest(`/api/Billing/GetInvoicesByPatient/${patientId}`),
  getOutstandingBalance: (invoiceId) => apiRequest(`/api/Billing/GetOutstandingBalance/${invoiceId}`),
};

