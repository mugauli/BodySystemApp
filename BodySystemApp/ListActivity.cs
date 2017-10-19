using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using Java.Net;
using Newtonsoft.Json.Linq;
using BodySystemApp.DTO;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Android.Graphics;
using System.Text.RegularExpressions;
using System.Globalization;

namespace BodySystemApp
{
    [Activity(Label = "ListActivity")]
    public class ListRegistroActivity : Activity
    {
        bool invalid = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here

            SetContentView(Resource.Layout.List);
            ActionBar.Hide();

            string url = Resources.GetString(Resource.String.Url).ToString() + "GetRegistros?key=" + GetKey();

            var lstRegistro = GetRegistro(url);

            ListView lvRegistros = FindViewById<ListView>(Resource.Id.lvRegistro);

            var ListAdapter = new ArrayAdapter<string>(this, Resource.Layout.list_item, (lstRegistro.Result.Select(x=> (x.Nombre + " " + x.ApellidoPaterno + " " + x.ApellidoMaterno)).ToList()));

            lvRegistros.TextFilterEnabled = true;

            lvRegistros.Adapter = ListAdapter;

            //ListView.ItemClick += delegate (object sender, ItemEventArgs args) {
            //    // When clicked, show a toast with the TextView text
            //    Toast.MakeText(Application, ((TextView)args.View).Text, ToastLength.Short).Show();
            //};

            

            

            Button btnGuardar = FindViewById<Button>(Resource.Id.btnEnviar);
            btnGuardar.SetBackgroundColor(new Color(110, 203, 221));            
            btnGuardar.Click += delegate
            {

                EditText txtEmail = FindViewById<EditText>(Resource.Id.txtEmail);

                if (!string.IsNullOrEmpty(txtEmail.Text) && IsValidEmail(txtEmail.Text))
                {
                    string urlSave = Resources.GetString(Resource.String.Url).ToString() + "SendRegistro?key=" + GetKey() + "&email="+ txtEmail.Text;

                    if (BooleanRequestWS(urlSave).Result)
                    {
                        AlertDialog alert = new AlertDialog.Builder(this)
                                     .SetTitle("Aviso")
                                     .SetPositiveButton("OK", delegate
                                     {
                                         base.OnBackPressed();
                                     })
                                     .SetMessage("Lista de registros enviada a su correo electronico.")
                                     .Create();
                        alert.Show();
                    }
                    else
                    {
                        AlertDialog alert = new AlertDialog.Builder(this)
                                    .SetTitle("Error")

                                    .SetMessage("Error al enviar correo electronico con registros.")
                                    .Create();
                        alert.Show();
                    }
                }
                else
                {
                    AlertDialog alert = new AlertDialog.Builder(this)
                                   .SetTitle("Error")

                                   .SetMessage("Introduzca una dirección de correo valida.")
                                   .Create();
                    alert.Show();
                }
                

            };
        }

        private async Task<List<RegistroDTO>> GetRegistro(string url)
        {
            var ltsRegistro = new List<RegistroDTO>();
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            using (HttpWebResponse httpRes = (HttpWebResponse)request.GetResponse())
            {
                StreamReader reader = new StreamReader(httpRes.GetResponseStream());

                string content = reader.ReadToEnd();

                var a = JObject.Parse(content);

                var success = ((Newtonsoft.Json.Linq.JValue)a.SelectToken("success"));
                if ((bool)success.Value)
                {
                
                    foreach (var result in ((JArray)a.SelectToken("result")))
                    {                        
                        ltsRegistro.Add(GetListRegistro(result));
                    }
                }


            }
            return ltsRegistro;
        }

        private RegistroDTO GetListRegistro(JToken result)
        {
            var reg = new RegistroDTO();

            reg.IdRegistro = Convert.ToInt32(((Newtonsoft.Json.Linq.JValue)result.SelectToken("IdRegistro")).Value.ToString());
            reg.Nombre = ((Newtonsoft.Json.Linq.JValue)result.SelectToken("Nombre")).Value.ToString();
            reg.ApellidoPaterno = ((Newtonsoft.Json.Linq.JValue)result.SelectToken("ApellidoPaterno")).Value.ToString();
            reg.ApellidoMaterno = ((Newtonsoft.Json.Linq.JValue)result.SelectToken("ApellidoMaterno")).Value.ToString();
            reg.Sexo = ((Newtonsoft.Json.Linq.JValue)result.SelectToken("Sexo")).Value.ToString();
            reg.Edad = GetValueToObject(((JObject)result.SelectToken("Edad")), "Descripcion");
            reg.Empresa = ((Newtonsoft.Json.Linq.JValue)result.SelectToken("Empresa")).Value.ToString();
            reg.Cargo = ((Newtonsoft.Json.Linq.JValue)result.SelectToken("Cargo")).Value.ToString();
            reg.Ciudad = GetValueToObject(((JObject)result.SelectToken("Ciudad")), "Descripcion");
            reg.Email = ((Newtonsoft.Json.Linq.JValue)result.SelectToken("Email")).Value.ToString();
            reg.Telefono = ((Newtonsoft.Json.Linq.JValue)result.SelectToken("Telefono")).Value.ToString();
            reg.Pais = GetValueToObject(((JObject)result.SelectToken("Pais")), "Descripcion");
            reg.Estatus = GetValueToObject(((JObject)result.SelectToken("ctStatusRegistro")), "Descripcion");
            reg.nombrePase = ((Newtonsoft.Json.Linq.JValue)result.SelectToken("nombrePase")).Value.ToString();
            reg.Costo = ((Newtonsoft.Json.Linq.JValue)result.SelectToken("Costo")).Value.ToString();

            return reg;

        }

        private string GetValueToObject(JObject obj, string value)
        {

            return ((Newtonsoft.Json.Linq.JValue)obj.SelectToken(value)).Value.ToString();

        }

        private string GetKey()
        {
            var all = Collections.List(NetworkInterface.NetworkInterfaces);
            string response = string.Empty;
            foreach (var interf in all)
            {
                var macBytes = (interf as NetworkInterface).GetHardwareAddress();

                if (macBytes == null) continue;

                var sb = new StringBuilder();
                foreach (var b in macBytes)
                {
                    sb.Append((b & 0xFF).ToString("X2") + ":");
                }
                response = sb.ToString().Remove(sb.Length - 1);
                //Console.WriteLine(sb.ToString().Remove(sb.Length - 1));
                
            }
            return response;
        }

        private async Task<bool> BooleanRequestWS(string url)
        {
            bool response = false;
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            using (HttpWebResponse httpRes = (HttpWebResponse)request.GetResponse())
            {
                StreamReader reader = new StreamReader(httpRes.GetResponseStream());

                string content = reader.ReadToEnd();

                var a = JObject.Parse(content);

                var success = ((Newtonsoft.Json.Linq.JValue)a.SelectToken("success"));
                if ((bool)success.Value)
                {
                    var resp = ((Newtonsoft.Json.Linq.JValue)a.SelectToken("result"));
                    response = (bool)resp.Value;
                }


            }
            return response;
        }

        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
    }
}