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
using BodySystemApp.DTO;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using Android.Graphics;
using SQLite;
using Java.Util;
using Java.Net;

namespace BodySystemApp
{
    [Activity(Label = "Scanner QR")]
    public class ScannerActivity : Activity
    {
        private int Registro = 0;
        private string KeyNeew = string.Empty;
        //private SQLiteConnection db;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GetKey();
            //CreateDataBase if not exists
            //string dbPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "ormNeew.db3");

            //db = new SQLiteConnection(dbPath);
            //if (!TableExists("Items", db))

            //    db.CreateTable<RegistrosDbDTO>();

            // Create your application here

            SetContentView(Resource.Layout.Scanner);
            ActionBar.Hide();

            Button btnCancelar = FindViewById<Button>(Resource.Id.btnCancelar);
            btnCancelar.SetBackgroundColor(new Color(236, 28, 73));
            btnCancelar.Click += delegate
            {
                base.OnBackPressed();
            };

            Button btnGuardar = FindViewById<Button>(Resource.Id.btnGuardar);
            btnGuardar.SetBackgroundColor(new Color(110, 203, 221, 50));
            btnGuardar.Enabled = false;
            btnGuardar.Click += delegate
            {
                if (Registro != 0)
                {
                    string urlSave = Resources.GetString(Resource.String.Url).ToString() + "SaveRegistro?Registro=" + Registro + "&key=" + KeyNeew;

                    if (BooleanRequestWS(urlSave).Result)
                    {
                        AlertDialog alert = new AlertDialog.Builder(this)
                                     .SetTitle("Aviso")
                                     .SetPositiveButton("OK", delegate
                                     {
                                         base.OnBackPressed();
                                     })
                                     .SetMessage("Registro guardado.")
                                     .Create();
                        alert.Show();
                    }
                    else
                    {
                        AlertDialog alert = new AlertDialog.Builder(this)
                                    .SetTitle("Error")

                                    .SetMessage("Error al guardar el registro.")
                                    .Create();
                        alert.Show();
                    }
                }                

            };

            LinearLayout linearLayout2 = FindViewById<LinearLayout>(Resource.Id.linearLayout2);
            linearLayout2.SetBackgroundColor(new Color(0, 0, 0, 70));



            Scaner();


        }

        private async void Scaner()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var result = await scanner.Scan();
            int IdRegistro = 0;
            if (result != null)
            {
                if (!Int32.TryParse(result.Text, out IdRegistro))
                {
                    AlertDialog alert = new AlertDialog.Builder(this)
                       .SetTitle("Error")
                        .SetPositiveButton("OK", delegate
                        {
                            base.OnBackPressed();
                        })
                       .SetMessage("El código QR no tiene la información correcta. Información obtenida: " + result.Text)
                       .Create();

                    alert.Show();
                }
                else
                {

                    string url = Resources.GetString(Resource.String.Url).ToString() + "SearchByIdQR?Registro=" + result.Text;


                    // Fetch the weather information asynchronously, 
                    // parse the results, then update the screen:
                    var json = GetRegistro(url);
                    if (json.Result.Nombre == null)
                    {
                        AlertDialog alert = new AlertDialog.Builder(this)
                                  .SetTitle("Error")
                                    .SetPositiveButton("OK", delegate
                                    {
                                        base.OnBackPressed();
                                    })
                                  .SetMessage("No existe registro para el código QR escaneado.")
                                  .Create();

                        alert.Show();
                    }
                    else
                    {

                        TextView tvNombres = FindViewById<TextView>(Resource.Id.tvNombres);
                        tvNombres.Text = json.Result.Nombre;

                        TextView tvApellidos = FindViewById<TextView>(Resource.Id.tvApellidos);
                        tvApellidos.Text = json.Result.ApellidoPaterno + " " + json.Result.ApellidoMaterno;

                        TextView tvEdad = FindViewById<TextView>(Resource.Id.tvEdad);
                        tvEdad.Text = json.Result.Edad;

                        TextView tvSexo = FindViewById<TextView>(Resource.Id.tvSexo);
                        tvSexo.Text = json.Result.Sexo.Equals("H") ? "MASCULINO" : "FEMENINO";

                        TextView tvEmpresa = FindViewById<TextView>(Resource.Id.tvEmpresa);
                        tvEmpresa.Text = json.Result.Empresa;

                        TextView tvCargo = FindViewById<TextView>(Resource.Id.tvCargo);
                        tvCargo.Text = json.Result.Cargo;

                        TextView tvPais = FindViewById<TextView>(Resource.Id.tvPais);
                        tvPais.Text = json.Result.Pais.ToUpperInvariant();

                        TextView tvEstado = FindViewById<TextView>(Resource.Id.tvEstado);
                        tvEstado.Text = json.Result.Estatus.ToUpperInvariant();

                        if (json.Result.Estatus.ToUpperInvariant().Contains("VALID"))
                        {
                            string urlEx = Resources.GetString(Resource.String.Url).ToString() + "ExistsRegistro?Registro=" + result.Text + "&key=" + KeyNeew;

                            if (!BooleanRequestWS(urlEx).Result)
                            {
                                Registro = json.Result.IdRegistro;
                                Button btnGuardar = FindViewById<Button>(Resource.Id.btnGuardar);
                                btnGuardar.SetBackgroundColor(new Color(110, 203, 221));
                                btnGuardar.Enabled = true;
                            }
                            else
                            {
                                AlertDialog alert = new AlertDialog.Builder(this)
                                     .SetTitle("Error")
                                     .SetMessage("El número de registro escaneado ya existe.")
                                     .Create();
                                alert.Show();
                            }
                        }
                        else
                        {
                            AlertDialog alert = new AlertDialog.Builder(this)
                                 .SetTitle("Error")
                                 .SetMessage("El número de registro escaneado no está validado.")
                                 .Create();
                            alert.Show();
                        }
                    }
                }
            }

        }

        private async Task<RegistroDTO> GetRegistro(string url)
        {
            var response = new RegistroDTO();
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
                    response = GetListRegistro(a.SelectToken("result"));
                }


            }
            return response;
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

        private async Task<bool> BooleanRequestWS(string url)
        {
            var response = false;
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

        private void GetKey()
        {
            var all = Collections.List(NetworkInterface.NetworkInterfaces);

            foreach (var interf in all)
            {
                var macBytes = (interf as NetworkInterface).GetHardwareAddress();

                if (macBytes == null) continue;

                var sb = new StringBuilder();
                foreach (var b in macBytes)
                {
                    sb.Append((b & 0xFF).ToString("X2") + ":");
                }

                Console.WriteLine(sb.ToString().Remove(sb.Length - 1));
                KeyNeew = sb.ToString().Remove(sb.Length - 1);
            }
        }
    }
}