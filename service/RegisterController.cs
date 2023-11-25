using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;
using System.Security.Claims;
using tps_apps.model;

namespace tps_apps.service
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public RegisterController(IConfiguration config, IWebHostEnvironment env)
        {
            this._config = config;
            this._env = env;
        }

        private string getConnection()
        {
            return _config["ConnectionStrings:mysql"];
        }

        [HttpPost]
        [Route("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] mst_user_param model)
        {
            var message = "";
            var status_code = 100;
            var result = new object();
            var token = "";
            var conn = new MySqlConnection(getConnection());
            try
            {
                if (model.instansi_id <= 0)
                {
                    message = "Instansi id tidak boleh kosong";
                }
                else if (string.IsNullOrEmpty(model.nama))
                {
                    message = "Nama tidak boleh kosong";
                }
                else if (string.IsNullOrEmpty(model.username))
                {
                    message = "Username tidak boleh kosong";
                }
                else if (model.username.Length < 5)
                {
                    message = "Username max 5 character";
                }
                else if (string.IsNullOrEmpty(model.password))
                {
                    message = "Password tidak boleh kosong";
                }
                else if (model.password.Length < 5)
                {
                    message = "Password max 5 character";
                }
                else if (model.tps_id <= 0)
                {
                    message = "TPS id tidak boleh kosong";
                }
                else if (string.IsNullOrEmpty(model.jenis_kelamin))
                {
                    message = "Jenis kelamin tidak boleh kosong";
                }
                else
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var sql = $"SELECT count(id) as total FROM mst_user WHERE username='" + model.username + "'";
                    var count = await SqlMapper.ExecuteScalarAsync<int>(conn, sql, null, null, null, CommandType.Text);
                    if (count > 0)
                    {
                        message = "Username sudah ada, silahkan gunakan yang lain";
                    }
                    else
                    {
                        sql = "";
                        sql = $"INSERT INTO mst_user(instansi_id,nama,jenis_kelamin,no_hp,tps_id,lon,lat,username,PASSWORD,is_enabled,status,created_date,created_user) " +
                            $" VALUES('{model.instansi_id}','{model.nama}','{model.jenis_kelamin}','{model.no_hp}',{model.tps_id},'{model.lon}','{model.lat}','{model.username}','{model.password}',0,'NEW',NOW(),'{model.user_id}')";

                        var save = await SqlMapper.ExecuteAsync(conn, sql, null, null, null, CommandType.Text);
                        if (save > 0)
                        {
                            status_code = 200;
                            message = "Create user baru success";
                        }
                        else
                        {
                            message = "Create user baru failed";
                        }
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
        [Route("approve-or-reject")]
        public async Task<IActionResult> ApproveOrReject([FromBody] mst_user_approve_param model)
        {
            var message = "";
            var status_code = 100;
            var result = new object();
            var token = "";
            var conn = new MySqlConnection(getConnection());
            try
            {
                if (model.id <= 0)
                {
                    message = "ID tidak boleh kosong";
                }
                else if (string.IsNullOrEmpty(model.username))
                {
                    message = "Username tidak boleh kosong";
                }
                else
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var sql = $"SELECT count(id) as total FROM mst_user WHERE id=" + model.id + " AND username='" + model.username + "' AND is_enabled=0 AND status='NEW'";
                    var count = await SqlMapper.ExecuteScalarAsync<int>(conn, sql, null, null, null, CommandType.Text);
                    if (count <= 0)
                    {
                        message = "Username tidak ditemukan";
                    }
                    else
                    {
                        if (model.status_approve == "APR")
                        {
                            sql = "";
                            sql = $"UPDATE mst_user SET is_enabled=1,status='APR',updated_user='" + model.user_id + "',updated_date=NOW() WHERE id=" + model.id + " AND username = '" + model.username + "' AND is_enabled=0 AND status='NEW'";

                            var update = await SqlMapper.ExecuteAsync(conn, sql, null, null, null, CommandType.Text);
                            if (update > 0)
                            {
                                var message2 = "";
                                sql = "";
                                sql = $"INSERT INTO mst_login(username,password,level,is_enabled,created_user,created_date) " +
                                    $"SELECT username,password,1 AS level,1 AS enabled,'" + model.user_id + "' AS created_user,NOW() AS created_date " +
                                    $"FROM mst_user WHERE id=" + model.id + " AND username='" + model.username + "' AND is_enabled=1 AND STATUS='APR'";
                                var update2 = await SqlMapper.ExecuteAsync(conn, sql, null, null, null, CommandType.Text);
                                if (update2 > 0)
                                {
                                    message2 = "Create login success";
                                }
                                else
                                {
                                    message2 = "Create login failed";
                                }
                                status_code = 200;
                                message = $"Approved user success,{message2}";
                            }
                            else
                            {
                                message = $"Approved user failed";
                            }
                        }
                        else
                        {
                            sql = "";
                            sql = $"UPDATE mst_user SET is_enabled=0,status='DE',updated_user='" + model.user_id + "',updated_date=NOW() WHERE id=id=" + model.id + " AND username = '" + model.username + "' AND is_enabled=0 AND status='NEW'";

                            var update = await SqlMapper.ExecuteAsync(conn, sql, null, null, null, CommandType.Text);
                            if (update > 0)
                            {
                                message = "Reject user success";
                            }
                            else
                            {
                                message = "Reject user failed";
                            }
                        }

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
