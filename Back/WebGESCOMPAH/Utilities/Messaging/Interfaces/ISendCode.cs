using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Messaging.Interfaces
{
    public interface ISendCode
    {
        Task SendRecoveryCodeEmail(string emailReceptor, string recoveryCode);
        Task SendTemporaryPasswordAsync(string email, string fullName, string tempPassword);
        Task SendContractWithPdfAsync(string email, string fullName, string contractNumber, byte[] pdfBytes);
    }
}