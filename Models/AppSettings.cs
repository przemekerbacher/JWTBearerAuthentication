namespace JWTAutentication.Models
{
    public class AppSettings
    {
        public string JWT_Secret { get; set; }
        public int JWT_Expire_Minutes { get; set; }
        public int JWT_Refresh_Token_Expire_Minutes { get; set; }
        public string AES_Key { get; set; }
        public string AES_IV { get; set; }
        public string MD5_Secret { get; set; }
    }
}