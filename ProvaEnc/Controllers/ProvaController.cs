using ProvaEnc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ProvaEnc.Controllers
{
    public class ProvaController : Controller
    {
        private Contexto db = new Contexto();
        private static string AesIV256BD = @"%j?TmFP6$BbMnY$@";
        private static string AesKey256BD = @"rxmBUJy]&,;3jKwDTzf(cui$<nc2EQr)";
        // GET: Mensagem
        public ActionResult Index()
        {
            List<ProvaModel> textos = db.Textos.ToList();

            return View(textos.ToList());
        }

        [HttpGet]
        public ActionResult Create(string? msgE, string? msgD)
        {
            if (msgE != null)
            {
                TempData["msgE"] = msgE;
                TempData["msgD"] = msgD;
            }

            return View();
        }
       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProvaModel prov)
        {
            if (ModelState.IsValid)
            {
                string msgD = prov.Texto;
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.IV = Encoding.UTF8.GetBytes(AesIV256BD);
                aes.Key = Encoding.UTF8.GetBytes(AesKey256BD);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                byte[] src = Encoding.Unicode.GetBytes(prov.Texto);
                using (ICryptoTransform encrypt = aes.CreateEncryptor())
                {
                    byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);

                    prov.Texto = Convert.ToBase64String(dest);

                }
                string msgE = prov.Texto;
                db.Textos.Add(prov);
                db.SaveChanges();
                return RedirectToAction(nameof(Create), new { @msgE = msgE, @msgD = msgD });
            }
            return RedirectToAction(nameof(Create));
        }
       

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProvaModel prov = db.Textos.Find(id);
            if (prov == null)
            {
                return HttpNotFound();
            }

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.BlockSize = 128;
            aes.KeySize = 256;
            aes.IV = Encoding.UTF8.GetBytes(AesIV256BD);
            aes.Key = Encoding.UTF8.GetBytes(AesKey256BD);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            string msgE = prov.Texto;
            byte[] src = Convert.FromBase64String(prov.Texto);
            using (ICryptoTransform decrypt = aes.CreateDecryptor())
            {
                byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                prov.Texto = Encoding.Unicode.GetString(dest);
            }
            string msgD = prov.Texto;
            TempData["msgE"] = msgE;
            TempData["msgD"] = msgD;
            
            return View();
        }
    }
}