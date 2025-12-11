import { apiRequest } from './api.js';

export const expenseApi = {
  // Expense Categories
  getAllExpenseCategories: () => apiRequest('/api/Expenses/GetAllExpenseCategories'),
  getExpenseCategoryById: (id) => apiRequest(`/api/Expenses/GetExpenseCategoryById/${id}`),
  createExpenseCategory: (data) => apiRequest('/api/Expenses/CreateExpenseCategory', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  updateExpenseCategory: (id, data) => apiRequest(`/api/Expenses/UpdateExpenseCategory/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
  deleteExpenseCategory: (id) => apiRequest(`/api/Expenses/DeleteExpenseCategory/${id}`, {
    method: 'DELETE',
  }),
  
  // Expenses
  getAllExpenses: () => apiRequest('/api/Expenses/GetAllExpenses'),
  getExpenseById: (id) => apiRequest(`/api/Expenses/GetExpenseById/${id}`),
  createExpense: (data) => apiRequest('/api/Expenses/CreateExpense', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  updateExpense: (id, data) => apiRequest(`/api/Expenses/UpdateExpense/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
  deleteExpense: (id) => apiRequest(`/api/Expenses/DeleteExpense/${id}`, {
    method: 'DELETE',
  }),
  getExpensesByBranch: (branchId, startDate, endDate) => {
    const params = new URLSearchParams();
    if (startDate) params.append('startDate', startDate.toISOString());
    if (endDate) params.append('endDate', endDate.toISOString());
    return apiRequest(`/api/Expenses/GetExpensesByBranch/${branchId}?${params.toString()}`);
  },
  
  // Salary Payments
  getAllSalaryPayments: () => apiRequest('/api/Expenses/GetAllSalaryPayments'),
  getSalaryPaymentById: (id) => apiRequest(`/api/Expenses/GetSalaryPaymentById/${id}`),
  createSalaryPayment: (data) => apiRequest('/api/Expenses/CreateSalaryPayment', {
    method: 'POST',
    body: JSON.stringify(data),
  }),
  updateSalaryPayment: (id, data) => apiRequest(`/api/Expenses/UpdateSalaryPayment/${id}`, {
    method: 'PUT',
    body: JSON.stringify(data),
  }),
  deleteSalaryPayment: (id) => apiRequest(`/api/Expenses/DeleteSalaryPayment/${id}`, {
    method: 'DELETE',
  }),
  getSalaryPaymentsByBranch: (branchId, month, year) => {
    const params = new URLSearchParams();
    if (month) params.append('month', month);
    if (year) params.append('year', year);
    return apiRequest(`/api/Expenses/GetSalaryPaymentsByBranch/${branchId}?${params.toString()}`);
  },
};

