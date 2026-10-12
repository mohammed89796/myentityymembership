using ClosedXML.Excel;
using MyEntityMembershipService.DTOs.Entity;
using MyEntityMembershipService.DTOs.Membership.BulckUploadDtos;


public static class GenerateExcelTemplateExtension
{
    public static byte[] GenerateExcelTemplate(List<string> headers,List<ExcelDropdownDataDto> dropdowns,string mainSheetName = "Template",string dropdownSheetName = "Dropdowns",int maxRows = 100)
    {
        using var workbook = new XLWorkbook();
        var mainSheet = workbook.Worksheets.Add(mainSheetName);
        var dropdownSheet = workbook.Worksheets.Add(dropdownSheetName);

        // Add headers to main sheet
        for (int i = 0; i < headers.Count; i++)
            mainSheet.Cell(1, i + 1).Value = headers[i];

        // Write dropdown source data and define ranges
        foreach (var dropdown in dropdowns)
        {
            // Header
            dropdownSheet.Cell(1, dropdown.SourceStartColumn).Value = dropdown.SourceHeader;
            dropdownSheet.Cell(1, dropdown.SourceStartColumn + 1).Value = dropdown.SourceHeader + "Name";

            // Data
            for (int i = 0; i < dropdown.SourceData.Count; i++)
            {
                dropdownSheet.Cell(i + 2, dropdown.SourceStartColumn).Value = dropdown.SourceData[i].Key;
                dropdownSheet.Cell(i + 2, dropdown.SourceStartColumn + 1).Value = dropdown.SourceData[i].Value;
            }

            // Define named range
            var range = dropdownSheet.Range(
                dropdownSheet.Cell(2, dropdown.SourceStartColumn),
                dropdownSheet.Cell(dropdown.SourceData.Count + 1, dropdown.SourceStartColumn));
            range.AddToNamed(dropdown.RangeName, XLScope.Workbook);

            // Apply dropdown to main sheet
            for (int row = 2; row <= maxRows; row++)
            {
                var cell = mainSheet.Cell(row, dropdown.TargetColumnInMainSheet);
                var validation = cell.CreateDataValidation();
                validation.IgnoreBlanks = true;
                validation.InCellDropdown = true;
                validation.AllowedValues = XLAllowedValues.List;
                validation.List($"={dropdown.RangeName}");
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
    public static byte[] GenerateExcelTemplateWithdropdownNameReferance(
     List<string> headers,
     List<ExcelDropdownDataDto> dropdowns,
     List<int> textFormatColumns, 
     string mainSheetName = "Template",
     string dropdownSheetName = "Dropdowns",
     int maxRows = 100)
    {
        using var workbook = new XLWorkbook();
        var mainSheet = workbook.Worksheets.Add(mainSheetName);
        var dropdownSheet = workbook.Worksheets.Add(dropdownSheetName);

        // Add headers to main sheet
        for (int i = 0; i < headers.Count; i++)
            mainSheet.Cell(1, i + 1).Value = headers[i];

        // Write dropdown source data and define ranges
        foreach (var dropdown in dropdowns)
        {
            dropdownSheet.Cell(1, dropdown.SourceStartColumn).Value = dropdown.SourceHeader + "Id";
            dropdownSheet.Cell(1, dropdown.SourceStartColumn + 1).Value = dropdown.SourceHeader + "Name";

            for (int i = 0; i < dropdown.SourceData.Count; i++)
            {
                dropdownSheet.Cell(i + 2, dropdown.SourceStartColumn).Value = dropdown.SourceData[i].Key;
                dropdownSheet.Cell(i + 2, dropdown.SourceStartColumn + 1).Value = dropdown.SourceData[i].Value;
            }

            var nameRange = dropdownSheet.Range(
                dropdownSheet.Cell(2, dropdown.SourceStartColumn + 1),
                dropdownSheet.Cell(dropdown.SourceData.Count + 1, dropdown.SourceStartColumn + 1));

            nameRange.AddToNamed(dropdown.RangeName, XLScope.Workbook);

            for (int row = 2; row <= maxRows + 1; row++)
            {
                var cell = mainSheet.Cell(row, dropdown.TargetColumnInMainSheet);
                var validation = cell.CreateDataValidation();
                validation.IgnoreBlanks = true;
                validation.InCellDropdown = true;
                validation.AllowedValues = XLAllowedValues.List;
                validation.List($"={dropdown.RangeName}");
            }
        }

        foreach (var colIndex in textFormatColumns)
        {
            for (int row = 2; row <= maxRows + 1; row++)
            {
                var cell = mainSheet.Cell(row, colIndex);
                cell.Style.NumberFormat.Format = "@"; // Text format
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }


    public static List<ExcelValidationError> ValidateTemplateAsync(this IFormFile file,List<string> expectedHeaders,List<ExcelDropdownDataDto> expectedDropdowns,int maxRows = 1000,string mainSheetName = "Template")
    {
        var errors = new List<ExcelValidationError>();

        if (file == null || file.Length == 0)
        {
            errors.Add(new ExcelValidationError { Cell = "File", Message = "Uploaded file is empty." });
            return errors;
        }

        using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);
        var sheet = workbook.Worksheet(mainSheetName);

        if (sheet == null)
        {
            errors.Add(new ExcelValidationError { Cell = "Sheet", Message = $"Worksheet '{mainSheetName}' not found." });
            return errors;
        }

        for (int i = 0; i < expectedHeaders.Count; i++)
        {
            var headerCell = sheet.Cell(1, i + 1);
            if (headerCell.Value.ToString()?.Trim() != expectedHeaders[i])
            {
                errors.Add(new ExcelValidationError
                {
                    Cell = headerCell.Address.ToString(),
                    Message = $"Expected header '{expectedHeaders[i]}', but found '{headerCell.Value}'."
                });
            }
        }

        int rowCount = sheet.RowsUsed().Count() - 1;
        if (rowCount == 0)
            errors.Add(new ExcelValidationError { Cell = "A2", Message = "Template contains no data rows." });
        if (rowCount > maxRows)
            errors.Add(new ExcelValidationError { Cell = $"A{maxRows + 2}", Message = $"Too many rows. Max allowed is {maxRows}." });

        foreach (var row in sheet.RowsUsed().Skip(1).Take(maxRows))
        {
            foreach (var dropdown in expectedDropdowns)
            {
                var cell = row.Cell(dropdown.TargetColumnInMainSheet);
                var value = cell.GetString()?.Trim();
                var validValues = dropdown.SourceData.Select(x => x.Key.ToString()).ToList();
                if (!string.IsNullOrEmpty(value) && !validValues.Contains(value))
                {
                    errors.Add(new ExcelValidationError
                    {
                        Cell = cell.Address.ToString(),
                        Message = $"Invalid value '{value}' for {dropdown.SourceHeader}."
                    });
                }
            }
        }

        return errors;
    }
    public static List<ExcelValidationError> ValidateTemplateByValueAsync(
        this IFormFile file,
        List<string> expectedHeaders,
        List<ExcelDropdownDataDto> expectedDropdowns,
        int maxRows = 50,
        string mainSheetName = "Template")
    {
        var errors = new List<ExcelValidationError>();

        if (file == null || file.Length == 0)
        {
            errors.Add(new ExcelValidationError { Cell = "File", Message = "Uploaded file is empty." });
            return errors;
        }

        using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);
        var sheet = workbook.Worksheet(mainSheetName);

        if (sheet == null)
        {
            errors.Add(new ExcelValidationError { Cell = "Sheet", Message = $"Worksheet '{mainSheetName}' not found." });
            return errors;
        }

        // Validate headers
        for (int i = 0; i < expectedHeaders.Count; i++)
        {
            var headerCell = sheet.Cell(1, i + 1);
            if (headerCell.Value.ToString()?.Trim() != expectedHeaders[i])
            {
                errors.Add(new ExcelValidationError
                {
                    Cell = headerCell.Address.ToString(),
                    Message = $"Expected header '{expectedHeaders[i]}', but found '{headerCell.Value}'."
                });
            }
        }

        int rowCount = sheet.RowsUsed().Count() - 1;
        if (rowCount == 0)
            errors.Add(new ExcelValidationError { Cell = "A2", Message = "Template contains no data rows." });
        if (rowCount > maxRows)
            errors.Add(new ExcelValidationError { Cell = $"A{maxRows + 2}", Message = $"Too many rows. Max allowed is {maxRows}." });

        var uniqueTrackers = new Dictionary<string, HashSet<string>>()
        {
            ["File Number"] = new HashSet<string>(),
            ["Email Address"] = new HashSet<string>(),
            ["National ID (Number)"] = new HashSet<string>(),
            ["Passport Number"] = new HashSet<string>()
        };

        var dateFields = new List<string>
    {
        "Date of Birth", "Date of Issuance", "Date of Acquisition"
    };

        foreach (var row in sheet.RowsUsed().Skip(1).Take(maxRows))
        {
            for (int colIndex = 1; colIndex <= expectedHeaders.Count; colIndex++)
            {
                var cell = row.Cell(colIndex);
                var header = expectedHeaders[colIndex - 1];
                var value = cell.GetString()?.Trim();

                // Unique check
                if (uniqueTrackers.ContainsKey(header))
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        if (!uniqueTrackers[header].Add(value))
                        {
                            errors.Add(new ExcelValidationError
                            {
                                Cell = cell.Address.ToString(),
                                Message = $"Duplicate value '{value}' found for column '{header}'."
                            });
                        }
                    }
                }

                // Date format validation
                if (dateFields.Contains(header))
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        if (!DateTime.TryParseExact(value, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out _))
                        {
                            errors.Add(new ExcelValidationError
                            {
                                Cell = cell.Address.ToString(),
                                Message = $"Invalid date format '{value}' for column '{header}'. Expected format is yyyy/MM/dd."
                            });
                        }
                    }

                }
                // Email format validation
                if (header == "Email Address" && !string.IsNullOrWhiteSpace(value))
                {
                    var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                    if (!emailRegex.IsMatch(value))
                    {
                        errors.Add(new ExcelValidationError
                        {
                            Cell = cell.Address.ToString(),
                            Message = $"Invalid email format: '{value}'"
                        });
                    }
                }

                // Phone Number format validation
                if (header == "Phone Number" && !string.IsNullOrWhiteSpace(value))
                {
                    var phoneRegex = new System.Text.RegularExpressions.Regex(@"^\d{10,15}$");
                    if (!phoneRegex.IsMatch(value))
                    {
                        errors.Add(new ExcelValidationError
                        {
                            Cell = cell.Address.ToString(),
                            Message = $"Invalid phone number format: '{value}'. It should contain 10 to 15 digits."
                        });
                    }
                }

                // National ID format validation
                if (header == "National ID (Number)" && !string.IsNullOrWhiteSpace(value))
                {
                    var nationalIdRegex = new System.Text.RegularExpressions.Regex(@"^\d{14}$");
                    if (!nationalIdRegex.IsMatch(value))
                    {
                        errors.Add(new ExcelValidationError
                        {
                            Cell = cell.Address.ToString(),
                            Message = $"Invalid National ID: '{value}'. It should contain exactly 14 digits."
                        });
                    }
                }


            }

            // Dropdown validation
            foreach (var dropdown in expectedDropdowns)
            {
                var dropdownCell = row.Cell(dropdown.TargetColumnInMainSheet);
                var dropdownValue = dropdownCell.GetString()?.Trim();
                var validValues = dropdown.SourceData.Select(x => x.Value.Trim()).ToList();

                if (!string.IsNullOrEmpty(dropdownValue) && !validValues.Contains(dropdownValue))
                {
                    errors.Add(new ExcelValidationError
                    {
                        Cell = dropdownCell.Address.ToString(),
                        Message = $"Invalid value '{dropdownValue}' for {dropdown.SourceHeader}."
                    });
                }
            }
        }

