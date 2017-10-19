using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics.Drawables;
using Android.Graphics;
using ZXing.Mobile;
using BodySystemApp.DTO;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.Mail;

namespace BodySystemApp
{
    [Activity(Label = "Neew", MainLauncher = true, Icon = "@drawable/evento")]
    public class MainActivity : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            MobileBarcodeScanner.Initialize(Application);
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            
            ActionBar.Hide();

            var color = new ColorDrawable(new Color(0, 0, 0, 3));


            ImageButton buttonQR = FindViewById<ImageButton>(Resource.Id.btnScannerQR);

            buttonQR.Click += delegate
            {

                StartActivity(typeof(ScannerActivity));
                
            };



            ImageButton buttonLts = FindViewById<ImageButton>(Resource.Id.btnList);

            buttonLts.Click += delegate { StartActivity(typeof(ListRegistroActivity)); };
        }

        //private async Task<RegistroDTO> GetRegistro()
        //{
        //    var responseGral = new RegistroDTO();
        //    try
        //    {
        //        TcpClient client = new TcpClient();
        //        client.BaseAddress = new Uri("http://grapesoft-001-site23.ctempurl.com");

        //        var url = string.Format("umbraco/surface/Validacion/Search?Registro={0}&Nombre={1}&ApPaterno={2}&ApMaterno={3}&Email={4}&Empresa={5}", Registro == null ? null : Registro, Nombre, ApPaterno, ApMaterno, Email, Empresa);
        //        HttpResponseMessage response = client.GetAsync(url).Result;  // Blocking call! 

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var customerJsonString = await response.Content.ReadAsStringAsync();

        //            JObject a = JObject.Parse(customerJsonString);

        //            var success = ((JValue)a.SelectToken("success"));

        //            var registroLts = new List<RegistroDTO>();

        //            foreach (var result in ((JArray)a.SelectToken("result")))
        //            {

        //                var reg = new RegistroDTO();

        //                reg.IdRegistro = Convert.ToInt32(((JValue)result.SelectToken("IdRegistro")).Value.ToString());
        //                reg.Nombre = ((JValue)result.SelectToken("Nombre")).Value.ToString();
        //                reg.ApellidoPaterno = ((JValue)result.SelectToken("ApellidoPaterno")).Value.ToString();
        //                reg.ApellidoMaterno = ((JValue)result.SelectToken("ApellidoMaterno")).Value.ToString();
        //                reg.Sexo = ((JValue)result.SelectToken("Sexo")).Value.ToString();
        //                reg.Edad = GetValueToObject(((JObject)result.SelectToken("Edad")), "Descripcion");
        //                reg.Empresa = ((JValue)result.SelectToken("Empresa")).Value.ToString();
        //                reg.Cargo = ((JValue)result.SelectToken("Cargo")).Value.ToString();
        //                reg.Ciudad = GetValueToObject(((JObject)result.SelectToken("Ciudad")), "Descripcion");
        //                reg.Email = ((JValue)result.SelectToken("Email")).Value.ToString();
        //                reg.Telefono = ((JValue)result.SelectToken("Telefono")).Value.ToString();
        //                reg.Pais = GetValueToObject(((JObject)result.SelectToken("Pais")), "Descripcion");
        //                reg.Estatus = GetValueToObject(((JObject)result.SelectToken("ctStatusRegistro")), "Descripcion");
        //                reg.nombrePase = ((JValue)result.SelectToken("nombrePase")).Value.ToString();
        //                reg.Costo = ((JValue)result.SelectToken("Costo")).Value.ToString();


        //                registroLts.Add(reg);

        //                responseGral.Result = registroLts;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        responseGral.Code = -100;
        //        responseGral.Message = ex.Message;
        //    }
        //    return responseGral;
        //}
    }
}

