using SmtpServer.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using SmtpServer;
using SmtpServer.Protocol;
using System.Threading;
using System.Threading.Tasks;
using Data;
using Data.Models;
using SmtpServer.Mail;
using System.IO;

namespace Email
{
    public class MainMessageStore : MessageStore
    {
        private readonly MainDbContext _dbContext;

        public MainMessageStore(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, CancellationToken cancellationToken)
        {
            try
            {
                if (!context.Properties.TryGetValue("AccountId", out object accountIdObj))
                    return SmtpResponse.AuthenticationFailed;

                var accountId = Guid.Parse(accountIdObj.ToString());

                var textMessage = (ITextMessage)transaction.Message;                
                string messageContent = null;
                using (var reader = new StreamReader(textMessage.Content))
                {
                    messageContent = await reader.ReadToEndAsync();
                }

                var dbMessage = new Message()
                {
                    Id = Guid.NewGuid(),
                    AccountId = accountId,
                    Content = messageContent,
                    Received = DateTimeOffset.UtcNow
                };

                _dbContext.Messages.Add(dbMessage);

                await _dbContext.SaveChangesAsync();
                return SmtpResponse.Ok;
            }
            catch (Exception exc)
            {
                return SmtpResponse.TransactionFailed; //todo: ????????????
            }
        }
    }
}
