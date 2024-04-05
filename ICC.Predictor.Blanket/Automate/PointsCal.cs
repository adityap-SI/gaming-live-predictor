using ICC.Predictor.Contracts.Admin;
using ICC.Predictor.Contracts.Common;
using ICC.Predictor.Contracts.Feeds;
using ICC.Predictor.Library.Utility;
using ICC.Predictor.Blanket.Common;
using ICC.Predictor.Contracts.Automate;
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
using System.Threading.Tasks;

namespace ICC.Predictor.Blanket.Automate
{
    class PointsCal : BaseBlanket
    {
        private readonly DataAccess.Automate.PointsCal _DBContext;
        private readonly int _TourId;

        public PointsCal(IOptions<Application> appSettings, IAWS aws, IPostgre postgre, IRedis redis, ICookies cookies, IAsset asset)
            : base(appSettings, aws, postgre, redis, cookies, asset)
        {
            _DBContext = new DataAccess.Automate.PointsCal(postgre);
            _TourId = appSettings.Value.Properties.TourId;
        }

        public Matchdays Matchdays()
        {
            int optType = 1;

            Matchdays matchdays = new Matchdays();

            return _DBContext.Matchdays(optType, _TourId);
        }

        public int UserPointsProcess(int gamedayId, int matchday)
        {
            int retVal = new int();

            try
            {
                int optType = 1;

                retVal = _DBContext.UserPointsProcess(optType, _TourId, gamedayId, matchday);
            }
            catch (Exception ex)
            {
                throw new Exception("Engine.Automate.PointsCal.UserPointsProcess: " + ex.Message);
            }

            return retVal;
        }

        public DataSet UserPointsProcessReports(int processRetVal, int gamedayId, int matchday)
        {
            DataSet ds = new DataSet();

            try
            {
                int optType = 1;

                ds = _DBContext.UserPointsProcessReports(optType, processRetVal, _TourId, gamedayId, matchday);
            }
            catch (Exception ex)
            {
                throw new Exception("Engine.Automate.PointsCal.UserPointsProcessReports: " + ex.Message);
            }

            return ds;
        }

        public static StringBuilder ParseReports(DataSet ds)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                if (ds != null && ds.Tables != null)
                {
                    if (ds.Tables[0] != null && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];

                        //Open tag
                        sb.Append("<table border='1px' cellpadding='2' cellspacing='1' bgcolor='#e6eeff' style='font-family:Garamond; font-size:smaller;border-collapse: collapse'>");

                        //Column Header row
                        sb.Append("<tr >");
                        foreach (DataColumn myColumn in dt.Columns)
                        {
                            sb.Append("<td>");
                            sb.Append(myColumn.ColumnName);
                            sb.Append("</td>");
                        }
                        sb.Append("</tr>");


                        foreach (DataRow myRow in dt.Rows)
                        {
                            //Value rows
                            sb.Append("<tr>");
                            foreach (DataColumn myColumn in dt.Columns)
                            {
                                sb.Append("<td>");
                                sb.Append(myRow[myColumn.ColumnName].ToString());
                                sb.Append("</td>");
                            }
                            sb.Append("</tr>");
                        }

                        //Close tag
                        sb.Append("</table>");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Engine.Automate.PointsCal.ParseReports: " + ex.Message);
            }

            return sb;
        }

    }
}
