import React, { useState, useEffect } from 'react';
import {
  Box, Typography, Button, TextField, Dialog, DialogTitle, DialogContent,
  DialogActions, Alert, CircularProgress, IconButton, Snackbar, Card,
  Chip, InputAdornment, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, Paper, Tooltip, FormControl, InputLabel, Select, MenuItem
} from '@mui/material';
import { Add as AddIcon, Edit as EditIcon, Delete as DeleteIcon, Search as SearchIcon } from '@mui/icons-material';
import { branchApi } from '../../utils/branchApi';

const BranchManagement = () => {
  const [branches, setBranches] = useState([]);
  const [loading, setLoading] = useState(true);
  const [openDialog, setOpenDialog] = useState(false);
  const [editingBranch, setEditingBranch] = useState(null);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
  const [searchTerm, setSearchTerm] = useState('');
  const [saving, setSaving] = useState(false);
  const [formData, setFormData] = useState({
    name: '', code: '', phone: '', whatsappNumber: '', email: '', address: '', city: '', isActive: true
  });
  const [errors, setErrors] = useState({});

  useEffect(() => {
    fetchBranches();
  }, []);

  const fetchBranches = async () => {
    try {
      setLoading(true);
      const response = await branchApi.getAllBranches();
      if (response.success) {
        setBranches(response.result || []);
      } else {
        showSnackbar(response.message || 'Error loading branches', 'error');
      }
    } catch (error) {
      showSnackbar('Error loading branches', 'error');
    } finally {
      setLoading(false);
    }
  };

  const showSnackbar = (message, severity = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  const handleOpenDialog = (branch = null) => {
    if (branch) {
      setEditingBranch(branch);
      setFormData({
        name: branch.name || '',
        code: branch.code || '',
        phone: branch.phone || '',
        whatsappNumber: branch.whatsAppNumber || '',
        email: branch.email || '',
        address: branch.address || '',
        city: branch.city || '',
        isActive: branch.isActive ?? true
      });
    } else {
      setEditingBranch(null);
      setFormData({
        name: '', code: '', phone: '', whatsappNumber: '', email: '', address: '', city: '', isActive: true
      });
    }
    setErrors({});
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setEditingBranch(null);
    setErrors({});
  };

  const validateForm = () => {
    const newErrors = {};
    if (!formData.name || !formData.name.trim()) {
      newErrors.name = 'Name is required';
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
      const response = editingBranch
        ? await branchApi.updateBranch(editingBranch.branchId, formData)
        : await branchApi.createBranch(formData);
      
      // Both CreateBranch and UpdateBranch now return ServiceResponse
      if (response.success) {
        showSnackbar(`Branch ${editingBranch ? 'updated' : 'created'} successfully`, 'success');
        handleCloseDialog();
        // Refetch immediately
        await fetchBranches();
      } else {
        showSnackbar(response.message || 'Error saving branch', 'error');
      }
    } catch (error) {
      console.error('Error saving branch:', error);
      showSnackbar(error.message || 'Error saving branch', 'error');
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this branch?')) return;
    try {
      const response = await branchApi.deleteBranch(id);
      if (response.success) {
        showSnackbar('Branch deleted successfully', 'success');
        // Refetch immediately
        await fetchBranches();
      } else {
        showSnackbar(response.message || 'Error deleting branch', 'error');
      }
    } catch (error) {
      showSnackbar('Error deleting branch', 'error');
    }
  };

  const filteredBranches = branches.filter(branch =>
    branch.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    branch.code?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    branch.city?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    branch.phone?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    branch.email?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="400px">
        <CircularProgress sx={{ color: '#228B22' }} />
      </Box>
    );
  }

  return (
    <Box sx={{ p: 3 }}>
      {/* Header with Add Button */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h5" sx={{ fontWeight: 700, color: '#228B22' }}>
          Branch Management
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
          Add Branch
        </Button>
      </Box>

      {/* Search Bar */}
      <Card sx={{ mb: 3, borderRadius: '16px', boxShadow: '0 4px 20px rgba(34, 139, 34, 0.08)' }}>
        <Box sx={{ p: 2 }}>
          <TextField
            fullWidth
            placeholder="Search branches by name, code, city, phone, or email..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchIcon sx={{ color: '#228B22' }} />
                </InputAdornment>
              ),
            }}
            sx={{
              '& .MuiOutlinedInput-root': {
                borderRadius: '12px',
                '&:hover .MuiOutlinedInput-notchedOutline': {
                  borderColor: '#228B22',
                },
                '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                  borderColor: '#228B22',
                  borderWidth: '2px',
                },
              },
            }}
          />
        </Box>
      </Card>

      {/* Branches Table */}
      <Card sx={{ borderRadius: '16px', overflow: 'hidden', boxShadow: '0 4px 20px rgba(34, 139, 34, 0.08)' }}>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow
                sx={{
                  background: 'linear-gradient(135deg, #228B22 0%, #32CD32 100%)',
                  '& th': {
                    color: 'white',
                    fontWeight: 700,
                    fontSize: '0.9rem',
                    borderBottom: 'none',
                    py: 2,
                  },
                }}
              >
                <TableCell>Name</TableCell>
                <TableCell>Code</TableCell>
                <TableCell>Phone</TableCell>
                <TableCell>Email</TableCell>
                <TableCell>City</TableCell>
                <TableCell>Status</TableCell>
                <TableCell align="center">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {filteredBranches.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={7} align="center" sx={{ py: 4 }}>
                    <Typography variant="body1" color="text.secondary">
                      {branches.length === 0 ? 'No branches found. Click "Add Branch" to create one.' : 'No branches match your search criteria.'}
                    </Typography>
                  </TableCell>
                </TableRow>
              ) : (
                filteredBranches.map((branch) => (
                  <TableRow
                    key={branch.branchId}
                    hover
                    sx={{
                      '&:nth-of-type(even)': { bgcolor: '#f8fafc' },
                      '&:hover': {
                        bgcolor: 'rgba(34, 139, 34, 0.02)',
                      },
                    }}
                  >
                    <TableCell>
                      <Typography variant="body2" sx={{ fontWeight: 600, color: '#1e293b' }}>
                        {branch.name}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2" sx={{ color: '#475569' }}>
                        {branch.code || 'N/A'}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2" sx={{ color: '#475569' }}>
                        {branch.phone || 'N/A'}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2" sx={{ color: '#475569' }}>
                        {branch.email || 'N/A'}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2" sx={{ color: '#475569' }}>
                        {branch.city || 'N/A'}
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Chip
                        label={branch.isActive ? 'Active' : 'Inactive'}
                        size="small"
                        sx={{
                          bgcolor: branch.isActive
                            ? 'rgba(34, 197, 94, 0.1)'
                            : 'rgba(239, 68, 68, 0.1)',
                          color: branch.isActive ? '#22c55e' : '#ef4444',
                          fontWeight: 600,
                          borderRadius: '8px',
                        }}
                      />
                    </TableCell>
                    <TableCell align="center">
                      <Box sx={{ display: 'flex', gap: 1, justifyContent: 'center' }}>
                        <Tooltip title="Edit Branch">
                          <IconButton
                            size="small"
                            onClick={() => handleOpenDialog(branch)}
                            sx={{
                              color: '#228B22',
                              p: 1,
                              bgcolor: 'rgba(34, 139, 34, 0.1)',
                              '&:hover': {
                                bgcolor: 'rgba(34, 139, 34, 0.2)',
                                transform: 'scale(1.1)',
                              },
                              transition: 'all 0.2s ease',
                            }}
                          >
                            <EditIcon sx={{ fontSize: '1.1rem' }} />
                          </IconButton>
                        </Tooltip>
                        <Tooltip title="Delete Branch">
                          <IconButton
                            size="small"
                            onClick={() => handleDelete(branch.branchId)}
                            sx={{
                              color: '#ef4444',
                              p: 1,
                              bgcolor: 'rgba(239, 68, 68, 0.1)',
                              '&:hover': {
                                bgcolor: 'rgba(239, 68, 68, 0.2)',
                                transform: 'scale(1.1)',
                              },
                              transition: 'all 0.2s ease',
                            }}
                          >
                            <DeleteIcon sx={{ fontSize: '1.1rem' }} />
                          </IconButton>
                        </Tooltip>
                      </Box>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Card>

      {/* Add/Edit Dialog */}
      <Dialog 
        open={openDialog} 
        onClose={handleCloseDialog} 
        maxWidth="md" 
        fullWidth
        PaperProps={{
          sx: {
            borderRadius: '16px',
            boxShadow: '0 4px 20px rgba(0,0,0,0.15)',
          }
        }}
      >
        <DialogTitle
          sx={{
            background: 'linear-gradient(135deg, #228B22 0%, #32CD32 100%)',
            color: 'white',
            fontWeight: 700,
            py: 2.5,
          }}
        >
          {editingBranch ? 'Edit Branch' : 'Add New Branch'}
        </DialogTitle>
        <DialogContent sx={{ p: 3 }}>
          {errors.name && (
            <Alert severity="error" sx={{ mb: 2, borderRadius: '12px' }}>
              {errors.name}
            </Alert>
          )}
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2.5, mt: 1 }}>
            <TextField
              fullWidth
              label="Name *"
              value={formData.name}
              onChange={(e) => {
                setFormData({ ...formData, name: e.target.value });
                setErrors({ ...errors, name: '' });
              }}
              error={!!errors.name}
              sx={{
                '& .MuiOutlinedInput-root': {
                  borderRadius: '12px',
                  '&:hover .MuiOutlinedInput-notchedOutline': {
                    borderColor: '#228B22',
                  },
                  '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                    borderColor: '#228B22',
                    borderWidth: '2px',
                  },
                },
              }}
            />
            <Box sx={{ display: 'flex', gap: 2 }}>
              <TextField
                fullWidth
                label="Code"
                value={formData.code}
                onChange={(e) => setFormData({ ...formData, code: e.target.value })}
                sx={{
                  '& .MuiOutlinedInput-root': {
                    borderRadius: '12px',
                    '&:hover .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22',
                    },
                    '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22',
                      borderWidth: '2px',
                    },
                  },
                }}
              />
              <TextField
                fullWidth
                label="City"
                value={formData.city}
                onChange={(e) => setFormData({ ...formData, city: e.target.value })}
                sx={{
                  '& .MuiOutlinedInput-root': {
                    borderRadius: '12px',
                    '&:hover .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22',
                    },
                    '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22',
                      borderWidth: '2px',
                    },
                  },
                }}
              />
            </Box>
            <Box sx={{ display: 'flex', gap: 2 }}>
              <TextField
                fullWidth
                label="Phone"
                value={formData.phone}
                onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                sx={{
                  '& .MuiOutlinedInput-root': {
                    borderRadius: '12px',
                    '&:hover .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22',
                    },
                    '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22',
                      borderWidth: '2px',
                    },
                  },
                }}
              />
              <TextField
                fullWidth
                label="WhatsApp Number"
                value={formData.whatsappNumber}
                onChange={(e) => setFormData({ ...formData, whatsappNumber: e.target.value })}
                sx={{
                  '& .MuiOutlinedInput-root': {
                    borderRadius: '12px',
                    '&:hover .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22',
                    },
                    '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                      borderColor: '#228B22',
                      borderWidth: '2px',
                    },
                  },
                }}
              />
            </Box>
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
                    borderColor: '#228B22',
                  },
                  '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                    borderColor: '#228B22',
                    borderWidth: '2px',
                  },
                },
              }}
            />
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
                    borderColor: '#228B22',
                  },
                  '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                    borderColor: '#228B22',
                    borderWidth: '2px',
                  },
                },
              }}
            />
            <FormControl fullWidth>
              <InputLabel>Status</InputLabel>
              <Select
                value={formData.isActive}
                onChange={(e) => setFormData({ ...formData, isActive: e.target.value === 'true' || e.target.value === true })}
                label="Status"
                sx={{
                  borderRadius: '12px',
                  '&:hover .MuiOutlinedInput-notchedOutline': {
                    borderColor: '#228B22',
                  },
                  '&.Mui-focused .MuiOutlinedInput-notchedOutline': {
                    borderColor: '#228B22',
                    borderWidth: '2px',
                  },
                }}
              >
                <MenuItem value={true}>Active</MenuItem>
                <MenuItem value={false}>Inactive</MenuItem>
              </Select>
            </FormControl>
          </Box>
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
                bgcolor: 'rgba(34, 139, 34, 0.04)',
              },
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
                bgcolor: '#1B5E20',
              },
              '&:disabled': {
                bgcolor: '#9CA3AF',
              },
            }}
          >
            {saving ? 'Saving...' : 'Save'}
          </Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={snackbar.open} autoHideDuration={6000} onClose={() => setSnackbar({ ...snackbar, open: false })}>
        <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  );
};

export default BranchManagement;

