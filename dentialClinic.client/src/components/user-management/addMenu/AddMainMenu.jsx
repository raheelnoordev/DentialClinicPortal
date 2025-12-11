// React Imports
import React, { useState, useEffect } from "react";

// Other Imports
import axios from "axios";
import { useSnackbar } from "notistack";

// MUI Imports
import {
  Grid,
  TextField,
  Button,
  MenuItem,
  Card,
  CardContent,
  Select,
  InputLabel,
  CardHeader,
  FormControl,
  Typography,
  Divider,
  CircularProgress,
  Box,
  Alert,
} from "@mui/material";
import { Menu, Save, Clear } from "@mui/icons-material";

// Utility Imports
import { getAuthHeaders, getCurrentUser } from "../../../utils/auth";
import { getBaseUrl } from "../../../utils/api";

const AddMenu = ({ menuData, onMenuSaved, onRefresh }) => {
  // Get current user from auth utility
  const user = getCurrentUser();
  const { enqueueSnackbar } = useSnackbar();

  const [formData, setFormData] = useState({
    menuId: 0,
    name: "",
    url: "",
    icon: "",
    sortOrder: 1,
    isActive: true,
  });

  const [isMenuSaveLoading, setIsMenuSaveLoading] = useState(false);
  const [errors, setErrors] = useState({});
  const [successMessage, setSuccessMessage] = useState("");

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
    setErrors((prev) => ({ ...prev, [name]: "" })); // Clear error when the user starts typing
  };

  const validateFields = () => {
    let isValid = true;
    const newErrors = {};

    if (!formData.name || !formData.name.trim()) {
      newErrors.name = "Menu name is required";
      isValid = false;
    }

    setErrors(newErrors);
    return isValid;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!validateFields()) {
      return;
    }

    setIsMenuSaveLoading(true);
    setErrors({});
    setSuccessMessage("");

    try {
      // Check if user is authenticated - removed empId check since we don't need it for menu operations
      if (!user || !user.userId) {
        enqueueSnackbar("User not authenticated. Please login again.", {
          variant: "error",
          autoHideDuration: 5000,
        });
        return;
      }

      console.log("Current user for menu operation:", {
        userId: user.userId,
        username: user.username || user.email || "N/A",
        role: user.roleName || "N/A",
      });

      let response;
      let isUpdate = !!menuData;

      if (isUpdate) {
        // Update existing menu
        const updateData = {
          menuId: formData.menuId,
          name: formData.name.trim(),
          url: formData.url || "",
          icon: formData.icon || "",
          sortOrder: formData.sortOrder || 1,
          isActive: formData.isActive,
        };

        console.log("Updating menu:", updateData);
        response = await axios.put(
          `${getBaseUrl()}/UserManagement/UpdateMenu`,
          updateData,
          {
            headers: getAuthHeaders(),
          }
        );
      } else {
        // Create new menu
        const createData = {
          menuId: 0, // API expects 0 for new menus
          name: formData.name.trim(),
          url: formData.url || "",
          icon: formData.icon || "",
          sortOrder: formData.sortOrder || 1,
          isActive: formData.isActive,
        };

        console.log("Creating menu:", createData);
        response = await axios.post(
          `${getBaseUrl()}/UserManagement/SaveMenu`,
          createData,
          {
            headers: getAuthHeaders(),
          }
        );
      }

      console.log("API Response:", response.data);

      if (response.data && response.data.success) {
        

        // Reset form for new menu creation
        if (!isUpdate) {
          setFormData({
            menuId: 0,
            name: "",
            url: "",
            icon: "",
            sortOrder: 1,
            isActive: true,
          });
        }

        // Notify parent component
        if (onMenuSaved) {
          onMenuSaved(response.data.result);
        }

        // Trigger refresh of menu list
        if (onRefresh) {
          onRefresh();
        }

        enqueueSnackbar(response.data.message || "Menu saved successfully", {
          variant: "success",
          autoHideDuration: 5000,
        });
      } else {
        const errorMsg = response.data?.message || "Failed to save menu";
        setErrors({ submit: errorMsg });
        enqueueSnackbar(errorMsg, {
          variant: "error",
          autoHideDuration: 5000,
        });
      }
    } catch (error) {
      console.error("Error saving menu:", error);
      console.error("Error response:", error.response?.data);
      console.error("Error status:", error.response?.status);

      let errorMessage = "Error saving menu. Please try again.";

      if (error.response?.status === 400) {
        errorMessage =
          error.response.data?.message ||
          "Validation error. Please check your input.";
      } else if (error.response?.status === 401) {
        errorMessage = "Unauthorized. Please check your authentication.";
      } else if (error.response?.status === 409) {
        errorMessage = "Menu name already exists.";
      } else if (error.response?.status === 500) {
        errorMessage = "Server error. Please try again later.";
      }

      setErrors({ submit: errorMessage });
      enqueueSnackbar(errorMessage, {
        variant: "error",
        autoHideDuration: 5000,
      });
    } finally {
      setIsMenuSaveLoading(false);
    }
  };

  const handleCancel = () => {
    // Reset form
    setFormData({
      menuId: 0,
      name: "",
      url: "",
      icon: "",
      sortOrder: 1,
      isActive: true,
    });
    setErrors({});
    setSuccessMessage("");
  };

  useEffect(() => {
    if (menuData) {
      console.log("Populating form with menu data:", menuData);

      const populatedFormData = {
        menuId: menuData.menuId || 0,
        name: menuData.name || menuData.menuName || "",
        url: menuData.url || "",
        icon: menuData.icon || menuData.iconUrl || "",
        sortOrder: menuData.sortOrder || menuData.orderNo || 1,
        isActive: menuData.isActive !== undefined ? menuData.isActive : (menuData.isEnabled === "Y" || menuData.isEnabled === true),
      };

      console.log("Setting form data for edit:", populatedFormData);
      setFormData(populatedFormData);
    } else {
      console.log("Initializing form for new menu creation");
      // Reset form for new menu
      const newFormData = {
        menuId: 0,
        name: "",
        url: "",
        icon: "",
        sortOrder: 1,
        isActive: true,
      };

      console.log("Setting form data for new menu:", newFormData);
      setFormData(newFormData);
    }
  }, [menuData]);

  return (
    <Box sx={{ width: "100%", p: 2 }}>
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
            <Menu sx={{ fontSize: "2rem" }} />
            <Box>
              <Typography
                variant='h5'
                color='#ffffff'
                sx={{ fontWeight: 700, mb: 0.5 }}
              >
                {menuData ? "Edit Menu" : "Add New Menu"}
              </Typography>
              {/* <Typography variant="body2" sx={{ opacity: 0.9 }}>
                {menuData ? 'Update menu information and settings' : 'Create a new menu with specific properties'}
              </Typography> */}
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
            <Grid container spacing={3} sx={{ mb: 4 }}>
              <Grid item xs={12} md={6}>
                <TextField
                  fullWidth
                  label='Menu Name *'
                  value={formData.name}
                  name='name'
                  placeholder='Enter menu name'
                  onChange={handleChange}
                  required
                  error={!!errors.name}
                  helperText={errors.name}
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
                  label='URL'
                  value={formData.url}
                  name='url'
                  placeholder='Enter menu URL (e.g., /dashboard)'
                  onChange={handleChange}
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

              <Grid item xs={12} md={4}>
                <TextField
                  fullWidth
                  label='Icon'
                  value={formData.icon}
                  name='icon'
                  placeholder='Enter icon (e.g., ri-dashboard-line)'
                  onChange={handleChange}
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

              <Grid item xs={12} md={4}>
                <TextField
                  fullWidth
                  label='Sort Order *'
                  type='number'
                  value={formData.sortOrder}
                  name='sortOrder'
                  placeholder='Enter sort order'
                  onChange={(e) => setFormData({ ...formData, sortOrder: parseInt(e.target.value) || 1 })}
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

              <Grid item xs={12} md={4}>
                <FormControl fullWidth>
                  <InputLabel>Status</InputLabel>
                  <Select
                    value={formData.isActive}
                    name='isActive'
                    onChange={(e) => setFormData({ ...formData, isActive: e.target.value === 'true' || e.target.value === true })}
                    label='Status'
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
                disabled={isMenuSaveLoading}
                
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
                {isMenuSaveLoading
                  ? "Saving..."
                  : menuData
                  ? "Update Menu"
                  : "Save Menu"}
              </Button>
            </Box>
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
};

export default AddMenu;
