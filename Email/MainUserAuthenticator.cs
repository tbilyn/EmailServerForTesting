using SmtpServer.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using SmtpServer;
using System.Threading;
using System.Threading.Tasks;
using Data;
using System.Linq;

namespace Email
{
    public class MainUserAuthenticator : UserAuthenticator
    {
        private readonly MainDbContext _dbContext;

        public MainUserAuthenticator(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<bool> AuthenticateAsync(ISessionContext context, string user, string password, CancellationToken cancellationToken)
        {
            var account = _dbContext.Accounts.Where(x => x.Username == user && x.Password == password).SingleOrDefault();
            if (account == null)
                return Task.FromResult(false);

            context.Properties["AccountId"] = account.Id.ToString();
            
            return Task.FromResult(true);
        }
    }
}
