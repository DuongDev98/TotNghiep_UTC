using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebApplication.Models;

namespace WebApplication.Utils
{
    public class DatabaseUtils
    {
        public static DataTable GetTable(string query, Dictionary<string, object> attrs = null)
        {
            DataTable dt = null;
            using (var context = new DOANEntities())
            {
                var conn = context.Database.Connection;
                var connectionState = conn.State;
                try
                {
                    if (connectionState != ConnectionState.Open) conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = query;
                        cmd.CommandType = CommandType.Text;
                        if (attrs != null && attrs.Count > 0)
                        {
                            foreach (string key in attrs.Keys)
                            {
                                cmd.Parameters.Add(new SqlParameter(key, attrs[key]));
                            }
                        }
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt = new DataTable();
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // error handling
                    throw;
                }
                finally
                {
                    if (connectionState != ConnectionState.Closed) conn.Close();
                }
            }
            return dt;
        }
    }
}