        return errors;
    }


    public static byte[] GenerateExcelWithoutDropdownTemplate(List<string> headers, string mainSheetName = "Template", int maxRows = 100)
    {
        using var workbook = new XLWorkbook();
        var mainSheet = workbook.Worksheets.Add(mainSheetName);

        for (int i = 0; i < headers.Count; i++)
            mainSheet.Cell(1, i + 1).Value = headers[i];

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public static byte[] GenerateExcelWithLookupsTemplate(
    List<string> headers,
    List<LookupSheetDto> lookups,          // each lookup = one named table in Lists sheet
    string mainSheetName = "Template")
    {
        using var workbook = new XLWorkbook();
        var mainSheet = workbook.Worksheets.Add(mainSheetName);
        var listsSheet = workbook.Worksheets.Add("Lists");
        listsSheet.Hide(); // optional: hide from end-users

        // ── Write headers on main sheet ──────────────────────────────────────
        for (int i = 0; i < headers.Count; i++)
        {
            var cell = mainSheet.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
        }

        int listCol = 1; // current column in Lists sheet

        foreach (var lookup in lookups)
        {
            // ── Write lookup table into Lists sheet ──────────────────────────
            listsSheet.Cell(1, listCol).Value = "ID";
            listsSheet.Cell(1, listCol + 1).Value = lookup.DisplayName;

            for (int r = 0; r < lookup.Items.Count; r++)
            {
                listsSheet.Cell(r + 2, listCol).Value = lookup.Items[r].Id;
                listsSheet.Cell(r + 2, listCol + 1).Value = lookup.Items[r].Label;
            }

            // ── Create a named range for the combined "ID – Label" column ───
            // We build a helper column (col+2) with concatenated display text
            int displayCol = listCol + 2;
            var displayValues = lookup.Items
                .Select(i => $"{i.Id} – {i.Label}")
                .ToList();

            for (int r = 0; r < displayValues.Count; r++)
                listsSheet.Cell(r + 2, displayCol).Value = displayValues[r];

            // Named range pointing to the display column
            string rangeAddress =
                $"Lists!${GetColumnLetter(displayCol)}$2:${GetColumnLetter(displayCol)}${displayValues.Count + 1}";
            workbook.NamedRanges.Add(lookup.RangeName, rangeAddress);

            // ── Apply dropdown validation on the target column in main sheet ─
            var targetCol = lookup.TargetColumnInMainSheet;
            var validationRange = mainSheet.Range(
                mainSheet.Cell(2, targetCol),
                mainSheet.Cell(200, targetCol));   // up to row 200

            var validation = validationRange.CreateDataValidation();
            validation.List(workbook.NamedRanges.NamedRange(lookup.RangeName).Ranges.First(), true);
            validation.InputMessage = $"Select a {lookup.DisplayName}";
            validation.InputTitle = lookup.DisplayName;
            validation.ShowInputMessage = true;
            validation.ErrorTitle = "Invalid value";
            validation.ErrorMessage = $"Please choose a value from the {lookup.DisplayName} list.";
            validation.ShowErrorMessage = true;

            listCol += 3; // advance 3 columns in Lists sheet (ID | Label | Display)
        }

        listsSheet.Columns().AdjustToContents();
        mainSheet.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    private static string GetColumnLetter(int col)
    {
        string result = "";
        while (col > 0)
        {
            col--;
            result = (char)('A' + col % 26) + result;
            col /= 26;
        }
        return result;
    }



}
