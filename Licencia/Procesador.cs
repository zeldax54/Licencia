using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.IO;
using System.Security.Cryptography;
namespace Licencia
{
   public class Procesador
    {


        public void Generar(string tipo)
        {
            if (tipo == "TRIAL")
            {
               string d = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
               string archivo = CrearArchivoTmp(d);

               string e = Encryptar("6");             

                using (File.Create(archivo)) { }
                File.WriteAllText(archivo, e);
            }
            else
            {
                string proc = "";
                List<string> CUP = GetCPUId();

                for (int i = 0; i < CUP.Count; i++)
                {
                    proc += CUP[i];
                }
                string result = proc;
                string d = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string archivo = CrearArchivoTmp(d);
                int final = 0;
                final = result.GetHashCode();
                string e = Encryptar(final.ToString());
                using (File.Create(archivo)) { }
                File.WriteAllText(archivo, e);

                if (tipo == "FULL")
                {
                    d = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    archivo = CrearArchivoTmp(d);
                    using (File.Create(archivo)) { }
                    File.WriteAllText(archivo, e);

                }
 
            }


           
                   
 
        }



        private List<string> GetCPUId()
        {
            string cpuInfo = String.Empty;      
            string nombre = "";
       
            List<string> datos = new List<string>();
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((cpuInfo == String.Empty))
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                    nombre = mo.Properties["Name"].Value.ToString();
                    datos.Add(nombre);
                    datos.Add(cpuInfo);
                }
            } return datos;
        }//ok


        private List<string> GetHDSerial()//ok
        {

            List<string> datos = new List<string>();
            long x = 0;
            try
            {
                ManagementObjectSearcher searcher =
                      new ManagementObjectSearcher("root\\CIMV2",
                      "SELECT * FROM Win32_DiskDrive WHERE MediaType = 'Fixed hard disk media'");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    datos.Add(queryObj["Model"].ToString());
                    datos.Add(queryObj["SerialNumber"].ToString());
                    x = Int64.Parse(queryObj["Size"].ToString());
                    x = x / 1024 / 1024 / 1024;
                    datos.Add(x.ToString() + " GB");
                    datos.Add("-------------------------------");
                }
            }
            catch (ManagementException e)
            {
                throw new Exception("An error occurred while querying for WMI data: " + e.Message);
            }
            return datos;
        }

        public string CrearArchivoTmp(string dir)
        {
            
            string dirTMp = "";
            string carpetaRaiz = dir;
            string dirtmp = Path.Combine(carpetaRaiz,"N");
            dirTMp = carpetaRaiz;
            Directory.CreateDirectory(dirtmp);
            string nombre = "lic.hl";
            string dirfinal = Path.Combine(dirtmp, nombre);
            return dirfinal;

        }


        public string  Encryptar(string cadena)
        {
            string key = "**H0H**D*L";
            byte[] keyArray;
            byte[] Arreglo_a_Cifrar =UTF8Encoding.UTF8.GetBytes(cadena);
            MD5CryptoServiceProvider hashmd5 =new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] ArrayResultado =
           cTransform.TransformFinalBlock(Arreglo_a_Cifrar,
           0, Arreglo_a_Cifrar.Length);

            tdes.Clear();

            return Convert.ToBase64String(ArrayResultado,0, ArrayResultado.Length);
        }
    }
}
