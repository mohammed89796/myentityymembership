using MyEntityMembershipService.DTOs.Entity.TenantDtos;
using System.Text;

namespace MyEntityMembershipService.Helper
{
    public static class ReachOutEmailTemplateBuilder
    {
        public static string BuildReachOutEmailBody(ReachOutToTenant model, string userName, string userEmail)
        {
            var sb = new StringBuilder();

            sb.Append(@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {
            font-family: Arial, sans-serif;
            line-height: 1.6;
        }
        .field {
            margin-bottom: 10px;
        }
        .label {
            font-weight: bold;
        }
        .attachment {
            margin-top: 15px;
            padding: 10px;
            background-color: #f5f5f5;
            border: 1px solid #ddd;
        }
    </style>
</head>
<body>

<h2>Tenant Communication Details</h2>
");

            AppendIfNotEmpty(sb, "Compliant", model.Compliant);
            AppendIfNotEmpty(sb, "Specific Selection", model.SpecificSelection);
            AppendIfNotEmpty(sb, "Transaction ID", model.TransactionId);
            AppendIfNotEmpty(sb, "Operation Type", model.OperationType);

            AppendIfNotEmpty(sb, "User Name", userName);
            AppendIfNotEmpty(sb, "User Email", userEmail);


            if (model.TransactionDateTime.HasValue)
            {
                AppendIfNotEmpty(
                    sb,
                    "Transaction Date",
                    model.TransactionDateTime.Value.ToString("f")
                );
            }

            if (model.Amount.HasValue)
            {
                AppendIfNotEmpty(
                    sb,
                    "Amount",
                    model.Amount.Value.ToString("C")
                );
            }

            AppendIfNotEmpty(sb, "Request Type", model.RequestType);
            AppendIfNotEmpty(sb, "Message", model.Message);



            sb.Append(@"
</body>
</html>
");

            return sb.ToString();
        }

        private static void AppendIfNotEmpty(StringBuilder sb, string label, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            sb.Append($@"
<div class='field'>
    <span class='label'>{label}:</span>
    <span>{System.Net.WebUtility.HtmlEncode(value)}</span>
</div>
");
        }
    }
}
