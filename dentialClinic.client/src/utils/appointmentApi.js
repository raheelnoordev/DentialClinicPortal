import { apiRequest } from './api.js';

export const appointmentApi = {
  getAllAppointments: (filters = {}) => {
    const params = new URLSearchParams();
    if (filters.branchId) params.append('branchId', filters.branchId);
    if (filters.doctorId) params.append('doctorId', filters.doctorId);
    if (filters.status) params.append('status', filters.status);
    if (filters.dateFrom) params.append('dateFrom', filters.dateFrom.toISOString());
    if (filters.dateTo) params.append('dateTo', filters.dateTo.toISOString());
    const queryString = params.toString();
    return apiRequest(`/Appointments${queryString ? `?${queryString}` : ''}`);
  },
  getAppointmentById: (id) => apiRequest(`/Appointments/${id}`),
  createAppointment: (data) => apiRequest('/Appointments', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  updateAppointment: (id, data) => apiRequest(`/Appointments/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
  updateAppointmentStatus: (id, status) => apiRequest(`/Appointments/${id}/status?status=${encodeURIComponent(status)}`, {
    method: 'PATCH',
  }),
  deleteAppointment: (id) => apiRequest(`/Appointments/${id}`, {
    method: 'DELETE',
  }),
};

