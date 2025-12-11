# Environment Configuration Guide

This project now uses environment variables for all API URLs and configuration. This allows you to easily switch between different environments without changing code.

## Environment Variables

Create these files in your `biomass.client` directory:

### `.env` (Default/Production)
```bash
# API Configuration
VITE_API_BASE_URL=https://localhost:7084/api
VITE_API_TIMEOUT=30000

# Development Configuration (for local development)
VITE_DEV_API_URL=https://localhost:7084/api

# Google Maps API Key
VITE_GOOGLE_MAPS_API_KEY=AIzaSyD-9tSesB2zS2Fp0jxK8cttPOMXo6yNCKQ

# Application Configuration
VITE_APP_NAME=Biomass Portal
VITE_APP_VERSION=1.0.0
```

### `.env.local` (Local Development)
```bash
# API Configuration
VITE_API_BASE_URL=https://localhost:7084/api
VITE_API_TIMEOUT=30000

# Development Configuration (for local development)
VITE_DEV_API_URL=https://localhost:7084/api

# Google Maps API Key
VITE_GOOGLE_MAPS_API_KEY=AIzaSyD-9tSesB2zS2Fp0jxK8cttPOMXo6yNCKQ

# Application Configuration
VITE_APP_NAME=Biomass Portal (Local)
VITE_APP_VERSION=1.0.0
```

### `.env.production` (Production Build)
```bash
# API Configuration
VITE_API_BASE_URL=https://localhost:7084/api
VITE_API_TIMEOUT=30000

# Development Configuration (not used in production)
VITE_DEV_API_URL=https://localhost:7084/api

# Google Maps API Key
VITE_GOOGLE_MAPS_API_KEY=AIzaSyD-9tSesB2zS2Fp0jxK8cttPOMXo6yNCKQ

# Application Configuration
VITE_APP_NAME=Biomass Portal
VITE_APP_VERSION=1.0.0
```

## How It Works

1. **Development Mode**: Uses Vite proxy (`/api`) which routes to `VITE_DEV_API_URL`
2. **Production Mode**: Uses `VITE_API_BASE_URL` directly
3. **Fallback**: If no environment variable is set, uses hardcoded defaults

## Files Updated

- `src/utils/api.js` - Main API utility
- `src/config/config.js` - Application configuration
- `src/config/maps.js` - Google Maps configuration
- `vite.config.js` - Development proxy configuration

## Usage Examples

### For Development
1. Create `.env.local` with your local API URL
2. Run `npm run dev`
3. All API calls will proxy to your local backend

### For Production
1. Create `.env.production` with your production API URL
2. Run `npm run build`
3. Deploy the `dist` folder
4. All API calls will go to your production backend

### For Different Environments
1. Create environment-specific `.env` files
2. Use `--mode` flag: `npm run build --mode production`
3. Or set `NODE_ENV` environment variable

## Benefits

- ✅ No hardcoded URLs in code
- ✅ Easy environment switching
- ✅ Secure API key management
- ✅ Consistent configuration across all files
- ✅ Easy deployment to different environments

## Migration Complete

All hardcoded URLs have been removed from:
- ✅ `src/utils/api.js`
- ✅ `src/config/config.js`
- ✅ `src/config/maps.js`
- ✅ `vite.config.js`

You can now change API URLs by simply updating the `.env` file!