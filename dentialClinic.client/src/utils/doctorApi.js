import { apiRequest } from './api.js';

export const doctorApi = {
  viewDoctors: () => apiRequest('/Doctors/ViewDoctors'),
  createDoctor: (data) => apiRequest('/Doctors/CreateDoctor', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  updateDoctor: (id, data) => apiRequest(`/Doctors/UpdateDoctor/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
  deleteDoctor: (id) => apiRequest(`/Doctors/DeleteDoctor/${id}`, {
    method: 'DELETE',
  }),
};

