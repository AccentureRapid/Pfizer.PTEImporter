﻿using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pfizer.PTEImporter.Services
{
    public interface IEmailService : ITransientDependency
    {
        void SendEmail(string from,
            string recipients,
            string subject,
            string body,
            object userToken = null);

        void SendEmail(string from, string recipients, string subject, string body, string[] files);
        void SendEmail(string from, string[] recipients, string subject, string body, string[] files);

    }
}
