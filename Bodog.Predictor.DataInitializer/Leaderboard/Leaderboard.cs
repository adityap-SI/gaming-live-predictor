using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Feeds;
using Bodog.Predictor.Library.Utility;
using Bodog.Predictor.Contracts.Leaderboard;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Bodog.Predictor.Contracts.Admin;

namespace Bodog.Predictor.DataInitializer.Leaderboard
{
    public class Leaderboard
    {
        public static ResponseObject InitializeTop(NpgsqlCommand mNpgsqlCmd, List<String> cursors, out Int64 retVal)
        {
            ResponseObject vLeaderboard = new ResponseObject();
            Top ranks = new Top();
            DataSet ds = null;
            retVal = -70;

            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            ranks.Users = (from a in ds.Tables[0].AsEnumerable()
                                           select new Users
                                           {
                                               UserTeamId = Convert.IsDBNull(a["cf_user_tour_teamid"]) ? "" : BareEncryption.BaseEncrypt(a["cf_user_tour_teamid"].ToString()),
                                               GUID = Convert.IsDBNull(a["user_guid"]) ? "" : a["user_guid"].ToString(),
                                               UserId = Convert.IsDBNull(a["cf_userid"]) ? "" : BareEncryption.BaseEncrypt(a["cf_userid"].ToString()),
                                               TeamName = Convert.IsDBNull(a["team_name"]) ? "" : a["team_name"].ToString(),
                                               FullName = Convert.IsDBNull(a["user_name"]) ? "" : a["user_name"].ToString(),
                                               RankNo = Convert.IsDBNull(a["cur_rno"]) ? "0" : a["cur_rno"].ToString(),
                                               Rank = Convert.IsDBNull(a["rank"]) ? "0" : a["rank"].ToString(),
                                               Trend = Convert.IsDBNull(a["trend"]) ? "" : a["trend"].ToString(),
                                               Points = Convert.IsDBNull(a["points"]) ? "0" : Math.Truncate(Convert.ToDouble(a["points"].ToString())).ToString(),
                                               CurrentGamedayPoints = Convert.IsDBNull(a["cur_gd_points"]) ? "0" : a["cur_gd_points"].ToString()
                                           }).ToList();
                        }

                        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        {
                            ranks.TotalMembers = Convert.IsDBNull(ds.Tables[1].Rows[0]["total_member"]) ? 0 : Int32.Parse(ds.Tables[1].Rows[0]["total_member"].ToString());
                            retVal = Convert.IsDBNull(ds.Tables[1].Rows[0]["ret_type"]) ? 0 : Convert.ToInt64(ds.Tables[1].Rows[0]["ret_type"]);
                        }

                        vLeaderboard.Value = ranks;
                        vLeaderboard.FeedTime = GenericFunctions.GetFeedTime();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return vLeaderboard;
        }


        public static ResponseObject InitializeUserRank(NpgsqlCommand mNpgsqlCmd, List<String> cursors)
        {
            ResponseObject res = new ResponseObject();
            List<Users> userRank = new List<Users>();
            DataSet ds = null;

            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            userRank = (from a in ds.Tables[0].AsEnumerable()
                                        select new Users
                                        {
                                            UserTeamId = Convert.IsDBNull(a["cf_user_tour_teamid"]) ? "" : BareEncryption.BaseEncrypt(a["cf_user_tour_teamid"].ToString()),
                                            GUID = Convert.IsDBNull(a["user_guid"]) ? "" : a["user_guid"].ToString(),
                                            UserId = Convert.IsDBNull(a["cf_userid"]) ? "" : BareEncryption.BaseEncrypt(a["cf_userid"].ToString()),
                                            TeamName = Convert.IsDBNull(a["team_name"]) ? "" : a["team_name"].ToString(),
                                            FullName = Convert.IsDBNull(a["user_name"]) ? "" : a["user_name"].ToString(),
                                            RankNo = Convert.IsDBNull(a["cur_rno"]) ? "-" : a["cur_rno"].ToString(),
                                            Rank = Convert.IsDBNull(a["rank"]) ? "-" : a["rank"].ToString(),
                                            Trend = Convert.IsDBNull(a["trend"]) ? "0" : a["trend"].ToString(),
                                            Points = Convert.IsDBNull(a["points"]) ? "-" : Math.Truncate(Convert.ToDouble(a["points"].ToString())).ToString(),
                                            CurrentGamedayPoints = Convert.IsDBNull(a["cur_gd_points"]) ? "-" : a["cur_gd_points"].ToString(),
                                            TotalMember = 0
                                        }).ToList();
                        }

