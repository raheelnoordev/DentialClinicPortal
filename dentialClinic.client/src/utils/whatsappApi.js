import { apiRequest } from './api.js';

export const whatsappApi = {
  // Settings
  getAllSettings: () => apiRequest('/api/WhatsApp/GetAllSettings'),
  getSettingsByBranch: (branchId) => apiRequest(`/api/WhatsApp/GetSettingsByBranch/${branchId}`),
  createOrUpdateSettings: (data) => apiRequest('/api/WhatsApp/CreateOrUpdateSettings', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  
  // Screens
  getAllScreens: (branchId) => {
    const param = branchId ? `?branchId=${branchId}` : '';
    return apiRequest(`/api/WhatsApp/GetAllScreens${param}`);
  },
  getScreenById: (id) => apiRequest(`/api/WhatsApp/GetScreenById/${id}`),
  createScreen: (data) => apiRequest('/api/WhatsApp/CreateScreen', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  updateScreen: (id, data) => apiRequest(`/api/WhatsApp/UpdateScreen/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
  deleteScreen: (id) => apiRequest(`/api/WhatsApp/DeleteScreen/${id}`, {
    method: 'DELETE',
  }),
  
  // Screen Options
  getOptionsByScreen: (screenId) => apiRequest(`/api/WhatsApp/GetOptionsByScreen/${screenId}`),
  createOption: (data) => apiRequest('/api/WhatsApp/CreateOption', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  updateOption: (id, data) => apiRequest(`/api/WhatsApp/UpdateOption/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
  deleteOption: (id) => apiRequest(`/api/WhatsApp/DeleteOption/${id}`, {
    method: 'DELETE',
  }),
  
  // Conversation State
  getConversationState: (fromNumber) => apiRequest(`/api/WhatsApp/GetConversationState?fromNumber=${encodeURIComponent(fromNumber)}`),
  updateConversationState: (data) => apiRequest('/api/WhatsApp/UpdateConversationState', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  
  // Bookings
  getAllBookings: (branchId) => {
    const param = branchId ? `?branchId=${branchId}` : '';
    return apiRequest(`/api/WhatsApp/GetAllBookings${param}`);
  },
  getBookingById: (id) => apiRequest(`/api/WhatsApp/GetBookingById/${id}`),
  createBooking: (data) => apiRequest('/api/WhatsApp/CreateBooking', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  updateBookingStatus: (id, status, patientId) => {
    const params = new URLSearchParams();
    params.append('status', status);
    if (patientId) params.append('patientId', patientId);
    return apiRequest(`/api/WhatsApp/UpdateBookingStatus/${id}?${params.toString()}`, {
      method: 'PUT',
    });
  },
};

