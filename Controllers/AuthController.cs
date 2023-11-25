using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace tps_apps.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public AuthController(IConfiguration config, IWebHostEnvironment env)
        {
            this._config = config;
            this._env = env;
        }

        private string getConnection()
        {
            return _config["ConnectionStrings:mysql"];
        }

        // GET: api/<AuthController>
        [HttpGet]
        [Route("Login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var message = "";
            var status_code = 100;
            var result = new object();
            var token = "";
            var conn = new MySqlConnection(getConnection());
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                var sql = $"SELECT a.*,b.nama,c.nama AS nama_instansi,d.nama_tps FROM (" +
                    $"  SELECT login_id,username,password,level FROM mst_login " +
                    $"  WHERE username='" + username + "' AND PASSWORD='" + password + "' AND is_enabled=1" +
                    $"  ) AS a " +
                    $"  INNER JOIN mst_user b ON a.login_id=b.id" +
                    $" LEFT JOIN mst_instansi c ON c.id=b.instansi_id" +
                    $" LEFT JOIN mst_tps d ON b.tps_id=d.id";
                result = await SqlMapper.QueryFirstOrDefaultAsync<object>(conn, sql, null, null, null, CommandType.Text);
                if (result == null)
                {
                    message = "Username atau password salah";
                }
                else
                {
                    var data = result as dynamic;
                    var authClaims = new List<Claim>
                    {
                       new Claim(ClaimTypes.Name, data.nama),
                       new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };
                     token = GenerateToken(authClaims);
                    status_code = 200;
                    message = "Success";
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
            return Ok(new { status_code, message, token, data = result });
        }


        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTKey:Secret"]));
            var _TokenExpiryTimeInHour = Convert.ToInt64(_config["JWTKey:TokenExpiryTimeInHour"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _config["JWTKey:ValidIssuer"],
                Audience = _config["JWTKey:ValidAudience"],
                //Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        //// GET api/<AuthController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<AuthController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<AuthController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<AuthController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
