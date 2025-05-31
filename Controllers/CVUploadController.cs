using jooneliwebsite.Models;
using jooneliwebsite.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace jooneliwebsite.Controllers
{
    public class CVUploadController : Controller
    {
        private readonly GridFSBucket _gridBucket;
        private readonly IMongoCollection<CVUploadModel> _cvCollection;

        public CVUploadController(MongoDbContext mongodbcontext)
        {

            _gridBucket = mongodbcontext.GridFsBucket;
            _cvCollection = mongodbcontext.CVUploadCollection;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(CVUploadModel model, IFormFile cvFile)
        {
            if (ModelState.IsValid)
            {
                if (cvFile != null && cvFile.Length > 0)
                {
                    byte[] cvFileByte; // byte array to hold 

                    using (var memorystream = new MemoryStream()) //memorystrem to read file content
                    {
                        //copy file file conntent to memorystrema and convert ms to byte array
                        await cvFile.CopyToAsync(memorystream);
                        cvFileByte = memorystream.ToArray();
                    }

                    //  _gridBucket for uploading the file  
                    var fileId = await _gridBucket.UploadFromBytesAsync(cvFile.FileName, cvFileByte);

                    model.CVFileId = fileId.ToString();
                    model.CVFileMetadata = cvFile.FileName;

                    //insert into model
                    await _cvCollection.InsertOneAsync(model);

                    return RedirectToAction("Index", "Career");
                }
                ModelState.AddModelError("CVFile", "Please upload a valid CV file.");
            }

            return BadRequest("could not upload the file");
        }
    }
}
