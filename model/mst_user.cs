namespace tps_apps.model
{
    public class mst_user
    {
        public int id { get; set; }
        public int instansi_id { get; set; }
        public string nama { get; set; }
        public string jenis_kelamin { get; set; }
        public string no_hp { get; set; }
        public int tps_id { get; set; }
        public string lon { get; set; }
        public string lat { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int is_enabled { get; set; }
        public DateTime created_date { get; set; }
        public string created_user { get; set; }
        public DateTime updated_date { get; set; }
        public string updated_user { get; set; }
    }

    public class mst_user_param
    {
        public int id { get; set; }
        public int instansi_id { get; set; }
        public string nama { get; set; }
        public string jenis_kelamin { get; set; }
        public string no_hp { get; set; }
        public int tps_id { get; set; }
        public string lon { get; set; }
        public string lat { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string user_id { get; set; }
    }

    public class mst_pendaftaran_param
    {
        public int id { get; set; }
        public string no_ktp { get; set; }
        public string nama { get; set; }
        public string jenis_kelamin { get; set; }
        public string address { get; set; }
        public string hp { get; set; }
        public int is_enabled { get; set; }
        public int level { get; set; }
        public string user_id { get; set; }
        public string nama_tps { get; set; }
        public int id_kelurahan { get; set; }
    }

    public class mst_user_approve_param
    {
        public int id { get; set; }
        public string username { get; set; }
        public string user_id { get; set; }
        public string status_approve { get; set; }
    }
}
