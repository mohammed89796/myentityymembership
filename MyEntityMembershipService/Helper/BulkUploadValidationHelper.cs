using MyEntityMembershipService.DTOs.Membership.BulckUploadDtos;
using MyEntityMembershipService.Repositories.Common;
namespace MyEntityMembershipService.Helper
{
    public static class BulkUploadValidationHelper
    {
        public static bool IsValidEgyptianNationalId(string nationalId)
        {
            if (string.IsNullOrWhiteSpace(nationalId) || nationalId.Length != 14 || !nationalId.All(char.IsDigit))
                return false;

            if (nationalId[0] != '2' && nationalId[0] != '3')
                return false;

            var year = int.Parse(nationalId.Substring(1, 2));
            var month = int.Parse(nationalId.Substring(3, 2));
            var day = int.Parse(nationalId.Substring(5, 2));

            year += (nationalId[0] == '2') ? 1900 : 2000;

            return DateTime.TryParse($"{year}-{month}-{day}", out _);
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email && email.Contains(".");
            }
            catch
            {
                return false;
            }
        }

        public static string NormalizeEgyptianPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return phone;

            phone = phone.Trim();

            phone = phone.Replace(" ", "");

            if (phone.StartsWith("+20"))
                phone = "0" + phone.Substring(3);
            else if (phone.StartsWith("0020"))
                phone = "0" + phone.Substring(4);

            if (phone.Length == 10 && phone.StartsWith("1"))
                phone = "0" + phone;

            return phone;
        }
        public static bool IsValidEgyptianPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return phone.Length == 11
                   && (phone.StartsWith("010") ||
                       phone.StartsWith("011") ||
                       phone.StartsWith("012") ||
                       phone.StartsWith("015"))
                   && phone.All(char.IsDigit);
        }

        public static Response<List<ExcelValidationError>> BuildErrorResponse(List<ExcelValidationError> errors)
        {
            var errorMessages = errors
                .Select(e => $"{(string.IsNullOrEmpty(e.Cell) ? "" : $"Cell {e.Cell}, ")}{e.Message}")
                .ToList();

            var fullErrorMessage = string.Join(" | ", errorMessages);

            return new Response<List<ExcelValidationError>>
            {
                Succeeded = false,
                Message = fullErrorMessage,
                Data = null
            };
        }
    }
}
