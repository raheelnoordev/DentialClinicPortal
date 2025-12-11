# Material Receiving API Implementation

## Overview
Complete API implementation for the `material_receiving` database table with image upload support to `wwwroot/uploads/Receipts` folder (already exists).

## Files Created/Modified

### Created Files:
1. **Biomass.Server/Models/Dispatch/MaterialReceiving.cs** - Entity model
2. **Biomass.Server/Models/Dispatch/MaterialReceivingDto.cs** - DTOs and request classes
3. **Biomass.Server/Interfaces/IMaterialReceivingService.cs** - Service interface
4. **Biomass.Server/Services/MaterialReceivingService.cs** - Service implementation with image upload
5. **Biomass.Server/Controllers/Api/MaterialReceivingController.cs** - API controller

### Modified Files:
1. **Biomass.Server/Data/ApplicationDbContext.cs** - Added DbSet and entity configuration
2. **Biomass.Server/Program.cs** - Registered service

## API Endpoints

### 1. POST - Create Material Receiving Record
**URL:** `POST /api/material-receiving`  
**Content-Type:** `multipart/form-data`

**Request Body:**
```
- MaterialDispatchId (required, int)
- Gatepass (optional, string)
- FirstWeight (optional, decimal)
- SecondWeight (optional, decimal)
- NetWeight (optional, decimal)
- Deductions (optional, decimal)
- DispatchMund (optional, decimal)
- ReceivedMund (optional, decimal)
- SupplierMund (optional, decimal)
- CreatedBy (required, int)
- Status (optional, string, default: "Active")
- SlipPictureFile (optional, file: PDF/JPG/PNG, max 10MB)
- SlipPictureUrl (optional, string - if path already provided)
```

**Response:**
```json
{
  "result": 123,
  "message": "Material receiving record created successfully",
  "success": true
}
```

### 2. GET - Get All Material Receiving Records
**URL:** `GET /api/material-receiving`

### 3. GET - Get Material Receiving by ID
**URL:** `GET /api/material-receiving/{id}`

### 4. GET - Get Material Receiving by Dispatch ID
**URL:** `GET /api/material-receiving/by-dispatch/{dispatchId}`

## Image Upload Details

### Folder Path
- Images are saved to: `wwwroot/uploads/Receipts/` (folder already exists)
- The folder is automatically created if it doesn't exist

### File Validation
- **Allowed Extensions:** PDF, JPG, JPEG, PNG
- **Max File Size:** 10MB
- **Database Field:** `slip_picture` (char(500))

### File Naming Convention
- Format: `material_receipt_{timestamp}_{guid}.{extension}`
- Example: `material_receipt_20240115_103000_a1b2c3d4.pdf`

### Storage Path
- Full path: `{ContentRootPath}/wwwroot/uploads/Receipts/`
- Database stores: `/uploads/Receipts/{filename}`

## Testing

### 1. Test POST with Image Upload
```bash
curl -X POST "https://localhost:7000/api/material-receiving" \
  -F "MaterialDispatchId=1" \
  -F "CreatedBy=1" \
  -F "SlipPictureFile=@test_receipt.pdf"
```

### 2. Test GET All
```bash
curl -X GET "https://localhost:7000/api/material-receiving"
```

### 3. Test GET by ID
```bash
curl -X GET "https://localhost:7000/api/material-receiving/1"
```

### 4. Test GET by Dispatch ID
```bash
curl -X GET "https://localhost:7000/api/material-receiving/by-dispatch/1"
```

## Notes
- The service validates that the referenced MaterialDispatch exists before creating a record
- Image uploads are optional - you can create records without images
- The folder `wwwroot/uploads/Receipts/` already exists and will be created automatically if removed
- The image path is limited to 500 characters as per database schema
- All timestamps are stored in UTC

