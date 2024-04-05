using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Session;
using System;

namespace ICC.Predictor.Interfaces.Session
{
    public interface ICookies
    {
        bool _HasUserCookies { get; }
        UserCookie _GetUserCookies { get; }
        bool _HasGameCookies { get; }
        GameCookie _GetGameCookies { get; }

        bool SetUserCookies(UserCookie uc);

        //bool UpdateUserCookies(UserCookie values);
        bool SetGameCookies(GameCookie gc);

        //bool UpdateGameCookies(GameCookie values);
        HTTPLog PopulateLog(string FunctionName, string Message);
    }
}