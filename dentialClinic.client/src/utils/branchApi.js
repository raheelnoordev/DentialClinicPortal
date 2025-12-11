import { apiRequest } from './api.js';

export const branchApi = {
  getAllBranches: () => apiRequest('/Branches/GetAllBranches'),
  getBranchById: (id) => apiRequest(`/Branches/GetBranchById/${id}`),
  createBranch: (data) => apiRequest('/Branches/CreateBranch', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  updateBranch: (id, data) => apiRequest(`/Branches/UpdateBranch/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
  deleteBranch: (id) => apiRequest(`/Branches/DeleteBranch/${id}`, {
    method: 'DELETE',
  }),
  getActiveBranches: () => apiRequest('/Branches/GetActiveBranches'),
};

