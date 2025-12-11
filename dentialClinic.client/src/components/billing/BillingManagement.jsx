import React, { useState, useEffect } from 'react';
import {
  Box, Typography, Button, Grid, TextField, Dialog, DialogTitle, DialogContent,
  DialogActions, Alert, CircularProgress, IconButton, Snackbar, Card, CardContent,
  Chip, MenuItem, Select, FormControl, InputLabel, Table, TableBody, TableCell, TableHead, TableRow
} from '@mui/material';
import { Add as AddIcon, Payment as PaymentIcon, Receipt as ReceiptIcon } from '@mui/icons-material';
import { billingApi } from '../../utils/billingApi';
import { visitApi } from '../../utils/visitApi';

const BillingManagement = () => {
  const [invoices, setInvoices] = useState([]);
  const [visits, setVisits] = useState([]);
  const [loading, setLoading] = useState(true);
  const [openInvoiceDialog, setOpenInvoiceDialog] = useState(false);
  const [openPaymentDialog, setOpenPaymentDialog] = useState(false);
  const [selectedInvoice, setSelectedInvoice] = useState(null);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
  const [paymentForm, setPaymentForm] = useState({
    amount: '', method: 'CASH', referenceNo: '', notes: '', paymentDate: new Date().toISOString().split('T')[0]
  });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [invoicesRes, visitsRes] = await Promise.all([
        billingApi.getAllInvoices(),
        visitApi.getAllVisits()
      ]);
      if (invoicesRes.success) setInvoices(invoicesRes.result || []);
      if (visitsRes.success) setVisits(visitsRes.result || []);
    } catch (error) {
      showSnackbar('Error loading data', 'error');
    } finally {
      setLoading(false);
    }
  };

  const showSnackbar = (message, severity = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  const handleCreateInvoice = async (visitId) => {
    try {
      const response = await billingApi.createInvoiceFromVisit(visitId);
      if (response.success) {
        showSnackbar('Invoice created successfully');
        fetchData();
      }
    } catch (error) {
      showSnackbar('Error creating invoice', 'error');
    }
  };

  const handleAddPayment = async () => {
    if (!selectedInvoice) return;
    try {
      const response = await billingApi.addPayment(selectedInvoice.invoiceId, paymentForm);
      if (response.success) {
        showSnackbar('Payment added successfully');
        fetchData();
        setOpenPaymentDialog(false);
        setPaymentForm({ amount: '', method: 'CASH', referenceNo: '', notes: '', paymentDate: new Date().toISOString().split('T')[0] });
      }
    } catch (error) {
      showSnackbar('Error adding payment', 'error');
    }
  };

  const getStatusColor = (status) => {
    const colors = { PAID: 'success', PARTIAL: 'warning', UNPAID: 'error' };
    return colors[status] || 'default';
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
        <Typography variant="h4" sx={{ fontWeight: 600, color: '#228B22' }}>Billing Management</Typography>
      </Box>

      <Grid container spacing={3}>
        {invoices.map((invoice) => (
          <Grid item xs={12} sm={6} md={4} key={invoice.invoiceId}>
            <Card sx={{ height: '100%', '&:hover': { boxShadow: 4 } }}>
              <CardContent>
                <Box display="flex" justifyContent="space-between" alignItems="start" mb={2}>
                  <Typography variant="h6" sx={{ fontWeight: 600 }}>Invoice #{invoice.invoiceId}</Typography>
                  <Chip label={invoice.status} color={getStatusColor(invoice.status)} size="small" />
                </Box>
                <Typography variant="body2" color="text.secondary" mb={1}>Patient: {invoice.patientName}</Typography>
                <Typography variant="body2" color="text.secondary" mb={1}>Net Amount: ${invoice.netAmount}</Typography>
                <Typography variant="body2" color="text.secondary" mb={1}>Paid: ${invoice.paidAmount}</Typography>
                <Typography variant="body2" color="text.secondary" mb={2}>Outstanding: ${invoice.outstandingAmount}</Typography>
                <Box display="flex" gap={1} mt={2}>
                  <Button size="small" startIcon={<PaymentIcon />} onClick={() => {
                    setSelectedInvoice(invoice);
                    setOpenPaymentDialog(true);
                  }} variant="outlined" disabled={invoice.status === 'PAID'}>Add Payment</Button>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      <Box sx={{ mt: 4 }}>
        <Typography variant="h5" sx={{ mb: 2 }}>Visits Without Invoices</Typography>
        <Grid container spacing={2}>
          {visits.filter(v => v.status === 'CLOSED').map((visit) => {
            const hasInvoice = invoices.some(inv => inv.visitId === visit.visitId);
            if (hasInvoice) return null;
            return (
              <Grid item xs={12} sm={6} md={4} key={visit.visitId}>
                <Card>
                  <CardContent>
                    <Typography variant="h6">{visit.patientName}</Typography>
                    <Typography variant="body2" color="text.secondary">Visit Date: {new Date(visit.visitTime).toLocaleDateString()}</Typography>
                    <Button size="small" startIcon={<ReceiptIcon />} onClick={() => handleCreateInvoice(visit.visitId)} sx={{ mt: 1 }}>Create Invoice</Button>
                  </CardContent>
                </Card>
              </Grid>
            );
          })}
        </Grid>
      </Box>

      <Dialog open={openPaymentDialog} onClose={() => setOpenPaymentDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Add Payment</DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={12}><Typography variant="body1">Invoice #{selectedInvoice?.invoiceId} - Outstanding: ${selectedInvoice?.outstandingAmount}</Typography></Grid>
            <Grid item xs={12} sm={6}><TextField fullWidth label="Amount *" type="number" value={paymentForm.amount} onChange={(e) => setPaymentForm({ ...paymentForm, amount: e.target.value })} /></Grid>
            <Grid item xs={12} sm={6}><TextField fullWidth type="date" label="Payment Date *" value={paymentForm.paymentDate} onChange={(e) => setPaymentForm({ ...paymentForm, paymentDate: e.target.value })} InputLabelProps={{ shrink: true }} /></Grid>
            <Grid item xs={12}><FormControl fullWidth><InputLabel>Payment Method *</InputLabel><Select value={paymentForm.method} onChange={(e) => setPaymentForm({ ...paymentForm, method: e.target.value })} label="Payment Method *"><MenuItem value="CASH">Cash</MenuItem><MenuItem value="CARD">Card</MenuItem><MenuItem value="BANK">Bank</MenuItem><MenuItem value="ONLINE">Online</MenuItem></Select></FormControl></Grid>
            <Grid item xs={12}><TextField fullWidth label="Reference No" value={paymentForm.referenceNo} onChange={(e) => setPaymentForm({ ...paymentForm, referenceNo: e.target.value })} /></Grid>
            <Grid item xs={12}><TextField fullWidth label="Notes" multiline rows={2} value={paymentForm.notes} onChange={(e) => setPaymentForm({ ...paymentForm, notes: e.target.value })} /></Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenPaymentDialog(false)}>Cancel</Button>
          <Button onClick={handleAddPayment} variant="contained" sx={{ bgcolor: '#228B22' }}>Add Payment</Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={snackbar.open} autoHideDuration={6000} onClose={() => setSnackbar({ ...snackbar, open: false })}>
        <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  );
};

export default BillingManagement;

