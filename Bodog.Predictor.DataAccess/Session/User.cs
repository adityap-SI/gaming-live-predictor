using Bodog.Predictor.Contracts.Common;
using Bodog.Predictor.Contracts.Session;
using Bodog.Predictor.Interfaces.Connection;
using Bodog.Predictor.Library.Utility;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Bodog.Predictor.DataAccess.Session
{
    public class User : Common.BaseDataAccess
    {

        public User(IPostgre postgre) : base(postgre)
        {
        }

        public UserDetails Login(Int32 optType, Int32 platformId, Int32 tourId, Int32 userId, String socialId, Int32 clientId, String fullName,
           String emailId, Int64 PhoneNo, String countryCode, String ProfilePicture, ref HTTPMeta httpMeta)
        {
            UserDetails details = new UserDetails();
            Int32 retVal = -50;
            String spName = String.Empty;
            NpgsqlTransaction transaction = null;

            spName = "cf_user_get";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    List<String> cursors = new List<String>() { "p_out_user_cur" };

                    using (NpgsqlCommand mNpgsqlCmd = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCmd.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_platformid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = platformId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_userid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = userId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_social_id", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = socialId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_clientid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = clientId;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_full_name", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = fullName;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_email_id", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = Encryption.AesEncrypt(emailId.Trim().ToLower()); ;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_cf_phone", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = Encryption.AesEncrypt(PhoneNo.ToString());
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_country_of_residence", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = countryCode;
                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_user_profile_pic", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = ProfilePicture;

                        mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_out_user_cur", NpgsqlDbType.Refcursor)).Value = cursors[0];
                        //mNpgsqlCmd.Parameters.Add(new NpgsqlParameter("p_out_user_gameday_cur", NpgsqlDbType.Refcursor)).Value = cursors[1];
                        if (connection.State != ConnectionState.Open) connection.Open();

                        transaction = connection.BeginTransaction();
                        mNpgsqlCmd.ExecuteNonQuery();

                        details = DataInitializer.Session.User.InitializeLogin(mNpgsqlCmd, cursors, ref retVal);

                        transaction.Commit();


                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);
                    }
                }
                catch (Exception ex)
                {
                    if (transaction != null)
                        transaction.Rollback();

                    throw ex;
                }
                finally
                {
                    if (transaction != null && transaction.IsCompleted == false)
                        transaction.Commit();

                    connection.Close();
                    connection.Dispose();
                }
            }

            return details;
        }

        public ResponseObject UserPhoneUpdate(Int32 optType, Int32 platformId, Int32 tourId, Int32 userId, Int32 clientId, Int64 phoneNumber,
                                               ref HTTPMeta httpMeta)
        {
            ResponseObject response = new ResponseObject();
            Int32 retVal = -50;
            String spName = String.Empty;

            spName = "cf_user_phone_upd";

            using (NpgsqlConnection connection = new NpgsqlConnection(_ConnectionString))
            {
                try
                {
                    using (NpgsqlCommand mNpgsqlCommand = new NpgsqlCommand(_Schema + spName, connection))
                    {
                        mNpgsqlCommand.CommandType = CommandType.StoredProcedure;

                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_opt_type", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = optType;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_platformid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = platformId;

                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_tourid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = tourId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_userid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = userId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_clientid", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Input }).Value = clientId;
                        mNpgsqlCommand.Parameters.Add(new NpgsqlParameter("p_cf_phoneno", NpgsqlDbType.Varchar) { Direction = ParameterDirection.Input }).Value = Encryption.AesEncrypt(phoneNumber.ToString());
                        NpgsqlParameter returnValue = new NpgsqlParameter("p_ret_type ", NpgsqlDbType.Numeric) { Direction = ParameterDirection.Output };
                        mNpgsqlCommand.Parameters.Add(returnValue);
                        mNpgsqlCommand.CommandTimeout = 0;

                        if (connection.State != ConnectionState.Open) connection.Open();

                        mNpgsqlCommand.ExecuteScalar();

                        Object value = returnValue.Value;

                        retVal = value != null && value.ToString().Trim() != "" ? Int32.Parse(value.ToString()) : retVal;

                        response.Value = retVal;
                        response.FeedTime = GenericFunctions.GetFeedTime();

                        GenericFunctions.AssetMeta(retVal, ref httpMeta, spName);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return response;
        }

    }
}
