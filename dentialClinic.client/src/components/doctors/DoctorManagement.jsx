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
  Card,
  CardContent,
  InputAdornment,
  MenuItem,
  Select,
  FormControl,
  InputLabel,
  Chip,
  Switch,
  FormControlLabel
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Search as SearchIcon
} from '@mui/icons-material';
import { doctorApi } from '../../utils/doctorApi';
import { branchApi } from '../../utils/branchApi';
import { userManagementApi } from '../../utils/api';

const DoctorManagement = () => {
  const [doctors, setDoctors] = useState([]);
  const [branches, setBranches] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [openDialog, setOpenDialog] = useState(false);
  const [editingDoctor, setEditingDoctor] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: '',
    severity: 'success'
  });
  const [formData, setFormData] = useState({
    userId: '',
    branchId: '',
    speciality: '',
    defaultFee: '',
    isActive: true
  });
  const [errors, setErrors] = useState({});

  // Define showSnackbar before it's used
  const showSnackbar = (message, severity = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      
      // Use Promise.allSettled so one failure doesn't break the others
      const [doctorsResult, branchesResult, usersResult] = await Promise.allSettled([
        doctorApi.viewDoctors(),
        branchApi.getAllBranches(),
        userManagementApi.getUsers()
      ]);

      // Handle doctors response
      if (doctorsResult.status === 'fulfilled' && doctorsResult.value?.success) {
        const doctorsData = Array.isArray(doctorsResult.value.result) 
          ? doctorsResult.value.result 
          : [];
        setDoctors(doctorsData);
        console.log('Doctors loaded:', doctorsData.length);
      } else {
        setDoctors([]);
        if (doctorsResult.status === 'rejected') {
          console.error('Error loading doctors:', doctorsResult.reason);
          showSnackbar('Error loading doctors', 'error');
        } else if (doctorsResult.value && !doctorsResult.value.success) {
          showSnackbar(doctorsResult.value.message || 'Error loading doctors', 'error');
        }
      }

      // Handle branches response
      if (branchesResult.status === 'fulfilled' && branchesResult.value?.success) {
        const branchesData = Array.isArray(branchesResult.value.result) 
          ? branchesResult.value.result 
          : [];
        setBranches(branchesData);
      } else {
        setBranches([]);
        if (branchesResult.status === 'rejected') {
          console.warn('Error loading branches:', branchesResult.reason);
        }
      }

      // Handle users response
      if (usersResult.status === 'fulfilled' && usersResult.value?.success) {
        const usersData = Array.isArray(usersResult.value.result) 
          ? usersResult.value.result 
          : [];
        setUsers(usersData);
      } else {
        setUsers([]);
        if (usersResult.status === 'rejected') {
          console.warn('Error loading users:', usersResult.reason);
        } else if (usersResult.value && !usersResult.value.success) {
          console.warn('Users API returned error:', usersResult.value.message);
        }
      }
    } catch (error) {
      console.error('Unexpected error fetching data:', error);
      showSnackbar('Error loading data', 'error');
      setDoctors([]);
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (doctor = null) => {
    if (doctor) {
      setEditingDoctor(doctor);
      setFormData({
        userId: doctor.userId ? String(doctor.userId) : '',
        branchId: doctor.branchId ? String(doctor.branchId) : '',
        speciality: doctor.speciality || '',
        defaultFee: doctor.defaultFee ? String(doctor.defaultFee) : '',
        isActive: doctor.doctorIsActive ?? true
      });
    } else {
      setEditingDoctor(null);
      setFormData({
        userId: '',
        branchId: '',
        speciality: '',
        defaultFee: '',
        isActive: true
      });
    }
    setErrors({});
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingDoctor(null);
    setErrors({});
  };

  const validateForm = () => {
    const newErrors = {};
    if (!formData.userId) {
      newErrors.userId = 'User is required';
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async () => {
    if (!validateForm()) {
      return;
    }

    try {
      setSaving(true);
      const saveData = {
        userId: parseInt(formData.userId, 10),
        branchId: formData.branchId ? parseInt(formData.branchId, 10) : null,
        speciality: formData.speciality?.trim() || null,
        defaultFee: formData.defaultFee ? parseFloat(formData.defaultFee) : null,
        isActive: formData.isActive
      };

      const response = editingDoctor
        ? await doctorApi.updateDoctor(editingDoctor.doctorId, saveData)
        : await doctorApi.createDoctor(saveData);

      if (response?.success) {
        showSnackbar(`Doctor ${editingDoctor ? 'updated' : 'created'} successfully`);
        await fetchData();
        handleCloseDialog();
      } else {
        showSnackbar(response?.message || 'Error saving doctor', 'error');
      }
    } catch (error) {
      console.error('Error saving doctor:', error);
      showSnackbar(error?.message || 'Error saving doctor', 'error');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this doctor?')) {
      return;
    }

    try {
      const response = await doctorApi.deleteDoctor(id);
      if (response?.success) {
        showSnackbar('Doctor deleted successfully');
        await fetchData();
      } else {
        showSnackbar(response?.message || 'Error deleting doctor', 'error');
      }
    } catch (error) {
      console.error('Error deleting doctor:', error);
      showSnackbar('Error deleting doctor', 'error');
    }
  };

  const filteredDoctors = useMemo(() => {
    if (!doctors || doctors.length === 0) return [];
    if (!searchTerm.trim()) return doctors;

    const term = searchTerm.toLowerCase();
    return doctors.filter(
      (doctor) =>
        (doctor.doctorName && doctor.doctorName.toLowerCase().includes(term)) ||
        (doctor.email && doctor.email.toLowerCase().includes(term)) ||
        (doctor.phone && doctor.phone.toLowerCase().includes(term)) ||
        (doctor.speciality && doctor.speciality.toLowerCase().includes(term)) ||
        (doctor.branchName && doctor.branchName.toLowerCase().includes(term))
    );
  }, [doctors, searchTerm]);

  if (loading) {
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
          Doctor Management
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
          Add Doctor
        </Button>
      </Box>

      {/* Search Bar */}
      <TextField
        fullWidth
        placeholder="Search doctors by name, email, phone, speciality, or branch..."
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

      {/* Doctors Grid */}
      {filteredDoctors.length === 0 ? (
        <Box sx={{ textAlign: 'center', py: 4 }}>
          <Typography variant="body1" color="text.secondary">
            {doctors.length === 0
              ? 'No doctors found. Click "Add Doctor" to create one.'
              : 'No doctors match your search criteria.'}
          </Typography>
        </Box>
      ) : (
        <Grid container spacing={3}>
          {filteredDoctors.map((doctor) => (
            <Grid item xs={12} sm={6} md={4} lg={3} key={doctor.doctorId || `doctor-${Math.random()}`}>
              <Card
                sx={{
                  height: '100%',
                  display: 'flex',
                  flexDirection: 'column',
                  borderRadius: '12px',
                  boxShadow: '0 2px 8px rgba(0,0,0,0.1)',
                  '&:hover': {
                    boxShadow: '0 4px 16px rgba(34, 139, 34, 0.2)',
                    transform: 'translateY(-2px)',
                    transition: 'all 0.3s ease'
                  }
                }}
              >
                <CardContent sx={{ flexGrow: 1 }}>
                  <Box display="flex" justifyContent="space-between" alignItems="start" mb={1.5}>
                    <Typography variant="h6" sx={{ fontWeight: 600, color: '#1e293b' }}>
                      {doctor.doctorName || 'Unnamed Doctor'}
                    </Typography>
                    <Chip
                      label={doctor.doctorIsActive ? 'Active' : 'Inactive'}
                      size="small"
                      sx={{
                        bgcolor: doctor.doctorIsActive 
                          ? 'rgba(34, 197, 94, 0.1)' 
                          : 'rgba(239, 68, 68, 0.1)',
                        color: doctor.doctorIsActive ? '#22c55e' : '#ef4444',
                        fontWeight: 600
                      }}
                    />
                  </Box>

                  {doctor.speciality && (
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                      <strong>Speciality:</strong> {doctor.speciality}
                    </Typography>
                  )}

                  {doctor.defaultFee && (
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                      <strong>Fee:</strong> ${doctor.defaultFee}
                    </Typography>
                  )}

                  {doctor.branchName && (
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                      <strong>Branch:</strong> {doctor.branchName}
                    </Typography>
                  )}

                  {doctor.email && (
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                      <strong>Email:</strong> {doctor.email}
                    </Typography>
                  )}

                  {doctor.phone && (
                    <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                      <strong>Phone:</strong> {doctor.phone}
                    </Typography>
                  )}

                  <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                    <strong>Total Patients:</strong> {doctor.totalPatients || 0}
                  </Typography>

                  <Box display="flex" gap={1} mt={2} justifyContent="flex-end">
                    <IconButton
                      size="small"
                      onClick={() => handleOpenDialog(doctor)}
                      sx={{
                        color: '#228B22',
                        bgcolor: 'rgba(34, 139, 34, 0.1)',
                        '&:hover': { bgcolor: 'rgba(34, 139, 34, 0.2)' }
                      }}
                    >
                      <EditIcon fontSize="small" />
                    </IconButton>
                    <IconButton
                      size="small"
                      onClick={() => handleDelete(doctor.doctorId)}
                      sx={{
                        color: '#ef4444',
                        bgcolor: 'rgba(239, 68, 68, 0.1)',
                        '&:hover': { bgcolor: 'rgba(239, 68, 68, 0.2)' }
                      }}
                    >
                      <DeleteIcon fontSize="small" />
                    </IconButton>
                  </Box>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
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
          {editingDoctor ? 'Edit Doctor' : 'Add New Doctor'}
        </DialogTitle>

        <DialogContent sx={{ p: 3 }}>
          {errors.userId && (
            <Alert severity="error" sx={{ mb: 2, borderRadius: '12px' }}>
              {errors.userId}
            </Alert>
          )}

          <Grid container spacing={2.5} sx={{ mt: 1 }}>
            <Grid item xs={12}>
              <FormControl fullWidth error={!!errors.userId}>
                <InputLabel>User *</InputLabel>
                <Select
                  value={formData.userId || ''}
                  onChange={(e) => {
                    setFormData({ ...formData, userId: e.target.value });
                    setErrors({ ...errors, userId: '' });
                  }}
                  label="User *"
                  disabled={!!editingDoctor}
                  sx={{
                    borderRadius: '12px',
                    '&:hover .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22'
                    },
                    '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22',
                      borderWidth: '2px'
                    }
                  }}
                >
                  <MenuItem value="">None</MenuItem>
                  {users.map((user) => (
                    <MenuItem key={user.userId} value={String(user.userId)}>
                      {user.fullName || `User ${user.userId}`}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel>Branch</InputLabel>
                <Select
                  value={formData.branchId || ''}
                  onChange={(e) => setFormData({ ...formData, branchId: e.target.value })}
                  label="Branch"
                  sx={{
                    borderRadius: '12px',
                    '&:hover .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22'
                    },
                    '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22',
                      borderWidth: '2px'
                    }
                  }}
                >
                  <MenuItem value="">None</MenuItem>
                  {branches.map((branch) => (
                    <MenuItem key={branch.branchId} value={String(branch.branchId)}>
                      {branch.name}
                    </MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Speciality"
                value={formData.speciality}
                onChange={(e) => setFormData({ ...formData, speciality: e.target.value })}
                sx={{
                  '& .MuiOutlinedInput-root': {
                    borderRadius: '12px',
                    '&:hover .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22'
                    },
                    '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22',
                      borderWidth: '2px'
                    }
                  }
                }}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Default Fee"
                type="number"
                value={formData.defaultFee}
                onChange={(e) => setFormData({ ...formData, defaultFee: e.target.value })}
                InputProps={{
                  startAdornment: <InputAdornment position="start">$</InputAdornment>
                }}
                sx={{
                  '& .MuiOutlinedInput-root': {
                    borderRadius: '12px',
                    '&:hover .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22'
                    },
                    '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22',
                      borderWidth: '2px'
                    }
                  }
                }}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <FormControlLabel
                control={
                  <Switch
                    checked={formData.isActive}
                    onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                    sx={{
                      '& .MuiSwitch-switchBase.Mui-checked': {
                        color: '#228B22'
                      },
                      '& .MuiSwitch-switchBase.Mui-checked + .MuiSwitch-track': {
                        backgroundColor: '#228B22'
                      }
                    }}
                  />
                }
                label="Is Active"
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

export default DoctorManagement;
