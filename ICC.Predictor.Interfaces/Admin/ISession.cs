using System;
using System.Collections.Generic;

namespace ICC.Predictor.Interfaces.Admin
{
    public interface ISession
    {
        List<string> Pages(string name = "");

        bool _HasAdminCookie { get; }

        bool SetAdminCookie(string value);

        string SlideAdminCookie();

        void DeleteAdminCookie();
    }
}