using Bodog.Predictor.Contracts.Session;
using Bodog.Predictor.Library.Utility;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Bodog.Predictor.DataInitializer.Session
{
    public class User
    {
        public static UserDetails InitializeLogin(NpgsqlCommand mNpgsqlCmd, List<String> cursors, ref Int32 vRet)
        {
            UserDetails details = new UserDetails();
            DataSet ds = null;
            vRet = 0;
            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            vRet = Convert.ToInt32(ds.Tables[0].Rows[0]["ret_type"].ToString());
                            if (vRet == 1)
                            {
                                details.User = (from a in ds.Tables[0].AsEnumerable()
                                                select new UserCookie
                                                {
                                                    UserId = Convert.IsDBNull(a["cf_userid"]) ? "" : a["cf_userid"].ToString(),
                                                    SocialId = Convert.IsDBNull(a["social_id"]) ? "" : a["social_id"].ToString(),
                                                    EmailId = Convert.IsDBNull(a["email_id"]) ? "" : Encryption.AesDecrypt(a["email_id"].ToString()),
                                                    PhoneNo = Convert.IsDBNull(a["phone_no"]) ? "" : Encryption.AesDecrypt(a["phone_no"].ToString()),
                                                    //SocialId = Convert.IsDBNull(a["social_id"]) ? "" : a["social_id"].ToString()
                                                }).FirstOrDefault();

                                details.Game = (from a in ds.Tables[0].AsEnumerable()
                                                select new GameCookie
                                                {
                                                    FullName = Convert.IsDBNull(a["full_name"]) ? "" : a["full_name"].ToString(),
                                                    GUID = Convert.IsDBNull(a["user_guid"]) ? "" : a["user_guid"].ToString(),
                                                    TeamId = Convert.IsDBNull(a["cf_user_tour_teamid"]) ? "" : BareEncryption.BaseEncrypt(a["cf_user_tour_teamid"].ToString()),
                                                    TeamName = Convert.IsDBNull(a["team_name"]) ? "" : a["team_name"].ToString(),
                                                    CountryId = Convert.IsDBNull(a["cf_countryid"]) ? "" : a["cf_countryid"].ToString(),
                                                    CountryName = Convert.IsDBNull(a["country_name"]) ? "" : a["country_name"].ToString(),
                                                    FavTeamId = Convert.IsDBNull(a["fv_teamid"]) ? "0" : a["fv_teamid"].ToString(),
                                                    FavTeamName = Convert.IsDBNull(a["favteam_name"]) ? "0" : a["favteam_name"].ToString(),
                                                    CurrGamedayId = Convert.IsDBNull(a["cr_tour_gamedayid"]) ? "0" : a["cr_tour_gamedayid"].ToString(),
                                                    IsTourActive = Convert.IsDBNull(a["is_tour_active"]) ? "0" : a["is_tour_active"].ToString(),
                                                    IsRegistered = Convert.IsDBNull(a["is_registered"]) ? "0" : a["is_registered"].ToString(),
                                                }).FirstOrDefault();
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return details;
        }
    }
}
