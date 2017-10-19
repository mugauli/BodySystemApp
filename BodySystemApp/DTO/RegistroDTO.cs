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

namespace BodySystemApp.DTO
{
    public class RegistroDTO
    {
        public int IdRegistro { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Sexo { get; set; }
        public string Edad { get; set; }
        public string Empresa { get; set; }
        public string Cargo { get; set; }
        public string Ciudad { get; set; }
        public string Pais { get; set; }
        public String StrPerfil { get; set; }
        public string SubPErfil { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public Nullable<System.DateTime> Fecha { get; set; }
        public string Estatus { get; set; }
        public string Costo { get; set; }
        public string nombrePase { get; set; }
    }
}