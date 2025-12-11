import React, { useState, useEffect } from 'react';
import {
  Box, Typography, Button, Grid, TextField, Dialog, DialogTitle, DialogContent,
  DialogActions, Alert, CircularProgress, IconButton, Snackbar, Card, CardContent,
  Chip, MenuItem, Select, FormControl, InputLabel, Tabs, Tab
} from '@mui/material';
import { Settings as SettingsIcon, Message as MessageIcon, BookOnline as BookIcon } from '@mui/icons-material';
import { whatsappApi } from '../../utils/whatsappApi';
import { branchApi } from '../../utils/branchApi';

const WhatsAppManagement = () => {
  const [settings, setSettings] = useState([]);
  const [screens, setScreens] = useState([]);
  const [bookings, setBookings] = useState([]);
  const [branches, setBranches] = useState([]);
  const [loading, setLoading] = useState(true);
  const [tabValue, setTabValue] = useState(0);
  const [openSettingsDialog, setOpenSettingsDialog] = useState(false);
  const [openScreenDialog, setOpenScreenDialog] = useState(false);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
  const [settingsForm, setSettingsForm] = useState({
    branchId: '', phoneNumberId: '', businessAccountId: '', apiBaseUrl: '', accessToken: '', isActive: true
  });
  const [screenForm, setScreenForm] = useState({
    branchId: '', code: '', languageCode: 'en', title: '', messageText: '', screenType: 'MENU', isActive: true
  });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [settingsRes, screensRes, bookingsRes, branchesRes] = await Promise.all([
        whatsappApi.getAllSettings(),
        whatsappApi.getAllScreens(),
        whatsappApi.getAllBookings(),
        branchApi.getActiveBranches()
      ]);
      if (settingsRes.success) setSettings(settingsRes.result || []);
      if (screensRes.success) setScreens(screensRes.result || []);
      if (bookingsRes.success) setBookings(bookingsRes.result || []);
      if (branchesRes.success) setBranches(branchesRes.result || []);
    } catch (error) {
      showSnackbar('Error loading data', 'error');
    } finally {
      setLoading(false);
    }
  };

  const showSnackbar = (message, severity = 'success') => {
    setSnackbar({ open: true, message, severity });
  };

  const handleSaveSettings = async () => {
    try {
      const response = await whatsappApi.createOrUpdateSettings(settingsForm);
      if (response.success) {
        showSnackbar('Settings saved successfully');
        fetchData();
        setOpenSettingsDialog(false);
      }
    } catch (error) {
      showSnackbar('Error saving settings', 'error');
    }
  };

  const handleSaveScreen = async () => {
    try {
      const response = await whatsappApi.createScreen(screenForm);
      if (response.success) {
        showSnackbar('Screen created successfully');
        fetchData();
        setOpenScreenDialog(false);
      }
    } catch (error) {
      showSnackbar('Error saving screen', 'error');
    }
  };

  const handleUpdateBookingStatus = async (id, status) => {
    try {
      const response = await whatsappApi.updateBookingStatus(id, status);
      if (response.success) {
        showSnackbar('Booking status updated');
        fetchData();
      }
    } catch (error) {
      showSnackbar('Error updating booking', 'error');
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
        <Typography variant="h4" sx={{ fontWeight: 600, color: '#228B22' }}>WhatsApp Management</Typography>
        <Box display="flex" gap={1}>
          {tabValue === 0 && <Button variant="contained" startIcon={<SettingsIcon />} onClick={() => setOpenSettingsDialog(true)} sx={{ bgcolor: '#228B22' }}>Add Settings</Button>}
          {tabValue === 1 && <Button variant="contained" startIcon={<MessageIcon />} onClick={() => setOpenScreenDialog(true)} sx={{ bgcolor: '#228B22' }}>Add Screen</Button>}
        </Box>
      </Box>

      <Tabs value={tabValue} onChange={(e, v) => setTabValue(v)} sx={{ mb: 3 }}>
        <Tab label="Settings" />
        <Tab label="Screens" />
        <Tab label="Bookings" />
      </Tabs>

      {tabValue === 0 && (
        <Grid container spacing={3}>
          {settings.map((setting) => (
            <Grid item xs={12} sm={6} md={4} key={setting.settingId}>
              <Card sx={{ height: '100%', '&:hover': { boxShadow: 4 } }}>
                <CardContent>
                  <Box display="flex" justifyContent="space-between" alignItems="start" mb={2}>
                    <Typography variant="h6" sx={{ fontWeight: 600 }}>Branch: {setting.branchId}</Typography>
                    <Chip label={setting.isActive ? 'Active' : 'Inactive'} color={setting.isActive ? 'success' : 'default'} size="small" />
                  </Box>
                  <Typography variant="body2" color="text.secondary" mb={1}>Phone Number ID: {setting.phoneNumberId}</Typography>
                  <Typography variant="body2" color="text.secondary" mb={2}>API Base URL: {setting.apiBaseUrl}</Typography>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}

      {tabValue === 1 && (
        <Grid container spacing={3}>
          {screens.map((screen) => (
            <Grid item xs={12} sm={6} md={4} key={screen.screenId}>
              <Card sx={{ height: '100%', '&:hover': { boxShadow: 4 } }}>
                <CardContent>
                  <Box display="flex" justifyContent="space-between" alignItems="start" mb={2}>
                    <Typography variant="h6" sx={{ fontWeight: 600 }}>{screen.title || screen.code}</Typography>
                    <Chip label={screen.isActive ? 'Active' : 'Inactive'} color={screen.isActive ? 'success' : 'default'} size="small" />
                  </Box>
                  <Typography variant="body2" color="text.secondary" mb={1}>Code: {screen.code}</Typography>
                  <Typography variant="body2" color="text.secondary" mb={1}>Type: {screen.screenType}</Typography>
                  <Typography variant="body2" color="text.secondary" mb={2}>Language: {screen.languageCode}</Typography>
                  <Typography variant="body2" sx={{ mb: 2 }}>{screen.messageText.substring(0, 100)}...</Typography>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}

      {tabValue === 2 && (
        <Grid container spacing={3}>
          {bookings.map((booking) => (
            <Grid item xs={12} sm={6} md={4} key={booking.whatsAppBookingId}>
              <Card sx={{ height: '100%', '&:hover': { boxShadow: 4 } }}>
                <CardContent>
                  <Box display="flex" justifyContent="space-between" alignItems="start" mb={2}>
                    <Typography variant="h6" sx={{ fontWeight: 600 }}>From: {booking.fromNumber}</Typography>
                    <Chip label={booking.status} color={booking.status === 'LINKED' ? 'success' : 'warning'} size="small" />
                  </Box>
                  <Typography variant="body2" color="text.secondary" mb={1}>Message: {booking.messageText.substring(0, 50)}...</Typography>
                  <Typography variant="body2" color="text.secondary" mb={1}>Received: {new Date(booking.receivedAt).toLocaleString()}</Typography>
                  {booking.parsedPatientName && <Typography variant="body2" color="text.secondary" mb={2}>Patient: {booking.parsedPatientName}</Typography>}
                  <Box display="flex" gap={1} mt={2}>
                    <Button size="small" onClick={() => handleUpdateBookingStatus(booking.whatsAppBookingId, 'LINKED')} variant="outlined">Link</Button>
                    <Button size="small" onClick={() => handleUpdateBookingStatus(booking.whatsAppBookingId, 'IGNORED')} variant="outlined" color="error">Ignore</Button>
                  </Box>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}

      <Dialog open={openSettingsDialog} onClose={() => setOpenSettingsDialog(false)} maxWidth="md" fullWidth>
        <DialogTitle>WhatsApp Settings</DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={12} sm={6}><FormControl fullWidth><InputLabel>Branch</InputLabel><Select value={settingsForm.branchId} onChange={(e) => setSettingsForm({ ...settingsForm, branchId: e.target.value })} label="Branch">{branches.map(b => <MenuItem key={b.branchId} value={b.branchId}>{b.name}</MenuItem>)}</Select></FormControl></Grid>
            <Grid item xs={12} sm={6}><TextField fullWidth label="Phone Number ID *" value={settingsForm.phoneNumberId} onChange={(e) => setSettingsForm({ ...settingsForm, phoneNumberId: e.target.value })} /></Grid>
            <Grid item xs={12} sm={6}><TextField fullWidth label="Business Account ID" value={settingsForm.businessAccountId} onChange={(e) => setSettingsForm({ ...settingsForm, businessAccountId: e.target.value })} /></Grid>
            <Grid item xs={12} sm={6}><TextField fullWidth label="API Base URL *" value={settingsForm.apiBaseUrl} onChange={(e) => setSettingsForm({ ...settingsForm, apiBaseUrl: e.target.value })} /></Grid>
            <Grid item xs={12}><TextField fullWidth label="Access Token *" type="password" value={settingsForm.accessToken} onChange={(e) => setSettingsForm({ ...settingsForm, accessToken: e.target.value })} /></Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenSettingsDialog(false)}>Cancel</Button>
          <Button onClick={handleSaveSettings} variant="contained" sx={{ bgcolor: '#228B22' }}>Save</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={openScreenDialog} onClose={() => setOpenScreenDialog(false)} maxWidth="md" fullWidth>
        <DialogTitle>Add WhatsApp Screen</DialogTitle>
        <DialogContent>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={12} sm={6}><FormControl fullWidth><InputLabel>Branch</InputLabel><Select value={screenForm.branchId} onChange={(e) => setScreenForm({ ...screenForm, branchId: e.target.value })} label="Branch">{branches.map(b => <MenuItem key={b.branchId} value={b.branchId}>{b.name}</MenuItem>)}</Select></FormControl></Grid>
            <Grid item xs={12} sm={6}><TextField fullWidth label="Code *" value={screenForm.code} onChange={(e) => setScreenForm({ ...screenForm, code: e.target.value })} /></Grid>
            <Grid item xs={12} sm={6}><TextField fullWidth label="Language Code" value={screenForm.languageCode} onChange={(e) => setScreenForm({ ...screenForm, languageCode: e.target.value })} /></Grid>
            <Grid item xs={12} sm={6}><FormControl fullWidth><InputLabel>Screen Type</InputLabel><Select value={screenForm.screenType} onChange={(e) => setScreenForm({ ...screenForm, screenType: e.target.value })} label="Screen Type"><MenuItem value="MENU">Menu</MenuItem><MenuItem value="INFO">Info</MenuItem><MenuItem value="QUESTION">Question</MenuItem></Select></FormControl></Grid>
            <Grid item xs={12}><TextField fullWidth label="Title" value={screenForm.title} onChange={(e) => setScreenForm({ ...screenForm, title: e.target.value })} /></Grid>
            <Grid item xs={12}><TextField fullWidth label="Message Text *" multiline rows={4} value={screenForm.messageText} onChange={(e) => setScreenForm({ ...screenForm, messageText: e.target.value })} /></Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenScreenDialog(false)}>Cancel</Button>
          <Button onClick={handleSaveScreen} variant="contained" sx={{ bgcolor: '#228B22' }}>Save</Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={snackbar.open} autoHideDuration={6000} onClose={() => setSnackbar({ ...snackbar, open: false })}>
        <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
      </Snackbar>
    </Box>
  );
};

export default WhatsAppManagement;

