using ICC.Predictor.Blanket.Common;
using ICC.Predictor.Contracts.Configuration;
using ICC.Predictor.Interfaces.Asset;
using ICC.Predictor.Interfaces.AWS;
using ICC.Predictor.Interfaces.Connection;
using ICC.Predictor.Interfaces.Session;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ICC.Predictor.Blanket.Analytics
{
    public class Analytics : BaseBlanket
    {
        private readonly DataAccess.Analytics.Analytics _DBContext;
        private readonly int _TourId;
        public Analytics(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
           : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Analytics.Analytics(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public DataSet AnalyticsData()
        {
            int optType = 1;
            DataSet ds = _DBContext.GetAnalytics(optType, _TourId);

            return ds;
        }

        public DataSet UserReportDetailsReportData()
        {
            int optType = 1;
            DataSet ds = _DBContext.GetUserDetailsReport(optType);

            return ds;
        }

        public string GetAnalytics(ref string error)
        {

            string vHtml = string.Empty;
            error += "";

            try
            {
                int optType = 1;

                DataSet mDSet = AnalyticsData();

                #region "Analytics Email Meta"

                vHtml = "<!DOCTYPE html><html> ";
                vHtml += "<head></head><body>";

                vHtml += "<div>";

                if (mDSet != null && mDSet.Tables.Count > 0)
                {
                    #region " TOTAL USERS COUNT "
                    if (mDSet.Tables[0] != null && mDSet.Tables[0].Rows.Count > 0)
                    {
                        vHtml += "<table style='border-collapse:collapse;text-align:center;line-height:25px;' border='1' width='100%'>";
                        vHtml += "<tr><th colspan='3'>Users</th></tr>";
                        vHtml += " <tr  bgcolor='#0B6DB1' color='#FFFFFF' style='color:#FFFFFF;'><th align='center'> Sr. no </th><th align='center'> Description </th><th align='center'> Count </th></tr>";
                        vHtml += "<tr>";
                        vHtml += "<td align='center'> 1 </td>";
                        vHtml += "<td>Total Users </td>";
                        vHtml += " <td>" + (Convert.IsDBNull(mDSet.Tables[0].Rows[0]["total_user"]) ? "" : mDSet.Tables[0].Rows[0]["total_user"].ToString()) + "</td>";
                        vHtml += "</tr>";
                        vHtml += "<tr>";
                        vHtml += "<td align='center'> 2 </td>";
                        vHtml += "<td>Registered Users </td>";
                        vHtml += " <td>" + (Convert.IsDBNull(mDSet.Tables[0].Rows[0]["register_user"]) ? "" : mDSet.Tables[0].Rows[0]["register_user"].ToString()) + "</td>";
                        vHtml += "</tr>";
                        vHtml += "<tr>";
                        vHtml += "<td align='center'> 3 </td>";
                        vHtml += "<td>Users who only predicted pre match questions </td>";
                        vHtml += " <td>" + (Convert.IsDBNull(mDSet.Tables[0].Rows[0]["us_pred_prm_count"]) ? "" : mDSet.Tables[0].Rows[0]["us_pred_prm_count"].ToString()) + "</td>";
                        vHtml += "</tr>";
                        vHtml += "<tr>";
                        vHtml += "<td align='center'> 4 </td>";
                        vHtml += "<td>Users who only played in-game questions </td>";
                        vHtml += " <td>" + (Convert.IsDBNull(mDSet.Tables[0].Rows[0]["us_pred_ing_count"]) ? "" : mDSet.Tables[0].Rows[0]["us_pred_ing_count"].ToString()) + "</td>";
                        vHtml += "</tr>";
                        vHtml += "<tr>";
                        vHtml += "<td align='center'> 5 </td>";
                        vHtml += "<td>Users who played both pre-match and in-game </td>";
                        vHtml += " <td>" + (Convert.IsDBNull(mDSet.Tables[0].Rows[0]["us_pred_prg_count"]) ? "" : mDSet.Tables[0].Rows[0]["us_pred_prg_count"].ToString()) + "</td>";
                        vHtml += "</tr>";
                        vHtml += "<tr>";
                        vHtml += "<td align='center'> 6 </td>";
                        vHtml += "<td>Active users </td>";
                        vHtml += " <td>" + (Convert.IsDBNull(mDSet.Tables[0].Rows[0]["us_pred_count"]) ? "" : mDSet.Tables[0].Rows[0]["us_pred_count"].ToString()) + "</td>";
                        vHtml += "</tr>";

                        vHtml += "</table>";
                    }
                    #endregion " TOTAL USERS COUNT "

                    #region " DAY WISE USER COUNT "
                    if (mDSet.Tables[1] != null && mDSet.Tables[1].Rows.Count > 0)
                    {

                        vHtml += "<br>";

                        vHtml += "<table style='border-collapse:collapse;text-align:center;line-height:25px;' border='1' width='100%'>";
                        vHtml += "<tr><th colspan='3'>Total User Day Wise</th></tr>";
                        vHtml += " <tr  bgcolor='#0B6DB1' color='#FFFFFF' style='color:#FFFFFF;'><th> Sr. no </th><th> Date </th><th> Registered Users Count </th></tr>";

                        int i = 1;

                        foreach (DataRow dataRow in mDSet.Tables[1].AsEnumerable())
                        {
                            vHtml += "<tr>";
                            vHtml += "<td>" + i + "</td>";
                            vHtml += "<td>" + (Convert.IsDBNull(dataRow["date1"]) ? "" : dataRow["date1"].ToString()) + "</td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["registered_users"]) ? "0" : dataRow["registered_users"].ToString()) + " </td>";
                            vHtml += "</tr> ";
                            i++;
                        }

                        vHtml += "</table>";
                    }
                    #endregion " DAY WISE USER COUNT "

                    //#region " MATCH DAY WISE COUNT "

                    //if (mDSet.Tables[2] != null && mDSet.Tables[2].Rows.Count > 0)
                    //{

                    //    vHtml += "<br>";

                    //    vHtml += "<table style='border-collapse:collapse;text-align:center;line-height:25px;' border='1' width='100%'>";
                    //    vHtml += "<tr><th colspan='5'> Match Day Wise Count </th></tr>";
                    //    vHtml += " <tr  bgcolor='#0B6DB1' color='#FFFFFF' style='color:#FFFFFF;'><th> Sr. no </th><th> Unique Users </th><th> Average Prediction Per Matchday </th><th> Matchdate </th><th> Total Prediction </th></tr>";

                    //    Int32 i = 1;

                    //    foreach (DataRow dataRow in mDSet.Tables[2].AsEnumerable())
                    //    {
                    //        vHtml += "<tr>";
                    //        vHtml += "<td>" + i + "</td>";
                    //        vHtml += "<td>" + (Convert.IsDBNull(dataRow["unique_users"]) ? "" : dataRow["unique_users"].ToString()) + "</td>";
                    //        vHtml += "<td> " + (Convert.IsDBNull(dataRow["avg_predict_per_matchday"]) ? "0" : dataRow["avg_predict_per_matchday"].ToString()) + " </td>";
                    //        vHtml += "<td> " + (Convert.IsDBNull(dataRow["matchdate"]) ? "0" : dataRow["matchdate"].ToString()) + " </td>";
                    //        vHtml += "<td> " + (Convert.IsDBNull(dataRow["total_predict_count"]) ? "0" : dataRow["total_predict_count"].ToString()) + " </td>";
                    //        vHtml += "</tr> ";
                    //        i++;
                    //    }

                    //    vHtml += "</table>";
                    //}

                    //#endregion " MATCH DAY WISE COUNT "

                    //#region " PHASE WISE COUNT "
                    //if (mDSet.Tables[3] != null && mDSet.Tables[3].Rows.Count > 0)
                    //{

                    //    vHtml += "<br>";
                    //    vHtml += "<table style='border-collapse:collapse;text-align:center;line-height:25px;' border='1' width='100%'>";
                    //    vHtml += "<tr><th colspan='5'> Phase Wise Count </th></tr>";
                    //    vHtml += " <tr  bgcolor='#0B6DB1' color='#FFFFFF' style='color:#FFFFFF;'><th> Sr. no </th><th> Unique Users </th><th> Average Prediction Per Week </th><th> WeekId </th><th> Total Prediction </th></tr>";

                    //    Int32 i = 1;
                    //    foreach (DataRow dataRow in mDSet.Tables[3].AsEnumerable())
                    //    {
                    //        vHtml += "<tr>";
                    //        vHtml += "<td>" + i + "</td>";
                    //        vHtml += "<td>" + (Convert.IsDBNull(dataRow["unique_users"]) ? "" : dataRow["unique_users"].ToString()) + "</td>";
                    //        vHtml += "<td> " + (Convert.IsDBNull(dataRow["avg_predict_per_week"]) ? "0" : dataRow["avg_predict_per_week"].ToString()) + " </td>";
                    //        vHtml += "<td> " + (Convert.IsDBNull(dataRow["weekid"]) ? "0" : "Week " + dataRow["weekid"].ToString()) + " </td>";
                    //        vHtml += "<td> " + (Convert.IsDBNull(dataRow["total_predict_count"]) ? "0" : dataRow["total_predict_count"].ToString()) + " </td>";
                    //        vHtml += "</tr> ";
                    //        i++;
                    //    }
                    //    vHtml += "</table>";
                    //}
                    //#endregion " PHASE WISE COUNT "

                    #region " MATCH WISE DATA "
                    if (mDSet.Tables[4] != null && mDSet.Tables[4].Rows.Count > 0)
                    {

                        vHtml += "<br>";

                        vHtml += "<table style='border-collapse:collapse;text-align:center;line-height:25px;' border='1' width='100%'>";
                        vHtml += "<tr><th colspan='7'> Matchwise Data </th></tr>";
                        vHtml += " <tr  bgcolor='#0B6DB1' color='#FFFFFF' style='color:#FFFFFF;'>" +
                            "<th> Sr. no </th>" +
                            "<th> Matches </th>" +
                            "<th> Questions Published </th>" +
                            "<th> Users with min 1 answer </ th>" +
                            "<th> Users who only attempted pre-match questions </ th>" +
                            "<th> Total answers </ th>" +
                            "<th> Avg. Questions Predicted </ th>" +
                            "</tr>";

                        int i = 1;

                        //List<DataRow> rows = mDSet.Tables[4].AsEnumerable().Reverse().Take(14).Reverse().ToList<DataRow>();
                        List<DataRow> rows = mDSet.Tables[4].AsEnumerable().Reverse().Take(14).Reverse().ToList();
                        //List<DataRow> rows = mDSet.Tables[4].AsEnumerable().ToList<DataRow>();/*.Reverse().Ta)ke(14)*/
                        foreach (DataRow dataRow in rows)
                        {
                            vHtml += "<tr>";
                            vHtml += "<td>" + i + "</td>";
                            vHtml += "<td>" + (Convert.IsDBNull(dataRow["home_team_name"]) && Convert.IsDBNull(dataRow["away_team_name"]) ? "" : dataRow["home_team_name"].ToString() + " Vs. " + dataRow["away_team_name"].ToString()) + "</td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["question_published_cnt"]) ? "0" : dataRow["question_published_cnt"].ToString()) + " </td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["user_predict_one_cnt"]) ? "0" : dataRow["user_predict_one_cnt"].ToString()) + " </td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["user_pre_match_pred_nt"]) ? "0" : dataRow["user_pre_match_pred_nt"].ToString()) + " </td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["user_total_prediciton"]) ? "0" : dataRow["user_total_prediciton"].ToString()) + " </td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["user_total_prediciton"]) && Convert.IsDBNull(dataRow["user_predict_one_cnt"]) ? "0" : (Convert.ToInt32(dataRow["user_total_prediciton"].ToString()) / Convert.ToInt32(dataRow["user_predict_one_cnt"].ToString())).ToString()) + " </td>";
                            vHtml += "</tr> ";
                            i++;
                        }
                        vHtml += "</table>";
                    }
                    #endregion " MATCH WISE DATA "

                    #region " WEEK WISE DATA "
                    if (mDSet.Tables[5] != null && mDSet.Tables[5].Rows.Count > 0)
                    {

                        vHtml += "<br>";

                        vHtml += "<table style='border-collapse:collapse;text-align:center;line-height:25px;' border='1' width='100%'>";
                        vHtml += "<tr><th colspan='6'> Phase Wise Data </th></tr>";
                        vHtml += " <tr  bgcolor='#0B6DB1' color='#FFFFFF' style='color:#FFFFFF;'>" +
                            "<th> Sr. no </th>" +
                            "<th> PhaseId </th>" +
                            "<th> Questions Published </th>" +
                            "<th> Users with min 1 answer </ th>" +
                            "<th> Users who only attempted pre-match questions </ th>" +
                            "<th> Total answers </ th>" +
                            "</tr>";

                        int i = 1;

                        //List<DataRow> rows = mDSet.Tables[4].AsEnumerable().Reverse().Take(14).Reverse().ToList<DataRow>();
                        List<DataRow> rows = mDSet.Tables[5].AsEnumerable().ToList();
                        //List<DataRow> rows = mDSet.Tables[4].AsEnumerable().ToList<DataRow>();/*.Reverse().Ta)ke(14)*/
                        foreach (DataRow dataRow in rows)
                        {
                            vHtml += "<tr>";
                            vHtml += "<td>" + i + "</td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["cf_phaseid"]) ? "0" : dataRow["cf_phaseid"].ToString()) + " </td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["question_published_cnt"]) ? "0" : dataRow["question_published_cnt"].ToString()) + " </td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["user_predict_one_cnt"]) ? "0" : dataRow["user_predict_one_cnt"].ToString()) + " </td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["user_pre_match_pred_nt"]) ? "0" : dataRow["user_pre_match_pred_nt"].ToString()) + " </td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["user_total_prediciton"]) ? "0" : dataRow["user_total_prediciton"].ToString()) + " </td>";
                            vHtml += "</tr> ";
                            i++;
                        }
                        vHtml += "</table>";
                    }
                    #endregion " WEEK WISE DATA "
                }

                vHtml += "</div> ";

                vHtml += "</body></html> ";

                #endregion " SI Statistics Email Meta"
            }
            catch (Exception ex)
            {
                error += "Blanket.Analytics.Analytics.GetAnalytics: " + ex.Message;
            }

            //vHtml += "<table style='border-collapse:collapse;text-align:center;line-height:25px;' border='1' width='100%'>";
            //vHtml += "<tr><th colspan='3'>Total Daily Fantasy Unique Teams Created in Last 7 days</th></tr>";
            //vHtml += " <tr  bgcolor='#0B6DB1' color='#FFFFFF' style='color:#FFFFFF;'><th> Sr. no </th><th> Date </th><th> Total Fantasy Teams Count </th></tr>";

            return vHtml;


        }

        public string GetUserDetailsReport(ref string error)
        {

            string vHtml = string.Empty;
            error += "";

            try
            {
                int optType = 1;

                DataSet mDSet = UserReportDetailsReportData();

                #region "SI USER DETAILS REPORT META"

                vHtml = "<!DOCTYPE html><html> ";
                vHtml += "<head></head><body>";

                vHtml += "<div>";

                if (mDSet != null && mDSet.Tables.Count > 0)
                {
                    #region " USER REPORT "
                    if (mDSet.Tables[0] != null && mDSet.Tables[0].Rows.Count > 0)
                    {

                        vHtml += "<br>";

                        vHtml += "<table style='border-collapse:collapse;text-align:center;line-height:25px;' border='1' width='100%'>";
                        vHtml += "<tr><th colspan='7'> Matchwise Data </th></tr>";
                        vHtml += " <tr  bgcolor='#0B6DB1' color='#FFFFFF' style='color:#FFFFFF;'>" +
                            "<th> Sr. no </th>" +
                            "<th> UserId </th>" +
                            "<th> User Name </th>" +
                            "<th> Created Date </ th>" +
                            "<th> Email Id </ th>" +
                            "<th> Social Id </ th>" +
                            "<th> Phone Number </ th>" +
                            "<th> Is Registered </ th>" +
                            "<th> Registered Date </ th>" +
                            "</tr>";

                        int i = 1;

                        //List<DataRow> rows = mDSet.Tables[4].AsEnumerable().Reverse().Take(14).Reverse().ToList<DataRow>();
                        //List<DataRow> rows = mDSet.Tables[0].AsEnumerable().Reverse().Take(14).Reverse().ToList<DataRow>();
                        //List<DataRow> rows = mDSet.Tables[4].AsEnumerable().ToList<DataRow>();/*.Reverse().Ta)ke(14)*/
                        foreach (DataRow dataRow in mDSet.Tables[0].Rows)
                        {
                            vHtml += "<tr>";
                            vHtml += "<td>" + i + "</td>";
                            vHtml += "<td>" + (Convert.IsDBNull(dataRow["cf_userid"]) ? "" : dataRow["cf_userid"].ToString() + "</td>");
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["user_name"]) ? "" : dataRow["user_name"].ToString()) + " </td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["created_date"]) ? "" : dataRow["created_date"].ToString()) + " </td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["email_id"]) ? "" : Library.Utility.Encryption.AesDecrypt(dataRow["email_id"].ToString())) + " </td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["social_id"]) ? "" : dataRow["social_id"].ToString()) + " </td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["cf_phoneno"]) ? "" : Library.Utility.Encryption.AesDecrypt(dataRow["cf_phoneno"].ToString())) + " </td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["is_registered"]) ? "" : dataRow["is_registered"].ToString()) + " </td>";
                            vHtml += "<td> " + (Convert.IsDBNull(dataRow["registered_date"]) ? "" : dataRow["registered_date"].ToString()) + " </td>";
                            vHtml += "</tr> ";
                            i++;
                        }
                        vHtml += "</table>";
                    }
                    #endregion " USER REPORT "
                }

                vHtml += "</div> ";

                vHtml += "</body></html> ";

                #endregion " SI USER DETAILS REPORT META"
            }
            catch (Exception ex)
            {
                error += "Blanket.Analytics.Analytics.GetUserDetailsReport: " + ex.Message;
            }

            //vHtml += "<table style='border-collapse:collapse;text-align:center;line-height:25px;' border='1' width='100%'>";
            //vHtml += "<tr><th colspan='3'>Total Daily Fantasy Unique Teams Created in Last 7 days</th></tr>";
            //vHtml += " <tr  bgcolor='#0B6DB1' color='#FFFFFF' style='color:#FFFFFF;'><th> Sr. no </th><th> Date </th><th> Total Fantasy Teams Count </th></tr>";

            return vHtml;


        }

    }
}
