import { apiRequest } from './api.js';

export const patientApi = {
  getAllPatients: () => apiRequest('/Patients/GetAllPatients'),
  getPatientById: (id) => apiRequest(`/Patients/GetPatientById/${id}`),
  createPatient: (data) => apiRequest('/Patients/CreatePatient', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  updatePatient: (id, data) => apiRequest(`/Patients/UpdatePatient/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
  deletePatient: (id) => apiRequest(`/Patients/DeletePatient/${id}`, {
    method: 'DELETE',
  }),
};

