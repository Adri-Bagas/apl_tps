using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using tps_apps.model;
using tps_apps.service;

namespace tps_apps.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public VoteController(IConfiguration config, IWebHostEnvironment env)
        {
            this._config = config;
            this._env = env;
        }

        private string getConnection()
        {
            return _config["ConnectionStrings:mysql"];
        }

        [HttpPost]
        [Route("validasi-ktp")]
        public async Task<IActionResult> ValidasiKtp(string no_ktp)
        {
            var message = "";
            var status_code = 100;
            var result = new object();
            var token = "";
            var conn = new MySqlConnection(getConnection());
            try
            {
                if (string.IsNullOrEmpty(no_ktp))
                {
                    message = "No KTP tidak boleh kosong";
                }
                else
                {
                    var sql = "";
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var _no_ktp = Regex.Replace(no_ktp, @"[^A-Za-z0-9 /]+|( )+", "$1").Trim().ToUpper();
                    sql = $"SELECT count(no_ktp) as total FROM mst_pendaftar_tps WHERE no_ktp='{no_ktp}'";
                    var count = await SqlMapper.ExecuteScalarAsync<int>(conn, sql, null, null, null, CommandType.Text);
                    if (count > 0)
                    {
                        status_code = 200;
                        message = $"Data NIK/KTP di temukan {no_ktp}";
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
        [Route("create-vote")]
        public async Task<IActionResult> CreateVote(string no_ktp, string lat, string lon, string address, string user_id)
        {
            var message = "";
            var status_code = 100;
            var result = new object();
            var token = "";
            var conn = new MySqlConnection(getConnection());
            try
            {
                if (string.IsNullOrEmpty(no_ktp))
                {
                    message = "No KTP tidak boleh kosong";
                }
                else
                {
                    var sql = "";
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    var _no_ktp = Regex.Replace(no_ktp, @"[^A-Za-z0-9 /]+|( )+", "$1").Trim().ToUpper();
                    sql = $"SELECT count(no_ktp) as total FROM trx_vote WHERE no_ktp='{no_ktp}' AND is_enabled=1";
                    var count = await SqlMapper.ExecuteScalarAsync<int>(conn, sql, null, null, null, CommandType.Text);
                    if (count > 0)
                    {
                        status_code = 100;
                        message = $"Input vote by NIK/KTP {no_ktp} sudah ada";
                    }
                    else
                    {
                        sql = "";
                        sql = "INSERT INTO trx_vote(no_ktp,lon,lat,address,is_enabled,created_date,created_user) " +
                            "VALUES('" + _no_ktp + "','" + lon + "','" + lat + "','" + address + "',1,'" + user_id + "',NOW())";
                        var save = await SqlMapper.ExecuteAsync(conn, sql, null, null, null, CommandType.Text);

                        status_code = 200;
                        message = $"Input vote by NIK/KTP {no_ktp} Berhasil!";
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
