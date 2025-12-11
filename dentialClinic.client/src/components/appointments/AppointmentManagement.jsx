import React, { useState, useEffect, useMemo } from 'react';
import {
  Box,
  Typography,
  Button,
  Grid,
  TextField,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Alert,
  CircularProgress,
  IconButton,
  Snackbar,
  InputAdornment,
  MenuItem,
  Select,
  FormControl,
  InputLabel,
  Chip,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Search as SearchIcon,
  Cancel as CancelIcon,
  CheckCircle as CheckCircleIcon
} from '@mui/icons-material';
import { appointmentApi } from '../../utils/appointmentApi';
import { branchApi } from '../../utils/branchApi';
import { patientApi } from '../../utils/patientApi';
import { doctorApi } from '../../utils/doctorApi';

const AppointmentManagement = () => {
  const [appointments, setAppointments] = useState([]);
  const [branches, setBranches] = useState([]);
  const [patients, setPatients] = useState([]);
  const [doctors, setDoctors] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [openDialog, setOpenDialog] = useState(false);
  const [editingAppointment, setEditingAppointment] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: '',
    severity: 'success'
  });

  // Filters
  const [filters, setFilters] = useState({
    branchId: '',
    doctorId: '',
    status: '',
    dateFrom: '',
    dateTo: ''
  });

  const [formData, setFormData] = useState({
    patientId: '',
    doctorId: '',
    branchId: '',
    appointmentAt: '',
    durationMin: 10,
    status: 'booked',
    source: 'call',
    reason: ''
  });

  const [errors, setErrors] = useState({});

  const showSnackbar = (message, severity = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  useEffect(() => {
    fetchData();
  }, []);

  useEffect(() => {
    fetchAppointments();
  }, [filters]);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [branchesRes, patientsRes, doctorsRes] = await Promise.allSettled([
        branchApi.getAllBranches(),
        patientApi.getAllPatients(),
        doctorApi.viewDoctors()
      ]);

      if (branchesRes.status === 'fulfilled' && branchesRes.value?.success) {
        setBranches(branchesRes.value.result || []);
      }

      if (patientsRes.status === 'fulfilled' && patientsRes.value?.success) {
        setPatients(patientsRes.value.result || []);
      }

      if (doctorsRes.status === 'fulfilled' && doctorsRes.value?.success) {
        setDoctors(doctorsRes.value.result || []);
      }
    } catch (error) {
      console.error('Error loading data:', error);
      showSnackbar('Error loading data', 'error');
    } finally {
      setLoading(false);
    }
  };

  const fetchAppointments = async () => {
    try {
      setLoading(true);
      const filterParams = {};
      if (filters.branchId) filterParams.branchId = parseInt(filters.branchId);
      if (filters.doctorId) filterParams.doctorId = parseInt(filters.doctorId);
      if (filters.status) filterParams.status = filters.status;
      if (filters.dateFrom) filterParams.dateFrom = new Date(filters.dateFrom);
      if (filters.dateTo) filterParams.dateTo = new Date(filters.dateTo);

      const response = await appointmentApi.getAllAppointments(filterParams);
      if (response?.success) {
        setAppointments(response.result || []);
      } else {
        showSnackbar(response?.message || 'Error loading appointments', 'error');
        setAppointments([]);
      }
    } catch (error) {
      console.error('Error fetching appointments:', error);
      showSnackbar('Error fetching appointments', 'error');
      setAppointments([]);
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (appointment = null) => {
    if (appointment) {
      setEditingAppointment(appointment);
      setFormData({
        patientId: String(appointment.patientId),
        doctorId: String(appointment.doctorId),
        branchId: String(appointment.branchId),
        appointmentAt: appointment.appointmentAt ? new Date(appointment.appointmentAt).toISOString().slice(0, 16) : '',
        durationMin: appointment.durationMin || 10,
        status: appointment.status || 'booked',
        source: appointment.source || 'call',
        reason: appointment.reason || ''
      });
    } else {
      setEditingAppointment(null);
      setFormData({
        patientId: '',
        doctorId: '',
        branchId: filters.branchId || '',
        appointmentAt: '',
        durationMin: 10,
        status: 'booked',
        source: 'call',
        reason: ''
      });
    }
    setErrors({});
    setOpenDialog(true);
  };

  // Filter doctors based on selected branch
  const filteredDoctors = useMemo(() => {
    if (!formData.branchId) {
      return [];
    }
    const branchIdNum = parseInt(formData.branchId);
    return doctors.filter(d => d.branchId === branchIdNum);
  }, [doctors, formData.branchId]);

  // Filter patients based on selected branch
  const filteredPatients = useMemo(() => {
    if (!formData.branchId) {
      return [];
    }
    const branchIdNum = parseInt(formData.branchId);
    return patients.filter(p => p.branchId === branchIdNum);
  }, [patients, formData.branchId]);

  const handleBranchChange = (branchId) => {
    setFormData({ 
      ...formData, 
      branchId: branchId,
      doctorId: '', // Clear doctor selection when branch changes
      patientId: '' // Clear patient selection when branch changes
    });
    setErrors({ ...errors, branchId: '', doctorId: '', patientId: '' });
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingAppointment(null);
    setErrors({});
  };

  const validateForm = () => {
    const newErrors = {};
    if (!formData.patientId) newErrors.patientId = 'Patient is required';
    if (!formData.doctorId) newErrors.doctorId = 'Doctor is required';
    if (!formData.branchId) newErrors.branchId = 'Branch is required';
    if (!formData.appointmentAt) newErrors.appointmentAt = 'Appointment date/time is required';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async () => {
    if (!validateForm()) return;

    try {
      setSaving(true);
      const saveData = {
        patientId: parseInt(formData.patientId),
        doctorId: parseInt(formData.doctorId),
        branchId: parseInt(formData.branchId),
        appointmentAt: new Date(formData.appointmentAt).toISOString(),
        durationMin: parseInt(formData.durationMin),
        status: formData.status,
        source: formData.source,
        reason: formData.reason?.trim() || null
      };

      const response = editingAppointment
        ? await appointmentApi.updateAppointment(editingAppointment.appointmentId, saveData)
        : await appointmentApi.createAppointment(saveData);

      if (response?.success) {
        showSnackbar(`Appointment ${editingAppointment ? 'updated' : 'created'} successfully`);
        await fetchAppointments();
        handleCloseDialog();
      } else {
        showSnackbar(response?.message || 'Error saving appointment', 'error');
      }
    } catch (error) {
      console.error('Error saving appointment:', error);
      showSnackbar(error?.message || 'Error saving appointment', 'error');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this appointment?')) return;

    try {
      const response = await appointmentApi.deleteAppointment(id);
      if (response?.success) {
        showSnackbar('Appointment deleted successfully');
        await fetchAppointments();
      } else {
        showSnackbar(response?.message || 'Error deleting appointment', 'error');
      }
    } catch (error) {
      console.error('Error deleting appointment:', error);
      showSnackbar('Error deleting appointment', 'error');
    }
  };

  const handleStatusChange = async (id, newStatus) => {
    try {
      const response = await appointmentApi.updateAppointmentStatus(id, newStatus);
      if (response?.success) {
        showSnackbar('Appointment status updated successfully');
        await fetchAppointments();
      } else {
        showSnackbar(response?.message || 'Error updating status', 'error');
      }
    } catch (error) {
      console.error('Error updating status:', error);
      showSnackbar('Error updating status', 'error');
    }
  };

  const handleCancel = async (id) => {
    if (!window.confirm('Are you sure you want to cancel this appointment?')) return;
    await handleStatusChange(id, 'cancelled');
  };

  const filteredAppointments = useMemo(() => {
    if (!searchTerm.trim()) return appointments;
    const term = searchTerm.toLowerCase();
    return appointments.filter(apt =>
      (apt.patientName && apt.patientName.toLowerCase().includes(term)) ||
      (apt.patientPhone && apt.patientPhone.toLowerCase().includes(term)) ||
      (apt.doctorName && apt.doctorName.toLowerCase().includes(term)) ||
      (apt.branchName && apt.branchName.toLowerCase().includes(term))
    );
  }, [appointments, searchTerm]);

  const formatDateTime = (dateTime) => {
    if (!dateTime) return 'N/A';
    const date = new Date(dateTime);
    return date.toLocaleString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const getStatusColor = (status) => {
    const colors = {
      booked: 'primary',
      pending: 'warning',
      completed: 'success',
      cancelled: 'error',
      no_show: 'default'
    };
    return colors[status?.toLowerCase()] || 'default';
  };

  const getStatusLabel = (status) => {
    const labels = {
      booked: 'Booked',
      pending: 'Pending',
      completed: 'Completed',
      cancelled: 'Cancelled',
      no_show: 'No Show'
    };
    return labels[status?.toLowerCase()] || status;
  };

  const getNextStatus = (currentStatus) => {
    const statusFlow = {
      booked: 'pending',
      pending: 'completed',
      completed: 'completed'
    };
    return statusFlow[currentStatus?.toLowerCase()] || currentStatus;
  };

  if (loading && appointments.length === 0) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress sx={{ color: '#228B22' }} />
      </Box>
    );
  }

  return (
    <Box sx={{ p: 3 }}>
      {/* Header */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h5" sx={{ fontWeight: 700, color: '#228B22' }}>
          Appointment Management
        </Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => handleOpenDialog()}
          sx={{
            bgcolor: '#228B22',
            borderRadius: '12px',
            px: 3,
            py: 1.5,
            '&:hover': { bgcolor: '#1a6b1a' }
          }}
        >
          New Appointment
        </Button>
      </Box>

      {/* Filters */}
      <Paper sx={{ p: 2, mb: 3, borderRadius: '12px' }}>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={6} md={2}>
            <FormControl fullWidth>
              <InputLabel>Branch</InputLabel>
              <Select
                value={filters.branchId}
                onChange={(e) => setFilters({ ...filters, branchId: e.target.value })}
                label="Branch"
              >
                <MenuItem value="">All Branches</MenuItem>
                {branches.map(b => (
                  <MenuItem key={b.branchId} value={String(b.branchId)}>{b.name}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} sm={6} md={2}>
            <FormControl fullWidth>
              <InputLabel>Doctor</InputLabel>
              <Select
                value={filters.doctorId}
                onChange={(e) => setFilters({ ...filters, doctorId: e.target.value })}
                label="Doctor"
              >
                <MenuItem value="">All Doctors</MenuItem>
                {doctors.map(d => (
                  <MenuItem key={d.doctorId} value={String(d.doctorId)}>{d.doctorName}</MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} sm={6} md={2}>
            <FormControl fullWidth>
              <InputLabel>Status</InputLabel>
              <Select
                value={filters.status}
                onChange={(e) => setFilters({ ...filters, status: e.target.value })}
                label="Status"
              >
                <MenuItem value="">All Status</MenuItem>
                <MenuItem value="booked">Booked</MenuItem>
                <MenuItem value="pending">Pending</MenuItem>
                <MenuItem value="completed">Completed</MenuItem>
                <MenuItem value="cancelled">Cancelled</MenuItem>
                <MenuItem value="no_show">No Show</MenuItem>
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} sm={6} md={2}>
            <TextField
              fullWidth
              label="Date From"
              type="date"
              value={filters.dateFrom}
              onChange={(e) => setFilters({ ...filters, dateFrom: e.target.value })}
              InputLabelProps={{ shrink: true }}
            />
          </Grid>
          <Grid item xs={12} sm={6} md={2}>
            <TextField
              fullWidth
              label="Date To"
              type="date"
              value={filters.dateTo}
              onChange={(e) => setFilters({ ...filters, dateTo: e.target.value })}
              InputLabelProps={{ shrink: true }}
            />
          </Grid>
          <Grid item xs={12} sm={6} md={2}>
            <Button
              fullWidth
              variant="outlined"
              onClick={() => setFilters({ branchId: '', doctorId: '', status: '', dateFrom: '', dateTo: '' })}
              sx={{ height: '56px' }}
            >
              Clear Filters
            </Button>
          </Grid>
        </Grid>
      </Paper>

      {/* Search */}
      <TextField
        fullWidth
        placeholder="Search by patient name, phone, doctor, or branch..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        sx={{ mb: 3 }}
        InputProps={{
          startAdornment: (
            <InputAdornment position="start">
              <SearchIcon sx={{ color: '#228B22' }} />
            </InputAdornment>
          )
        }}
      />

      {/* Appointments Table */}
      {filteredAppointments.length === 0 ? (
        <Box sx={{ textAlign: 'center', py: 4 }}>
          <Typography variant="body1" color="text.secondary">
            {appointments.length === 0
              ? 'No appointments found. Click "New Appointment" to create one.'
              : 'No appointments match your search criteria.'}
          </Typography>
        </Box>
      ) : (
        <TableContainer component={Paper} sx={{ borderRadius: '12px', boxShadow: '0 2px 8px rgba(0,0,0,0.1)' }}>
          <Table>
            <TableHead>
              <TableRow sx={{ bgcolor: '#f5f5f5' }}>
                <TableCell sx={{ fontWeight: 700 }}>Date/Time</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Patient Name</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Patient Phone</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Doctor</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Branch</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Status</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Source</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Reason</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {filteredAppointments.map((apt) => (
                <TableRow key={apt.appointmentId} hover>
                  <TableCell>{formatDateTime(apt.appointmentAt)}</TableCell>
                  <TableCell>{apt.patientName || 'N/A'}</TableCell>
                  <TableCell>{apt.patientPhone || 'N/A'}</TableCell>
                  <TableCell>{apt.doctorName || 'N/A'}</TableCell>
                  <TableCell>{apt.branchName || 'N/A'}</TableCell>
                  <TableCell>
                    <Chip
                      label={getStatusLabel(apt.status)}
                      size="small"
                      color={getStatusColor(apt.status)}
                    />
                  </TableCell>
                  <TableCell>{apt.source || 'N/A'}</TableCell>
                  <TableCell>{apt.reason || 'N/A'}</TableCell>
                  <TableCell align="center">
                    <Box display="flex" gap={1} justifyContent="center" flexWrap="wrap">
                      <IconButton
                        size="small"
                        onClick={() => handleOpenDialog(apt)}
                        sx={{
                          color: '#228B22',
                          bgcolor: 'rgba(34, 139, 34, 0.1)',
                          '&:hover': { bgcolor: 'rgba(34, 139, 34, 0.2)' }
                        }}
                        title="Edit"
                      >
                        <EditIcon fontSize="small" />
                      </IconButton>
                      {apt.status !== 'cancelled' && apt.status !== 'completed' && (
                        <>
                          <IconButton
                            size="small"
                            onClick={() => handleStatusChange(apt.appointmentId, getNextStatus(apt.status))}
                            sx={{
                              color: '#3b82f6',
                              bgcolor: 'rgba(59, 130, 246, 0.1)',
                              '&:hover': { bgcolor: 'rgba(59, 130, 246, 0.2)' }
                            }}
                            title="Change Status"
                          >
                            <CheckCircleIcon fontSize="small" />
                          </IconButton>
                          <IconButton
                            size="small"
                            onClick={() => handleCancel(apt.appointmentId)}
                            sx={{
                              color: '#ff9800',
                              bgcolor: 'rgba(255, 152, 0, 0.1)',
                              '&:hover': { bgcolor: 'rgba(255, 152, 0, 0.2)' }
                            }}
                            title="Cancel"
                          >
                            <CancelIcon fontSize="small" />
                          </IconButton>
                        </>
                      )}
                      <IconButton
                        size="small"
                        onClick={() => handleDelete(apt.appointmentId)}
                        sx={{
                          color: '#ef4444',
                          bgcolor: 'rgba(239, 68, 68, 0.1)',
                          '&:hover': { bgcolor: 'rgba(239, 68, 68, 0.2)' }
                        }}
                        title="Delete"
                      >
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </Box>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}

      {/* Add/Edit Dialog */}
      <Dialog
        open={openDialog}
        onClose={handleCloseDialog}
        maxWidth="md"
        fullWidth
        PaperProps={{
          sx: {
            borderRadius: '16px',
            boxShadow: '0 4px 20px rgba(0,0,0,0.15)'
          }
        }}
      >
        <DialogTitle
          sx={{
            background: 'linear-gradient(135deg, #228B22 0%, #32CD32 100%)',
            color: 'white',
            fontWeight: 700,
            py: 2.5
          }}
        >
          {editingAppointment ? 'Edit Appointment' : 'New Appointment'}
        </DialogTitle>
        <DialogContent sx={{ p: 3 }}>
          {Object.keys(errors).length > 0 && (
            <Alert severity="error" sx={{ mb: 2, borderRadius: '12px' }}>
              Please fill in all required fields.
            </Alert>
          )}

          <Grid container spacing={2.5} sx={{ mt: 1 }}>
            <Grid item xs={12} sm={6}>
              <FormControl fullWidth error={!!errors.branchId}>
                <InputLabel>Branch *</InputLabel>
                <Select
                  value={formData.branchId}
                  onChange={(e) => handleBranchChange(e.target.value)}
                  label="Branch *"
                >
                  <MenuItem value="">Select Branch</MenuItem>
                  {branches.map(b => (
                    <MenuItem key={b.branchId} value={String(b.branchId)}>{b.name}</MenuItem>
                  ))}
                </Select>
                {errors.branchId && (
                  <Typography variant="caption" color="error" sx={{ mt: 0.5, ml: 1.75 }}>
                    {errors.branchId}
                  </Typography>
                )}
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth error={!!errors.doctorId} disabled={!formData.branchId}>
                <InputLabel>Doctor *</InputLabel>
                <Select
                  value={formData.doctorId}
                  onChange={(e) => {
                    setFormData({ ...formData, doctorId: e.target.value });
                    setErrors({ ...errors, doctorId: '' });
                  }}
                  label="Doctor *"
                >
                  <MenuItem value="">
                    {formData.branchId ? 'Select Doctor' : 'Select Branch First'}
                  </MenuItem>
                  {filteredDoctors.map(d => (
                    <MenuItem key={d.doctorId} value={String(d.doctorId)}>
                      {d.doctorName}
                    </MenuItem>
                  ))}
                </Select>
                {errors.doctorId && (
                  <Typography variant="caption" color="error" sx={{ mt: 0.5, ml: 1.75 }}>
                    {errors.doctorId}
                  </Typography>
                )}
                {formData.branchId && filteredDoctors.length === 0 && (
                  <Typography variant="caption" color="text.secondary" sx={{ mt: 0.5, ml: 1.75 }}>
                    No doctors available for this branch
                  </Typography>
                )}
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth error={!!errors.patientId} disabled={!formData.branchId}>
                <InputLabel>Patient *</InputLabel>
                <Select
                  value={formData.patientId}
                  onChange={(e) => {
                    setFormData({ ...formData, patientId: e.target.value });
                    setErrors({ ...errors, patientId: '' });
                  }}
                  label="Patient *"
                >
                  <MenuItem value="">
                    {formData.branchId ? 'Select Patient' : 'Select Branch First'}
                  </MenuItem>
                  {filteredPatients.map(p => (
                    <MenuItem key={p.patientId} value={String(p.patientId)}>
                      {p.fullName} {p.phone ? `(${p.phone})` : ''}
                    </MenuItem>
                  ))}
                </Select>
                {errors.patientId && (
                  <Typography variant="caption" color="error" sx={{ mt: 0.5, ml: 1.75 }}>
                    {errors.patientId}
                  </Typography>
                )}
                {formData.branchId && filteredPatients.length === 0 && (
                  <Typography variant="caption" color="text.secondary" sx={{ mt: 0.5, ml: 1.75 }}>
                    No patients available for this branch
                  </Typography>
                )}
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Appointment Date & Time *"
                type="datetime-local"
                value={formData.appointmentAt}
                onChange={(e) => {
                  setFormData({ ...formData, appointmentAt: e.target.value });
                  setErrors({ ...errors, appointmentAt: '' });
                }}
                error={!!errors.appointmentAt}
                helperText={errors.appointmentAt}
                InputLabelProps={{ shrink: true }}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Duration (minutes)"
                type="number"
                value={formData.durationMin}
                onChange={(e) => setFormData({ ...formData, durationMin: parseInt(e.target.value) || 10 })}
                inputProps={{ min: 1 }}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel>Status</InputLabel>
                <Select
                  value={formData.status}
                  onChange={(e) => setFormData({ ...formData, status: e.target.value })}
                  label="Status"
                >
                  <MenuItem value="booked">Booked</MenuItem>
                  <MenuItem value="pending">Pending</MenuItem>
                  <MenuItem value="completed">Completed</MenuItem>
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel>Source</InputLabel>
                <Select
                  value={formData.source}
                  onChange={(e) => setFormData({ ...formData, source: e.target.value })}
                  label="Source"
                >
                  <MenuItem value="call">Call</MenuItem>
                  <MenuItem value="walkin">Walk-in</MenuItem>
                  <MenuItem value="whatsapp">WhatsApp</MenuItem>
                  <MenuItem value="online">Online</MenuItem>
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Reason / Complaint"
                multiline
                rows={3}
                value={formData.reason}
                onChange={(e) => setFormData({ ...formData, reason: e.target.value })}
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions sx={{ p: 3, pt: 2 }}>
          <Button
            onClick={handleCloseDialog}
            sx={{
              borderColor: '#228B22',
              color: '#228B22',
              borderRadius: '12px',
              px: 3,
              py: 1,
              '&:hover': {
                borderColor: '#1B5E20',
                bgcolor: 'rgba(34, 139, 34, 0.04)'
              }
            }}
          >
            Cancel
          </Button>
          <Button
            onClick={handleSave}
            variant="contained"
            disabled={saving}
            sx={{
              bgcolor: '#228B22',
              borderRadius: '12px',
              px: 3,
              py: 1,
              '&:hover': {
                bgcolor: '#1B5E20'
              },
              '&:disabled': {
                bgcolor: '#9CA3AF'
              }
            }}
          >
            {saving ? 'Saving...' : 'Save'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Snackbar */}
      <Snackbar
        open={snackbar.open}
        autoHideDuration={6000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
      >
        <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  );
};

export default AppointmentManagement;
