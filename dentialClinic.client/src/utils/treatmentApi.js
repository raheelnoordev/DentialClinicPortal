import { apiRequest } from './api.js';

export const treatmentApi = {
  getAllTreatments: () => apiRequest('/api/Treatments/GetAllTreatments'),
  getTreatmentById: (id) => apiRequest(`/api/Treatments/GetTreatmentById/${id}`),
  createTreatment: (data) => apiRequest('/api/Treatments/CreateTreatment', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  updateTreatment: (id, data) => apiRequest(`/api/Treatments/UpdateTreatment/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
  deleteTreatment: (id) => apiRequest(`/api/Treatments/DeleteTreatment/${id}`, {
    method: 'DELETE',
  }),
  getActiveTreatments: () => apiRequest('/Treatments/GetActiveTreatments'),
};

