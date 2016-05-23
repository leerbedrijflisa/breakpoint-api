using Newtonsoft.Json;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography;

namespace Lisa.Breakpoint.Api
{
    public class RSAKeyUtils
    {
        public static RSAParameters GetRandomKey()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    return rsa.ExportParameters(true);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        public static void GenerateKeyAndSave(string file)
        {
            var p = GetRandomKey();
            RSAParametersWithPrivate t = new RSAParametersWithPrivate();
            t.SetParameters(p);
            File.WriteAllText(file, JsonConvert.SerializeObject(t));
        }

        public static RSAParameters GetKeyParameters(string file)
        {
            if (!File.Exists(file)) throw new FileNotFoundException("Check configuration - cannot find auth key file: " + file);
            var keyParams = JsonConvert.DeserializeObject<RSAParametersWithPrivate>(File.ReadAllText(file));
            return keyParams.ToRSAParameters();
        }

        public static RsaSecurityKey GetRSAKey()
        {
            if (!File.Exists("rsa.json"))
            {
                GenerateKeyAndSave("rsa.json");
            }

            RSAParameters keyParams = GetKeyParameters("rsa.json");

            return new RsaSecurityKey(keyParams);
        }

        private class RSAParametersWithPrivate
        {
            public byte[] D { get; set; }
            public byte[] DP { get; set; }
            public byte[] DQ { get; set; }
            public byte[] Exponent { get; set; }
            public byte[] InverseQ { get; set; }
            public byte[] Modulus { get; set; }
            public byte[] P { get; set; }
            public byte[] Q { get; set; }

            public void SetParameters(RSAParameters p)
            {
                D = p.D;
                DP = p.DP;
                DQ = p.DQ;
                Exponent = p.Exponent;
                InverseQ = p.InverseQ;
                Modulus = p.Modulus;
                P = p.P;
                Q = p.Q;
            }
            public RSAParameters ToRSAParameters()
            {
                return new RSAParameters()
                {
                    D = this.D,
                    DP = this.DP,
                    DQ = this.DQ,
                    Exponent = this.Exponent,
                    InverseQ = this.InverseQ,
                    Modulus = this.Modulus,
                    P = this.P,
                    Q = this.Q
                };
            }
        }
    }
}