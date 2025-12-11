import React, { useState, useEffect } from 'react';
import {
  Box, Typography, Button, Grid, TextField, Dialog, DialogTitle, DialogContent,
  DialogActions, Alert, CircularProgress, IconButton, Snackbar, Card, CardContent,
  Chip, MenuItem, Select, FormControl, InputLabel, Table, TableBody, TableCell, TableHead, TableRow
} from '@mui/material';
import { Add as AddIcon, Edit as EditIcon, Delete as DeleteIcon, CheckCircle as CheckCircleIcon } from '@mui/icons-material';
import { visitApi } from '../../utils/visitApi';
import { branchApi } from '../../utils/branchApi';
import { patientApi } from '../../utils/patientApi';
import { doctorApi } from '../../utils/doctorApi';
import { treatmentApi } from '../../utils/treatmentApi';
import { appointmentApi } from '../../utils/appointmentApi';

const VisitManagement = () => {
  const [visits, setVisits] = useState([]);
  const [branches, setBranches] = useState([]);
  const [patients, setPatients] = useState([]);
  const [doctors, setDoctors] = useState([]);
  const [treatments, setTreatments] = useState([]);
  const [appointments, setAppointments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [openDialog, setOpenDialog] = useState(false);
  const [editingVisit, setEditingVisit] = useState(null);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
  const [formData, setFormData] = useState({
    branchId: '', appointmentId: '', patientId: '', doctorId: '', visitTime: new Date().toISOString().slice(0, 16),
    diagnosis: '', notes: '', status: 'OPEN', treatments: []
  });
  const [treatmentForm, setTreatmentForm] = useState({ treatmentId: '', toothNumber: '', quantity: 1, unitPrice: '', discountAmount: 0, notes: '' });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [branchesRes, patientsRes, doctorsRes, treatmentsRes] = await Promise.all([
        branchApi.getActiveBranches(),
        patientApi.getAllPatients(),
        doctorApi.getActiveDoctors(),
        treatmentApi.getActiveTreatments()
      ]);
      if (branchesRes.success) setBranches(branchesRes.result || []);
      if (patientsRes.success) setPatients(patientsRes.result || []);
      if (doctorsRes.success) setDoctors(doctorsRes.result || []);
      if (treatmentsRes.success) setTreatments(treatmentsRes.result || []);
      
      if (branchesRes.result?.length > 0) {
        const visitsRes = await visitApi.getVisitsByBranch(branchesRes.result[0].branchId, null);
        if (visitsRes.success) setVisits(visitsRes.result || []);
      }
    } catch (error) {
      showSnackbar('Error loading data', 'error');
    } finally {
      setLoading(false);
    }
  };

  const showSnackbar = (message, severity = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  const handleOpenDialog = (visit = null) => {
    if (visit) {
      setEditingVisit(visit);
      setFormData({
        branchId: visit.branchId || '',
        appointmentId: visit.appointmentId || '',
        patientId: visit.patientId || '',
        doctorId: visit.doctorId || '',
        visitTime: visit.visitTime ? new Date(visit.visitTime).toISOString().slice(0, 16) : new Date().toISOString().slice(0, 16),
        diagnosis: visit.diagnosis || '',
        notes: visit.notes || '',
        status: visit.status || 'OPEN',
        treatments: visit.treatments || []
      });
    } else {
      setEditingVisit(null);
      setFormData({
        branchId: branches[0]?.branchId || '', appointmentId: '', patientId: '', doctorId: '',
        visitTime: new Date().toISOString().slice(0, 16), diagnosis: '', notes: '', status: 'OPEN', treatments: []
      });
    }
    setOpenDialog(true);
  };

  const addTreatment = () => {
    if (!treatmentForm.treatmentId) return;
    const treatment = treatments.find(t => t.treatmentId == treatmentForm.treatmentId);
    setFormData({
      ...formData,
      treatments: [...formData.treatments, {
        ...treatmentForm,
        treatmentName: treatment?.name || '',
        unitPrice: treatmentForm.unitPrice || treatment?.defaultPrice || 0
      }]
    });
    setTreatmentForm({ treatmentId: '', toothNumber: '', quantity: 1, unitPrice: '', discountAmount: 0, notes: '' });
  };

  const removeTreatment = (index) => {
    setFormData({
      ...formData,
      treatments: formData.treatments.filter((_, i) => i !== index)
    });
  };

  const handleSave = async () => {
    try {
      const response = editingVisit
        ? await visitApi.updateVisit(editingVisit.visitId, formData)
        : await visitApi.createVisit(formData);
      
      if (response.success) {
        showSnackbar(`Visit ${editingVisit ? 'updated' : 'created'} successfully`);
        fetchData();
        setOpenDialog(false);
      }
    } catch (error) {
      showSnackbar('Error saving visit', 'error');
    }
  };

  const handleCloseVisit = async (id) => {
    try {
      const response = await visitApi.closeVisit(id);
      if (response.success) {
        showSnackbar('Visit closed successfully');
        fetchData();
      }
    } catch (error) {
      showSnackbar('Error closing visit', 'error');
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
        <Typography variant="h4" sx={{ fontWeight: 600, color: '#228B22' }}>Visit Management</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => handleOpenDialog()} sx={{ bgcolor: '#228B22' }}>Add Visit</Button>
      </Box>

      <Grid container spacing={3}>
        {visits.map((visit) => (
          <Grid item xs={12} sm={6} md={4} key={visit.visitId}>
            <Card sx={{ height: '100%', '&:hover': { boxShadow: 4 } }}>
              <CardContent>
                <Box display="flex" justifyContent="space-between" alignItems="start" mb={2}>
                  <Typography variant="h6" sx={{ fontWeight: 600 }}>{visit.patientName}</Typography>
                  <Chip label={visit.status} color={visit.status === 'CLOSED' ? 'success' : 'warning'} size="small" />
                </Box>
                <Typography variant="body2" color="text.secondary" mb={1}>Doctor: {visit.doctorName}</Typography>
                <Typography variant="body2" color="text.secondary" mb={1}>Time: {new Date(visit.visitTime).toLocaleString()}</Typography>
                <Typography variant="body2" color="text.secondary" mb={2}>Treatments: {visit.treatments?.length || 0}</Typography>
                <Box display="flex" gap={1} mt={2} flexWrap="wrap">
                  <IconButton size="small" onClick={() => handleOpenDialog(visit)} color="primary"><EditIcon /></IconButton>
                  {visit.status === 'OPEN' && (
                    <IconButton size="small" onClick={() => handleCloseVisit(visit.visitId)} color="success"><CheckCircleIcon /></IconButton>
                  )}
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="lg" fullWidth>
        <DialogTitle>{editingVisit ? 'Edit Visit' : 'Add New Visit'}</DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={12} sm={6}><FormControl fullWidth><InputLabel>Branch *</InputLabel><Select value={formData.branchId} onChange={(e) => setFormData({ ...formData, branchId: e.target.value })} label="Branch *">{branches.map(b => <MenuItem key={b.branchId} value={b.branchId}>{b.name}</MenuItem>)}</Select></FormControl></Grid>
            <Grid item xs={12} sm={6}><FormControl fullWidth><InputLabel>Patient *</InputLabel><Select value={formData.patientId} onChange={(e) => setFormData({ ...formData, patientId: e.target.value })} label="Patient *">{patients.map(p => <MenuItem key={p.patientId} value={p.patientId}>{p.fullName}</MenuItem>)}</Select></FormControl></Grid>
            <Grid item xs={12} sm={6}><FormControl fullWidth><InputLabel>Doctor *</InputLabel><Select value={formData.doctorId} onChange={(e) => setFormData({ ...formData, doctorId: e.target.value })} label="Doctor *">{doctors.map(d => <MenuItem key={d.doctorId} value={d.doctorId}>{d.userName}</MenuItem>)}</Select></FormControl></Grid>
            <Grid item xs={12} sm={6}><TextField fullWidth type="datetime-local" label="Visit Time *" value={formData.visitTime} onChange={(e) => setFormData({ ...formData, visitTime: e.target.value })} InputLabelProps={{ shrink: true }} /></Grid>
            <Grid item xs={12}><TextField fullWidth label="Diagnosis" multiline rows={3} value={formData.diagnosis} onChange={(e) => setFormData({ ...formData, diagnosis: e.target.value })} /></Grid>
            <Grid item xs={12}><TextField fullWidth label="Notes" multiline rows={2} value={formData.notes} onChange={(e) => setFormData({ ...formData, notes: e.target.value })} /></Grid>
            
            <Grid item xs={12}><Typography variant="h6" sx={{ mt: 2, mb: 1 }}>Treatments</Typography></Grid>
            <Grid item xs={12} sm={4}><FormControl fullWidth><InputLabel>Treatment</InputLabel><Select value={treatmentForm.treatmentId} onChange={(e) => {
              const treatment = treatments.find(t => t.treatmentId == e.target.value);
              setTreatmentForm({ ...treatmentForm, treatmentId: e.target.value, unitPrice: treatment?.defaultPrice || '' });
            }} label="Treatment">{treatments.map(t => <MenuItem key={t.treatmentId} value={t.treatmentId}>{t.name}</MenuItem>)}</Select></FormControl></Grid>
            <Grid item xs={12} sm={2}><TextField fullWidth label="Tooth" value={treatmentForm.toothNumber} onChange={(e) => setTreatmentForm({ ...treatmentForm, toothNumber: e.target.value })} /></Grid>
            <Grid item xs={12} sm={2}><TextField fullWidth label="Qty" type="number" value={treatmentForm.quantity} onChange={(e) => setTreatmentForm({ ...treatmentForm, quantity: parseInt(e.target.value) || 1 })} /></Grid>
            <Grid item xs={12} sm={2}><TextField fullWidth label="Price" type="number" value={treatmentForm.unitPrice} onChange={(e) => setTreatmentForm({ ...treatmentForm, unitPrice: e.target.value })} /></Grid>
            <Grid item xs={12} sm={2}><Button fullWidth variant="outlined" onClick={addTreatment} sx={{ height: '56px' }}>Add</Button></Grid>
            
            {formData.treatments.length > 0 && (
              <Grid item xs={12}>
                <Table size="small">
                  <TableHead><TableRow><TableCell>Treatment</TableCell><TableCell>Tooth</TableCell><TableCell>Qty</TableCell><TableCell>Price</TableCell><TableCell>Discount</TableCell><TableCell>Action</TableCell></TableRow></TableHead>
                  <TableBody>
                    {formData.treatments.map((t, idx) => (
                      <TableRow key={idx}>
                        <TableCell>{t.treatmentName}</TableCell>
                        <TableCell>{t.toothNumber || '-'}</TableCell>
                        <TableCell>{t.quantity}</TableCell>
                        <TableCell>${t.unitPrice}</TableCell>
                        <TableCell>${t.discountAmount || 0}</TableCell>
                        <TableCell><IconButton size="small" onClick={() => removeTreatment(idx)} color="error"><DeleteIcon /></IconButton></TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </Grid>
            )}
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenDialog(false)}>Cancel</Button>
          <Button onClick={handleSave} variant="contained" sx={{ bgcolor: '#228B22' }}>Save</Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={snackbar.open} autoHideDuration={6000} onClose={() => setSnackbar({ ...snackbar, open: false })}>
        <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  );
};

export default VisitManagement;

