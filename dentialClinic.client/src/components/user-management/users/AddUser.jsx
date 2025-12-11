"use client";

import React, { useState, useEffect } from "react";
import {
  Box,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Button,
  Alert,
  Grid,
  InputAdornment,
  Typography,
  Divider,
  Card,
  CardContent,
  Paper,
  Chip,
  OutlinedInput,
  Checkbox,
  ListItemText,
  CircularProgress,
  FormHelperText,
  } from "@mui/material";
import {
  Person,
  Phone,
  Badge,
  Security,
  Lock,
  Comment,
  Save,
  Clear,
  Add,
  Business,
} from "@mui/icons-material";
import axios from "axios";
import { getAuthHeaders } from "../../../utils/auth";
import { getBaseUrl } from "../../../utils/api";

const AddUser = ({ userData, setUserData, onSuccess, onError }) => {
  // Get current user from localStorage
  const getCurrentUser = () => {
    try {
      const storedUser = localStorage.getItem("user");
      return storedUser ? JSON.parse(storedUser) : { empId: 1 }; // Default empId if no user
    } catch (error) {
      console.error("Error parsing user data:", error);
      return { empId: 1 }; // Default empId
    }
  };
  const currentUser = getCurrentUser();

  const [formData, setFormData] = useState({
    userId: 0,
    branchId: null,
    fullName: "",
    email: "",
    phone: "",
    passwordHash: "",
    isActive: true,
    roleId: 0,
  });

  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState({});
  const [successMessage, setSuccessMessage] = useState("");
  const [roles, setRoles] = useState([]);
  const [branches, setBranches] = useState([]);

  // Fetch roles and branches on component mount
  useEffect(() => {
    fetchRoles();
    fetchBranches();
  }, []);

  // Load user data for editing
  useEffect(() => {
    if (userData) {
      console.log("Setting form data from userData:", userData);
      
      // Handle roleIds - convert single roleId to array or use existing array
      const roleId = userData.roleId ? 
        userData.roleId : 0;
      
      const newFormData = {
        userId: userData.userId || 0,
        branchId: userData.branchId || null,
        fullName: userData.fullName || "",
        email: userData.email || "",
        phone: userData.phone || "",
        passwordHash: userData.passwordHash || "",
        isActive: userData.isActive !== undefined ? userData.isActive : true,
        roleId: roleId,
      };
      
      console.log("Setting form data to:", newFormData);
      setFormData(newFormData);
    } else {
      // Reset form when no userData
      setFormData({
        userId: 0,
        branchId: null,
        fullName: "",
        email: "",
        phone: "",
        passwordHash: "",
        isActive: true,
        roleId: 0,
      });
    }
  }, [userData]);

  const fetchRoles = async () => {
    try {
      const response = await axios.get(
        `${getBaseUrl()}/UserManagement/GetRoleList`,
        {
          headers: getAuthHeaders(),
        }
      );
      if (response.data && response.data.success) {
        setRoles(response.data.result || []);
      }
    } catch (error) {
      console.error("Error fetching roles:", error);
    }
  };

  const fetchBranches = async () => {
    try {
      const response = await axios.get(
        `${getBaseUrl()}/Branches/GetAllBranches`,
        {
          headers: getAuthHeaders(),
        }
      );
      if (response.data && response.data.success) {
        setBranches(response.data.result || []);
      }
    } catch (error) {
      console.error("Error fetching branches:", error);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;

    // Handle field changes
    setFormData({
      ...formData,
      [name]: value,
    });

    if (errors[name]) {
      setErrors({
        ...errors,
        [name]: "",
      });
    }
  };

  const validateForm = () => {
    const newErrors = {};

    console.log("Validating form with userData:", userData);
    console.log("Form data for validation:", formData);

    if (!formData.fullName.trim())
      newErrors.fullName = "Full name is required";
    if (!formData.email.trim()) 
      newErrors.email = "Email is required";
    if (!userData && !formData.passwordHash.trim())
      newErrors.passwordHash = "Password is required";
    if (!formData.roleId)
      newErrors.roleId = "Role is required";

    console.log("Validation errors found:", newErrors);
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    console.log("Form submitted! userData:", userData);
    console.log("Form data:", formData);

    if (!validateForm()) {
      console.log("Form validation failed, returning");
      return;
    }

    console.log("Form validation passed, proceeding with submission");
    setLoading(true);
    setSuccessMessage("");

    try {
      let response;

      if (userData && userData.userId) {
        console.log("Updating existing user with ID:", userData.userId);
        // Update existing user
        const updateData = {
          userId: userData.userId,
          branchId: formData.branchId || null,
          fullName: formData.fullName,
          email: formData.email,
          phone: formData.phone,
          passwordHash: formData.passwordHash || undefined, // Only send if provided
          isActive: formData.isActive,
          roleId: formData.roleId,
        };

        console.log("Update data being sent:", updateData);

        response = await axios.put(
          `${getBaseUrl()}/UserManagement/UpdateUser`,
          updateData,
          {
            headers: getAuthHeaders(),
          }
        );

        console.log("Update response:", response.data);
      } else {
        console.log("Creating new user");
        // Create new user
        const createData = {
          branchId: formData.branchId || null,
          fullName: formData.fullName,
          email: formData.email,
          phone: formData.phone,
          passwordHash: formData.passwordHash,
          isActive: formData.isActive,
          roleId: formData.roleId,
        };

        console.log("Create data being sent:", createData);

        response = await axios.post(
          `${getBaseUrl()}/UserManagement/SaveUser`,
          createData,
          {
            headers: getAuthHeaders(),
          }
        );

        console.log("Create response:", response.data);
      }

      if (response.data && response.data.success) {
        //const successMessage = userData ? "User updated successfully!" : "User created successfully!";
        
        // Always reset form after successful operation (both add and edit)
        setFormData({
          userId: 0,
          branchId: null,
          fullName: "",
          email: "",
          phone: "",
          passwordHash: "",
          isActive: true,
          roleId: 0,
        });
        
        setUserData(null); // Clear editing state
        setErrors({}); // Clear any errors
        setSuccessMessage(successMessage);
        
        // Call onSuccess with the message to trigger data refetch and show notification
        if (onSuccess) {
          onSuccess(successMessage);
        }
      } else {
        const errorMessage = response.data?.message || "Failed to save user";
        setErrors({ submit: errorMessage });
        if (onError) {
          onError(errorMessage);
        }
      }
    } catch (error) {
      console.error("Error saving user:", error);
      const errorMessage = "Failed to save user. Please try again.";
      setErrors({ submit: errorMessage });
      if (onError) {
        onError(errorMessage);
      }
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = () => {
    setUserData(null);
    setFormData({
      userId: 0,
      branchId: null,
      fullName: "",
      email: "",
      phone: "",
      passwordHash: "",
      isActive: true,
      roleId: 0,
    });
    setErrors({});
    setSuccessMessage("");
  };

  return (
    <Box sx={{ width: "100%", p: 3 }} id="userForm">
      <Card
        sx={{
          borderRadius: "16px",
          overflow: "hidden",
          boxShadow: "0 4px 20px rgba(0,0,0,0.08)",
        }}
      >
        {/* Header */}
        <Box
          sx={{
            background: "linear-gradient(135deg, #228B22 0%, #32CD32 100%)",
            p: 3,
            color: "white",
          }}
        >
          <Box sx={{ display: "flex", alignItems: "center", gap: 2 }}>
            <Person sx={{ fontSize: "2rem" }} />
            <Box>
              <Typography
                variant='h5'
                sx={{ fontWeight: 500, mb: 0.5, color: "white" }}
              >
                {userData ? "Edit User" : "Add New User"}
              </Typography>
              <Typography variant='body2' sx={{ opacity: 0.9, color: "white" }}>
                {/* {userData
                  ? "Update user information and permissions"
                  : "Create a new user account with role assignments"} */}
              </Typography> 
            </Box>
          </Box>
        </Box>

        <CardContent sx={{ p: 4 }}>
          {/* Success Message */}
          {successMessage && (
            <Alert severity='success' sx={{ mb: 3, borderRadius: "12px" }}>
              {successMessage}
            </Alert>
          )}

          {/* Error Message */}
          {errors.submit && (
            <Alert severity='error' sx={{ mb: 3, borderRadius: "12px" }}>
              {errors.submit}
            </Alert>
          )}

          {/* Form */}
          <Box component='form' onSubmit={handleSubmit} noValidate>
            {/* Form Fields */}
            <Grid container spacing={3} sx={{ mb: 4 }}>
              {/* Basic Information Fields */}
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  name='fullName'
                  label='Full Name'
                  value={formData.fullName}
                  onChange={handleChange}
                  error={!!errors.fullName}
                  helperText={errors.fullName}
                  required
                  sx={{
                    "& .MuiOutlinedInput-root": {
                      borderRadius: "12px",
                      "&:hover .MuiOutlinedInput-notchedOutline": {
                        borderColor: "#228B22",
                      },
                      "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
                        borderColor: "#228B22",
                        borderWidth: "2px",
                      },
                    },
                  }}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  name='email'
                  label='Email'
                  type='email'
                  value={formData.email}
                  onChange={handleChange}
                  error={!!errors.email}
                  helperText={errors.email}
                  required
                  sx={{
                    "& .MuiOutlinedInput-root": {
                      borderRadius: "12px",
                      "&:hover .MuiOutlinedInput-notchedOutline": {
                        borderColor: "#228B22",
                      },
                      "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
                        borderColor: "#228B22",
                        borderWidth: "2px",
                      },
                    },
                  }}
                />
              </Grid>

              {/* Contact & Security Fields */}
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  name='phone'
                  label='Phone'
                  value={formData.phone}
                  onChange={handleChange}
                  error={!!errors.phone}
                  helperText={errors.phone}
                  sx={{
                    "& .MuiOutlinedInput-root": {
                      borderRadius: "12px",
                      "&:hover .MuiOutlinedInput-notchedOutline": {
                        borderColor: "#228B22",
                      },
                      "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
                        borderColor: "#228B22",
                        borderWidth: "2px",
                      },
                    },
                  }}
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  name='passwordHash'
                  label='Password'
                  type='password'
                  value={formData.passwordHash}
                  onChange={handleChange}
                  error={!!errors.passwordHash}
                  helperText={errors.passwordHash || (userData ? "Leave blank to keep current password" : "")}
                  required={!userData}
                  sx={{
                    "& .MuiOutlinedInput-root": {
                      borderRadius: "12px",
                      "&:hover .MuiOutlinedInput-notchedOutline": {
                        borderColor: "#228B22",
                      },
                      "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
                        borderColor: "#228B22",
                        borderWidth: "2px",
                      },
                    },
                  }}
                />
              </Grid>

              {/* Role & Status Fields */}
              <Grid item xs={12} md={6}>
                <FormControl fullWidth required error={!!errors.roleId}>
                  <InputLabel>Roles</InputLabel>
                  <Select
                    name='roleId'
                    value={formData.roleId}
                    onChange={(e) => {
                      const value = e.target.value;
                      setFormData({ ...formData, roleId: value });
                    }}
                    input={<OutlinedInput label="Role" />}
                    renderValue={(selected) => {
                      const role = roles.find(r => r.roleId === selected);
                      return role ? role.roleName : selected;
                    }}
                    sx={{
                      borderRadius: "12px",
                      "&:hover .MuiOutlinedInput-notchedOutline": {
                        borderColor: "#228B22",
                      },
                      "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
                        borderColor: "#228B22",
                        borderWidth: "2px",
                      },
                    }}
                  >
                    {roles.map((role) => (
                      <MenuItem key={role.roleId} value={role.roleId}>
                        <Checkbox checked={formData.roleId === role.roleId} />
                        <ListItemText primary={role.roleName} />
                      </MenuItem>
                    ))}
                  </Select>
                  {errors.roleId && (
                    <FormHelperText error={!!errors.roleId}>{errors.roleId}</FormHelperText>
                  )}
                </FormControl>
              </Grid>
              <Grid item xs={12} md={6}>
                <FormControl fullWidth>
                  <InputLabel>Status</InputLabel>
                  <Select
                    name='isActive'
                    value={formData.isActive}
                    onChange={(e) => setFormData({ ...formData, isActive: e.target.value === 'true' })}
                    sx={{
                      borderRadius: "12px",
                      "&:hover .MuiOutlinedInput-notchedOutline": {
                        borderColor: "#228B22",
                      },
                      "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
                        borderColor: "#228B22",
                        borderWidth: "2px",
                      },
                    }}
                  >
                    <MenuItem value={true}>Active</MenuItem>
                    <MenuItem value={false}>Inactive</MenuItem>
                  </Select>
                </FormControl>
              </Grid>

              {/* Branch Field */}
              <Grid item xs={12} md={6}>
                <FormControl fullWidth>
                  <InputLabel>Branch</InputLabel>
                  <Select
                    name='branchId'
                    value={formData.branchId || ''}
                    onChange={(e) => setFormData({ ...formData, branchId: e.target.value ? parseInt(e.target.value) : null })}
                    label='Branch'
                    sx={{
                      borderRadius: "12px",
                      "&:hover .MuiOutlinedInput-notchedOutline": {
                        borderColor: "#228B22",
                      },
                      "&.Mui-focused .MuiOutlinedInput-notchedOutline": {
                        borderColor: "#228B22",
                        borderWidth: "2px",
                      },
                    }}
                  >
                    <MenuItem value=''>
                      <em>None</em>
                    </MenuItem>
                    {branches.map((branch) => (
                      <MenuItem key={branch.branchId} value={branch.branchId}>
                        {branch.name}
                      </MenuItem>
                    ))}
                  </Select>
                </FormControl>
              </Grid>
            </Grid>

            {/* Action Buttons */}
            <Box
              sx={{
                display: "flex",
                gap: 2,
                justifyContent: "flex-end",
                mt: 4,
              }}
            >
              <Button
                type='button'
                variant='outlined'
                onClick={handleCancel}
                startIcon={<Clear />}
                sx={{
                  borderColor: "#228B22",
                  color: "#228B22",
                  borderRadius: "12px",
                  px: 4,
                  py: 1.5,
                  "&:hover": {
                    borderColor: "#1B5E20",
                    bgcolor: "rgba(34, 139, 34, 0.04)",
                  },
                }}
              >
                Cancel
              </Button>
              <Button
                type='submit'
                variant='contained'
                disabled={loading}
                //startIcon={loading ? <CircularProgress size={20} /> : <Save />}
                sx={{
                  bgcolor: "#228B22",
                  borderRadius: "12px",
                  px: 4,
                  py: 1.5,
                  "&:hover": {
                    bgcolor: "#1B5E20",
                  },
                  "&:disabled": {
                    bgcolor: "#9CA3AF",
                  },
                }}
              >
                {loading ? "Saving..." : userData ? "Update User" : "Save User"}
              </Button>
            </Box>
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
};

export default AddUser;
