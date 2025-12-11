import React, { useState, useEffect } from 'react';
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
  Paper,
  Checkbox,
  FormControlLabel
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Search as SearchIcon
} from '@mui/icons-material';
import { patientApi } from '../../utils/patientApi';
import { branchApi } from '../../utils/branchApi';

const PatientManagement = () => {
  const [patients, setPatients] = useState([]);
  const [branches, setBranches] = useState([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [openPatientDialog, setOpenPatientDialog] = useState(false);
  const [editingPatient, setEditingPatient] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: '',
    severity: 'success'
  });
  const [formData, setFormData] = useState({
    fullName: '',
    phone: '',
    email: '',
    gender: '',
    dateOfBirth: '',
    address: '',
    branchId: '',
    isActive: true
  });
  const [errors, setErrors] = useState({});

  const showSnackbar = (message, severity = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      
      const [patientsResult, branchesResult] = await Promise.allSettled([
        patientApi.getAllPatients(),
        branchApi.getAllBranches()
      ]);

      if (patientsResult.status === 'fulfilled' && patientsResult.value?.success) {
        const patientsData = Array.isArray(patientsResult.value.result) 
          ? patientsResult.value.result 
          : [];
        setPatients(patientsData);
      } else {
        setPatients([]);
        if (patientsResult.status === 'rejected') {
          console.error('Error loading patients:', patientsResult.reason);
          showSnackbar('Error loading patients', 'error');
        }
      }

      if (branchesResult.status === 'fulfilled' && branchesResult.value?.success) {
        const branchesData = Array.isArray(branchesResult.value.result) 
          ? branchesResult.value.result 
          : [];
        setBranches(branchesData);
      } else {
        setBranches([]);
      }
    } catch (error) {
      console.error('Unexpected error fetching data:', error);
      showSnackbar('Error loading data', 'error');
      setPatients([]);
    } finally {
      setLoading(false);
    }
  };

  const handleOpenPatientDialog = (patient = null) => {
    if (patient) {
      setEditingPatient(patient);
      setFormData({
        fullName: patient.fullName || '',
        phone: patient.phone || '',
        email: patient.email || '',
        gender: patient.gender || '',
        dateOfBirth: patient.dateOfBirth ? patient.dateOfBirth.split('T')[0] : '',
        address: patient.address || '',
        branchId: patient.branchId ? String(patient.branchId) : '',
        isActive: patient.isActive !== undefined ? patient.isActive : true
      });
    } else {
      setEditingPatient(null);
      setFormData({
        fullName: '',
        phone: '',
        email: '',
        gender: '',
        dateOfBirth: '',
        address: '',
        branchId: '',
        isActive: true
      });
    }
    setErrors({});
    setOpenPatientDialog(true);
  };

  const handleClosePatientDialog = () => {
    setOpenPatientDialog(false);
    setEditingPatient(null);
    setErrors({});
  };

  const validateForm = () => {
    const newErrors = {};
    if (!formData.fullName?.trim()) {
      newErrors.fullName = 'Full Name is required';
    }
    if (!formData.phone?.trim()) {
      newErrors.phone = 'Phone is required';
    }
    if (!formData.branchId) {
      newErrors.branchId = 'Branch is required';
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSavePatient = async () => {
    if (!validateForm()) {
      return;
    }

    try {
      setSaving(true);
      const saveData = {
        fullName: formData.fullName.trim(),
        phone: formData.phone.trim(),
        email: formData.email?.trim() || null,
        gender: formData.gender || null,
        dateOfBirth: formData.dateOfBirth || null,
        address: formData.address?.trim() || null,
        branchId: parseInt(formData.branchId, 10),
        isActive: formData.isActive
      };

      const response = editingPatient
        ? await patientApi.updatePatient(editingPatient.patientId, saveData)
        : await patientApi.createPatient(saveData);

      if (response?.success) {
        showSnackbar(`Patient ${editingPatient ? 'updated' : 'created'} successfully`);
        await fetchData();
        handleClosePatientDialog();
      } else {
        showSnackbar(response?.message || 'Error saving patient', 'error');
      }
    } catch (error) {
      console.error('Error saving patient:', error);
      showSnackbar(error?.message || 'Error saving patient', 'error');
    } finally {
      setSaving(false);
    }
  };

  const handleDeletePatient = async (patientId) => {
    if (!window.confirm('Are you sure you want to delete this patient?')) {
      return;
    }

    try {
      const response = await patientApi.deletePatient(patientId);
      if (response?.success) {
        showSnackbar('Patient deleted successfully');
        await fetchData();
      } else {
        showSnackbar(response?.message || 'Error deleting patient', 'error');
      }
    } catch (error) {
      console.error('Error deleting patient:', error);
      showSnackbar('Error deleting patient', 'error');
    }
  };

  const filteredPatients = patients.filter((patient) => {
    if (!searchTerm.trim()) return true;
    const term = searchTerm.toLowerCase();
    return (
      (patient.fullName && patient.fullName.toLowerCase().includes(term)) ||
      (patient.phone && patient.phone.toLowerCase().includes(term)) ||
      (patient.email && patient.email.toLowerCase().includes(term))
    );
  });

  const formatDate = (date) => {
    if (!date) return 'N/A';
    const d = new Date(date);
    return d.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  const getBranchName = (branchId) => {
    const branch = branches.find(b => b.branchId === branchId);
    return branch ? branch.name : 'N/A';
  };

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
          Patient Management
        </Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => handleOpenPatientDialog()}
          sx={{
            bgcolor: '#228B22',
            borderRadius: '12px',
            px: 3,
            py: 1.5,
            '&:hover': { bgcolor: '#1a6b1a' }
          }}
        >
          Add Patient
        </Button>
      </Box>

      {/* Search Bar */}
      <TextField
        fullWidth
        placeholder="Search by patient name, phone, or email..."
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

      {/* Patients Table */}
      {filteredPatients.length === 0 ? (
        <Box sx={{ textAlign: 'center', py: 4 }}>
          <Typography variant="body1" color="text.secondary">
            {patients.length === 0
              ? 'No patients found. Click "Add Patient" to create one.'
              : 'No patients match your search criteria.'}
          </Typography>
        </Box>
      ) : (
        <TableContainer component={Paper} sx={{ borderRadius: '12px', boxShadow: '0 2px 8px rgba(0,0,0,0.1)' }}>
          <Table>
            <TableHead>
              <TableRow sx={{ bgcolor: '#f5f5f5' }}>
                <TableCell sx={{ fontWeight: 700 }}>Full Name</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Phone</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Email</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Gender</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Date of Birth</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Branch</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Status</TableCell>
                <TableCell sx={{ fontWeight: 700 }}>Created At</TableCell>
                <TableCell sx={{ fontWeight: 700 }} align="center">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {filteredPatients.map((patient) => (
                <TableRow key={patient.patientId} hover>
                  <TableCell>{patient.fullName || 'N/A'}</TableCell>
                  <TableCell>{patient.phone || 'N/A'}</TableCell>
                  <TableCell>{patient.email || 'N/A'}</TableCell>
                  <TableCell>{patient.gender || 'N/A'}</TableCell>
                  <TableCell>{formatDate(patient.dateOfBirth)}</TableCell>
                  <TableCell>{getBranchName(patient.branchId)}</TableCell>
                  <TableCell>
                    <Chip
                      label={patient.isActive ? 'Active' : 'Inactive'}
                      size="small"
                      sx={{
                        bgcolor: patient.isActive 
                          ? 'rgba(34, 197, 94, 0.1)' 
                          : 'rgba(239, 68, 68, 0.1)',
                        color: patient.isActive ? '#22c55e' : '#ef4444'
                      }}
                    />
                  </TableCell>
                  <TableCell>{formatDate(patient.createdAt)}</TableCell>
                  <TableCell align="center">
                    <Box display="flex" gap={1} justifyContent="center">
                      <IconButton
                        size="small"
                        onClick={() => handleOpenPatientDialog(patient)}
                        sx={{
                          color: '#228B22',
                          bgcolor: 'rgba(34, 139, 34, 0.1)',
                          '&:hover': { bgcolor: 'rgba(34, 139, 34, 0.2)' }
                        }}
                        title="Edit Patient"
                      >
                        <EditIcon fontSize="small" />
                      </IconButton>
                      <IconButton
                        size="small"
                        onClick={() => handleDeletePatient(patient.patientId)}
                        sx={{
                          color: '#ef4444',
                          bgcolor: 'rgba(239, 68, 68, 0.1)',
                          '&:hover': { bgcolor: 'rgba(239, 68, 68, 0.2)' }
                        }}
                        title="Delete Patient"
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

      {/* Add/Edit Patient Dialog */}
      <Dialog
        open={openPatientDialog}
        onClose={handleClosePatientDialog}
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
          {editingPatient ? 'Edit Patient' : 'Add New Patient'}
        </DialogTitle>

        <DialogContent sx={{ p: 3 }}>
          {Object.keys(errors).length > 0 && (
            <Alert severity="error" sx={{ mb: 2, borderRadius: '12px' }}>
              Please fill in all required fields.
            </Alert>
          )}

          <Grid container spacing={2.5} sx={{ mt: 1 }}>
            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Full Name *"
                value={formData.fullName}
                onChange={(e) => {
                  setFormData({ ...formData, fullName: e.target.value });
                  setErrors({ ...errors, fullName: '' });
                }}
                error={!!errors.fullName}
                helperText={errors.fullName}
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
                label="Phone *"
                value={formData.phone}
                onChange={(e) => {
                  setFormData({ ...formData, phone: e.target.value });
                  setErrors({ ...errors, phone: '' });
                }}
                error={!!errors.phone}
                helperText={errors.phone}
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
                label="Email"
                type="email"
                value={formData.email}
                onChange={(e) => setFormData({ ...formData, email: e.target.value })}
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
              <FormControl fullWidth>
                <InputLabel>Gender</InputLabel>
                <Select
                  value={formData.gender || ''}
                  onChange={(e) => setFormData({ ...formData, gender: e.target.value })}
                  label="Gender"
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
                  <MenuItem value="Male">Male</MenuItem>
                  <MenuItem value="Female">Female</MenuItem>
                  <MenuItem value="Other">Other</MenuItem>
                </Select>
              </FormControl>
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                fullWidth
                label="Date of Birth"
                type="date"
                value={formData.dateOfBirth}
                onChange={(e) => setFormData({ ...formData, dateOfBirth: e.target.value })}
                InputLabelProps={{ shrink: true }}
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
              <FormControl fullWidth error={!!errors.branchId}>
                <InputLabel>Branch *</InputLabel>
                <Select
                  value={formData.branchId || ''}
                  onChange={(e) => {
                    setFormData({ ...formData, branchId: e.target.value });
                    setErrors({ ...errors, branchId: '' });
                  }}
                  label="Branch *"
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
                  <MenuItem value="">Select Branch</MenuItem>
                  {branches.map((branch) => (
                    <MenuItem key={branch.branchId} value={String(branch.branchId)}>
                      {branch.name}
                    </MenuItem>
                  ))}
                </Select>
                {errors.branchId && (
                  <Typography variant="caption" color="error" sx={{ mt: 0.5, ml: 1.75 }}>
                    {errors.branchId}
                  </Typography>
                )}
              </FormControl>
            </Grid>

            <Grid item xs={12}>
              <TextField
                fullWidth
                label="Address"
                multiline
                rows={3}
                value={formData.address}
                onChange={(e) => setFormData({ ...formData, address: e.target.value })}
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

            <Grid item xs={12}>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={formData.isActive}
                    onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                    sx={{
                      color: '#228B22',
                      '&.Mui-checked': {
                        color: '#228B22'
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
            onClick={handleClosePatientDialog}
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
            onClick={handleSavePatient}
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

export default PatientManagement;
