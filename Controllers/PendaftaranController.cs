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
    public class PendaftaranController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public PendaftaranController(IConfiguration config, IWebHostEnvironment env)
        {
            this._config = config;
            this._env = env;
        }

        private string getConnection()
        {
            return _config["ConnectionStrings:mysql"];
        }
        [HttpPost]
        [Route("create-user-vote-tps")]//pendaftaran user by KTP
        public async Task<IActionResult> CreateUserTps([FromBody] mst_pendaftaran_param model)
        {
            var message = "";
            var status_code = 100;
            var result = new object();
            var token = "";
            var conn = new MySqlConnection(getConnection());
            try
            {
                if (string.IsNullOrEmpty(model.no_ktp))
                {
                    message = "No KTP tidak boleh kosong";
                }
                else if (string.IsNullOrEmpty(model.nama))
                {
                    message = "Nama tidak boleh kosong";
                }
                else
                {
                    var sql = "";
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var no_ktp = Regex.Replace(model.no_ktp, @"[^A-Za-z0-9 /]+|( )+", "$1").Trim().ToUpper();
                    sql = $"SELECT no_ktp,nama,jenis_kelamin,address FROM mst_pendaftar_tps WHERE no_ktp='{no_ktp}'";
                    var dataExists = await SqlMapper.QueryAsync<mst_pendaftaran_param>(conn, sql, null, null, null, CommandType.Text);
                    if (dataExists != null && dataExists.Count() > 0)
                    {
                        message = $"Duplicate NIK/KTP {model.no_ktp}";
                        var dateSend = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        var cus = new CustomFunction();
                        var dataModel = dataExists.First();
                        var messageBody = $"Alarm,No KTP di gunakan lebih dari sekali:\n" +
                            $"NoKTP :{dataModel.no_ktp}\n" +
                            $"Nama :{dataModel.nama}\n" +
                            $"Alamat :{dataModel.address}\n" +
                            $"JK :{dataModel.jenis_kelamin}\n" +
                            $"Sender Date :{dateSend}";
                        cus.SendMessageNotif(messageBody);

                    }
                    else
                    {
                        sql = $"INSERT INTO mst_pendaftar_tps(no_ktp,nama,jenis_kelamin,address,hp,is_enabled,created_date,created_user,tps_id) " +
                        $" VALUES('{no_ktp}','{model.nama}','{model.jenis_kelamin}','{model.address}',{model.hp},1,NOW(),'{model.user_id}','{model.tps_id}')";

                        var save = await SqlMapper.ExecuteAsync(conn, sql, null, null, null, CommandType.Text);
                        if (save > 0)
                        {
                            status_code = 200;
                            message = "Input data user tps success";
                        }
                        else
                        {
                            message = "Input data user tps failed";
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
