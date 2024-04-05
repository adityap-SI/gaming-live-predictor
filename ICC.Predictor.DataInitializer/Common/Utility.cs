using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ICC.Predictor.DataInitializer.Common
{
    public class Utility
    {
        public static DataSet GetDataSetFromCursor(NpgsqlCommand mNpgsqlCmd, List<string> cursors)
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();

            foreach (string cursor in cursors)
            {
                mNpgsqlCmd.CommandText = "fetch all in \"" + cursor + "\"";
                mNpgsqlCmd.CommandType = CommandType.Text;

                da.SelectCommand = mNpgsqlCmd;

                DataTable dt = new DataTable(cursor);
                da.Fill(dt);
                ds.Tables.Add(dt);
            }

            da.Dispose();
            return ds;
        }

        public static int[] GetPagePoints(int pageOneChunk, int pageChunk, int pageNo)
        {
            int[] address = new int[2];

            int mPageOneSize = pageOneChunk;
            int mCurrPageSize = pageChunk;
            int mPageNo = pageNo;

            int mFrom = 0;
            int mTo = 0;

            mTo = mPageOneSize + (mPageNo - 1) * mCurrPageSize;
            if (mPageNo == 1)
                mFrom = mTo - mPageOneSize;
            else
                mFrom = mTo - mCurrPageSize;

            mFrom = mFrom + 1;

            address[0] = mFrom;
            address[1] = mTo;

            return address;
        }

        public static string MemberNotation(int count)
        {
            string notation = count.ToString();

            if (count > 9 && count < 100)
                notation = "9+";
            else if (count > 99 && count < 1000)
                notation = "99+";
            else if (count > 999 && count < 10000)
                notation = "1k+";
            else if (count > 9999 && count < 100000)
                notation = "10k+";
            else if (count > 99999)
                notation = "100k+";

            return notation;
        }
    }
}
