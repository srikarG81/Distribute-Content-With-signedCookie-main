using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Amazon.Runtime;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.S3;
using Amazon.S3.Transfer;

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using System.IO;
using System.Security.Cryptography;
using Amazon.CloudFront;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace ReadRSAKey
{
    class Program
    {
        static async Task Main(string[] args)
        {

            //Sign a cookie with the private key
            CookiesForCustomPolicy cookies = AmazonCloudFrontCookieSigner.GetCookiesForCustomPolicy(
                @"https://d28xdv8qytgo56.cloudfront.net/test4/*",
                new StreamReader(@"private_key2.pem"),
                "K1WKUOIIW4CXLV",
                DateTime.Now.AddHours(1),
                DateTime.Now.AddHours(-1),
                "155.190.53.4/32");

            // use the cookie values to upload file to S3
            string domain = ".cloudfront.net";
            try
            {
                var cookieContainer = new CookieContainer();
                cookieContainer.Add(new Cookie(cookies.Signature.Key, cookies.Signature.Value) { Domain = domain });
                cookieContainer.Add(new Cookie(cookies.KeyPairId.Key, cookies.KeyPairId.Value) { Domain = domain });
                cookieContainer.Add(new Cookie(cookies.Policy.Key, cookies.Policy.Value) { Domain = domain });
                using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
                using (var client = new HttpClient(handler)
                { BaseAddress = new Uri(@"https://d28xdv8qytgo56.cloudfront.net") })
                {
                    HttpContent content = new StringContent("test", Encoding.UTF8, "text/plain");
                    var response = await client.PutAsync("/test4/text2.txt", content);
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Console.Write("Upload failed");
                        throw new Exception("Request failed");
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("Uploaded successfully");
        }
    }


}
