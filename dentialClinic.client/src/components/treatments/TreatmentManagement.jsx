import React, { useState, useEffect } from 'react';
import {
  Box, Typography, Button, Grid, TextField, Dialog, DialogTitle, DialogContent,
  DialogActions, Alert, CircularProgress, IconButton, Snackbar, Card, CardContent, Chip
} from '@mui/material';
import { Add as AddIcon, Edit as EditIcon, Delete as DeleteIcon } from '@mui/icons-material';
import { treatmentApi } from '../../utils/treatmentApi';

const TreatmentManagement = () => {
  const [treatments, setTreatments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [openDialog, setOpenDialog] = useState(false);
  const [editingTreatment, setEditingTreatment] = useState(null);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
  const [formData, setFormData] = useState({
    name: '', description: '', defaultPrice: '', defaultDurationMinutes: '', isActive: true
  });

  useEffect(() => {
    fetchTreatments();
  }, []);

  const fetchTreatments = async () => {
    try {
      setLoading(true);
      const response = await treatmentApi.getAllTreatments();
      if (response.success) setTreatments(response.result || []);
    } catch (error) {
      showSnackbar('Error loading treatments', 'error');
    } finally {
      setLoading(false);
    }
  };

  const showSnackbar = (message, severity = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  const handleOpenDialog = (treatment = null) => {
    if (treatment) {
      setEditingTreatment(treatment);
      setFormData({
        name: treatment.name || '',
        description: treatment.description || '',
        defaultPrice: treatment.defaultPrice || '',
        defaultDurationMinutes: treatment.defaultDurationMinutes || '',
        isActive: treatment.isActive ?? true
      });
    } else {
      setEditingTreatment(null);
      setFormData({ name: '', description: '', defaultPrice: '', defaultDurationMinutes: '', isActive: true });
    }
    setOpenDialog(true);
  };

  const handleSave = async () => {
    try {
      const response = editingTreatment
        ? await treatmentApi.updateTreatment(editingTreatment.treatmentId, formData)
        : await treatmentApi.createTreatment(formData);
      
      if (response.success) {
        showSnackbar(`Treatment ${editingTreatment ? 'updated' : 'created'} successfully`);
        fetchTreatments();
        setOpenDialog(false);
      }
    } catch (error) {
      showSnackbar('Error saving treatment', 'error');
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this treatment?')) return;
    try {
      const response = await treatmentApi.deleteTreatment(id);
      if (response.success) {
        showSnackbar('Treatment deleted successfully');
        fetchTreatments();
      }
    } catch (error) {
      showSnackbar('Error deleting treatment', 'error');
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
        <Typography variant="h4" sx={{ fontWeight: 600, color: '#228B22' }}>Treatment Management</Typography>
        <Button variant="contained" startIcon={<AddIcon />} onClick={() => handleOpenDialog()} sx={{ bgcolor: '#228B22' }}>Add Treatment</Button>
      </Box>

      <Grid container spacing={3}>
        {treatments.map((treatment) => (
          <Grid item xs={12} sm={6} md={4} key={treatment.treatmentId}>
            <Card sx={{ height: '100%', '&:hover': { boxShadow: 4 } }}>
              <CardContent>
                <Box display="flex" justifyContent="space-between" alignItems="start" mb={2}>
                  <Typography variant="h6" sx={{ fontWeight: 600 }}>{treatment.name}</Typography>
                  <Chip label={treatment.isActive ? 'Active' : 'Inactive'} color={treatment.isActive ? 'success' : 'default'} size="small" />
                </Box>
                {treatment.defaultPrice && <Typography variant="body2" color="text.secondary" mb={1}>Price: ${treatment.defaultPrice}</Typography>}
                {treatment.defaultDurationMinutes && <Typography variant="body2" color="text.secondary" mb={1}>Duration: {treatment.defaultDurationMinutes} min</Typography>}
                {treatment.description && <Typography variant="body2" color="text.secondary" mb={2}>{treatment.description}</Typography>}
                <Box display="flex" gap={1} mt={2}>
                  <IconButton size="small" onClick={() => handleOpenDialog(treatment)} color="primary"><EditIcon /></IconButton>
                  <IconButton size="small" onClick={() => handleDelete(treatment.treatmentId)} color="error"><DeleteIcon /></IconButton>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      <Dialog open={openDialog} onClose={() => setOpenDialog(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{editingTreatment ? 'Edit Treatment' : 'Add New Treatment'}</DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={12}><TextField fullWidth label="Name *" value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} /></Grid>
            <Grid item xs={12}><TextField fullWidth label="Description" multiline rows={3} value={formData.description} onChange={(e) => setFormData({ ...formData, description: e.target.value })} /></Grid>
            <Grid item xs={12} sm={6}><TextField fullWidth label="Default Price" type="number" value={formData.defaultPrice} onChange={(e) => setFormData({ ...formData, defaultPrice: e.target.value })} /></Grid>
            <Grid item xs={12} sm={6}><TextField fullWidth label="Duration (minutes)" type="number" value={formData.defaultDurationMinutes} onChange={(e) => setFormData({ ...formData, defaultDurationMinutes: e.target.value })} /></Grid>
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

export default TreatmentManagement;

