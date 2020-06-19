using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KTVSys
{
    public class DBHelper
    {
        private static readonly string conStr = "Data Source = .; Initial Catalog = KTVSys;User Id = sa;pwd=1";
        private static SqlConnection con;

        public static SqlConnection Con
        {
            get {
                if (con == null)
                {
                    con = new SqlConnection(conStr);
                    con.Open();
                }
                else if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                return con; 
            }
        }

        public static int GetNonQuery(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, Con);
            int i = cmd.ExecuteNonQuery();
            Con.Close();
            return i;
        }

        public static object GetScalar(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, Con);
            object i = cmd.ExecuteScalar();
            Con.Close();
            return i;
        }

        public static SqlDataReader GetReader(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql,Con);
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public static DataSet GetSet(string sql)
        {
            SqlDataAdapter da = new SqlDataAdapter(sql, Con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }
    }
}
