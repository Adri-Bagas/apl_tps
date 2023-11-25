using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;
using System.Text.RegularExpressions;
using tps_apps.model;
using tps_apps.service;

namespace tps_apps.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RefferenceController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public RefferenceController(IConfiguration config, IWebHostEnvironment env)
        {
            this._config = config;
            this._env = env;
        }

        private string getConnection()
        {
            return _config["ConnectionStrings:mysql"];
        }

        [HttpPost]
        [Route("get-list-user")]
        public async Task<IActionResult> GetListUser(int level)
        {
            var message = "";
            var status_code = 100;
            var result = new object();
            var token = "";
            var conn = new MySqlConnection(getConnection());
            try
            {
                if (level == 0)
                {
                    var sql = "";
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    sql = $"SELECT * FROM mst_user WHERE id!=1";

                    result = await SqlMapper.QueryAsync<object>(conn, sql, null, null, null, CommandType.Text);
                    if (result != null)
                    {
                        status_code = 200;
                        message = "Success";
                    }
                    else
                    {
                        message = "Data not found";
                    }
                }
            }
            catch (Exception ex)
            {

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

        [HttpPost]
        [Route("get-list-pendaftar")]
        public async Task<IActionResult> GetListPendaftar(int level,string user_id)
        {
            var message = "";
            var status_code = 100;
            var result = new object();
            var token = "";
            var conn = new MySqlConnection(getConnection());
            try
            {
                var sql = "";
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (level == 0)
                {
                    sql = $"SELECT no_ktp,MAX(nama) AS nama,MAX(jenis_kelamin) AS jenis_kelamin,\r\nMAX(address) AS address,MAX(hp) AS hp,COUNT(no_ktp) AS total_pencoblosan\r\nFROM mst_pendaftar_tps\r\nGROUP BY no_ktp;\r\nSELECT * FROM mst_pendaftar_tps WHERE is_enabled=1";
                }
                else
                {
                    sql = $"SELECT no_ktp,MAX(nama) AS nama,MAX(jenis_kelamin) AS jenis_kelamin,\r\nMAX(address) AS address,MAX(hp) AS hp,COUNT(no_ktp) AS total_pencoblosan\r\nFROM mst_pendaftar_tps\r\nGROUP BY no_ktp;\r\nSELECT * FROM mst_pendaftar_tps WHERE created_user='"+ user_id + "' AND is_enabled=1";
                }
               

                result = await SqlMapper.QueryAsync<object>(conn, sql, null, null, null, CommandType.Text);
                if (result != null)
                {
                    status_code = 200;
                    message = "Success";
                }
                else
                {
                    message = "Data not found";
                }
            }
            catch (Exception ex)
            {

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

        [HttpPost]
        [Route("get-list-tempat-tps")]
        public async Task<IActionResult> GetListTps(int level)
        {
            var message = "";
            var status_code = 100;
            var result = new object();
            var conn = new MySqlConnection(getConnection());
            try
            {
                if(level == 0)
                {
                    
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var sql = "SELECT * FROM mst_tps WHERE is_enabled=1";

                    result = await SqlMapper.QueryAsync<object>(conn, sql, null, null, null, CommandType.Text);
                    if (result != null)
                    {
                        status_code = 200;
                        message = "Success";
                    }
                    else
                    {
                        message = "Data not found";
                    }
                }
                
            }
            catch (Exception ex)
            {

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