                        res.Value = userRank;
                        res.FeedTime = GenericFunctions.GetFeedTime();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;
        }

        public static Reports InitializeAdminLeaderboard(NpgsqlCommand mNpgsqlCmd, List<String> cursors)
        {
            ResponseObject res = new ResponseObject();
            DataSet ds = null;
            Reports reports = new Reports();
            reports.LeaderBoardList = new List<AdminLeaderBoard>();
            try
            {
                ds = Common.Utility.GetDataSetFromCursor(mNpgsqlCmd, cursors);

                if (ds != null)
                {
                    if (ds.Tables != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                        {
                            reports.LeaderBoardList = (from a in ds.Tables[0].AsEnumerable()
                                                       select new AdminLeaderBoard
                                                       {
                                                           UserTeamId = Convert.IsDBNull(a["cf_user_tour_teamid"]) ? "" : a["cf_user_tour_teamid"].ToString(),
                                                           GUID = Convert.IsDBNull(a["user_guid"]) ? "" : a["user_guid"].ToString(),
                                                           UserId = Convert.IsDBNull(a["cf_userid"]) ? "" : a["cf_userid"].ToString(),
                                                           TeamName = Convert.IsDBNull(a["team_name"]) ? "" : a["team_name"].ToString(),
                                                           FullName = Convert.IsDBNull(a["user_name"]) ? "" : a["user_name"].ToString(),
                                                           RankNo = Convert.IsDBNull(a["cur_rno"]) ? "-" : a["cur_rno"].ToString(),
                                                           Rank = Convert.IsDBNull(a["rank"]) ? "-" : a["rank"].ToString(),
                                                           Trend = Convert.IsDBNull(a["trend"]) ? "0" : a["trend"].ToString(),
                                                           Points = Convert.IsDBNull(a["points"]) ? "-" : Math.Truncate(Convert.ToDouble(a["points"].ToString())).ToString(),
                                                           CurrentGamedayPoints = Convert.IsDBNull(a["cur_gd_points"]) ? "-" : a["cur_gd_points"].ToString(),
                                                           //PhoneNumber = Convert.IsDBNull(a["cf_phoneno"]) ? "-" : Encryption.AesDecrypt(a["cf_phoneno"].ToString()),
                                                           EmailId = Convert.IsDBNull(a["email_id"]) ? "-" : Encryption.AesDecrypt(a["email_id"].ToString()),
                                                           //SocialId = Convert.IsDBNull(a["social_id"]) ? "-" : a["social_id"].ToString(),
                                                           TotalMember = 0
                                                       }).ToList();

                            //DataTable dt = new DataTable();
                            //dt = ds.Tables[0];

                            //foreach (DataRow dr in ds.Tables[0].Rows)
                            //{
                            //    dr["cf_phoneno"] = Convert.IsDBNull(dr["cf_phoneno"]) ? "-" : Encryption.BaseDecrypt(dr["cf_phoneno"].ToString());
                            //    dr["emailid"] = Convert.IsDBNull(dr["emailid"]) ? "-" : Encryption.BaseDecrypt(dr["emailid"].ToString());
                            //}
                        }

                        if (ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
                        {

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return reports;
        }
    }
}
