using DIGISIGN_iTextSharp.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DIGISIGN_iTextSharp.Controllers
{
    public class ValuesController : ApiController
    {
        public ResponseModel Test(InformationModel model)
        {
            try
            {

                string path = ConfigurationManager.AppSettings["Path"].ToString();
                int x = Convert.ToInt32(model.X1);
                int y = Convert.ToInt32(model.Y1);
                int width = Convert.ToInt32(model.X2);
                int height = Convert.ToInt32(model.Y2);

                FileStream fs = new FileStream(model.dspath, FileMode.Open);

                var store = new Pkcs12Store(fs, model.certpwd.ToCharArray());

                fs.Close();

                var alias = "";

                // searching for private key
                foreach (string al in store.Aliases)
                    if (store.IsKeyEntry(al) && store.GetKey(al).Key.IsPrivate)
                    {
                        alias = al;
                        break;
                    }

                var pk = store.GetKey(alias);

                ICollection<X509Certificate> chain = store.GetCertificateChain(alias).Select(c => c.Certificate).ToList();

                var parameters = pk.Key as RsaPrivateCrtKeyParameters;

                string pathPdf = path + "Invoice.pdf";

                string pathToSigPdf = path + "Invoice_WithSign.pdf";

                PdfReader reader = new PdfReader(pathPdf);

                FileStream fileStreamSigPdf = new FileStream(pathToSigPdf, FileMode.Create);

                PdfStamper stamper = PdfStamper.CreateSignature(reader, fileStreamSigPdf, '\0', null, true);
                PdfSignatureAppearance appearance = stamper.SignatureAppearance;


                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    Rectangle rectangle = new Rectangle(x, y, width, height);// funciona vertical
                    appearance.Reason = "Reason_" + i;
                    appearance.Acro6Layers = true;
                    appearance.SignDate = DateTime.Now;
                    appearance.SetVisibleSignature(rectangle, i, "SIGNATURE_" + i);
                    stamper.AddSignature("SIGNATURE_" + i, i, x, y, width, height);

                }

                IExternalSignature pks = new PrivateKeySignature(parameters, DigestAlgorithms.SHA256);
                MakeSignature.SignDetached(appearance, pks, chain, null, null, null, 0, CryptoStandard.CMS);

                fileStreamSigPdf.Close();
                reader.Close();
                stamper.Close();

                //Convert Output PDF into Base64String
                byte[] bytes = System.IO.File.ReadAllBytes(path + "Invoice_WithSign.pdf");
                string base64 = Convert.ToBase64String(bytes);

                return new ResponseModel()
                {
                    Status = "S",
                    StatusCode = 200,
                    Message = "Documnet Signed Successfully.",
                    Data = base64
                };

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(ConfigurationManager.AppSettings["Path"].ToString() + "Log.txt", ex.Message);
                if (ex.InnerException != null)
                {
                    System.IO.File.AppendAllText(ConfigurationManager.AppSettings["Path"].ToString() + "Log.txt", ex.InnerException.Message);
                }

                return new ResponseModel()
                {
                    Status = "E",
                    StatusCode = 500,
                    Message = "Documnet Signed fail.",
                    Data = ex.Message
                };
            }
        }


        //public PdfFormField addMultiAnnotationSignatureField(PdfStamper pdfStamper, String name, float llx, float lly, float urx, float ury)
        //{
        //    PdfWriter pdfStamperImp = pdfStamper.getWriter();
        //    PdfAcroForm acroForm = pdfStamperImp.getAcroForm();

        //    PdfFormField signature = PdfFormField.createSignature(pdfStamperImp);
        //    signature.setFieldName(name);

        //    for (int page = pdfStamper.getReader().getNumberOfPages(); page > 0; page--)
        //    {
        //        PdfFormField annotation = createAnnotation(pdfStamperImp);
        //        acroForm.setSignatureParams(annotation, null, llx, lly, urx, ury);
        //        acroForm.drawSignatureAppearences(annotation, llx, lly, urx, ury);
        //        annotation.setPlaceInPage(page);
        //        annotation.setPage(page);
        //        annotation.remove(PdfName.FT);
        //        signature.addKid(annotation);
        //        llx += page * 5;
        //        lly += page * 5;
        //        urx += page * 10;
        //        ury += page * 10;
        //    }
        //    pdfStamper.addAnnotation(signature, 1);
        //    return signature;
        //}
    }
}
