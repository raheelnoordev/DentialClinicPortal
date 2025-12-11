import { apiRequest } from './api.js';

export const visitApi = {
  getAllVisits: () => apiRequest('/api/Visits/GetAllVisits'),
  getVisitById: (id) => apiRequest(`/api/Visits/GetVisitById/${id}`),
  createVisit: (data) => apiRequest('/api/Visits/CreateVisit', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  updateVisit: (id, data) => apiRequest(`/api/Visits/UpdateVisit/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
  deleteVisit: (id) => apiRequest(`/api/Visits/DeleteVisit/${id}`, {
    method: 'DELETE',
  }),
  getVisitsByBranch: (branchId, date) => {
    const dateParam = date ? `&date=${date.toISOString()}` : '';
    return apiRequest(`/api/Visits/GetVisitsByBranch/${branchId}?${dateParam}`);
  },
  getVisitsByPatient: (patientId) => apiRequest(`/api/Visits/GetVisitsByPatient/${patientId}`),
  closeVisit: (id) => apiRequest(`/api/Visits/CloseVisit/${id}`, {
    method: 'PUT',
  }),
};

