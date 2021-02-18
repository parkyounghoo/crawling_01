using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seoulTrobl
{
    class Program
    {
        static string connectionString = "server = localhost; uid = sa; pwd = 1111; database = PrivateData;";
        static void Main(string[] args)
        {
            Console.WriteLine("서울시 장애인 현황(장애유형별) 통계");
            TroblType troblType = new TroblType();
            troblType.getTroblType();

            Console.WriteLine("서울시 장애인 현황(등급별/연령별) 통계");
            GradeAge gradeAge = new GradeAge();
            gradeAge.getGradeAge();
        }

        public static void insert(string query)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.CommandTimeout = 0; // timeout
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static DataSet selectDS(string query)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter();
                _SqlDataAdapter.SelectCommand = new SqlCommand(query, conn);
                _SqlDataAdapter.Fill(ds);

                return ds;
            }
        }
    }
}
