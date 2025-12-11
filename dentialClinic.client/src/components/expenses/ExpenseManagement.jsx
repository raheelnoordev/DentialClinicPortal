import React, { useState, useEffect } from 'react';
import {
  Box, Typography, Button, Grid, TextField, Dialog, DialogTitle, DialogContent,
  DialogActions, Alert, CircularProgress, IconButton, Snackbar, Card, CardContent,
  Chip, MenuItem, Select, FormControl, InputLabel, Tabs, Tab
} from '@mui/material';
import { Add as AddIcon, Edit as EditIcon, Delete as DeleteIcon, AttachMoney as MoneyIcon } from '@mui/icons-material';
import { expenseApi } from '../../utils/expenseApi';
import { branchApi } from '../../utils/branchApi';
import { userManagementApi } from '../../utils/api';

const ExpenseManagement = () => {
  const [expenses, setExpenses] = useState([]);
  const [categories, setCategories] = useState([]);
  const [salaryPayments, setSalaryPayments] = useState([]);
  const [branches, setBranches] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [tabValue, setTabValue] = useState(0);
  const [openExpenseDialog, setOpenExpenseDialog] = useState(false);
  const [openSalaryDialog, setOpenSalaryDialog] = useState(false);
  const [openCategoryDialog, setOpenCategoryDialog] = useState(false);
  const [editingExpense, setEditingExpense] = useState(null);
  const [editingSalary, setEditingSalary] = useState(null);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
  const [expenseForm, setExpenseForm] = useState({
    branchId: '', expenseCategoryId: '', description: '', amount: '', expenseDate: new Date().toISOString().split('T')[0],
    paidTo: '', paymentMethod: 'CASH', notes: ''
  });
  const [salaryForm, setSalaryForm] = useState({
    branchId: '', userId: '', month: new Date().getMonth() + 1, year: new Date().getFullYear(),
    basicAmount: '', bonusAmount: 0, deductions: 0, netPaidAmount: '', paymentDate: new Date().toISOString().split('T')[0],
    paymentMethod: 'CASH', notes: ''
  });
  const [categoryForm, setCategoryForm] = useState({ name: '', isSalaryCategory: false });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [expensesRes, categoriesRes, salaryRes, branchesRes, usersRes] = await Promise.all([
        expenseApi.getAllExpenses(),
        expenseApi.getAllExpenseCategories(),
        expenseApi.getAllSalaryPayments(),
        branchApi.getActiveBranches(),
        userManagementApi.getUsers()
      ]);
      if (expensesRes.success) setExpenses(expensesRes.result || []);
      if (categoriesRes.success) setCategories(categoriesRes.result || []);
      if (salaryRes.success) setSalaryPayments(salaryRes.result || []);
      if (branchesRes.success) setBranches(branchesRes.result || []);
      if (usersRes.success) setUsers(usersRes.result?.map(u => u.user || u) || []);
    } catch (error) {
      showSnackbar('Error loading data', 'error');
    } finally {
      setLoading(false);
    }
  };

  const showSnackbar = (message, severity = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  const handleSaveExpense = async () => {
    try {
      const response = editingExpense
        ? await expenseApi.updateExpense(editingExpense.expenseId, expenseForm)
        : await expenseApi.createExpense(expenseForm);
      
      if (response.success) {
        showSnackbar(`Expense ${editingExpense ? 'updated' : 'created'} successfully`);
        fetchData();
        setOpenExpenseDialog(false);
      }
    } catch (error) {
      showSnackbar('Error saving expense', 'error');
    }
  };

  const handleSaveSalary = async () => {
    try {
      const response = editingSalary
        ? await expenseApi.updateSalaryPayment(editingSalary.salaryPaymentId, salaryForm)
        : await expenseApi.createSalaryPayment(salaryForm);
      
      if (response.success) {
        showSnackbar(`Salary payment ${editingSalary ? 'updated' : 'created'} successfully`);
        fetchData();
        setOpenSalaryDialog(false);
      }
    } catch (error) {
      showSnackbar('Error saving salary payment', 'error');
    }
  };

  const handleSaveCategory = async () => {
    try {
      const response = await expenseApi.createExpenseCategory(categoryForm);
      if (response.success) {
        showSnackbar('Category created successfully');
        fetchData();
        setOpenCategoryDialog(false);
      }
    } catch (error) {
      showSnackbar('Error saving category', 'error');
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box sx={{ p: 3 }}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4" sx={{ fontWeight: 600, color: '#228B22' }}>Expense Management</Typography>
        <Box display="flex" gap={1}>
          {tabValue === 0 && <Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpenExpenseDialog(true)} sx={{ bgcolor: '#228B22' }}>Add Expense</Button>}
          {tabValue === 1 && <Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpenSalaryDialog(true)} sx={{ bgcolor: '#228B22' }}>Add Salary Payment</Button>}
          {tabValue === 2 && <Button variant="contained" startIcon={<AddIcon />} onClick={() => setOpenCategoryDialog(true)} sx={{ bgcolor: '#228B22' }}>Add Category</Button>}
        </Box>
      </Box>

      <Tabs value={tabValue} onChange={(e, v) => setTabValue(v)} sx={{ mb: 3 }}>
        <Tab label="Expenses" />
        <Tab label="Salary Payments" />
        <Tab label="Categories" />
      </Tabs>

      {tabValue === 0 && (
        <Grid container spacing={3}>
          {expenses.map((expense) => (
            <Grid item xs={12} sm={6} md={4} key={expense.expenseId}>
              <Card sx={{ height: '100%', '&:hover': { boxShadow: 4 } }}>
                <CardContent>
                  <Typography variant="h6" sx={{ fontWeight: 600, mb: 1 }}>{expense.expenseCategoryName}</Typography>
                  <Typography variant="body2" color="text.secondary" mb={1}>Amount: ${expense.amount}</Typography>
                  <Typography variant="body2" color="text.secondary" mb={1}>Date: {new Date(expense.expenseDate).toLocaleDateString()}</Typography>
                  {expense.paidTo && <Typography variant="body2" color="text.secondary" mb={2}>Paid To: {expense.paidTo}</Typography>}
                  <Box display="flex" gap={1} mt={2}>
                    <IconButton size="small" onClick={() => { setEditingExpense(expense); setExpenseForm({ ...expense, expenseDate: expense.expenseDate.split('T')[0] }); setOpenExpenseDialog(true); }} color="primary"><EditIcon /></IconButton>
                    <IconButton size="small" onClick={async () => {
                      if (!window.confirm('Delete this expense?')) return;
                      const res = await expenseApi.deleteExpense(expense.expenseId);
                      if (res.success) { showSnackbar('Expense deleted'); fetchData(); }
                    }} color="error"><DeleteIcon /></IconButton>
                  </Box>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}

      {tabValue === 1 && (
        <Grid container spacing={3}>
          {salaryPayments.map((salary) => (
            <Grid item xs={12} sm={6} md={4} key={salary.salaryPaymentId}>
              <Card sx={{ height: '100%', '&:hover': { boxShadow: 4 } }}>
                <CardContent>
                  <Typography variant="h6" sx={{ fontWeight: 600, mb: 1 }}>{salary.userName}</Typography>
                  <Typography variant="body2" color="text.secondary" mb={1}>Month: {salary.month}/{salary.year}</Typography>
                  <Typography variant="body2" color="text.secondary" mb={1}>Net Amount: ${salary.netPaidAmount}</Typography>
                  <Typography variant="body2" color="text.secondary" mb={2}>Date: {new Date(salary.paymentDate).toLocaleDateString()}</Typography>
                  <Box display="flex" gap={1} mt={2}>
                    <IconButton size="small" onClick={() => { setEditingSalary(salary); setSalaryForm({ ...salary, paymentDate: salary.paymentDate.split('T')[0] }); setOpenSalaryDialog(true); }} color="primary"><EditIcon /></IconButton>
                  </Box>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}

      {tabValue === 2 && (
        <Grid container spacing={3}>
          {categories.map((category) => (
            <Grid item xs={12} sm={6} md={4} key={category.expenseCategoryId}>
              <Card>
                <CardContent>
                  <Box display="flex" justifyContent="space-between" alignItems="center">
                    <Typography variant="h6">{category.name}</Typography>
                    {category.isSalaryCategory && <Chip label="Salary" color="primary" size="small" />}
                  </Box>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}

      <Dialog open={openExpenseDialog} onClose={() => { setOpenExpenseDialog(false); setEditingExpense(null); }} maxWidth="sm" fullWidth>
        <DialogTitle>{editingExpense ? 'Edit Expense' : 'Add New Expense'}</DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={12} sm={6}><FormControl fullWidth><InputLabel>Branch *</InputLabel><Select value={expenseForm.branchId} onChange={(e) => setExpenseForm({ ...expenseForm, branchId: e.target.value })} label="Branch *">{branches.map(b => <MenuItem key={b.branchId} value={b.branchId}>{b.name}</MenuItem>)}</Select></FormControl></Grid>
            <Grid item xs={12} sm={6}><FormControl fullWidth><InputLabel>Category *</InputLabel><Select value={expenseForm.expenseCategoryId} onChange={(e) => setExpenseForm({ ...expenseForm, expenseCategoryId: e.target.value })} label="Category *">{categories.map(c => <MenuItem key={c.expenseCategoryId} value={c.expenseCategoryId}>{c.name}</MenuItem>)}</Select></FormControl></Grid>
            <Grid item xs={12} sm={6}><TextField fullWidth label="Amount *" type="number" value={expenseForm.amount} onChange={(e) => setExpenseForm({ ...expenseForm, amount: e.target.value })} /></Grid>
            <Grid item xs={12} sm={6}><TextField fullWidth type="date" label="Date *" value={expenseForm.expenseDate} onChange={(e) => setExpenseForm({ ...expenseForm, expenseDate: e.target.value })} InputLabelProps={{ shrink: true }} /></Grid>
            <Grid item xs={12}><TextField fullWidth label="Description" value={expenseForm.description} onChange={(e) => setExpenseForm({ ...expenseForm, description: e.target.value })} /></Grid>
            <Grid item xs={12} sm={6}><TextField fullWidth label="Paid To" value={expenseForm.paidTo} onChange={(e) => setExpenseForm({ ...expenseForm, paidTo: e.target.value })} /></Grid>
            <Grid item xs={12} sm={6}><FormControl fullWidth><InputLabel>Payment Method</InputLabel><Select value={expenseForm.paymentMethod} onChange={(e) => setExpenseForm({ ...expenseForm, paymentMethod: e.target.value })} label="Payment Method"><MenuItem value="CASH">Cash</MenuItem><MenuItem value="BANK">Bank</MenuItem><MenuItem value="OTHER">Other</MenuItem></Select></FormControl></Grid>
            <Grid item xs={12}><TextField fullWidth label="Notes" multiline rows={2} value={expenseForm.notes} onChange={(e) => setExpenseForm({ ...expenseForm, notes: e.target.value })} /></Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => { setOpenExpenseDialog(false); setEditingExpense(null); }}>Cancel</Button>
          <Button onClick={handleSaveExpense} variant="contained" sx={{ bgcolor: '#228B22' }}>Save</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={openSalaryDialog} onClose={() => { setOpenSalaryDialog(false); setEditingSalary(null); }} maxWidth="sm" fullWidth>
        <DialogTitle>{editingSalary ? 'Edit Salary Payment' : 'Add New Salary Payment'}</DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={12} sm={6}><FormControl fullWidth><InputLabel>Branch *</InputLabel><Select value={salaryForm.branchId} onChange={(e) => setSalaryForm({ ...salaryForm, branchId: e.target.value })} label="Branch *">{branches.map(b => <MenuItem key={b.branchId} value={b.branchId}>{b.name}</MenuItem>)}</Select></FormControl></Grid>
            <Grid item xs={12} sm={6}><FormControl fullWidth><InputLabel>User *</InputLabel><Select value={salaryForm.userId} onChange={(e) => setSalaryForm({ ...salaryForm, userId: e.target.value })} label="User *">{users.map(u => <MenuItem key={u.userId} value={u.userId}>{u.fullName || u.firstName}</MenuItem>)}</Select></FormControl></Grid>
            <Grid item xs={12} sm={4}><TextField fullWidth label="Month *" type="number" inputProps={{ min: 1, max: 12 }} value={salaryForm.month} onChange={(e) => setSalaryForm({ ...salaryForm, month: parseInt(e.target.value) })} /></Grid>
            <Grid item xs={12} sm={4}><TextField fullWidth label="Year *" type="number" value={salaryForm.year} onChange={(e) => setSalaryForm({ ...salaryForm, year: parseInt(e.target.value) })} /></Grid>
            <Grid item xs={12} sm={4}><TextField fullWidth type="date" label="Payment Date *" value={salaryForm.paymentDate} onChange={(e) => setSalaryForm({ ...salaryForm, paymentDate: e.target.value })} InputLabelProps={{ shrink: true }} /></Grid>
            <Grid item xs={12} sm={4}><TextField fullWidth label="Basic Amount *" type="number" value={salaryForm.basicAmount} onChange={(e) => setSalaryForm({ ...salaryForm, basicAmount: e.target.value })} /></Grid>
            <Grid item xs={12} sm={4}><TextField fullWidth label="Bonus" type="number" value={salaryForm.bonusAmount} onChange={(e) => setSalaryForm({ ...salaryForm, bonusAmount: e.target.value })} /></Grid>
            <Grid item xs={12} sm={4}><TextField fullWidth label="Deductions" type="number" value={salaryForm.deductions} onChange={(e) => setSalaryForm({ ...salaryForm, deductions: e.target.value })} /></Grid>
            <Grid item xs={12} sm={6}><TextField fullWidth label="Net Paid Amount *" type="number" value={salaryForm.netPaidAmount} onChange={(e) => setSalaryForm({ ...salaryForm, netPaidAmount: e.target.value })} /></Grid>
            <Grid item xs={12} sm={6}><FormControl fullWidth><InputLabel>Payment Method</InputLabel><Select value={salaryForm.paymentMethod} onChange={(e) => setSalaryForm({ ...salaryForm, paymentMethod: e.target.value })} label="Payment Method"><MenuItem value="CASH">Cash</MenuItem><MenuItem value="BANK">Bank</MenuItem></Select></FormControl></Grid>
            <Grid item xs={12}><TextField fullWidth label="Notes" multiline rows={2} value={salaryForm.notes} onChange={(e) => setSalaryForm({ ...salaryForm, notes: e.target.value })} /></Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => { setOpenSalaryDialog(false); setEditingSalary(null); }}>Cancel</Button>
          <Button onClick={handleSaveSalary} variant="contained" sx={{ bgcolor: '#228B22' }}>Save</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={openCategoryDialog} onClose={() => setOpenCategoryDialog(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Add Expense Category</DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={12}><TextField fullWidth label="Name *" value={categoryForm.name} onChange={(e) => setCategoryForm({ ...categoryForm, name: e.target.value })} /></Grid>
            <Grid item xs={12}><FormControl fullWidth><InputLabel>Type</InputLabel><Select value={categoryForm.isSalaryCategory} onChange={(e) => setCategoryForm({ ...categoryForm, isSalaryCategory: e.target.value })} label="Type"><MenuItem value={false}>Regular Expense</MenuItem><MenuItem value={true}>Salary Category</MenuItem></Select></FormControl></Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenCategoryDialog(false)}>Cancel</Button>
          <Button onClick={handleSaveCategory} variant="contained" sx={{ bgcolor: '#228B22' }}>Save</Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={snackbar.open} autoHideDuration={6000} onClose={() => setSnackbar({ ...snackbar, open: false })}>
        <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  );
};

export default ExpenseManagement;

