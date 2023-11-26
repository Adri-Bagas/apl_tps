using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;

namespace tps_apps.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public DashboardController(IConfiguration config, IWebHostEnvironment env)
        {
            this._config = config;
            this._env = env;
        }

        private string getConnection()
        {
            return _config["ConnectionStrings:mysql"];
        }

        [HttpPost]
        [Route("summary")]
        public async Task<IActionResult> Summary()
        {
            var message = "Success";
            var status_code = 200;
            var result = new object();
            var conn = new MySqlConnection(getConnection());
            try
            {
                var sql = "";
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                sql = $"SELECT COUNT(no_ktp) AS total_vote FROM  trx_vote WHERE is_enabled=1";
                var total_finish = await SqlMapper.ExecuteScalarAsync<int>(conn, sql, null, null, null, CommandType.Text);
                sql = "";
                sql = $"SELECT COUNT(no_ktp) AS total_pendukung FROM mst_pendaftar_tps WHERE is_enabled=1";
                var total_pendukung = await SqlMapper.ExecuteScalarAsync<int>(conn, sql, null, null, null, CommandType.Text);
                result = new 
                {
                    total_pendukung= total_pendukung,
                    total_finish=total_finish,
                    total_unfinish= total_pendukung - total_finish,
                };
                
            }
            catch (Exception ex)
            {
                status_code = 100;
                message = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                conn.Dispose();
            }
            return Ok(new { status_code, message, data = result });
        }
    }
}
