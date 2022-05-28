using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DIGISIGN_iTextSharp.Models
{
    public class ResponseModel
    {
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }

    public class InfoModel
    {
        public string PDF_ENCODED_STRING_ { get; set; }
        public string ERRORCODE { get; set; }
        public string ERROR_MESSAGE_ { get; set; }
        public string SIGNING_COORDINATE_X1_ { get; set; }
        public string SIGNING_COORDINATE_Y1_ { get; set; }
        public string SIGNING_COORDINATE_X2_ { get; set; }
        public string SIGNING_COORDINATE_Y2_ { get; set; }
        public string URL { get; set; }
        public string CERT_ALIAS_ { get; set; }
        public string CERT_PWD_ { get; set; }
        public string LICENSE_KEY { get; set; }
        public string DS_PATH { get; set; }
        public string NO_PAGES { get; set; }
    }

    public class InformationModel
    {
        public string encodedstring { get; set; }
        public string message { get; set; }
        public string X1 { get; set; }
        public string Y1 { get; set; }
        public string X2 { get; set; }
        public string Y2 { get; set; }
        public string url {get;set;}
        public string certalias{get;set;}
        public string licensekey { get; set; }
        public string certpwd { get; set; }
        public string dspath { get; set; }
        public string nopages { get; set; }

    }
}