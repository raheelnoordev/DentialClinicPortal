// Application Configuration
// This file now uses environment variables for all URLs

export const config = {
  // API Configuration
  api: {
    // Base URL for API calls - must be set in environment variable
    baseUrl: import.meta.env.VITE_API_BASE_URL,

    // Timeout for API requests (in milliseconds)
    timeout: import.meta.env.VITE_API_TIMEOUT || 30000,

    // Retry configuration
    retry: {
      attempts: 3,
      delay: 1000,
    },
  },

  // Application Configuration
  app: {
    name: import.meta.env.VITE_APP_NAME || "DentialClinic Portal",
    version: import.meta.env.VITE_APP_VERSION || "1.0.0",
    environment: import.meta.env.MODE || "development",
  },
};

// Helper function to get API base URL
export const getApiBaseUrl = () => {
  // Always use environment variable if available (for live API)
  if (import.meta.env.VITE_API_BASE_URL) {
    return import.meta.env.VITE_API_BASE_URL;
  }

  // Fallback to live API if no environment variable
  return "https://localhost:7084/api";
};

// Export default config
export default config;

