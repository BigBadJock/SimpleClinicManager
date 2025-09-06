# Print Statistics Feature

This document describes the printable statistics feature implemented for the Simple Clinic Manager.

## Overview

The Statistics page now includes a **Print Report** button that generates a clean, print-friendly version of all statistics data. This allows clinicians and administrators to easily produce paper reports or save as PDF for offline use.

## Features

### 1. Print Button
- Located in the top-right corner of the Statistics page header
- Enabled only when statistics data is loaded
- Uses a printer icon with "Print Report" text
- Triggers the browser's native print dialog

### 2. Print-Friendly Layout
- **Hidden Elements**: Navigation menu, sidebar, filter controls, and other UI elements
- **Clean Styling**: White background with black/grey text optimized for printing
- **Responsive Design**: Content scales appropriately for A4/Letter paper sizes

### 3. Report Header
- **Clinic Name**: "Simple Clinic Manager"
- **Report Title**: "Statistics Report"
- **Generation Date**: Current date and time when print is initiated

### 4. Applied Filters Section
- Shows the date range filter if applied (start date to end date)
- Displays "All available data" if no date filters are set
- Includes report generation timestamp

### 5. Statistics Content
- All statistics cards (Total Patients, Average Times, etc.)
- Charts and tables with print-optimized styling
- Treatment types, counsellor metrics, demographics
- Operational metrics and referral trends

## Technical Implementation

### Files Modified/Added:

1. **`ClinicTracking.Client/wwwroot/print.css`** (NEW)
   - Comprehensive print media queries
   - Hides unnecessary UI elements
   - Optimizes layout for paper printing

2. **`ClinicTracking.Client/Components/App.razor`**
   - Added print.css reference with media="print"
   - Added printPage() JavaScript function

3. **`ClinicTracking.Client/Components/Pages/Statistics.razor`**
   - Added Print Report button
   - Added print header and filters sections
   - Added PrintReport() method

4. **`ClinicTracking.Client/Program.cs`**
   - Updated API base URLs to use HTTP for development

### CSS Classes:

- `.no-print` - Elements hidden during print
- `.print-header` - Print-only header section
- `.print-filters` - Print-only applied filters section

### JavaScript:

```javascript
window.printPage = function() {
    setTimeout(() => {
        window.print();
    }, 500);
};
```

## Usage

1. Navigate to the Statistics page
2. Apply any desired date filters (optional)
3. Click the **Print Report** button
4. The browser print dialog will open
5. Choose to print to paper or save as PDF

## Browser Compatibility

The print functionality works with all modern browsers that support:
- CSS `@media print` queries
- JavaScript `window.print()` method
- Standard HTML5 and CSS3 features

## Print Layout Features

- Page breaks are avoided within important sections
- Tables and charts are kept together when possible
- Typography is optimized for readability on paper
- Colors are converted to grayscale for print
- Proper margins and spacing for standard paper sizes

## Future Enhancements

Potential improvements could include:
- Server-side PDF generation for higher fidelity
- Custom page layouts and formatting options
- Scheduled report generation and email delivery
- Additional export formats (Word, Excel, etc.)