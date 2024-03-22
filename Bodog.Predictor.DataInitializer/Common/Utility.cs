using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Bodog.Predictor.DataInitializer.Common
{
    public class Utility
    {
        public static DataSet GetDataSetFromCursor(NpgsqlCommand mNpgsqlCmd, List<String> cursors)
        {
            DataSet ds = new DataSet();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter();

            foreach (String cursor in cursors)
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

        public static Int32[] GetPagePoints(Int32 pageOneChunk, Int32 pageChunk, Int32 pageNo)
        {
            Int32[] address = new Int32[2];

            Int32 mPageOneSize = pageOneChunk;
            Int32 mCurrPageSize = pageChunk;
            Int32 mPageNo = pageNo;

            Int32 mFrom = 0;
            Int32 mTo = 0;

            mTo = mPageOneSize + ((mPageNo - 1) * mCurrPageSize);
            if (mPageNo == 1)
                mFrom = mTo - mPageOneSize;
            else
                mFrom = mTo - mCurrPageSize;

            mFrom = mFrom + 1;

            address[0] = mFrom;
            address[1] = mTo;

            return address;
        }

        public static String MemberNotation(Int32 count)
        {
            String notation = count.ToString();

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
