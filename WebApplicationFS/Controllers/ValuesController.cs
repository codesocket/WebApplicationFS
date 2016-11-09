//SELECT file_stream.PathName() AS FilePath FROM AuditDocument where name = 'CQRS_Journey_Guide.pdf'

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Transactions;
using System.Web.Http;

namespace WebApplicationFS.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            using (var tx = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
            {
                var db = new SampleFSEntities();

                using (db.Database.Connection)
                {
                    db.Database.Connection.Open();


                    var ctx = GetTransactionContext(db.Database.Connection as SqlConnection);

                    using (var fs = new SqlFileStream(@"\\US1171390W2\MSSQLSERVER\v02-A60EC2F8-2B24-11DF-9CC3-AF2E56D89593\SampleFS\dbo\AuditDocument\file_stream\62972138-21A6-E611-8508-3402867C2166\VolumeHint-HarddiskVolume1", ctx, FileAccess.Read))
                    {
                        var content = new byte[fs.Length];
                        fs.Read(content, 0, content.Length);

                        response.Content = new ByteArrayContent(content);
                        response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                        response.Content.Headers.ContentDisposition.FileName = "mama.pdf";
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                    }

                }

                tx.Complete();
            }

            return response;
        }

        private byte[] GetTransactionContext(SqlConnection sqlConnection)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT GET_FILESTREAM_TRANSACTION_CONTEXT()";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection;

            return cmd.ExecuteScalar() as byte[];
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
